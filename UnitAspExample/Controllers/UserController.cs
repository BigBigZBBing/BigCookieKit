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
    /// 用户响应模型
    /// </summary>
    public class UserResponse
    {
        /// <summary>
        /// 返回的用户名
        /// </summary>
        public string UserName { get; set; } = "ZHANGBINGBIN";

        /// <summary>
        /// 是否为超级管理员
        /// </summary>
        public bool IsSuper { get; set; } = true;
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
        public ApiResponse Add(UserAddDto dto)
        {
            return new ApiResponseSuccess();
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Update")]
        public ApiResponse<UserResponse> Update(long? Id, UserAddDto dto)
        {
            return new ApiResponseSuccess<UserResponse>(new UserResponse());
        }
    }
}
