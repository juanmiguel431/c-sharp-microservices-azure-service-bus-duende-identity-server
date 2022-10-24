﻿using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    
    public async Task<IActionResult> ProductIndex()
    {
        List<ProductDto> list;
        var response = await _productService.GetAllProductsAsync<ResponseDto>();
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
    
    public async Task<IActionResult> ProductCreate()
    {
        return View();
    }
    
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ProductCreate(ProductDto productDto)
    {
        if (!ModelState.IsValid) return View(productDto);

        var response = await _productService.CreateProductAsync<ResponseDto>(productDto);
        if (response != null && response.IsSuccess)
        {
            return RedirectToAction(nameof(ProductIndex));
        }

        return View(productDto);
    }
}