using CommonLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace CommonLibrary.Extensions
{
    public static class ReflectionExtensions
    {
        public static T GetCustomAttribute<T>(this MemberInfo memberInfo)
            where T : Attribute
        {
            var attributes = memberInfo.GetCustomAttributes(typeof(T), true);
            if (attributes.Length > 0)
            {
                return (T)attributes[0];
            }
            else
            {
                return null;
            }
        }

        public static T GetCustomAttribute<T>(this Assembly assembly)
            where T : Attribute
        {
            var attributes = assembly.GetCustomAttributes(typeof(T), true);
            if (attributes.Length > 0)
            {
                return (T)attributes[0];
            }
            else
            {
                return null;
            }
        }

        public static bool HasAttribute<T>(this PropertyInfo propertyInfo)
            where T : Attribute
        {
            return propertyInfo.GetCustomAttribute<T>() != null;
        }

        public static void SetPropertyValue(this PropertyInfo propertyInfo, object targetObj, object value)
        {
            object valueToSet = null;

            if (value != null && value != DBNull.Value)
            {
                var propertyType = propertyInfo.PropertyType;

                if (propertyType.IsEnum)
                {
                    valueToSet = Enum.ToObject(propertyType, value);
                }
                else if (propertyType.IsValueType && propertyType.IsNullable())
                {
                    Type underlyingType = Nullable.GetUnderlyingType(propertyType);
                    valueToSet = underlyingType.IsEnum ? Enum.ToObject(underlyingType, value) : Convert.ChangeType(value, underlyingType);
                }
                else
                {
                    valueToSet = Convert.ChangeType(value, propertyType);
                }
            }

            propertyInfo.SetValue(targetObj, valueToSet, null);
        }

        public static bool IsNullable(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return true;
            else
                return false;
        }

        public static string GetAssociatedColumnName(this PropertyInfo prop)
        {
            var columnNameAttribute = prop.GetCustomAttribute<ColumnNameAttribute>();
            if (columnNameAttribute != null)
            {
                return columnNameAttribute.ColumnName;
            }
            else
            {
                return prop.Name;
            }
        }

        public static IEnumerable<Type> GetTypeListWhichImplements<I>(this Assembly assembly)
        {
            var assemblyTypes = assembly.GetTypes();
            return assemblyTypes.Where(t => t.GetInterfaces().Contains(typeof(I)) && t != typeof(I)).ToList();
        }

        public static IEnumerable<Assembly> GetAssembliesInDirectory(this AppDomain domain, string relativePath)
        {
            string binPath = Path.Combine(domain.BaseDirectory, relativePath);

            foreach (string dll in Directory.GetFiles(binPath, "*.dll", SearchOption.AllDirectories))
            {
                Assembly loadedAssembly = null;

                try
                {
                    loadedAssembly = Assembly.LoadFile(dll);
                }
                catch (FileLoadException) // Zaten yüklenmiş
                { }
                catch (BadImageFormatException) // Assembly değil
                { }

                if (loadedAssembly != null)
                    yield return loadedAssembly;
            }
        }

        public static bool IsAnonymous(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }
}