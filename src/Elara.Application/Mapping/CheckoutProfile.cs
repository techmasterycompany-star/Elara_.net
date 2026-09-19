using AutoMapper;
using Elara.Application.DTOs.Checkout;
using Elara.Domain.Entities;

namespace Elara.Application.Mapping
{
    public class CheckoutProfile : Profile
    {
        public CheckoutProfile()
        {
            CreateMap<ShippingMethod, ShippingMethodDto>()
                .ForMember(dest => dest.ShippingMethodId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
