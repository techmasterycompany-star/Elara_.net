using AutoMapper;
using Elara.Application.DTOs.Inventory;
using Elara.Domain.Entities;

namespace Elara.Application.Mapping
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile()
        {
            CreateMap<Product, InventoryProductDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<Product, ProductStockDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
