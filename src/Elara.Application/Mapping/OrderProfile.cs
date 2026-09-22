using AutoMapper;
using Elara.Application.DTOs.Order;
using Elara.Domain.Entities;
using Elara.Domain.Enums;

namespace Elara.Application.Mapping
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, AdminOrderListDto>()
                .ForMember(
                    dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.User != null? src.User.FullName  : src.GuestFullName))
                .ForMember(
                    dest => dest.CustomerEmail,
                    opt => opt.MapFrom(src => src.User != null? src.User.Email  : src.GuestEmail))
                .ForMember(
                    dest => dest.ItemsCount,
                    opt => opt.MapFrom(src => src.Items.Count))
                .ForMember(
                    dest => dest.ShipmentsCount,
                    opt => opt.MapFrom(src => src.Shipments.Count));

            CreateMap<Order, AdminOrderDetailsDto>()
                .ForMember(
                    dest => dest.Customer,
                    opt => opt.MapFrom(src => new CustomerSummaryDto
                    {
                        Id = src.UserId,
                        Name = src.User != null ? src.User.FullName : src.GuestFullName,
                        Email = src.User != null ? src.User.Email : src.GuestEmail,
                        PhoneNumber = src.User != null ? src.User.PhoneNumber : src.GuestPhoneNumber
                    }))
                .ForMember(
                    dest => dest.ShippingAddress,
                    opt => opt.MapFrom(src => new ShippingAddressDto
                    {
                        FullName = src.ShippingFullName,
                        Phone = src.ShippingPhone,
                        Street = src.ShippingStreet,
                        City = src.ShippingCity,
                        State = src.ShippingState,
                        PostalCode = src.ShippingPostalCode,
                        Country = src.ShippingCountry
                    }));

            CreateMap<OrderItem, AdminOrderItemDto>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl,
                    opt => opt.MapFrom(src => src.Product.Images.OrderBy(i => i.DisplayOrder) .Select(i => i.ImageUrl) .FirstOrDefault()))
                .ForMember(dest => dest.SellerId,
                    opt => opt.MapFrom(src => src.Product.SellerProfileId))
                .ForMember(dest => dest.SellerName,
                    opt => opt.MapFrom(src => src.Product.SellerProfile.User.FullName));

            CreateMap<OrderStatusHistory, OrderStatusHistoryDto>();

            CreateMap<ShippingMethod, ShippingMethodSummaryDto>();

            CreateMap<PromoCode, PromoCodeSummaryDto>();

            CreateMap<Payment, PaymentSummaryDto>();

            CreateMap<Shipment, AdminShipmentSummaryDto>()
                .ForMember(
                    dest => dest.SellerId,
                    opt => opt.MapFrom(src => src.SellerProfileId))
                .ForMember(
                    dest => dest.SellerName,
                    opt => opt.MapFrom(src => src.SellerProfile.User.FullName))
                .ForMember(
                    dest => dest.ItemsCount,
                    opt => opt.MapFrom(src => src.Items.Count));

            CreateMap<Order, SellerOrderDetailsDto>();

            CreateMap<OrderItem, SellerOrderItemDto>()
                .ForMember(d => d.OrderItemId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.QuantityShipped, o => o.Ignore())
                .ForMember(d => d.QuantityRemaining, o => o.Ignore());

            CreateMap<Order, CustomerOrderListDto>()
                .ForMember(
                    dest => dest.ItemCount,
                    opt => opt.MapFrom(src => src.Items.Count));

            CreateMap<Order, CustomerOrderDetailsDto>();

            CreateMap<OrderItem, CustomerOrderItemDto>()
                .ForMember(
                    dest => dest.OrderItemId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(
                    dest => dest.ProductId,
                    opt => opt.MapFrom(src => src.ProductId))
                .ForMember(
                    dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<OrderStatusHistory, CustomerOrderStatusHistoryDto>()
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => Enum.Parse<OrderStatus>(src.Status)));
        }
    }
    
}
