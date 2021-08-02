using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Simucraft.Client.Pages;
using Simucraft.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Simucraft.Server.Middleware
{
    public class ExceptionFilterMiddleware : IMiddleware
    {
        private readonly IDictionary<Type, (HttpStatusCode HttpStatusCode, string ErrorMessage)> _exceptionFilters = new Dictionary<Type, (HttpStatusCode, string)>
        {
            [typeof(NullReferenceException)] = (HttpStatusCode.NotFound, "Resource not found."),
            [typeof(InvalidOperationException)] = (HttpStatusCode.BadRequest, "Invalid Request."),
            [typeof(SubscriptionException)] = (HttpStatusCode.PaymentRequired, null),
            [typeof(MaxEntityException)] = (HttpStatusCode.TooManyRequests, null),
        };

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next?.Invoke(context);
            }
            catch(Exception exception)
            {
                await this.OnExceptionAsync(context, exception);
            }
        }

        private async Task OnExceptionAsync(HttpContext context, Exception exception)
        {

            // TODO: Determine fluent validation exception type on requests.
            // TODO: Log.

            var filterKey = exception.GetType();

            var errorMessage = _exceptionFilters.ContainsKey(filterKey)
                ? _exceptionFilters[filterKey].ErrorMessage
                : "An error occured";

            // If filter didn't provide a canned error response then populate one from exception message.
            if (string.IsNullOrEmpty(errorMessage))
                errorMessage = exception.Message;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)(_exceptionFilters.ContainsKey(filterKey)
                ? _exceptionFilters[filterKey].HttpStatusCode
                : HttpStatusCode.InternalServerError);

            await context.Response.WriteAsync(
                new ApiException { Message = errorMessage }.ToJson());
        }
    }
}
