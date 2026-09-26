using AutoMapper;
using Elara.Application.DTOs;
using Elara.Domain.Entities;

namespace Elara.Application.Mapping
{
    public class PromoCodeProfile : Profile
    {
        public PromoCodeProfile()
        {
            CreateMap<CreatePromoCodeDto, PromoCode>();
            CreateMap<UpdatePromoCodeDto, PromoCode>();
            CreateMap<PromoCode, PromoCodeDto>();
        }
    }
}
