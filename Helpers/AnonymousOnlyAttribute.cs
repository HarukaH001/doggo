
// using Microsoft.AspNetCore.Authorization;
// public class AnonymousOnly : AuthorizeAttribute
// {
//     public override void OnAuthorization(AuthorizationContext filterContext)
//     {
//         if (filterContext.HttpContext.User.Identity.IsAuthenticated)
//         {
//              // Do what you want to do here, e.g. show a 404 or redirect
//         }
//     }
// }