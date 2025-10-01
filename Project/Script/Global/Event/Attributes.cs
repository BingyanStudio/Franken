using Godot;
using System;

namespace Franken;

/// <summary>
/// 事件总线类打上的属性，用于简化<see cref="Signal"/>的调用和监听<br/>
/// 本身无意义
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class EventBusAttribute : Attribute { }