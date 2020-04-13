using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
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

        public async Task<Like> GetLike(int userId, int recepientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recepientId);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(x => x.Id == id);
            return photo;

        }

        public async Task<Photo> GetProfilePictureForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).
                                        FirstOrDefaultAsync(p => p.IsProfilePicture);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(x => x.Photos).Where(u => u.Id == id).FirstOrDefaultAsync();
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            // Previous implementation before the paginations
            //var users = await _context.Users.Include(x=>x.Photos).ToListAsync();
            //return users;

            var users = _context.Users.Include(x => x.Photos).OrderByDescending(x => x.LastActive).AsQueryable();

            users = users.Where(u => u.Id != userParams.UserID);

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserID, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserID, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }
            // Show the opposite gender only for Matches and not for Liked Lists
            if(userParams.Likers == false && userParams.Likees == false)
            {
                users = users.Where(u => u.Gender == userParams.Gender);
            }

            if (userParams.MinAge > 0 && userParams.MaxAge > 0)
            {
                var minage = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxage = DateTime.Today.AddYears(-userParams.MinAge);
                users = users.Where(u => u.DateOfBirth >= minage && u.DateOfBirth <= maxage);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.CreationDate);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var users = await _context.Users.
                               Include(x => x.Likers).
                               Include(x => x.Likees).
                               FirstOrDefaultAsync(u => u.Id == id);

            if (likers)
            {
                return users.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return users.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }

        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}