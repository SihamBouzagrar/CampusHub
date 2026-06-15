using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CampusHub.Models;

namespace CampusHub.Controllers;

public class HomeController : Controller
{
 

    public IActionResult Privacy()
    {
        return View();
    }
    private readonly IConfiguration _configuration;
public HomeController(IConfiguration configuration)
{
_configuration = configuration;
}
  public IActionResult Index()
{
    //if (!User.Identity.IsAuthenticated)
      //  return RedirectToAction("Login", "Account");

    return View();
}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
