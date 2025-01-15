using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entites;
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

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            // Exclude the logged-in user
            if (!string.IsNullOrEmpty(userParams.CurrentUsername))
            {
                query = query.Where(user => user.UserName != userParams.CurrentUsername);
            }

            // Filter by gender
            if (!string.IsNullOrEmpty(userParams.Gender))
            {
                query = query.Where(user => user.Gender == userParams.Gender);
            }

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };
            
            // Project to DTO and paginate
            var memberQuery = query
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            return await PagedList<MemberDto>.CreateAsync(memberQuery, userParams.PageNumber, userParams.PageSize);
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
