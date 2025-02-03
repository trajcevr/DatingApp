using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using API.Entites;

namespace API.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }  
        public string SenderPhotoUrl { get; set; }  
        public int RecipientId { get; set; }

        public string ReipientUsername { get; set; }
        public string RecipientPhotoUrl { get; set; }
        public string Content { get; set; } 
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } 

        [JsonIgnore]
        public bool SenderDeleted { get; set; }
        
        [JsonIgnore]
        public bool RecipientDeleted { get; set; }
    }
}