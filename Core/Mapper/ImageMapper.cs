﻿using AutoMapper;
using Core.Models.Product;
using Domain.Entities;

namespace Core.Mapper;

public class ImageMapper : Profile
{
    public ImageMapper()
    {
        CreateMap<ProductImageEntity, ProductImageModel>();
    }
}
