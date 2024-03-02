using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Data
{
    public class SocialRepository : ISocialRepository
    {
        private readonly SocialContext _context;
        public SocialRepository(SocialContext context)
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

        public async Task<bool> SaveChanges()
        {
            //SaveChangesAsync normalde int bir değer döner. 
            //0 dan büyük bir değer dönmüşse true olarak değerlendirip true döneceğim.
            return await _context.SaveChangesAsync() > 0; 
        }

        public async Task<User> GetUser(int id)
        {
            var user =  await _context.Users.Include(i=>i.Images).FirstOrDefaultAsync(x=>x.Id==id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users.Include(i=>i.Images).ToListAsync();
            return users;
        }

        public async Task<bool> IsAlreadyFollowed(int followerUserId, int userId)
        {
            return await _context.UserToUser
            .AnyAsync(i => i.UserId == userId && i.FollowerId == followerUserId);
        }
    }
}