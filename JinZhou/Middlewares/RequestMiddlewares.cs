using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JinZhou.Services;
using Microsoft.AspNetCore.Http;

namespace JinZhou.Middlewares
{
    public class RequestMiddlewares
    {
        private readonly RequestDelegate _next;

        public RequestMiddlewares(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            LogService.GetInstance().AddLog("Middleware", null, "request "+context.Request.PathAndQuery(), "", "Debug");
            await _next(context);
        }
    }
}
