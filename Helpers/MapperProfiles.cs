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
            CreateMap<User, UserForListDTO>().ForMember(dest => dest.ImageUrl, opt => 
                opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsProfile).Name))
                .ForMember(dest => dest.Age, opt => 
                opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<User, UserForDetailsDTO>().ForMember(dest => dest.ProfileImageUrl, opt => 
                opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsProfile).Name))
                .ForMember(dest => dest.Age, opt => 
                opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                .ForMember(dest => dest.Images, opt => 
                opt.MapFrom(src => src.Images.Where(i => !i.IsProfile).ToList()));
            CreateMap<Image, ImagesForDetailsDTO>();
            CreateMap<User, UserForUpdateDTO>().ReverseMap();
        }
    }
}