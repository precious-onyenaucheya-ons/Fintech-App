using Newtonsoft.Json;
using Payment.Core.DTOs;
using System.Net;
using ILogger = Serilog.ILogger;

namespace Payment.API.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex).ConfigureAwait(false);
            }
        }

        public async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            var response = httpContext.Response;
            var responseModel = ResponseDto<string>.Fail(ex.Message);

            switch (ex)
            {
                case UnauthorizedAccessException e:
                    _logger.Error(e, e.StackTrace, e.Source, e.Message, e.ToString());
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    responseModel.Message = e.Message;
                    break;
                case ArgumentNullException e:
                    _logger.Error(e, e.StackTrace, e.Source, e.Message, e.ToString());
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.Message = e.Message;
                    break;
                case ArgumentOutOfRangeException e:
                    _logger.Error(e, e.StackTrace, e.Source, e.Message, e.ToString());
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.Message = e.Message;
                    break;
                case ArgumentException e:
                    _logger.Error(e, e.StackTrace, e.Source, e.Message, e.ToString());
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.Message = e.Message;
                    break;
                default:
                    _logger.Error(ex, ex.StackTrace, ex.Source, ex.Message, ex.ToString());
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseModel.Message = "Internal Server Error. Try Again Later.";
                    break;
            }

            var result = JsonConvert.SerializeObject(responseModel);

            await response.WriteAsync(result);
        }
    }
}
