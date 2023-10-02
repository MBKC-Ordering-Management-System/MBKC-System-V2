using MBKC.Service.Errors;
using MBKC.Service.Exceptions;
using Newtonsoft.Json;

namespace MBKC.API.Middlewares
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            //define logging
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                //logging
                await next(context);
            }
            catch (Exception ex)
            {
                //logging
                await HandleException(context, ex);
            }
        }

        private static async Task HandleException(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            switch (ex)
            {
                case NotFoundException _:
                    context.Response.StatusCode = (int)StatusCodes.Status404NotFound;
                    break;
                case BadRequestException _:
                    context.Response.StatusCode = (int)StatusCodes.Status400BadRequest;
                    break;
                case ConflictException _:
                    context.Response.StatusCode = (int)StatusCodes.Status409Conflict;
                    break;
                default:
                    context.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
                    break;
            }

            Error error = new Error()
            {
                StatusCode = context.Response.StatusCode,
                Message = JsonConvert.DeserializeObject<List<ErrorDetail>>(ex.Message)
            };



            await context.Response.WriteAsync(error.ToString());
        }
    }
}
