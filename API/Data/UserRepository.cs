
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public Task<MemberDto> GetMe { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

         public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
           var query = _context.Users
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .AsNoTracking();
            return await PagedList<MemberDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);

           
           
         
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

        public async Task<IEnumerable<MemberDto>>
        
         GetMemberAsync (string username)
        {
            return await _context.Users
            
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

           
        }

        public Task<MemberDto> GetMemberAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            throw new System.NotImplementedException();
        }

        Task<MemberDto> IUserRepository.GetMemberAsync(string username)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
