using DziennikPodrozy.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace DziennikPodrozy.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiAuthAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        var login = request.Headers["X-Api-Login"].FirstOrDefault()
            ?? request.Query["login"].FirstOrDefault();
        var token = request.Headers["X-Api-Token"].FirstOrDefault()
            ?? request.Query["apiToken"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(token))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Podaj login i apiToken (nagłówki X-Api-Login, X-Api-Token lub parametry ?login=&apiToken=)." });
            return;
        }

        var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var user = await db.Users.FirstOrDefaultAsync(u =>
            u.Login == login.Trim() && u.ApiToken == token.Trim());

        if (user == null)
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Nieprawidłowy login lub token API." });
            return;
        }

        context.HttpContext.Items["ApiUser"] = user;
        await next();
    }
}
