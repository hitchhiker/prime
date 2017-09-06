using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Prime.Utility
{
    public static class ReflectionExtensionMethods
    {
        public static IEnumerable<Type> Implements<T>(this IEnumerable<Type> source, bool includeAbstractClasses = false, bool includeInterfaces = false)
        {
            var type = typeof(T);
            return Implements(source, type, includeAbstractClasses, includeInterfaces);
        }

        public static IEnumerable<Type> Implements(this IEnumerable<Type> source, Type type, bool includeAbstractClasses = false, bool includeInterfaces = false)
        {
            return source.Where(x => x != null && (includeInterfaces || x.GetTypeInfo().IsClass) && (includeAbstractClasses || !x.GetTypeInfo().IsAbstract) && type.IsAssignableFrom(x));
        }

        public static List<Type> Implements<T>(this List<Type> source, bool includeAbstractClasses = false, bool includeInterfaces = false)
        {
            return source.AsEnumerable().Implements<T>(includeAbstractClasses, includeInterfaces).ToList();
        }

        public static List<Type> Implements(this List<Type> source, Type type, bool includeAbstractClasses = false, bool includeInterfaces = false)
        {
            return source.AsEnumerable().Implements(type, includeAbstractClasses, includeInterfaces).ToList();
        }

        public static List<T> ImplementInstances<T>(this IEnumerable<Type> source, bool includeAbstractClasses = false, bool includeInterfaces = false) where T : class
        {
            var type = typeof(T);
            return Implements(source, type, includeAbstractClasses, includeInterfaces).Select(x=> x.InstanceAny<T>()).Where(x=>x!=null).ToList();
        }

        public static List<T> ImplementInstancesWith<T>(this IEnumerable<Type> source, params object[] o) where T : class
        {
            var type = typeof(T);

            T Selector(Type t)
            {
                var types = o.Select(x => x.GetType()).ToArray();
                return Reflection.InstanceAny(t, types, o) as T;
            }

            return Implements(source, type).Select(Selector).Where(x => x != null).ToList();
        }

        public static List<T> ImplementInstancesUninitialised<T>(this IEnumerable<Type> source, bool includeAbstractClasses = false, bool includeInterfaces = false) where T : class
        {
            var type = typeof(T);
            return Implements(source, type, includeAbstractClasses, includeInterfaces).Select(x => x.InstanceUninitialised<T>()).Where(x => x != null).ToList();
        }

        public static List<T> ImplementInstancesReal<T>(this IEnumerable<Type> source, bool includeAbstractClasses = false, bool includeInterfaces = false) where T : class, IGetRealInstance
        {
            return ImplementInstancesUninitialised<T>(source, includeAbstractClasses, includeInterfaces).Select(x=>x.GetRealInstance<T>()).ToList();
        }

        public static List<T> ImplementInstancesI<T>(this IEnumerable<Type> source, bool includeAbstractClasses = false, bool includeInterfaces = false) where T : class
        {
            return Implements<T>(source, includeAbstractClasses, includeInterfaces).Select(x => x.GetPublicStaticProperty<T>("I")).ToList();
        }

        public static T InstanceAny<T>(this Type type) where T : class
        {
            return Reflection.InstanceAny(type) as T;
        }

        public static T InstanceUninitialised<T>(this Type type) where T : class
        {
            return FormatterServices.GetUninitializedObject(type) as T;
        }

        public static T Instance<T>(this CustomAttributeData customAttributeData) where T : class
        {
            return customAttributeData.Constructor.Invoke(customAttributeData.ConstructorArguments.Select(x => x.Value).ToArray()) as T;
        }

        /// <summary>
        /// http://stackoverflow.com/a/1650895/1318333
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Boolean IsAnonymous(this Type type)
        {
            return type.GetTypeInfo().GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any() && type.FullName.Contains("AnonymousType");
        }

        public static Boolean HasBsonAttributes(this Type type)
        {
            return type.GetMembers().Any(c=>c.GetCustomAttributes().Any(x => x.GetType().Name.StartsWith("Bson")));
        }

        public static IEnumerable<CustomAttributeData> InheritedCustomAttributes(this Type type)
        {
            while (true)
            {
                if (type == null)
                    yield break;

                var ti = type.GetTypeInfo();

                foreach (var i in ti.CustomAttributes)
                    yield return i;

                if (ti.BaseType == typeof(object))
                    yield break;

                type = ti.BaseType;
            }
        }

        public static IEnumerable<CustomAttributeData> InheritedCustomAttributes(this Type type, MemberInfo member)
        {
            while (true)
            {
                if (type == null)
                    yield break;

                var mi = type.GetMember(member.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (mi.Length == 0)
                {
                    type = type.GetTypeInfo().BaseType;
                    continue;
                }

                foreach (var i in mi[0].CustomAttributes)
                    yield return i;

                var ti = type.GetTypeInfo();

                if (ti.BaseType == typeof(object))
                    yield break;

                type = ti.BaseType;
            }
        }

        public static T InheritedCustomAttributeInstance<T>(this Type type, MemberInfo m) where T : Attribute
        {
            var ca = type.InheritedCustomAttributes(m).FirstOrDefault(x => x.AttributeType == typeof(T));
            return ca == null ? null : ca.Instance<T>();
        }

        public static IEnumerable<CustomAttributeData> InheritedCustomAttributes<T>(this Type type, MemberInfo m) where T : Attribute
        {
            return type.InheritedCustomAttributes(m).Where(x => x.AttributeType == typeof(T));
        }

        public static Type GetNonGenericBaseType(this Type type)
        {
            while (true)
            {
                if (type == null)
                    return null;

                var ti = type.GetTypeInfo();

                if (!ti.IsGenericType)
                    return type;

                type = ti.BaseType;
            }
        }

        public static T GenericMethod<T>(this object instance, string methodName, params object[] objects)
            where T : class
        {
            var itype = instance.GetType();
            var mtd = itype.GetMethod(methodName);
            var genericArguments = new[] { itype };
            var genericMethodInfo = mtd.MakeGenericMethod(genericArguments);
            return genericMethodInfo.Invoke(null, objects) as T;
        }

        public static void Invoke(this object instance, string methodName, params object[] objects)
        {
            if (instance == null)
                throw new Exception("You can't invoke '" + methodName + "' on a null instance.");

            var itype = instance.GetType();
            var mtd = itype.GetMethod(methodName,
                                      BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic |
                                      BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy);
            try
            {
                mtd.Invoke(instance, objects);
            }
            catch (Exception e)
            {
                throw new Exception("Problem invoking method '" + methodName + "' from '" + instance.GetType(),
                                    e.InnerException);
            }
        }

        public static void SetField(this object instance, string fieldName, object value)
        {
            if (instance == null)
                throw new Exception("You can't invoke '" + fieldName + "' on a null instance.");

            var itype = instance.GetType();
            var mtd = itype.GetField(fieldName,
                                        BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic |
                                        BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy);
            try
            {
                mtd.SetValue(instance, value);
            }
            catch (Exception e)
            {
                throw new Exception("Problem setting field '" + fieldName + "' on '" + instance.GetType(),
                                    e.InnerException);
            }
        }

        public static void SetProperty(this object instance, string propertyName, object value)
        {
            if (instance == null)
                throw new Exception("You can't invoke '" + propertyName + "' on a null instance.");

            var itype = instance.GetType();
            var mtd = itype.GetProperty(propertyName,
                                        BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic |
                                        BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy);
            try
            {
                mtd.SetValue(instance, value);
            }
            catch (Exception e)
            {
                throw new Exception("Problem invoking set '" + propertyName + "' on '" + instance.GetType(),
                                    e.InnerException);
            }
        }

        public static void SetProperty<T, TKey>(this T instance, Expression<Func<T, TKey>> selector, object value)
        {
            if (selector == null)
                throw new Exception("You can't invoke 'SetInternalProperty' without specifying a 'selector'.");

            SetProperty(instance, Reflection.GetPropertyName(selector), value);
        }

        private static string GetShortName(this string fullname)
        {
            var io = fullname.IndexOf(',');
            return io == -1 ? fullname : fullname.Substring(0, io);
        }

        /// <summary>
        /// This only hashes the actual assembly name, not the version or any other part of 'FullName'
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static int GetHashShortName(this Assembly assembly)
        {
            return assembly.GetShortName().GetHashCode();
        }

        public static string GetShortName(this Assembly assembly)
        {
            return assembly.FullName.GetShortName();
        }

        public static string GetShortName(this Type type)
        {
            return type.FullName.GetShortName();
        }

        /// <summary>
        /// This only hashes the short name, not the version or any other part of 'FullName'.
        /// It then appends assembly the short type name.
        /// </summary>
        public static int GetHashShortName(this Type type)
        {
            return (type.GetShortName() + ":" + type.GetTypeInfo().Assembly.GetShortName()).GetHashCode();
        }

        /// <summary>
        /// This only hashes the short name, not the version or any other part of 'FullName'.
        /// It then appends assembly the short type name.
        /// </summary>
        public static int GetHashShortName(this object instance)
        {
            return instance.GetType().GetHashShortName();
        }

        public static object GetMemberValue(this object container, MemberInfo mi)
        {
            switch (mi.MemberType)
            {
                case MemberTypes.Property:
                    var pi = mi as PropertyInfo;
                    return pi.GetValue(container);
                case MemberTypes.Field:
                    var fi = mi as FieldInfo;
                    return fi.GetValue(container);
                default:
                    throw new Exception(mi.MemberType + " is not supported.");
            }
        }

        public static Type GetMemberType(this MemberInfo mi)
        {
            switch (mi.MemberType)
            {
                case MemberTypes.Property:
                    var pi = mi as PropertyInfo;
                    return pi.PropertyType;
                case MemberTypes.Field:
                    var fi = mi as FieldInfo;
                    return fi.FieldType;
                default:
                    throw new Exception(mi.MemberType + " is not supported.");
            }
        }

        public static bool HasCustomAttribute<T>(this MemberInfo mi) where T: Attribute
        {
            return mi.CustomAttributes.Any(x => x.AttributeType == typeof (T));
        }

        public static T GetPublicStaticProperty<T>(this Type type, string propertyName)
        {
            // Get a PropertyInfo of specific property type(T).GetProperty(....)
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
            if (propertyInfo==null)
                throw new Exception("No static public property '" + propertyName + "' on class " + type.GetShortName());

            // Use the PropertyInfo to retrieve the value from the type by not passing in an instance
            var value = propertyInfo.GetValue(null, null);

            // Cast the value to the desired type
            return (T)value;
        }
    }
}
