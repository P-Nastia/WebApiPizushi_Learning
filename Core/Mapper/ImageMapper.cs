using AutoMapper;
using Core.Models.Product;
using Domain.Entities;

namespace Core.Mapperl
{
    public class ImageMapper : Profile
    {
        public ImageMapper()
        {
            CreateMap<ProductImageEntity, ProductImageModel>();
            CreateMap<CreateImageModel, ProductImageEntity>()
                .ForMember(x => x.Name, opt => opt.Ignore());
        }
    }
}
