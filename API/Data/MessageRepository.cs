using System;
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
    public class MessageRepository : IMessageRepository
    {
        public readonly DataContext context;
        private readonly IMapper mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public void AddGroup(Group group)
        {
            context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await context.Groups
                .Include(c => c.Connections)
                .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await context.Messages
            .Include(u => u.Sender)
            .Include(u => u.Recipient)
            .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            //.Where(m => m.RecipientUsername == messageParams.Username || m.SenderUsername == messageParams.Username)
            var query = context.Messages
                .OrderByDescending(m => m.MessageSent)
                .AsQueryable();

            if (messageParams.Container == "Inbox")
            {
                query = query.Where(m => m.RecipientUsername == messageParams.Username && !m.RecipientDeleted );
            }
            else if (messageParams.Container == "Outbox")
            {
                query = query.Where(m => m.SenderUsername == messageParams.Username && !m.SenderDeleted);
            }
            else if (messageParams.Container == "Unread")
            {
                query = query.Where(m => m.RecipientUsername == messageParams.Username && !m.RecipientDeleted && !m.DateRead.HasValue);
            }

            var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }


        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await context.Messages
                        .Include(u => u.Sender).ThenInclude(p => p.Photos)
                        .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                        .Where(m =>
                            (m.SenderUsername == currentUsername && !m.RecipientDeleted && m.RecipientUsername == recipientUsername) ||
                            (m.SenderUsername == recipientUsername && m.RecipientUsername == currentUsername && !m.SenderDeleted))
                        .OrderBy(m => m.MessageSent)
                        .ToListAsync();

            var unreadMessages = messages.Where(m=> m.DateRead == null && m.Recipient.UserName == currentUsername).ToList();
            
            if (unreadMessages.Any()) 
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }

                await context.SaveChangesAsync();
            }
            var messageDtos = mapper.Map<IEnumerable<MessageDto>>(messages);

            return messageDtos;
        }

        public void RemoveConnection(Connection connection)
        {
            context.Connections.Remove(connection);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}