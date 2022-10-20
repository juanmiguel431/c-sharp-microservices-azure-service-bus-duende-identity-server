using Mango.Services.ProductApi.Models.Dtos;
using Mango.Services.ProductApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductApi.Controllers
{
    [Route("api/products")]
    public class ProductApiController : Controller
    {

        protected ResponseDto _response;
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
    }
}
