using AutoMapper;
using Domain.Entities;
using Core.Models.Seeder;

namespace Core.Mapper;

public class IngredientMapper : Profile
{
    public IngredientMapper()
    {
        CreateMap<SeederIngredientModel, IngredientEntity>();
    }
}
