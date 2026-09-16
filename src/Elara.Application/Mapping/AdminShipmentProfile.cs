using AutoMapper;
using Elara.Application.DTOs.Shipment;
using Elara.Domain.Entities;

namespace Elara.Application.Mapping
{
    public class AdminShipmentProfile : Profile
    {
        public AdminShipmentProfile()
        {
            CreateMap<Shipment, AdminShipmentListDto>()
                .ForMember(
                    dest => dest.SellerName,
                    opt => opt.MapFrom(src =>
                        src.SellerProfile.User.FullName));

            CreateMap<Shipment, AdminShipmentDetailsDto>()
                .ForMember(
                    dest => dest.SellerName,
                    opt => opt.MapFrom(src => src.SellerProfile.User.FullName));

            CreateMap<ShipmentItem, AdminShipmentItemDto>()
                .ForMember(
                    dest => dest.ProductId,
                    opt => opt.MapFrom(src => src.OrderItem.ProductId))
                .ForMember(
                    dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.OrderItem.Product.Name));
        }
    }
}
