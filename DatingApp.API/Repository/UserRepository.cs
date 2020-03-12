using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Model;
using DatingApp.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
             _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user =  await _context.Users.Include(x=>x.Photos).Where(u=>u.Id == id).FirstOrDefaultAsync();
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users.Include(x=>x.Photos).ToListAsync();
            return users;

        }
        
        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() >0;
        }
    }
}