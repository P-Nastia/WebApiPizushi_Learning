using AutoMapper;
using WebApiPizushi.Data.Entities.Identity;
using WebApiPizushi.Models.Seeder;

namespace WebApiPizushi.Mapper;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<SeederUserModel, UserEntity>()
            .ForMember(x=>x.UserName,opt=>opt.MapFrom(x=>x.Email));
    }
}
