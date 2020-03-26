using System;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Model;
using DatingApp.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _dbContext;
        public AuthRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<User> Login(string username, string password)
        {
            User user = await _dbContext.Users.Include(p=>p.Photos).FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
                return null;
            else
            {
                if (!VerifyPasswordHash(user, password))
                {
                    return null;
                }
                else
                {
                    return user;
                }
            }
        }

        private bool VerifyPasswordHash(User user, string password)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt))
            {
                var computedpasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedpasswordHash.Length; i++)
                {
                    if (computedpasswordHash[i] != user.PasswordHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _dbContext.Users.AnyAsync(x=>x.UserName == username))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}