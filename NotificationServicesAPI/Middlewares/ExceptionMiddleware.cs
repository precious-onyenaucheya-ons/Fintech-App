using Newtonsoft.Json;
using NotificationServicesAPI.Core.DTOs;
using System.Net;
using ILogger = Serilog.ILogger;

namespace NotificationServices.API.Middlewares
{
    /// <summary>
    /// Implementation for exception handler
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public ExceptionMiddleware(RequestDelegate next,
            ILogger logger)
        {
            _logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = ResponseDto<string>.Fail(error.Message);
                switch (error)
                {
                    case UnauthorizedAccessException e:
                        _logger.Error(e, e.StackTrace, e.Source, e.ToString());
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        responseModel.Message = e.Message;
                        break;
                    case ArgumentOutOfRangeException e:
                        _logger.Error(e, e.StackTrace, e.Source, e.ToString());
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Message = e.Message;
                        break;
                    case ArgumentNullException e:
                        _logger.Error(e, e.StackTrace, e.Source, e.ToString());
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Message = e.Message;
                        break;
                    default:
                        // unhandled error
                        _logger.Error(error, error.Source, error.InnerException, error.Message, error.ToString());
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel.Message = "Internal Server Error. Please Try Again Later.";
                        break;
                }
                var result = JsonConvert.SerializeObject(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}