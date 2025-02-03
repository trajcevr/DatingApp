using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [Authorize]

    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork unitOfWork;

        public LikesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("{username}")]

        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You already like this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id,
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await unitOfWork.Complete())
            {
                // Return JSON response
                return Ok(new { message = "You have liked the user" });
            }

            return BadRequest(new { message = "Failed to like the user" });
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            if (string.IsNullOrWhiteSpace(likesParams.Predicate))
            {
                return BadRequest("Predicate is required.");
            }

            likesParams.UserId = User.GetUserId();
            var likes = await unitOfWork.LikesRepository.GetUserLikes(likesParams);

            if (likes == null)
            {
                return NotFound("No likes found for the given criteria.");
            }

            Response.AddPaginationHeader(likes.CurrentPage, likes.PageSize, likes.TotalCount, likes.TotalPages);
            return Ok(likes);
        }

    }
}