using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Dot.Util;

namespace Dot.Extension
{
    public static partial class EnumExtension
    {
        private static ConcurrentDictionary<Type, List<EnumData>> Datas = new ConcurrentDictionary<Type, List<EnumData>>();

        public static List<EnumData> GetEnumDatas(this Type enumType)
        {
            Ensure.True(enumType.IsEnum, "enumType", string.Format("enumType must be typeof enum, current type is {0}", enumType.Name));

            List<EnumData> datas = null;
            if (!Datas.TryGetValue(enumType, out datas))
            {
                datas = enumType.GetFields()
                                .Where(t => !t.Name.Equals("value__"))
                                .Select(field => field.ToEnumData())
                                .ToList();

                Datas.TryAdd(enumType, datas);
            }

            return datas;
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
            return e.GetType().GetEnumDatas().First(data => data.Name == e.ToString()).Description;
        }

        public static string Name(this Enum e)
        {
            return e.GetType().GetEnumDatas().First(t => t.Name == e.ToString()).Name;
        }

        public static int Value(this Enum e)
        {
            return e.GetType().GetEnumDatas().First(t => t.Name == e.ToString()).Value;
        }

        /// <summary>
        /// 位枚举辅助方法，要保证此方法正常工作，须保证：Enum.Value = 1, 2, 4, 8....
        /// </summary>
        public static bool InEnumRange(this Enum source, Enum target)
        {
            if (source.GetType() != target.GetType())
                return false;

            var targetCode = (int)(object)target;
            var sourceCode = (int)(object)source;
            var compareValue = sourceCode & targetCode;

            return compareValue > 0;
        }

        /// <summary>
        /// 位枚举辅助方法，要保证此方法正常工作，须保证：Enum.Value = 1, 2, 4, 8....
        /// </summary>
        public static bool ContainsEnum(this Enum source, Enum target)
        {
            if (source.GetType() != target.GetType())
                return false;

            var targetCode = (int)(object)target;
            var sourceCode = (int)(object)source;
            var compareValue = targetCode & sourceCode;

            return compareValue > 0;
        }
    }

    /// <summary>
    /// 私有扩展方法，不要暴露出去
    /// </summary>
    public static partial class EnumExtension
    {
        private static EnumData ToEnumData(this FieldInfo field)
        {
            var name = field.Name;
            var value = (int)field.GetRawConstantValue();
            var attr = field.GetCustomAttribute<DescriptionAttribute>();
            var description = (attr == null) ? name : attr.Description;

            return new EnumData(name, value, description);
        }

        private static object ToEnumByDescription(this string description, Type enumType)
        {
            var data = enumType.GetEnumDatas().FirstOrDefault(t => t.Description == description);
            return (data == null) ? null : Enum.Parse(enumType, data.Name);
        }

        private static object ToEnumByName(this string name, Type enumType)
        {
            var data = enumType.GetEnumDatas().FirstOrDefault(t => t.Name == name);
            return (data == null) ? null : Enum.Parse(enumType, data.Name);
        }

        private static object ToEnumByValue(this int value, Type enumType)
        {
            var data = enumType.GetEnumDatas().FirstOrDefault(t => t.Value == value);
            return (data == null) ? null : Enum.Parse(enumType, data.Name);
        }
    }

    public class EnumData
    {
        public string Name { get; private set; }
        public int Value { get; private set; }
        public string Description { get; private set; }

        public EnumData(string name, int value, string description = "")
        {
            this.Name = name;
            this.Value = value;
            this.Description = string.IsNullOrEmpty(description) ? name : description;
        }
    }
}