using CoreObject.Mongo;
using CoreRepository.Infrastructures;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoreRepository.Classes
{
    public abstract class MongoRepositoryBase<TEntity> where TEntity:class
    {
        protected MongoClient _client;
        protected IMongoDatabase _db { get; set; }
        MongoConnectionConfig _mongoConnection;
        internal readonly IBsonSerializerRegistry _serializerRegistry;
        internal readonly IBsonSerializer<TEntity> _documentSerializer;
        public MongoRepositoryBase(IOptions<MongoConnectionConfig> setting)
        {
            _mongoConnection = setting.Value;
            Connect();
            _serializerRegistry = BsonSerializer.SerializerRegistry;
            _documentSerializer = _serializerRegistry.GetSerializer<TEntity>();
        }
        private void Connect()
        {
            _client = new MongoClient($"mongodb://{_mongoConnection.UserName}:{_mongoConnection.Password}@{_mongoConnection.ServerURl}");
            if (_db == null)
                _db = _client.GetDatabase(_mongoConnection.Database);
        }
        protected async Task<bool> UpdateAsync(IMongoCollection<TEntity> collection,
           FilterDefinition<TEntity> filter
           , TEntity entity, params Expression<Func<TEntity, object>>[] properties)
        {
            var update = Builders<TEntity>.Update;
            var updates = new List<UpdateDefinition<TEntity>>();
            foreach (var obj in properties)
            {
                string propertyName = GetPropertyName(obj);
                if (!string.IsNullOrWhiteSpace(propertyName))
                {
                    var value = entity.GetType().GetProperty(propertyName).GetValue(entity);
                    updates.Add(update.Set(propertyName, value));
                }
            }
            await collection.UpdateOneAsync(filter, update.Combine(updates));
            return true;
        }
        protected async Task<MGPagination<TEntity>> GetPaginationAsync(IMongoCollection<TEntity> collection,
            int page, int pageSize,
            FilterDefinition<TEntity> filter = null,
            SortDefinition<TEntity> sort = null,
            ProjectionDefinition<TEntity> projection = null)
        {
            var pipeline = new List<BsonDocument>();
            if (filter != null)
                pipeline.Add(new BsonDocument { { "$match", filter.Render(_documentSerializer, _serializerRegistry) } });
            if (sort != null)
                pipeline.Add(new BsonDocument { { "$sort", sort.Render(_documentSerializer, _serializerRegistry) } });
            if (projection != null)
            {
                //var projectionBuilder = Builders<TEntity>.Projection.Expression(projection);

                //pipeline.Add(new BsonDocument { { "$project", projection.Render(_documentSerializer, _serializerRegistry).Document } });
                pipeline.Add(new BsonDocument { { "$project", projection.Render(_documentSerializer, _serializerRegistry) } });
            }

            pipeline.Add(new BsonDocument
            {
                {
                    "$group", new BsonDocument
                    {
                        { "_id", 0 },
                        { "total", new BsonDocument { { "$sum", 1 } } },
                        { "datas", new BsonDocument { { "$push", "$$ROOT" } } }
                    }
                }
            });

            pipeline.Add(new BsonDocument
            {
                {
                    "$project", new BsonDocument
                    {
                        { "_id", 0 },
                        { "total", 1 },
                        { "datas", new BsonDocument { { "$slice", new BsonArray(new object[] { "$datas", (page-1) * pageSize, pageSize }) } } }
                    }
                }
            });

            var aggregate = collection.Aggregate<BsonDocument>(pipeline);

            var data = await aggregate.FirstOrDefaultAsync();

            var result = data == null
                ? new MGPagination<TEntity>() { datas = new TEntity[0], total = 0 }
                : BsonSerializer.Deserialize<MGPagination<TEntity>>(data);

            return result;
        }
        public string ArrStrIdToArrObjectId(string[] Ids)
        {
            string temp = Ids.Aggregate("", (a, b) => a + $"ObjectId('{b}'),");
            if (string.IsNullOrWhiteSpace(temp)) return string.Empty;
            return temp.Remove(temp.LastIndexOf(','));
        }
        private string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            MemberExpression member = null;
            if (expression.Body is UnaryExpression)
            {
                UnaryExpression express = (UnaryExpression)expression.Body;
                member = (MemberExpression)express.Operand;
            }
            if (expression.Body is MemberExpression)
            {
                member = expression.Body as MemberExpression;
            }

            if (member == null) throw new InvalidOperationException("Expression must be a member expression");
            string propertyName = member.Member.Name;
            return propertyName;
        }
    }
}
