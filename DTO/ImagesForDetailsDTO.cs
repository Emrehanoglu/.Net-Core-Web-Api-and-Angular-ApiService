using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.DTO
{
    public class ImagesForDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsProfile { get; set; } //birden fazla resim vardır ama bir tanesi profil resmidir
    }
}