using System;

namespace Franken;

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
///         OnIdxChange?.(idx, value);
///         idx = value;
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