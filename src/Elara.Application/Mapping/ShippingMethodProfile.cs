using AutoMapper;
using Elara.Application.DTOs.ShippingMethods;
using Elara.Domain.Entities;

namespace Elara.Application.Mapping
{
    public partial class UserProfile
    {
        public class ShippingMethodProfile : Profile
        {
            public ShippingMethodProfile()
            {
                CreateMap<ShippingMethod, ShippingMethodDto>();

                CreateMap<CreateShippingMethodDto, ShippingMethod>();

                CreateMap<UpdateShippingMethodDto, ShippingMethod>();
                
                CreateMap<UpdateShippingMethodStatusDto, ShippingMethod>();
            }
        }
    }
}
