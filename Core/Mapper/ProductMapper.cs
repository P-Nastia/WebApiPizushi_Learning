using AutoMapper;
using Domain.Entities;
using Core.Models.Seeder;
using Core.Models.Product;

namespace Core.Mapper;

public class ProductMapper : Profile
{
    public ProductMapper()
    {
        CreateMap<ProductEntity, ProductItemModel>()
            .ForMember(x => x.ProductIngredients, opt => opt.MapFrom(
                x => x.ProductIngredients.Select(x => x.Ingredient)))
            .ForMember(x=>x.ProductImages,opt=>opt.MapFrom(x=>x.ProductImages.OrderBy(i=>i.Priority)));
        CreateMap<ProductCreateModel, ProductEntity>()
            .ForMember(x => x.ProductImages, opt => opt.Ignore())
            .ForMember(x => x.ProductIngredients, opt => opt.Ignore());
        CreateMap<ProductEditModel, ProductEntity>()
            .ForMember(x => x.ProductImages, opt => opt.Ignore())
            .ForMember(x => x.ProductIngredients, opt => opt.Ignore())
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Category, opt => opt.Ignore())
            .ForMember(x => x.ProductSize, opt => opt.Ignore());
    }
}
