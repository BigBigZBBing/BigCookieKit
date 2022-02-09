using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BigCookieKit.Standard
{
    public abstract class ApiResponse
    {
        public virtual HttpStatusCode Code { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
    }

    public class ApiResponseSuccess : ApiResponse
    {

        public ApiResponseSuccess(string message = "success!")
        {
            Message = message;
        }

        public ApiResponseSuccess(object data, string message)
        {
            Data = data;
            Message = message;
        }

        public override HttpStatusCode Code { get; set; } = HttpStatusCode.OK;
    }

    public class ApiResponseFail : ApiResponse
    {
        public ApiResponseFail(string message = "fail!")
        {
            Message = message;
        }

        public override HttpStatusCode Code { get; set; } = HttpStatusCode.InternalServerError;
    }
}
