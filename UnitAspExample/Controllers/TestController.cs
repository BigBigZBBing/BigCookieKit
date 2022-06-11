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
            return Ok("成功");
        }
    }
}
