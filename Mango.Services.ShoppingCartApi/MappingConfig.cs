using AutoMapper;
using Mango.Services.ShoppingCartApi.Models;
using Mango.Services.ShoppingCartApi.Models.Dtos;

namespace Mango.Services.ShoppingCartApi;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<ProductDto, Product>().ReverseMap();
            config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
            config.CreateMap<CartDetailDto, CartDetail>().ReverseMap();
            config.CreateMap<CartDto, Cart>().ReverseMap();
        });

        return mappingConfig;
    }
}