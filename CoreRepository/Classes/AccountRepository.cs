using CoreObject;
using CoreObject.Mongo;
using CoreRepository.Infrastructures;
using CoreRepository.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreRepository.Classes
{
    public class AccountRepository : MongoRepositoryBase<MongoAccount>, IAccountRepository
    {
        IMongoCollection<MongoAccount> _collection;
        FilterDefinition<MongoAccount> _filter;
        public AccountRepository(IOptions<MongoConnectionConfig> setting) : base(setting)
        {
            _collection = _db.GetCollection<MongoAccount>("Account");
            _filter = FilterDefinition<MongoAccount>.Empty;
        }
        public async Task<string> Create(MongoAccount account)
        {
            try
            {
                await _collection.InsertOneAsync(account);
                return account.PersonId;
            }
            catch(Exception e)
            {
                return string.Empty;
            }
        }
        public async Task<MongoAccount> Get(string id)
        {
            _filter = Builders<MongoAccount>.Filter.Eq(p => p.PersonId, id);
            return await _collection.Find(_filter).FirstOrDefaultAsync();
        }
        public async Task<List<string>> GetPersonSkill(string personId)
        {
            _filter = Builders<MongoAccount>.Filter.Eq(p => p.PersonId, personId);
            var result = await _collection.Find(_filter).FirstOrDefaultAsync();
            return result.SkillTags.ToList();
        }
        public async Task<bool> SaveJobSkill(string[] skills, string personId)
        {
            var _filter = Builders<MongoAccount>.Filter.Eq(p => p.PersonId, personId);
            var update = Builders<MongoAccount>.Update
                            .Set(s => s.SkillTags, skills);

            await _collection.UpdateOneAsync(_filter, update);
            return true;

        }
        public async Task<bool> ChangePass(string newPass, string salt, string personId)
        {
            var _filter = Builders<MongoAccount>.Filter.Eq(p => p.PersonId, personId);
            var update = Builders<MongoAccount>.Update
                            .Set(s => s.Password, newPass)
                            .Set(s => s.Salt, salt);

            await _collection.UpdateOneAsync(_filter, update);
            return true;
        }
        public async Task<bool> Update(MongoAccount account)
        {
            var _filter = Builders<MongoAccount>.Filter.Eq(p => p.PersonId, account.PersonId);
            var update = Builders<MongoAccount>.Update
                           .Set(s => s.DisplayName, account.DisplayName)
                            .Set(s => s.ProfileName, account.ProfileName)
                            .Set(s => s.Avatar, account.Avatar)
                            .Set(s => s.WorkAs, account.WorkAs)
                            .Set(s => s.Company, account.Company)
                            .Set(s => s.Rate, account.Rate)
                            .Set(s => s.Firstname, account.Firstname)
                            .Set(s => s.Lastname, account.Lastname)
                            .Set(s => s.Email, account.Email)
                            .Set(s => s.Password, account.Password)
                            .Set(s => s.Role, account.Role)
                            .Set(s => s.Scopes, account.Scopes)
                            .Set(s => s.ProjectId, account.ProjectId)
                            .Set(s => s.SkillTags, account.SkillTags)
                            .Set(s => s.Salt, account.Salt);
            try
            {
                await _collection.UpdateOneAsync(_filter, update);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UpdateAvatar(string fileName, string accountId)
        {
            var _filter = Builders<MongoAccount>.Filter.Eq(p => p.PersonId, accountId);
            var update = Builders<MongoAccount>.Update
                            .Set(s => s.Avatar, fileName);
            try
            {
                await _collection.UpdateOneAsync(_filter, update);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> Delete(string id)
        {
            try
            {
                _filter = Builders<MongoAccount>.Filter.Eq(p => p.PersonId, id);
                await _collection.DeleteOneAsync(_filter);
                return true;
            }
            catch { return false; }
        }
        public async Task<List<MongoAccount>> Search(string searchStr)
        {
            return null;
        }
        public async Task<MongoAccount> Login(string email)
        {
            var match = new BsonDocument { { "Email", email }, { "IsBanned", false } };
            return await _collection.Find(match).FirstOrDefaultAsync();
        }
        public async Task<MongoAccount> GetByEmail(string email)
        {
            var match = new BsonDocument { { "Email", email } };
            return await _collection.Find(match).FirstOrDefaultAsync();
        }
        public async Task<Pagination<MongoAccount>> Search(AccountSearch search, string projectId)
        {
            var wrapperQuery = new List<FilterDefinition<MongoAccount>>();
            if (!string.IsNullOrWhiteSpace(search.searchStr))
            {
                wrapperQuery.Add(Builders<MongoAccount>.Filter.Regex(p => p.DisplayName, new BsonRegularExpression($"/{search.searchStr}/i")));
                wrapperQuery.Add(Builders<MongoAccount>.Filter.Regex(p => p.Firstname, new BsonRegularExpression($"/{search.searchStr}/i")));
                wrapperQuery.Add(Builders<MongoAccount>.Filter.Regex(p => p.Lastname, new BsonRegularExpression($"/{search.searchStr}/i")));
            }
            if (!string.IsNullOrWhiteSpace(search.role))
            {
                wrapperQuery.Add($@"{{Role':'{search.role}'}}");
            }
            wrapperQuery.Add(Builders<MongoAccount>.Filter.Eq(p => p.ProjectId, projectId));
            _filter = Builders<MongoAccount>.Filter.Or(wrapperQuery.ToArray());
            var sortBuilder = Builders<MongoAccount>.Sort;
            SortDefinition<MongoAccount> sort = null;
            var response = await GetPaginationAsync(_collection, search.page, search.limit, _filter, sort);
            Pagination<MongoAccount> result = new Pagination<MongoAccount>
            {
                TotalRecord = response.total,
                Datas = response.datas.ToList()
            };
            return result;
        }

        public async Task<MongoAccount> GetByProfileName(string profileName)
        {
            PipelineDefinition<MongoAccount, MongoAccount> pipeline = new BsonDocument[]
           {
                 new BsonDocument { { "$match",new BsonDocument{ {"ProfileName",profileName } } }

                 },
                 new BsonDocument{ { "$project",new BsonDocument {
                     {"_id", 1 },
                     {"DisplayName", 1 },
                     {"ProfileName", 1 },
                     {"Avatar",1 },
                     {"Email", 1 },
                     {"SkillTags", 1 }
                 } } },
                 new BsonDocument { { "$limit", 1 } }
           };
            var result = await _collection.Aggregate(pipeline).FirstOrDefaultAsync();
            return result;
        }
    }
}
