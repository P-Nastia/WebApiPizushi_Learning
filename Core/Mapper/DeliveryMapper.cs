using AutoMapper;
using Core.Models.Delivery;
using Core.Models.Order;
using Domain.Entities;
using Domain.Entities.Delivery;

namespace Core.Mapper;

public class DeliveryMapper : Profile
{
    public DeliveryMapper()
    {
        CreateMap<CityEntity, CityModel>();
        CreateMap<PostDepartmentEntity, PostDepartmentModel>();
        CreateMap<PaymentTypeEntity, PaymentTypeModel>();
    }
}
