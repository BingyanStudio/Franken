using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Franken;

/// <summary>
/// 存取数据用的结构体
/// </summary>
[StructLayout(LayoutKind.Explicit), JsonConverter(typeof(NumberJsonConverter))]
public readonly struct Number
{
    public enum ValueType : uint
    {
        None,
        Int,
        Float,
    }
    [FieldOffset(0)]
    private readonly ValueType type;
    public readonly ValueType Type => type;

    private const char separator = '|';

    [FieldOffset(4)]
    private readonly int i;
    [FieldOffset(4)]
    private readonly float f;

    public Number(string data) : this()
    {
        var raw = data[0] - '0';
        type = (ValueType)raw;

        switch (type)
        {
            case ValueType.Int: i = int.Parse(data[2..]); break;
            case ValueType.Float: f = float.Parse(data[2..]); break;
        }
    }

    public Number(int i) : this()
    {
        this.i = i;
        type = ValueType.Int;
    }

    public Number(float f) : this()
    {
        this.f = f;
        type = ValueType.Float;
    }

    public static implicit operator int(Number u)
    {
        if (u.type != ValueType.Int) LogTool.Error("DataStruct", $"当前Union不是int而是{u.type}！");
        return u.i;
    }

    public static implicit operator float(Number u)
    {
        if (u.type != ValueType.Float) LogTool.Error("DataStruct", $"当前Union不是float而是{u.type}！");
        return u.f;
    }

    public override readonly string ToString() => ((int)type).ToString() + separator + type switch
    {
        ValueType.Int => i.ToString(),
        ValueType.Float => f.ToString(),
        _ => string.Empty
    };
}

public class NumberJsonConverter : JsonConverter<Number>
{
    public override Number Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options) => new(reader.GetString());
    public override void Write(Utf8JsonWriter writer, Number value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
}