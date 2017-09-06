using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Prime.Utility
{
    public static class Reflection
    {
        /// http://stackoverflow.com/questions/708952/how-to-instantiate-an-object-with-a-private-constructor-in-c
        public static T InstanceNonPublic<T>(Type[] paramTypes = null, object[] paramValues = null) where T : class
        {
            return InstanceNonPublic(typeof (T), paramTypes, paramValues) as T;
        }

        public static T InstanceAny<T>(Type[] paramTypes = null, object[] paramValues = null) where T : class
        {
            return InstanceAny(typeof(T), paramTypes, paramValues) as T;
        }
        public static object StaticInstanceI(Type type, bool nullonerror = false)
        {
            try
            {
                var prop = type.GetProperty("I", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                return prop?.GetMethod?.Invoke(null, null);
            }
            catch
            {
                if (!nullonerror)
                    throw;
            }
            return null;
        }
        public static T StaticInstanceI<T>(Type type) where T : class
        {
            return StaticInstanceI(type) as T;
        }

        public static object InstanceNonPublic(Type type, Type[] paramTypes = null, object[] paramValues = null)
        {
            paramTypes = paramTypes ?? new Type[0];
            paramValues = paramValues ?? new object[0];

            var ci = type.GetConstructor(paramTypes);

            if (ci == null || ci.IsPublic)
            {
                if (paramTypes.Length == 0)
                    throw new Exception("'" + type + "' requires a non-public empty constructor");

                throw new Exception("'" + type + "' requires a non-public constructor with " + paramTypes.Length +
                                    " parameter(s)");
            }

            return ci.Invoke(paramValues);
        }

        public static object InstanceAny(Type type, Type[] paramTypes = null, object[] paramValues = null)
        {
            paramTypes = paramTypes ?? new Type[0];
            paramValues = paramValues ?? new object[0];
            var ci = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, paramTypes, null);

            if (ci == null)
            {
                if (paramTypes.Length == 0)
                    throw new Exception("'" + type + "' requires an empty constructor");

                throw new Exception("'" + type + "' requires a constructor with " + paramTypes.Length +
                                    " parameter(s)");
            }

            return ci.Invoke(paramValues);

        }
        public static T InstanceAnyOneParam<T>(Type type, object param1) where T : class
        {
            return InstanceAny(type, new[] { param1.GetType() }, new[] { param1 }) as T;
        }

        public static T InstanceAny<T>(Type type, object param1, object param2) where T : class
        {
            return InstanceAny(type,  new[] { param1.GetType(), param2.GetType() }, new[] { param1, param2 }) as T;
        }

        public static T InstanceAny<T>(Type type, object param1, object param2, object param3) where T : class
        {
            return InstanceAny(type, new[] { param1.GetType(), param2.GetType(), param3.GetType() }, new[] { param1, param2, param3 }) as T;
        }
        public static T InstanceAny<T>(Type type, object param1, object param2, object param3, object param4) where T : class
        {
            return InstanceAny(type, new[] { param1.GetType(), param2.GetType(), param3.GetType(), param4.GetType() }, new[] { param1, param2, param3, param4 }) as T;
        }

        public static T InstanceAny<T>(Type type, object param1) where T : class
        {
            return InstanceAny(type, new[] { param1.GetType() }, new[] { param1 }) as T;
        }

        public static object InstanceGeneric(Type baseType, Type genericType, Type[] paramTypes = null, object[] paramValues = null)
        {
            paramValues = paramValues ?? new object[0];
            paramTypes = paramTypes ?? new Type[0];
            var type = baseType.MakeGenericType(new[] {genericType});

            return InstanceAny(type, paramTypes, paramValues);
        }

        public static T InstanceNonPublic<T>(object param1) where T : class
        {
            return InstanceNonPublic<T>(new[] {param1.GetType()}, new[] {param1});
        }

        public static T InstanceAny<T>(object param1, object param2) where T : class
        {
            return InstanceAny<T>(new[] { param1.GetType(), param2.GetType() }, new[] { param1, param2 });
        }

        public static T InstanceAny<T>(object param1, object param2, object param3) where T : class
        {
            return InstanceAny<T>(new[] { param1.GetType(), param2.GetType(), param3.GetType() }, new[] { param1, param2, param3 });
        }
        public static T InstanceAny<T>(object param1, object param2, object param3, object param4) where T : class
        {
            return InstanceAny<T>(new[] { param1.GetType(), param2.GetType(), param3.GetType(), param4.GetType() }, new[] { param1, param2, param3, param4 });
        }

        public static T InstanceNonPublic<T>(object param1, object param2) where T : class
        {
            return InstanceNonPublic<T>(new[] {param1.GetType(), param2.GetType()}, new[] {param1, param2});
        }

        public static object InstanceNonPublic(Type type, object param1, object param2)
        {
            return InstanceNonPublic(type, new[] { param1.GetType(), param2.GetType() }, new[] { param1, param2 });
        }

        /// <summary>
        /// http://stackoverflow.com/questions/671968/retrieving-property-name-from-lambda-expression
        /// </summary>
        public static MemberInfo GetMemberInfo<T, TKey>(Expression<Func<T, TKey>> expr)
        {
            var body = expr.Body as MemberExpression;
            if (body == null)
            {
                var ubody = (UnaryExpression) expr.Body;
                body = ubody.Operand as MemberExpression;
            }

            if (body == null)
                throw new ArgumentException("Invalid property expression", nameof(expr));

            return body.Member;
        }

        public static T GetMemberValue<T>(this object instance, string memberName)
        {
            var m = instance.GetType()
                .GetMember(memberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (m.Length == 0)
                throw new Exception("Reflection can't find member '" + memberName + "' in '" + instance.GetType() + "'");
            var mi = m[0];

            return GetMemberValue<T>(instance, mi);
        }

        public static T GetMemberValue<T>(this object instance, MemberInfo memberInfo)
        {
            var pi = memberInfo as PropertyInfo;
            if (pi != null)
                return (T)pi.GetValue(instance);

            var fi = memberInfo as FieldInfo;
            if (fi != null)
                return (T)fi.GetValue(instance);

            return default(T);
        }

        public static void SetMemberValue(this object instance, MemberInfo memberInfo, object value)
        {
            var pi = memberInfo as PropertyInfo;
            if (pi != null)
            {
                pi.SetValue(instance, value);
                return;
            }
            var fi = memberInfo as FieldInfo;
            if (fi != null)
            {
                fi.SetValue(instance, value);
            }
        }

        /// <summary>
        /// http://stackoverflow.com/questions/2820660/get-name-of-property-as-a-string
        /// </summary>
        public static string GetPropertyName<TPropertySource, TReturn>(Expression<Func<TPropertySource, TReturn>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression = null;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                if (unaryExpression != null) memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            Debug.Assert(memberExpression != null,
                         "Please provide a lambda expression like 'n => n.PropertyName'");
            var propertyInfo = memberExpression.Member as PropertyInfo;

            return propertyInfo != null ? propertyInfo.Name : null;
        }

        public static string GetPropertyName<TPropertySource>(Expression<Action<TPropertySource>> expression)
        {
            var lambda = expression as LambdaExpression;
            var memberExpression = lambda.Body as MethodCallExpression;

            Debug.Assert(memberExpression != null, "Please provide a lambda expression like 'n => n.MethodName()'");

            return memberExpression.Method.Name;
        }

        public static string GetMemberPath<TPropertySource, TReturn>(Expression<Func<TPropertySource, TReturn>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression = null;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            Debug.Assert(memberExpression != null,
                         "Please provide a lambda expression like 'n => n.PropertyName'");

            var me = memberExpression.ToString();
            return me.Substring(me.IndexOf('.') + 1);
        }


        /// <summary>
        /// Finds all classes in the loaded domain, that can be assigned to the specified 'Type'
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Type> FindClassesImplementing(Type type)
        {
            var result = new List<Type>();
            var iassemblyName = type.GetTypeInfo().Assembly.GetName().Name;
            foreach (var a in AssemblyCatalogue.I.Assemblies) //check all types in the assemblies that reference the 'type's assembly and that can be assigned to 'type'
            {
                if (a.GetName().Name != iassemblyName && !(a.GetReferencedAssemblies().Any(x => x.Name == iassemblyName)))
                    continue;

                try
                {
                    var types = a.GetTypes().Where(t => CheckType(t, type));
                    result.AddRange(types);
                }
                catch (ReflectionTypeLoadException tle)
                {
                   throw new Exception(string.Join(", ", tle.LoaderExceptions.Select(x=>x.Message)) + " This is normally due to an installation issue or missing extension.");
                }
                catch (Exception e)
                {
                    throw new Exception("Problem loading types: " + e);
                }
            }
            return result;
        }

        private static bool CheckType(Type t, Type compareToType)
        {
            return !t.GetTypeInfo().IsGenericType && !t.GetTypeInfo().IsAbstract && compareToType.IsAssignableFrom(t);
        }


        public static (object, MemberInfo) ResolveMemberPath(object container, string memberPath)
        {
            if (!memberPath.Contains('.'))
                return ResolveMember(container, memberPath);

            var parts = memberPath.Split('.');
            return ResolveMemberPath(container, parts);
        }

        private static (object, MemberInfo) ResolveMemberPath(object container, string[] memberPath)
        {
            if (memberPath.Length == 1)
                return ResolveMember(container, memberPath[0]);

            var val = container.GetMemberValue(ResolveMember(container, memberPath[0]).Item2);

            return ResolveMemberPath(val, memberPath.Skip(1).ToArray());
        }

        private static (object, MemberInfo) ResolveMember(object container, string memberName)
        {
            return (container, container.GetType().GetMember(memberName).FirstOrDefault());
        }
    }
}
