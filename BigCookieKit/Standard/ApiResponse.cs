using System;
using System.Collections.Generic;
using System.Text;

namespace BigCookieKit.Standard
{
    public abstract class ApiResponse
    {
        public virtual int Code { get; set; }
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

        public override int Code { get; set; } = 200;
    }

    public class ApiResponseFail : ApiResponse
    {
        public ApiResponseFail(string message = "fail!")
        {
            Message = message;
        }

        public override int Code { get; set; } = 500;
    }
}
