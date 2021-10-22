using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pokemon.Data.Exceptions;
using Pokemon.Data.Models;
using Pokemon.Data.Utils;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Pokemon.Data.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;


        public ExceptionMiddleware(RequestDelegate next, ILogger logger)
        {
            _logger = logger;
            _next = next;
        }


        public async Task InvokeAsync(HttpContext httpContext, IHostEnvironment env)
        {
            try
            {
                await _next(httpContext);
            }
            catch (PokemonException ex)
            {
                _logger.LogError($"Something went wrong: {ex}. " +
                    $"Log Reference ID: {httpContext.Request.Headers.FirstOrDefault(x => x.Key == Constants.LOG_REFERENCE_ID).Value}");
                await HandleExceptionAsync(httpContext, env, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}. " +
                    $"Log Reference ID: {httpContext.Request.Headers.FirstOrDefault(x => x.Key == Constants.LOG_REFERENCE_ID).Value}");
                await HandleExceptionAsync(httpContext, env, ex);
            }
        }


        private static Task HandleExceptionAsync(HttpContext httpContext, IHostEnvironment environment, PokemonException exception)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = exception.PokemonError.StatusCode;

            return httpContext.Response.WriteAsync(new Error()
            {
                StatusCode = httpContext.Response.StatusCode,
                ErrorCode = exception.PokemonError.ErrorCode,
                Message = !environment.IsProduction() ? exception.Message : "An error has occurred and we're working to fix the problem! We'll be up and running shortly.",
                LogReferenceId = httpContext.Request.Headers.FirstOrDefault(x => x.Key == Constants.LOG_REFERENCE_ID).Value
            }.ToString());
        }


        private static Task HandleExceptionAsync(HttpContext httpContext, IHostEnvironment environment, Exception exception)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return httpContext.Response.WriteAsync(new Error()
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = !environment.IsProduction() ? exception.Message : "An error has occurred and we're working to fix the problem! We'll be up and running shortly.",
                LogReferenceId = httpContext.Request.Headers.FirstOrDefault(x => x.Key == Constants.LOG_REFERENCE_ID).Value
            }.ToString());
        }
    }
}

