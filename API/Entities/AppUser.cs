
using API.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Entites
{
    public class AppUser : IdentityUser<int>
    {
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Intrests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<UserLike> LikedByUsers { get; set; } // Users koj sto go imaat lajknato najaveniot user
        public ICollection<UserLike> LikedUsers { get; set; } // Users koj sto se lajknati od najaveniot user
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesRecived { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }


    }
}



