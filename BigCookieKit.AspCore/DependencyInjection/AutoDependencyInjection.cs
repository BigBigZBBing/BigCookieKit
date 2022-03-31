using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BigCookieKit.AspCore.DependencyInjection
{
    /// <summary>
    /// 自动依赖注入类
    /// </summary>
    public static class DependencyInjectionImplementation
    {
        /// <summary>
        /// 自动注入方法
        /// <code>
        /// 规则：
        /// 1.接口类为实现类名称前面增加I标识
        /// 2.实现类标注[AutoInjection]属性
        /// </code>
        /// </summary>
        /// <param name="services"></param>
        public static void AutoDependencyInjection(this IServiceCollection services)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var dir = new System.IO.DirectoryInfo(path);
            var dlls = dir.GetFiles("*.dll");
            foreach (var dll in dlls)
            {
                var assembly = Assembly.LoadFrom(dll.FullName);
                if (assembly.ExportedTypes.Count() > 0)
                {
                    var autoInjectionService = assembly.ExportedTypes.Where(x => x.GetCustomAttribute<AutoInjectionAttribute>() != null);
                    if (autoInjectionService.Count() > 0)
                    {
                        foreach (var service in autoInjectionService)
                        {
                            var Interface = service.GetInterface("I" + service.Name);
                            services.AddScoped(Interface, service);
                        }
                    }
                }
            }
        }
    }
}
