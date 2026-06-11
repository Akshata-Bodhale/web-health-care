using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CareProjct.web.Filter
{
    public class UserAuthenication: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            if (string.IsNullOrEmpty(session.GetString("userId")))
            {
                context.Result = new RedirectToActionResult("Login", "Home",
                    new { returnUrl = context.HttpContext.Request.Path });
            }
            base.OnActionExecuting(context);
        }
    }
}
