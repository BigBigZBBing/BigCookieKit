using BigCookieKit.Standard;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UnitAspExample.Controllers
{
    public class UserAddDto
    {
        /// <summary>
        /// 测试注释
        /// </summary>
        public string String1 { get; set; }

        [Required]
        public DateTime DateTime1 { get; set; }

        public DateTime? DateTime2 { get; set; }

        public int Int1 { get; set; }

        public int? Int2 { get; set; }

        /// <summary>
        /// 所属的公司
        /// </summary>
        public List<UserCompany> Companys { get; set; }
    }

    public class UserCompany
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 用户管理
    /// </summary>
    [Route("Management/[controller]")]
    public class UserController : Controller
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Add")]
        public IActionResult Add(UserAddDto dto)
        {
            return Ok();
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Update")]
        public ApiResponse<object> Update(long? Id, UserAddDto dto)
        {
            return new ApiResponseSuccess();
        }
    }
}
