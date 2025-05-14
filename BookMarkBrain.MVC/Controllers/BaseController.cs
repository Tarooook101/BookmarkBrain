using BookMarkBrain.MVC.Services.APIServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookMarkBrain.MVC.Controllers;

public abstract class BaseController : Controller
{
    protected readonly IApiService _apiService;
    protected readonly ILogger _logger;

    protected BaseController(IApiService apiService, ILogger logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    protected void SetNotification(string message, string type = "info")
    {
        TempData["Notification"] = message;
        TempData["NotificationType"] = type; // info, success, warning, error
    }
}
