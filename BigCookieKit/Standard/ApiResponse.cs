﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BigCookieKit.Standard
{
    public abstract class Response
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public virtual HttpStatusCode Code { get; set; }

        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get => Code == HttpStatusCode.OK; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
    }

    public class ApiResponse : Response
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data { get; set; }
    }

    public class ApiResponse<T> : Response
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }
    }

    public class ApiResponseSuccess : ApiResponse
    {

        public ApiResponseSuccess(string message = "success!")
        {
            Message = message;
        }

        public ApiResponseSuccess(object data, string message = "success!")
        {
            Data = data;
            Message = message;
        }

        public override HttpStatusCode Code { get; set; } = HttpStatusCode.OK;
    }

    public class ApiResponseSuccess<T> : ApiResponse<T>
    {
        public ApiResponseSuccess(T data, string message = "success!")
        {
            Data = data;
            Message = message;
        }

        public override HttpStatusCode Code { get; set; } = HttpStatusCode.OK;
    }

    public class ApiPagerResponse : ApiResponse
    {
        /// <summary>
        /// 数据总量
        /// </summary>
        public long Total { get; set; }

        public ApiPagerResponse(object data, long total, string message = "success!")
        {
            Data = data;
            Total = total;
            Message = message;
        }

        public override HttpStatusCode Code { get; set; } = HttpStatusCode.OK;
    }

    public class ApiPagerResponse<T> : ApiResponse<T>
    {
        /// <summary>
        /// 数据总量
        /// </summary>
        public long Total { get; set; }

        public ApiPagerResponse(T data, long total, string message = "success!")
        {
            Data = data;
            Total = total;
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

    public class ApiResponseFail<T> : ApiResponse<T>
    {
        public ApiResponseFail(string message = "fail!")
        {
            Message = message;
        }

        public override HttpStatusCode Code { get; set; } = HttpStatusCode.InternalServerError;
    }
}
