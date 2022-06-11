using BigCookie.XML;
using BigCookie;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

using System.Collections.Generic;
using System.Linq;
using BigCookieKit;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BigCookie.Document.Controllers
{
    [Route("Document")]
    public class DocumentController : Controller
    {
        static DocumentController()
        {
            xmlCommentHelper.LoadAll();
        }

        private static Dictionary<string, MethodInfo> m_actions = new Dictionary<string, MethodInfo>();
        private static XmlCommentHelper xmlCommentHelper = new XmlCommentHelper();

        public IActionResult Index()
        {
            m_actions.Clear();

            var endpointRoute = DocumentExtension._application.Properties["__EndpointRouteBuilder"] as IEndpointRouteBuilder;
            var GroupUri = new Dictionary<string, List<UriInfo>>();
            var GroupInfo = new Dictionary<string, string>();
            foreach (var item in endpointRoute.DataSources)
            {
                foreach (RouteEndpoint endpoint in item.Endpoints)
                {
                    var descriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                    if (descriptor != null)
                    {
                        string Uri = "";
                        var httpMethod = descriptor.MethodInfo.CustomAttributes.FirstOrDefault(x => x.AttributeType.IsInheritType(typeof(HttpMethodAttribute)));
                        if (endpoint.Order > 0)
                        {
                            if (descriptor.RouteValues.ContainsKey("area") && !string.IsNullOrEmpty(descriptor.RouteValues["area"]))
                                Uri += "/" + descriptor.RouteValues["area"];
                            if (descriptor.RouteValues.ContainsKey("controller") && !string.IsNullOrEmpty(descriptor.RouteValues["controller"]))
                                Uri += "/" + descriptor.RouteValues["controller"];
                            if (descriptor.RouteValues.ContainsKey("action") && !string.IsNullOrEmpty(descriptor.RouteValues["action"]))
                                Uri += "/" + descriptor.RouteValues["action"];
                        }
                        else
                        {
                            Uri = endpoint.RoutePattern.RawText;
                        }

                        var groupName = descriptor.ControllerName.Replace("Controller", "");
                        if (!GroupUri.ContainsKey(groupName))
                        {
                            GroupUri.Add(groupName, new List<UriInfo>());
                            GroupInfo.Add(groupName, xmlCommentHelper.GetTypeComment(descriptor.ControllerTypeInfo.AsType()));
                        }

                        if (!GroupUri[groupName].Exists(x => x.Uri == Uri))
                        {
                            if (httpMethod != null)
                            {
                                var UriInfo = new UriInfo();
                                UriInfo.Uri = Uri;
                                UriInfo.Comment = xmlCommentHelper.GetMethodComment(descriptor.MethodInfo);
                                var method = Activator.CreateInstance(httpMethod.AttributeType) as HttpMethodAttribute;
                                UriInfo.Method = method.HttpMethods.FirstOrDefault();
                                GroupUri[groupName].Add(UriInfo);
                                m_actions.TryAdd(Uri, descriptor.MethodInfo);
                            }
                        }
                    }
                }
            }


            ViewBag.GroupUri = GroupUri;
            ViewBag.GroupInfo = GroupInfo;
            ViewBag.Description = DocumentExtension._description;
            return View();
        }

        [Route("Detail")]
        public IActionResult Detail(string Uri, string Method)
        {
            if (m_actions.ContainsKey(Uri))
            {
                var ReuqestBody = new List<BodyTable>();
                var ResponseBody = new List<BodyTable>();
                var parameters = m_actions[Uri].GetParameters();
                var returnParameter = m_actions[Uri].ReturnType;
                if (parameters.Length > 0)
                {
                    if (parameters.Length == 1 && parameters[0].ParameterType.IsCustomClass())
                    {
                        ParameterAnalytics(ReuqestBody, parameters[0].ParameterType);
                    }
                    else
                    {
                        foreach (var parameter in parameters)
                        {
                            var ParameterName = parameter.ParameterType.Name;
                            if (parameter.ParameterType.IsNullable())
                                ParameterName = parameter.ParameterType.GetGenericArguments()[0].Name + "?";

                            if (parameter.ParameterType.IsCustomClass() || parameter.ParameterType.IsCollection())
                            {
                                var TypeName = "";
                                Type NextType = parameter.ParameterType;
                                if (parameter.ParameterType.IsCustomClass()) TypeName = "Object";
                                if (parameter.ParameterType.IsCollection())
                                {
                                    TypeName = "Array";
                                    NextType = parameter.ParameterType.GetGenericArguments()[0];
                                }
                                ReuqestBody.Add(new BodyTable()
                                {
                                    FieldName = parameter.Name,
                                    TypeName = TypeName,
                                    IsRequire = false,
                                    Comment = xmlCommentHelper.GetParameterComment(parameter),
                                });
                                ParameterAnalytics(ReuqestBody, NextType, parameter.Name + "->");
                            }
                            else
                            {
                                ReuqestBody.Add(new BodyTable()
                                {
                                    FieldName = parameter.Name,
                                    TypeName = ParameterName,
                                    IsRequire = false,
                                    Comment = xmlCommentHelper.GetParameterComment(parameter),
                                });
                            }
                        }
                    }
                }

                if (returnParameter != typeof(void))
                {
                    ParameterAnalytics(ResponseBody, returnParameter);
                }

                ViewBag.Uri = "/" + Uri;
                ViewBag.Method = Method;
                ViewBag.RequestBody = ReuqestBody;
                ViewBag.ResponseBody = ResponseBody;
                return View();
            }
            return Content("<h1>生成文档异常!</h1>");
        }

        private static void ParameterAnalytics(List<BodyTable> body, Type parameter, string parentName = null)
        {
            var members = parameter.GetMembers();
            foreach (var member in members)
            {
                if (member.MemberType != MemberTypes.Property) continue;
                PropertyInfo prop = member as PropertyInfo;
                var require = member.GetCustomAttribute<RequiredAttribute>();
                var TypeName = prop.PropertyType.Name;
                if (prop.PropertyType.IsNullable())
                    TypeName = prop.PropertyType.GetGenericArguments()[0].Name + "?";
                var FieldName = parentName + prop.Name;

                if (prop.PropertyType.IsCustomClass() || prop.PropertyType.IsCollection())
                {
                    Type NextType = prop.PropertyType;
                    if (prop.PropertyType.IsCustomClass()) TypeName = "Object";
                    if (prop.PropertyType.IsCollection())
                    {
                        TypeName = "Array";
                        NextType = prop.PropertyType.GetGenericArguments()[0];
                    }
                    body.Add(new BodyTable()
                    {
                        FieldName = FieldName,
                        TypeName = TypeName,
                        IsRequire = require != null,
                        Comment = xmlCommentHelper.GetFieldOrPropertyComment(member),
                    });
                    ParameterAnalytics(body, NextType, FieldName + "->");
                }
                else
                {
                    body.Add(new BodyTable()
                    {
                        FieldName = FieldName,
                        TypeName = TypeName,
                        IsRequire = require != null,
                        Comment = xmlCommentHelper.GetFieldOrPropertyComment(member),
                    });
                }
            }
        }
    }
}
