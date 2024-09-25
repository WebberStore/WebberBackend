using AutoMapper;
using Webber.Application.DTOs;
using Webber.Domain.Entities;

namespace Webber.API.Mappers;
/// <summary>
/// Mapping profile for AutoMapper.
/// </summary>

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Address Mapping
        CreateMap<AddressDto, Address>().ReverseMap();

        // Category Mappings
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.ParentCategoryName, 
                       opt => opt.MapFrom(src => src.ParentCategory.Name));
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        // Product Mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews));
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        // Product Variant Mappings
        CreateMap<ProductVariant, ProductVariantDto>().ReverseMap();
        CreateMap<CreateProductVariantDto, ProductVariant>();
        CreateMap<UpdateProductVariantDto, ProductVariant>();

        // Product Variant Option Mappings
        CreateMap<ProductVariantOption, ProductVariantOptionDto>().ReverseMap();
        // User Mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
    }
}