using AutoMapper;
using Domain.Entities.Identity;
using WebApiPizushi.Models.Account;
using WebApiPizushi.Models.Seeder;

namespace WebApiPizushi.Mapper;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<SeederUserModel, UserEntity>()
            .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.Email));

        CreateMap<RegisterModel, UserEntity>()
            .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.Email))
            .ForMember(x => x.Image, opt => opt.Ignore());
    }
}
