using AutoMapper;
using Elara.Application.DTOs.Category;
using Elara.Domain.Entities;

namespace Elara.Application.Mapping
{
    public partial class UserProfile
    {
        public class CategoryProfile : Profile
        {
            public CategoryProfile()
            {
                CreateMap<Category, CategoryDto>()
                    .ForMember(
                        dest => dest.ParentCategoryName,
                        opt => opt.MapFrom(src =>
                            src.ParentCategory != null
                                ? src.ParentCategory.Name
                                : null));

                CreateMap<CreateCategoryDto, Category>();

                CreateMap<UpdateCategoryDto, Category>();
            }
        }
    }
}
