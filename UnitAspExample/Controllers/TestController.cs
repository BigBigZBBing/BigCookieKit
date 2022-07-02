using BigCookieKit.AspCore.RouteSelector;

using Microsoft.AspNetCore.Mvc;

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
        [Route("TestSelector")]
        public IActionResult TestSelector()
        {
            var dendpoint = HttpContext.RequestServices.MathEndpoint("/BigTest/Test/Index").Result;
            var selectorResult = dendpoint.InvokeAsync(HttpContext).Result;
            var data = selectorResult.ViewBag.Data;
            return Ok("测试成功！");
        }
    }
}
