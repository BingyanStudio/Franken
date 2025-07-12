using System.Runtime.InteropServices;

namespace Franken;

/// <summary>
/// 存取数据用的结构体
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct Number : ICustomSerializable
{
    public enum ValueType : uint
    {
        None,
        Int,
        Float,
    }
    [FieldOffset(0)]
    private ValueType type;
    public readonly ValueType Type => type;

    [FieldOffset(4)]
    private int i;
    [FieldOffset(4)]
    private float f;

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

    public readonly void Serialize(CustomSerializeData data)
    {
        data.Add(((int)type).ToString());
        data.Add(
            type switch
            {
                ValueType.Int => i.ToString(),
                ValueType.Float => f.ToString(),
                _ => string.Empty
            }
        );
    }

    public void Deserialize(CustomSerializeData data)
    {
        type = (ValueType)data.Get(0);

        var value = data.Get(string.Empty);
        switch (type)
        {
            case ValueType.Int: i = int.Parse(value); break;
            case ValueType.Float: f = float.Parse(value); break;
        }
    }
}