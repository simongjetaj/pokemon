using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.IO;
using Pokemon.Data.Utils;

namespace Pokemon.Data.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>();
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            await LogRequest(context);
            await LogResponse(context);
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.Headers.Add("LogReferenceId", Guid.NewGuid().ToString());
            context.Request.EnableBuffering();

            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            _logger.LogInformation($"Http Request Information: " +
                                   $"Log Reference ID: {context.Request.Headers.FirstOrDefault(x => x.Key == Constants.LOG_REFERENCE_ID).Value} " +
                                   $"Host: {context.Request.Host} " +
                                   $"Path: {context.Request.Path} " +
                                   $"QueryString: {context.Request.QueryString} " +
                                   $"Request Body: {ReadStreamInChunks(requestStream)}");
            context.Request.Body.Position = 0;
        }

        private async Task LogResponse(HttpContext context)
        {
            using Stream originalBodyStream = context.Response.Body;

            try
            {
                using MemoryStream responseBody = _recyclableMemoryStreamManager.GetStream();
                context.Response.Body = responseBody;

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                string text = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                _logger.LogInformation($"Http Response Information: " +
                                       $"Log Reference ID: {context.Request.Headers.FirstOrDefault(x => x.Key == Constants.LOG_REFERENCE_ID).Value} " +
                                       $"Host: {context.Request.Host} " +
                                       $"Path: {context.Request.Path} " +
                                       $"QueryString: {context.Request.QueryString} " +
                                       $"Response Body: {text}");

                await responseBody.CopyToAsync(originalBodyStream);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);

            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }
    }
}
