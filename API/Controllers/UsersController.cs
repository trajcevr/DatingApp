using System.Runtime.CompilerServices;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entites;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            userParams.CurrentUsername = User.GetUsername();

            if(string.IsNullOrEmpty(userParams.Gender)) {
                  userParams.Gender = user.Gender == "male" ? "female" : "male"; // Default preference
            }

            var users = await _userRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }

        ///[Authorize(Roles = "Member")]
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _userRepository.GetMemberAsync(username);

            return user;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Faild request for update");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync())
            {
                // return CreatedAtRoute("GetUsers",  _mapper.Map<PhotoDto>(photo));
                return CreatedAtAction("GetUsers", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return NotFound("User not found");

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
            if (photo == null) return NotFound("Photo not found");

            if (photo.IsMain) return BadRequest("This photo is already you main");

            var currentMainPhoto = user.Photos.FirstOrDefault(p => p.IsMain);
            if (currentMainPhoto != null)
            {
                currentMainPhoto.IsMain = false;
            }

            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to set the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return NotFound("User not found");

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
            if (photo == null) return NotFound("Photo not found");

            // If the photo is the main photo, you might want to handle this case separately.
            if (photo.IsMain)
            {
                return BadRequest("You cannot delete your main photo");
            }

            // Remove the photo from the user's photos collection
            user.Photos.Remove(photo);

            // If you are using a photo service to store the photos (like cloud storage),
            // make sure to delete the photo from there as well.
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);

            if (result.Error != null) return BadRequest(result.Error.Message);

            if (await _userRepository.SaveAllAsync())
            {
                return NoContent(); // Successfully deleted
            }

            return BadRequest("Failed to delete the photo");
        }

    }
}