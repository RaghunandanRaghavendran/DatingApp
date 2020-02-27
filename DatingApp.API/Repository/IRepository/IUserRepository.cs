using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Model;

namespace DatingApp.API.Repository.IRepository
{
    public interface IUserRepository
    {
         void Add<T>(T entity) where T:class;
         void Deleter<T>(T entity) where T:class;
         Task<bool> SaveChanges();
         Task<IEnumerable<User>> GetUsers();
         Task<User> GetUser(int id);

    }
}