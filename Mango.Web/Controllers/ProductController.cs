using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    private const string AccessToken = "access_token";

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    
    public async Task<IActionResult> ProductIndex()
    {
        List<ProductDto> list;
        var token = await GetToken();
        var response = await _productService.GetAllProductsAsync<ResponseDto>(token);
        if (response != null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
        }
        else
        {
            list = new List<ProductDto>();
        }
        return View(list);
    }
    
    public IActionResult ProductCreate()
    {
        return View();
    }
    
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ProductCreate(ProductDto productDto)
    {
        if (!ModelState.IsValid) return View(productDto);

        var token = await GetToken();
        var response = await _productService.CreateProductAsync<ResponseDto>(productDto, token);
        if (response != null && response.IsSuccess)
        {
            return RedirectToAction(nameof(ProductIndex));
        }

        return View(productDto);
    }
    
    public async Task<IActionResult> ProductEdit(int productId)
    {
        var token = await GetToken();
        var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, token);
        if (response != null && response.IsSuccess && response.Result != null)
        {
            var serializedObject = Convert.ToString(response.Result);
            var product = JsonConvert.DeserializeObject<ProductDto>(serializedObject);
            return View(product);
        }

        return NotFound();
    }

    private async Task<string?> GetToken()
    {
        return await HttpContext.GetTokenAsync(AccessToken);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ProductEdit(ProductDto productDto)
    {
        if (!ModelState.IsValid) return View(productDto);

        var token = await GetToken();
        var response = await _productService.UpdateProductAsync<ResponseDto>(productDto, token);
        if (response != null && response.IsSuccess)
        {
            return RedirectToAction(nameof(ProductIndex));
        }

        return View(productDto);
    }
    
    public async Task<IActionResult> ProductDelete(int productId)
    {
        var token = await GetToken();
        var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, token);
        if (response != null && response.IsSuccess && response.Result != null)
        {
            var serializedObject = Convert.ToString(response.Result);
            var product = JsonConvert.DeserializeObject<ProductDto>(serializedObject);
            return View(product);
        }

        return NotFound();
    }
    
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ProductDelete(ProductDto productDto)
    {
        var token = await GetToken();
        var response = await _productService.DeleteProductAsync<ResponseDto>(productDto.ProductId, token);
        if (response != null && response.IsSuccess)
        {
            return RedirectToAction(nameof(ProductIndex));
        }

        return View(productDto);
    }
}