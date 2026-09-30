using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OnlineExamSystem.Models
{
    public class RoleAuthorizeAttribute : ActionFilterAttribute
    {
        private string _role; public RoleAuthorizeAttribute(string role) { _role = role; }
        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
         
            if (ctx.HttpContext.Session.GetString("Role") != _role || !ctx.HttpContext.Session.GetInt32("UserId").HasValue)
                ctx.Result = new RedirectToActionResult("Login", "Auth", null);
        }
    }
}
