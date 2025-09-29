using System;
using System.Collections.Generic;
using System.Linq;

namespace Franken;

/// <summary>
/// 从表格导入的数据
/// <br/>记得先加载再使用
/// </summary>
public partial class CSV
{
    private static class Util
    {
        public static string GetString(string data) => string.IsNullOrEmpty(data) ? string.Empty : data;

        public static int GetInt(string data) => string.IsNullOrEmpty(data) ? 0 : int.Parse(data);

        public static float GetFloat(string data) => string.IsNullOrEmpty(data) ? 0 : float.Parse(data);

        public static T GetEnum<T>(string data) where T : struct
            => Enum.Parse<T>(string.IsNullOrEmpty(data) ? "0" : data);

        public static List<T> GetList<T>(string data, char delimiter = '|')
        {
            var type = typeof(T).Name;
            var tokens = string.IsNullOrEmpty(data) ? [] : data.Split(delimiter);
            return type switch
            {
                "Int32" => tokens.Select(GetInt).ToList() as List<T>,
                "Single" => tokens.Select(GetFloat).ToList() as List<T>,
                "String" => tokens.Select(GetString).ToList() as List<T>,
                _ => throw new NotImplementedException($"不支持{type}类型")
            };
        }
    }
}