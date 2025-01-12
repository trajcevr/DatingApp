using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entites;
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

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
             var user = await _context.Users
                .Where(u => u.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider) // AutoMap to MemberDto
                .SingleOrDefaultAsync();

            return user;
            
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            var users = await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider) // AutoMap to MemberDto
                .ToListAsync();

            return users;
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {  
            return await _context.Users
                .Include(u => u.Photos) // Include related data if needed
                .SingleOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Photos) // Include related data if needed
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            // Save changes and return true if at least one record is affected
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            // Mark the user entity as modified
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
