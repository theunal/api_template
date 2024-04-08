using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Core.Helpers;
using Core.Models;
using Core.Exceptions;

namespace Core.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is UnauthorizedAccessException)
            {
                context.Result = new ObjectResult(new { success = false, message = "unauthorized" }) { StatusCode = 401 };
                context.ExceptionHandled = true;
            }
            else if (context.Exception is InvalidValidationException)
            {
                context.Result = new ObjectResult(new { success = false, message = context.Exception.Message }) { StatusCode = 400 };
                context.ExceptionHandled = true;
            }
            else
                GlobalExceptionLog(context);
        }

        private void GlobalExceptionLog(ExceptionContext context)
        {
            var http_method_type = context.HttpContext?.Request?.Method;
            var route = context.ActionDescriptor?.AttributeRouteInfo?.Template;
            var func_name = context.ActionDescriptor?.DisplayName;
            var data = context.Exception?.Data;

            var message = context.Exception?.Message;
            var source = context.Exception?.Source;
            var stack = context.Exception?.StackTrace;

            var error = new StringBuilder();

            if (string.IsNullOrEmpty(message) is false)
                error.Append($"\n\nmessage: {message}");

            if (string.IsNullOrEmpty(route) is false)
                error.Append($"\n\nroute: {route}{(http_method_type is not null ? $" ({http_method_type})" : "")}");

            if (string.IsNullOrEmpty(func_name) is false)
                error.Append($"\n\nfunc name: {func_name}");

            if (data?.Count > 0)
                error.Append($"\n\ndata: {System.Text.Json.JsonSerializer.Serialize(data)}");

            if (string.IsNullOrEmpty(source) is false)
                error.Append($"\n\nsource: {source}");

            if (string.IsNullOrEmpty(stack) is false)
                error.Append($"\n\nstack: {stack.Trim()}");

            GeneralStaticHelper.LogWrite(error.ToString(), LogType.ERROR, context.Exception);
        }
    }
}