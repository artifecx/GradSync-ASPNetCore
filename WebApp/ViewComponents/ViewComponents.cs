using Microsoft.AspNetCore.Mvc;
using Services.ServiceModels;

namespace WebApp.ViewComponents
{
    public class SearchInputViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string searchValue, string placeholder = null)
        {
            ViewData["SearchValue"] = searchValue;
            ViewData["Placeholder"] = placeholder;
            return View();
        }
    }
}
