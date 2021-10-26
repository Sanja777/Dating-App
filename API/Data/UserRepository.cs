
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public Task<MemberDto> GetMe { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public UserRepository(DataContext context)
        {
            _context = context;
        }

         public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
           .Include(p => p.Photos)
           .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }

       

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
            .Where(x=>x.UserName==username)
            .Select(user => new MemberDto
            {
                Id=user.Id,
                UserName = user.UserName

            }).SingleOrDefaultAsync();
        }

        public Task<MemberDto> GetMemberAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
