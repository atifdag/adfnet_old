using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

namespace ADF.Net.Web.Api
{
    public class SerilogMiddleware
    {
        private const string MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        private static readonly ILogger Log = Serilog.Log.ForContext<SerilogMiddleware>();
        //private static CustomIdentity identity;
        private readonly RequestDelegate _next;

        public SerilogMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            var path = httpContext.Request.Path;
            if (path.StartsWithSegments(new PathString("/Home")) ||
                path.StartsWithSegments(new PathString("/ContentFiles")))
            {
                await _next(httpContext);
                return;
            }

            //identity = (CustomIdentity)httpContext.User.Identity;
            var requestBody = string.Empty;

            var sw = Stopwatch.StartNew();
            try
            {
                //if (httpContext.Request.Path.StartsWithSegments(new PathString("/api")))
                //{
                httpContext.Request.EnableBuffering();
                using var reader = new StreamReader(httpContext.Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                httpContext.Request.Body.Position = 0;
                //}

                await _next(httpContext);
                sw.Stop();
                var statusCode = httpContext.Response?.StatusCode;
                var level = statusCode > 399 ? LogEventLevel.Error : LogEventLevel.Information;
                var log = level == LogEventLevel.Error ? LogForErrorContext(httpContext, requestBody) : Log;
                if (!string.IsNullOrEmpty(requestBody) && (requestBody != "{}" || requestBody != ""))
                    log = log.ForContext("RequestForm", requestBody);
                //if (identity != null)
                //{ log = log.ForContext("UserId", identity.UserId).ForContext("UserName", identity.Username); }

                log.Write(level, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, statusCode, sw.Elapsed.TotalMilliseconds);
            }
            // Never caught, because `LogException()` returns false.
            catch (Exception ex) when (LogException(httpContext, sw, ex, requestBody)) { }
        }

        private static bool LogException(HttpContext httpContext, Stopwatch sw, Exception ex, string requestBody)
        {
            sw.Stop();
            LogForErrorContext(httpContext, requestBody)
                .Error(ex, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, 500, sw.Elapsed.TotalMilliseconds);
            return false;
        }

        private static ILogger LogForErrorContext(HttpContext httpContext, string requestBody)
        {
            var request = httpContext.Request;

            var result = Log
                .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
                .ForContext("RequestHost", request.Host)
                .ForContext("RequestProtocol", request.Protocol);

            //if (identity != null)
            //{ result = result.ForContext("UserId", identity.UserId).ForContext("UserName", identity.Username); }

            if (!string.IsNullOrEmpty(requestBody) || requestBody != "{}")
                result = result.ForContext("RequestForm", requestBody);
            else if (request.HasFormContentType)
                result = result.ForContext("RequestForm", request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));

            return result;
        }
    }
}
