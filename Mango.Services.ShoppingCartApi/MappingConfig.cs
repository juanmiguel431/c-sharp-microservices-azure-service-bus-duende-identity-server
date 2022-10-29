using AutoMapper;

namespace Mango.Services.ShoppingCartApi;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            // config.CreateMap<ProductDto, Product>();
            // config.CreateMap<Product, ProductDto>();
        });

        return mappingConfig;
    }
}