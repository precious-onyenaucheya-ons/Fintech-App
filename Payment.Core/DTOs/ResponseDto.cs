using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Core.DTOs
{
    public class ResponseDto<T>
    {
        public int StatusCode { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; } = null!;
        public T Data { get; set; }
        public static ResponseDto<T> Fail(string errorMessage, int statusCode = (int)HttpStatusCode.NotFound)
        {
            return new ResponseDto<T> { Status = false, Message = errorMessage, StatusCode = statusCode };
        }
        public static ResponseDto<T> Success(string successMessage, T data, int statusCode = (int)HttpStatusCode.OK)
        {
            return new ResponseDto<T> { Status = true, Message = successMessage, Data = data, StatusCode = statusCode };
        }
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
