using BigCookieKit.AspCore.RouteSelector;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitAspExample.Controllers
{
    /// <summary>
    /// 测试控制器
    /// </summary>
    [Route("BigTest/[controller]")]
    public class TestController : Controller
    {
        /// <summary>
        /// 测试接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            ViewBag.Data = "测试";
            return Ok("成功");
        }

        /// <summary>
        /// 测试路由选择器
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("TestSelector")]
        public IActionResult TestSelector()
        {
            var dendpoint = HttpContext.RequestServices.MathEndpoint("/Home").Result;
            var selectorResult = dendpoint.InvokeAsync(HttpContext)?.Result;
            var data = selectorResult.ViewBag.Data;
            return Ok("测试成功！");
        }

        /// <summary>
        /// 测试路由描述符
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Test2Selector")]
        public IActionResult Test2Selector([FromServices] EndpointDataSource dataSource)
        {
            var endpoint = dataSource.Endpoints.FirstOrDefault(x => x.GetRoutePath() == "/Home");
            var selectorResult = endpoint.InvokeAsync(HttpContext)?.Result;
            var data = selectorResult.ViewBag.Data;
            return Ok("测试成功！");
        }
    }
}
