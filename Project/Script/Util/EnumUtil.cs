using System;

namespace Franken;

public static class EnumUtil
{
    public static void SerializeToNum<T>(T e, CustomSerializeData data)
        where T : Enum => data.Add(e.GetHashCode().ToString());

    public static T DeserializeFromNum<T>(CustomSerializeData data)
        where T : struct => Enum.Parse<T>(data.Get("0"));
}