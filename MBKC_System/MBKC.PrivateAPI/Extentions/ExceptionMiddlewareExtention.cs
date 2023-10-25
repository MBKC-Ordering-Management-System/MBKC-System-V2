using MBKC.PrivateAPI.Middlewares;

namespace MBKC.PrivateAPI.Extentions
{
    public static class ExceptionMiddlewareExtention
    {
        public static void ConfigureExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
