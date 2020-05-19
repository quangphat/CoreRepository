using CoreObject;
using CoreObject.Mongo;
using CoreRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreRepository.Classes.Tests
{
    public class AccountRepositoryTest : IAccountRepository
    {
        private readonly List<MongoAccount> _account = new List<MongoAccount> {
            new MongoAccount{
                PersonId = "1",
                Email = "quangphat@gmail.com",
                Password = "number8",
                ProjectId="greencode",
                DisplayName ="Quang Phát",
                Avatar ="",
                Role="manager",
                WorkAs = "greencode",
                IsComfirmEmail = true
            },
            new MongoAccount
            {
                PersonId ="2",
                Email = "mongnhan@gmail.com",
                Password = "mongnhan"
            }
        };

        public Task<bool> ChangePass(string newPass, string salt, string personId)
        {
            throw new NotImplementedException();
        }

        public Task<string> Create(MongoAccount account)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<MongoAccount> Get(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<MongoAccount> GetByEmail(string email)
        {
            return _account.Where(p => p.Email == email).FirstOrDefault();
        }

        public Task<MongoAccount> GetByProfileName(string profileName)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetPersonSkill(string personId)
        {
            throw new NotImplementedException();
        }

        public Task<MongoAccount> Login(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveJobSkill(string[] skills, string personId)
        {
            throw new NotImplementedException();
        }

        public Task<List<MongoAccount>> Search(string searchStr)
        {
            throw new NotImplementedException();
        }

        public Task<Pagination<MongoAccount>> Search(AccountSearch search, string projectId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(MongoAccount account)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAvatar(string fileName, string accountId)
        {
            throw new NotImplementedException();
        }
    }
}
