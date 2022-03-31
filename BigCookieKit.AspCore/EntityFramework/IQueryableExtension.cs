using BigCookieKit.AspCore.Standard;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BigCookieKit.AspCore.EntityFramework
{
    public static class IQueryableExtension
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> source, object dto)
            where T : class
        {
            var originProps = typeof(T).GetProperties().ToList();
            var props = dto.GetType().GetProperties().ToList();
            if (dto is ApiPermission)
            {
                var permissionProps = typeof(ApiPermission).GetProperties();
                props = props.Where(x => permissionProps.FirstOrDefault(r => r.Name == x.Name && r.PropertyType == x.PropertyType) == null).ToList();
            }

            if (dto is ApiPermissionPager)
            {
                var permissionProps = typeof(ApiPermissionPager).GetProperties();
                props = props.Where(x => permissionProps.FirstOrDefault(r => r.Name == x.Name && r.PropertyType == x.PropertyType) == null).ToList();
            }

            if (dto is ApiPager)
            {
                var permissionProps = typeof(ApiPager).GetProperties();
                props = props.Where(x => permissionProps.FirstOrDefault(r => r.Name == x.Name && r.PropertyType == x.PropertyType) == null).ToList();
                var pager = dto as ApiPager;
                source = source.Skip((pager.PageIndex - 1) * pager.PageCount).Take(pager.PageCount);
            }

            props = props.Where(x => originProps.FirstOrDefault(r => r.Name == x.Name && r.PropertyType == x.PropertyType) != null).ToList();

            foreach (var prop in props)
            {
                var value = prop.GetValue(dto);
                if (value == null) continue;
                source = Queryable.Where(source, AutoWhere<T>(prop, prop.GetValue(dto)));
            }

            return source;
        }

        public static IEnumerable<T> Select<T, T1, T2>(this IEnumerable<JoinSelect<T1, T2>> source, Func<T1, T2, T> selector)
            where T : class
            where T1 : class
            where T2 : class
        {
            foreach (var item in source)
            {
                yield return item.As(selector);
            }
        }

        private static Expression<Func<T, bool>> AutoWhere<T>(PropertyInfo item, object value)
        {
            ParameterExpression @var = Expression.Parameter(typeof(T));
            var attr = item.GetCustomAttribute(typeof(QueryMeta)) as QueryMeta;
            Expression cache = null;
            string[] list = null;
            Type genericType = null;
            Type[] genericTypes = null;
            MethodInfo genericMethod = null;
            string fieldName = string.IsNullOrEmpty(attr.Name) ? item.Name : attr.Name;
            if (attr == null)
            {
                cache = Expression.Equal(Expression.Property(@var, fieldName), Expression.Constant(value, item.PropertyType));
            }
            else
            {
                switch (attr.Where)
                {
                    case QueryFunc.Equal:
                        cache = Expression.Equal(Expression.Property(@var, fieldName), Expression.Constant(value, item.PropertyType));
                        break;
                    case QueryFunc.NoEqual:
                        cache = Expression.NotEqual(Expression.Property(@var, fieldName), Expression.Constant(value, item.PropertyType));
                        break;
                    case QueryFunc.Like:
                        cache = Expression.Call(typeof(DbFunctionsExtensions).GetMethod("Like", new Type[] { typeof(DbFunctions), typeof(string), typeof(string) }),
                            Expression.Constant(EF.Functions),
                            Expression.Property(@var, fieldName),
                            Expression.Constant($"%{value}%"));
                        break;
                    case QueryFunc.NotLike:
                        cache = Expression.Call(typeof(DbFunctionsExtensions).GetMethod("Like", new Type[] { typeof(DbFunctions), typeof(string), typeof(string) }),
                            Expression.Constant(EF.Functions),
                            Expression.Property(@var, fieldName),
                            Expression.Constant($"%{value}%"));
                        cache = Expression.Condition(cache, Expression.Constant(false), Expression.Constant(true));
                        break;
                    case QueryFunc.In:
                        list = Convert.ToString(value).Split(',');

                        genericType = Type.MakeGenericMethodParameter(0);
                        genericTypes = new Type[] { genericType.MakeArrayType(), genericType };
                        genericMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Contains), genericTypes);
                        genericMethod = genericMethod.MakeGenericMethod(typeof(string));

                        cache = Expression.Call(genericMethod,
                            Expression.Constant(list),
                            Expression.Convert(Expression.Property(@var, fieldName), typeof(string)));
                        break;
                    case QueryFunc.NotIn:
                        list = Convert.ToString(value).Split(',');

                        genericType = Type.MakeGenericMethodParameter(0);
                        genericTypes = new Type[] { genericType.MakeArrayType(), genericType };
                        genericMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Contains), genericTypes);
                        genericMethod = genericMethod.MakeGenericMethod(typeof(string));

                        cache = Expression.Call(genericMethod,
                            Expression.Constant(list),
                            Expression.Convert(Expression.Property(@var, fieldName), typeof(string)));
                        cache = Expression.Condition(cache, Expression.Constant(false), Expression.Constant(true));
                        break;
                    case QueryFunc.GrThen:
                        cache = Expression.GreaterThan(Expression.Property(@var, fieldName), Expression.Constant(value, item.PropertyType));
                        break;
                    case QueryFunc.GrThenOrEqual:
                        cache = Expression.GreaterThanOrEqual(Expression.Property(@var, fieldName), Expression.Constant(value, item.PropertyType));
                        break;
                    case QueryFunc.LeThen:
                        cache = Expression.LessThan(Expression.Property(@var, fieldName), Expression.Constant(value, item.PropertyType));
                        break;
                    case QueryFunc.LeThenOrEqual:
                        cache = Expression.LessThanOrEqual(Expression.Property(@var, fieldName), Expression.Constant(value, item.PropertyType));
                        break;
                    default:
                        return x => true;
                }
            }
            return Expression.Lambda<Func<T, bool>>(cache, new ParameterExpression[] { @var });
        }
    }
}
