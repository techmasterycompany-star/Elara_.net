using AutoMapper;
using Elara.Application.DTOs.Shipment;
using Elara.Domain.Entities;

namespace Elara.Application.Mapping
{
    public class ShipmentProfile : Profile
    {
        public ShipmentProfile()
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

            CreateMap<Shipment, SellerShipmentListDto>();

            CreateMap<Shipment, SellerShipmentDetailsDto>();

            CreateMap<ShipmentItem, SellerShipmentItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.OrderItem.ProductId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.OrderItem.Product.Name));
        }
    }
}
