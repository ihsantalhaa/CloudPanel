using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CloudPanel.WebApp.Controllers
{
    public class ErrorController: Controller
    {
        public IActionResult Index()
        {
            var errorMessage = TempData["ErrorMessage"] as string ?? "Bir hata oluştu";
            var errorType = TempData["ErrorType"] as string ?? "Genel Hata";

            ViewBag.ErrorMessage = errorMessage;
            ViewBag.ErrorType = errorType;

            return View();
        }

        
    }
}
