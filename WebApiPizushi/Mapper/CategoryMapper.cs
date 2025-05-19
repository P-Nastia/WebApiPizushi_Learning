using AutoMapper;
using WebApiPizushi.Data.Entities;
using WebApiPizushi.Models.Category;
using WebApiPizushi.Models.Seeder;

namespace WebApiPizushi.Mapper;

public class CategoryMapper : Profile
{
    public CategoryMapper()
    {
        CreateMap<SeederCategoryModel, CategoryEntity>();
        CreateMap<CategoryEntity, CategoryItemModel>();
        CreateMap<CategoryCreateModel, CategoryEntity>()
            .ForMember(x => x.Image, opt => opt.Ignore());
        CreateMap<CategoryEditModel, CategoryEntity>();
        CreateMap<CategoryEntity, CategoryEditModel>()
            .ForMember(x => x.ImageFile, opt => opt.Ignore());
    }
}
