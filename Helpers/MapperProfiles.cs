using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ServerApp.DTO;
using ServerApp.Models;

namespace ServerApp.Helpers
{
    //Profile sınıfından implemente alması gerekiyor
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            //ctor içerisinde mapleyeceğim objeleri tanıtmam gerekiyor
            CreateMap<User, UserForListDTO>();
            CreateMap<User, UserForDetailsDTO>();
        }
    }
}