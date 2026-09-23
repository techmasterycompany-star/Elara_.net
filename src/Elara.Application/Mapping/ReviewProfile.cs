using AutoMapper;
using Elara.Application.DTOs.Review;
using Elara.Domain.Entities;

namespace Elara.Application.Mapping
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, ReviewResponseDto>()
                .ForMember(
                    dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User.FullName));
        }
    }
}