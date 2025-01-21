using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IMessageRepository messageRepository;

        public MessagesController(IUserRepository userRepository, IMapper mapper,
        IMessageRepository messageRepository)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.messageRepository = messageRepository;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
            {
                return BadRequest("You cannot send messages to yourself");
            }

            var sender = await userRepository.GetUserByUsernameAsync(username);

            var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null)
            {
                return NotFound();
            }

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())
            {
                return Ok(mapper.Map<MessageDto>(message));
            }

            return BadRequest("Failed to send message");

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {

            messageParams.Username = User.GetUsername();

            var messages = await messageRepository.GetMessagesForUser(messageParams);

            // Add pagination headers
            Response.AddPaginationHeader
            (

                messages.CurrentPage,
                messages.TotalPages,
                messages.PageSize,
                messages.TotalCount
            );

            return Ok(messages);


        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {

            var currentUsername = User.GetUsername();

            if (string.IsNullOrEmpty(currentUsername))
            {
                return Unauthorized("User not authorized.");
            }

            var messages = await messageRepository.GetMessageThread(currentUsername, username);

            return Ok(messages);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await messageRepository.GetMessage(id);

            if (message.Sender.UserName != username && message.Recipient.UserName != username) {
                return Unauthorized();
            }

            if (message.Sender.UserName == username) {
                message.SenderDeleted = true;
            }

            if (message.Recipient.UserName == username) {
                message.RecipientDeleted = true;
            }

            if (message.SenderDeleted && message.RecipientDeleted) {
                messageRepository.DeleteMessage(message);
            }

            if (await messageRepository.SaveAllAsync()) {
                return Ok();
            }

            return BadRequest("Problem deleting the message");
        }

    }
}
