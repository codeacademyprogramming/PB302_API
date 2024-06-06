using System.Net;
using UniversityApp.Service.Exceptions;

namespace UniversityApp.Api.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {

                //switch (ex)
                //{
                //    case NotFoundException e:
                //        context.Response.StatusCode = StatusCodes.Status404NotFound;
                //        break;
                //    case DublicateEntityException e:
                //        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                //        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
                //        break;
                //    case LimitException e:
                //        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                //        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
                //        break;
                //    default:
                //        await context.Response.WriteAsJsonAsync(new { error = "Bilinmedik xeta bas verdi!" });
                //        break;
                //}

                var message = ex.Message;
                var errors = new List<RestExceptionError>();
                context.Response.StatusCode = 500;

                if (ex is RestException rex)
                {
                    message = rex.Message;
                    errors = rex.Errors;
                    context.Response.StatusCode = rex.Code;
                }

                await context.Response.WriteAsJsonAsync(new { message, errors });
            }
        }
    }
}
