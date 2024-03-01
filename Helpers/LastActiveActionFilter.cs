using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using ServerApp.Data;

namespace ServerApp.Helpers
{
    public class LastActiveActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //middleware sürecinde araya atmıs olduğum metodun calısması içinm aşağıdaki gibi
            //süreci durdurup calışmasını istediğim kodların calısmasını sağladım
            var resultContext = await next();

            var id = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            //Startup.cs içerisindeki ConfigureServices(IServiceCollection services) metodu içerisindeki
            //AddScoped ile birbirine bağladığım ISocialRepository ve SocialRepository servislerinden
            //ISocialRepository 'i aldım. Bu sayede elimdeki id bilgisi hangi kullanıcı olduğunu ögrendim
            //Devamında ilgili alanı güncelledim.
            var repository = (ISocialRepository)resultContext.HttpContext.RequestServices.GetService(typeof(ISocialRepository));
        
            var user = await repository.GetUser(id);
            user.LastActive = DateTime.Now;

            //bu noktada kullanıcının LastActive alanını güncelledim.
            await repository.SaveChanges();
        }
    }
}