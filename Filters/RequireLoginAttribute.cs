using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DziennikPodrozy.Filters;

public class RequireLoginAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (string.IsNullOrEmpty(context.HttpContext.Session.GetString("User")))
            context.Result = new RedirectToActionResult("Logowanie", "Auth", null);
    }
}
