using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace cw3.Middlewares
{
    public class Middleware
    {
        private readonly RequestDelegate _next;

        public Middleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var stream = new MemoryStream();
            await httpContext.Request.Body.CopyToAsync(stream);
            var tmp = stream.ToArray();
            httpContext.Request.Body = new MemoryStream(tmp);
            stream = new MemoryStream(tmp);
            var bodyStream = string.Empty;
            using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
            {
                bodyStream = await reader.ReadToEndAsync();
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("=================");
            sb.AppendLine();
            sb.Append(httpContext.Request.Method);
            sb.AppendLine();
            sb.Append(httpContext.Request.Path);
            sb.AppendLine();
            sb.Append(bodyStream);
            sb.AppendLine();
            sb.Append(httpContext.Request.QueryString);
            sb.AppendLine();
            sb.Append("=================");
            sb.AppendLine();


            File.AppendAllText("log.txt", sb.ToString());
            await _next(httpContext);
        }
    }
}
