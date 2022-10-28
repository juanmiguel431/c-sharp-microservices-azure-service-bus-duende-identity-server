using Mango.Services.ProductApi.Models.Dtos;
using Mango.Services.ProductApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductApi.Controllers;

[Route("api/products")]
public class ProductApiController : Controller
{
    private readonly ResponseDto _response;
    private readonly IProductRepository _productRepository;

    public ProductApiController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
        _response = new ResponseDto();
    }

    [HttpGet]
    public async Task<ResponseDto> Get()
    {
        try
        {
            IEnumerable<ProductDto> productDtos = await _productRepository.GetProducts();
            _response.Result = productDtos;
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.ToString() };
        }
        return _response;
    }

    [HttpGet, Route("{id}")]
    public async Task<ResponseDto> Get(int id)
    {
        try
        {
            var productDto = await _productRepository.GetProductById(id);
            _response.Result = productDto;
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.ToString() };
        }
        return _response;
    }

    [Authorize, HttpPost]
    public async Task<ResponseDto> Post([FromBody] ProductDto productDto)
    {
        try
        {
            var model = await _productRepository.CreateUpdateProduct(productDto);
            _response.Result = model;
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.ToString() };
        }
        return _response;
    }

    [Authorize, HttpPut]
    public async Task<ResponseDto> Put([FromBody] ProductDto productDto)
    {
        try
        {
            var model = await _productRepository.CreateUpdateProduct(productDto);
            _response.Result = model;
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.ToString() };
        }
        return _response;
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete, Route("{id}")]
    public async Task<ResponseDto> Delete(int id)
    {
        try
        {
            var isSuccess = await _productRepository.DeleteProduct(id);
            _response.IsSuccess= isSuccess;
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.ToString() };
        }
        return _response;
    }
}