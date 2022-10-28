using Mango.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private const string AccessToken = "access_token";

    public HomeController(ILogger<HomeController> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        List<ProductDto> list;
        var response = await _productService.GetAllProductsAsync<ResponseDto>("");

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

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Authorize]
    public IActionResult Login()
    {
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Logout()
    {
        return SignOut("Cookies", "oidc");
    }

    [Authorize]
    public async Task<IActionResult> Details(int productId)
    {
        ProductDto model;
        var token = await GetToken();
        var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, "");

        if (response != null && response.IsSuccess)
        {
            model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
        }
        else
        {
            model = new ProductDto();
        }
            
        return View(model);
    }
        
    private async Task<string?> GetToken()
    {
        return await HttpContext.GetTokenAsync(AccessToken);
    }

}