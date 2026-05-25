using DziennikPodrozy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DziennikPodrozy.Filters;

public class RequireAdminAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        if (string.IsNullOrEmpty(session.GetString("User")))
        {
            context.Result = new RedirectToActionResult("Logowanie", "Auth", null);
            return;
        }
        if (!UserScope.IsAdmin(session))
            context.Result = new RedirectToActionResult("Index", "Home", null);
    }
}
