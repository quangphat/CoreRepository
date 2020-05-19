using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CoreBusiness
{
    public static class TypeExtensions
    {
        public static T GetDefaultValue<T>()
        {
            return (T)GetDefaultValue(typeof(T));
        }
        public static object GetDefaultValue(this Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
