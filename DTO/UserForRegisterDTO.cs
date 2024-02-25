using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.DTO
{
    public class UserForRegisterDTO
    {
        [Required(ErrorMessage ="Name Alanı Gereklidir")]
        [StringLength(50,MinimumLength=10)] //max 50, min 10 karakter olmalı
        public string Name { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress] //email formatında olmalı
        public string Email { get; set; }
        public string Gender { get; set; }
        [Required]
        public string Password { get; set; }
    }
}