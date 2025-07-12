using System;

namespace Franken;

/// <summary>
/// 从表格导入的数据
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
    }
}