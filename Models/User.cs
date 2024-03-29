using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ServerApp.Models
{
    //buradaki <int> ile id bilgisi 1,2,3 şeklinde artacak
    public class User:IdentityUser<int>
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Introduction { get; set; }
        public string Hobbies { get; set; }
        public ICollection<Image> Images { get; set; }
        public ICollection<UserToUser> Following { get; set; } //takip ettiği kişilerin listesi
        public ICollection<UserToUser> Followers { get; set; } //takipçilerinin listesi
    }
}