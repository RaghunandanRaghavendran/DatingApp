using System.Linq;
using AutoMapper;
using DatingApp.API.DataTransferObjects;
using DatingApp.API.Model;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,UsersForList>().ForMember(dest=>dest.PhotoUrl, 
                                           opt=> opt.MapFrom(src=>src.Photos.FirstOrDefault(x=>x.IsProfilePicture).Url))
                                           .ForMember(dest=>dest.Age, 
                                           opt=> opt.MapFrom(src=>src.DateOfBirth.CalculateAge()));

            CreateMap<User,UserForDetails>().ForMember(dest=>dest.PhotoUrl, 
                                           opt=> opt.MapFrom(src=>src.Photos.FirstOrDefault(x=>x.IsProfilePicture).Url))
                                           .ForMember(dest=>dest.Age, 
                                           opt=> opt.MapFrom(src=>src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotosForDetailedDTO>();
            CreateMap<UserForUpdate,User>();
            CreateMap<Photo,PhotoForReturnDto>();
            CreateMap<PhotoForCreationDTO, Photo>();
        }
    }
}