using BookMarkBrain.MVC.Models.Common;
using BookMarkBrain.MVC.Services.APIServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookMarkBrain.MVC.Controllers;

public class HomeController : BaseController
{
    public HomeController(IApiService apiService, ILogger<HomeController> logger)
        : base(apiService, logger)
    {
    }

    public IActionResult Index()
    {
        return View();
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
}
