using AutoMapper;
using Core.Models.Product;
using Domain.Entities;

namespace Core.Mapper;

public class CartMapper : Profile
{
    public CartMapper()
    {
        CreateMap<ProductImageEntity, ProductImageModel>();
    }
}
