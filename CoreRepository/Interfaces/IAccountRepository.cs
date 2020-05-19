using CoreObject;
using CoreObject.Mongo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreRepository.Interfaces
{
    public interface IAccountRepository
    {
        Task<string> Create(MongoAccount account);
        Task<MongoAccount> Get(string id);
        Task<MongoAccount> GetByProfileName(string profileName);
        Task<bool> Update(MongoAccount account);
        Task<bool> ChangePass(string newPass, string salt, string personId);
        Task<bool> UpdateAvatar(string fileName, string accountId);
        Task<bool> Delete(string id);
        Task<List<MongoAccount>> Search(string searchStr);
        Task<Pagination<MongoAccount>> Search(AccountSearch search, string projectId);
        Task<MongoAccount> Login(string email);
        Task<MongoAccount> GetByEmail(string email);
        Task<bool> SaveJobSkill(string[] skills, string personId);
        Task<List<string>> GetPersonSkill(string personId);
    }
}
