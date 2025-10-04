using System;

namespace Franken;

/// <summary>
/// 通过形如
/// <code>
/// [UIRef]
/// private GameObject itsMyGO = 0;
/// </code>
/// 的代码，在<see cref="UIWindowBase.GetRef"/>中生成
/// <code>itsMyGO = children["ItsMyGO"].First() as GameObject;</code>
/// 由于严格通过名称索引，请务必保证名称唯一且正确！！！！！
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class UIRefAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ObservableObjectAttribute : Attribute { }

/// <summary>
/// 通过形如
/// <code>
/// [ObservableProperty]
/// private int idx = 0;
/// </code>
/// 的代码，生成
/// <code>
/// public Action&lt;int, int&gt; OnIdxChange;
/// public int Idx
/// {
///     get => idx;
///     set 
///     {
///         if (idx == value) return;
///         var temp = idx;
///         idx = value;
///         OnIdxChange?.(temp, value);
///     }
/// }
/// </code>
/// 特别的，如果<see cref="Validate"/>不为空，那么会使用同名方法进行数据验证
/// <br/>假如上例中使用的是
/// <code>[ObservableProperty("IsPositive")]</code>
/// 那么setter会在第一行添加
/// <code>if (!IsPositive(value)) return;</code>
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class ObservablePropertyAttribute : Attribute
{
    public string Validate { get; set; }
    public ObservablePropertyAttribute(string validate = null) => Validate = validate;
}

/// <summary>
/// 通过形如
/// <code>
/// [ObservableClampProperty(minimum: "minidx", maximum: "maxidx")]
/// private int idx = 0;
/// </code>
/// 的代码，生成
/// <code>
/// public Action&lt;int, int&gt; OnIdxChange;
/// public int Idx
/// {
///     get => idx;
///     set 
///     {
///         value = Math.Clamp(value, minidx, maxidx);
///         if (idx == value) return;
///         var temp = idx;
///         idx = value;
///         OnIdxChange?.(temp, value);
///     }
/// }
/// </code>
/// 特别的，如果<see cref="Validate"/>不为空，那么会使用同名方法进行数据验证
/// <br/>假如上例中使用的是
/// <code>[ObservableProperty("IsPositive")]</code>
/// 那么setter会在第一行添加
/// <code>if (!IsPositive(value)) return;</code>
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class ObservableClampPropertyAttribute : ObservablePropertyAttribute
{
    public string Minimum { get; set; }
    public string Maximum { get; set; }

    public ObservableClampPropertyAttribute(string minimum = null, string maximum = null, string validate = null) : base(validate)
    {
        Minimum = minimum;
        Maximum = maximum;
    }
}