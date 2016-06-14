using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Dot.Extension
{
    public static class EnumExtension
    {
        private static ConcurrentDictionary<Type, List<dynamic>> EnumDatas = new ConcurrentDictionary<Type, List<dynamic>>();

        public static List<dynamic> GetEnumInfos(this Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException(string.Format("Type {0} is not enum", enumType.FullName));

            List<dynamic> datas = null;
            if (!EnumDatas.TryGetValue(enumType, out datas))
            {
                datas = new List<dynamic>();
                var fields = enumType.GetFields().Where(t => !t.Name.Equals("value__"));
                foreach (var field in fields)
                {
                    var value = (int)field.GetRawConstantValue();
                    var name = field.Name;
                    var description = name;
                    var attr = field.GetCustomAttribute<DescriptionAttribute>();
                    if (attr != null)
                        description = attr.Description;

                    dynamic data = new ExpandoObject();
                    data.Value = value;
                    data.Name = name;
                    data.Description = description;

                    datas.Add(data);
                }

                EnumDatas.TryAdd(enumType, datas);
            }

            return datas;
        }

        public static object ToEnumByDescription(this string description, Type enumType)
        {
            var info = enumType.GetEnumInfos().Where(t => t.Description == description).FirstOrDefault();
            if (info != null)
                return Enum.Parse(enumType, info.Name);
            else
                return null;
        }

        public static object ToEnumByName(this string name, Type enumType)
        {
            var info = enumType.GetEnumInfos().Where(t => t.Name == name).FirstOrDefault();
            if (info != null)
                return Enum.Parse(enumType, info.Name);
            else
                return null;
        }

        public static object ToEnumByValue(this int value, Type enumType)
        {
            var info = enumType.GetEnumInfos().Where(t => t.Value == value).FirstOrDefault();
            if (info != null)
                return Enum.Parse(enumType, info.Name);
            else
                return null;
        }

        public static T ToEnumByName<T>(this string name)
        {
            return (T)name.ToEnumByName(typeof(T));
        }

        public static T ToEnumByDescription<T>(this string description)
        {
            return (T)description.ToEnumByDescription(typeof(T));
        }

        public static T ToEnumByValue<T>(this int value)
        {
            return (T)value.ToEnumByValue(typeof(T));
        }

        public static string Description(this Enum e)
        {
            var info = e.GetType().GetEnumInfos().Where(t => t.Name == e.ToString()).FirstOrDefault();
            if (info != null)
                return info.Description;
            else
                return null;
        }

        public static string Name(this Enum e)
        {
            var info = e.GetType().GetEnumInfos().Where(t => t.Name == e.ToString()).FirstOrDefault();
            if (info != null)
                return info.Name;
            else
                return null;
        }

        public static int Value(this Enum e)
        {
            var info = e.GetType().GetEnumInfos().Where(t => t.Name == e.ToString()).FirstOrDefault();
            if (info != null)
                return info.Value;
            else
                return -1;
        }

        public static bool ChildOf(this Enum source, Enum target)
        {
            if (source.GetType() != target.GetType())
                return false;

            var targetCode = (int)(object)target;
            var sourceCode = (int)(object)source;
            var compareValue = sourceCode & targetCode;

            return compareValue > 0;
        }

        public static bool ParentOf(this Enum source, Enum target)
        {
            if (source.GetType() != target.GetType())
                return false;

            var targetCode = (int)(object)target;
            var sourceCode = (int)(object)source;
            var compareValue = targetCode & sourceCode;

            return compareValue > 0;
        }
    }
}