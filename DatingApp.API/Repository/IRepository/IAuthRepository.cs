using System.Threading.Tasks;
using DatingApp.API.Model;

namespace DatingApp.API.Repository.IRepository
{
    public interface IAuthRepository
    {
         Task<User> Register(User user , string password);
         Task<User> Login (string usename, string password);

         Task<bool> UserExists(string username);
         
    }
}