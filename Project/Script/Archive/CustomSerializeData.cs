using System;
using System.IO;

namespace Franken;

/// <summary>
/// 自定义序列化接口
/// </summary>
public interface ICustomSerializable
{
    void Serialize(CustomSerializeData data);
    void Deserialize(CustomSerializeData data);
}

/// <summary>
/// 自定义序列化数据
/// <br/>存储：
/// <code>
/// using var data = new CustomSerializeData();
/// target.Serialize(data);
/// data.Save(path);
/// </code>
/// <br/>读取：
/// <code>
/// using var data = new CustomSerializeData();
/// data.Load(path);
/// target.Deserialize(data);
/// </code>
/// <br/>其中target继承自<see cref="ICustomSerializable"/>
/// </summary>
public sealed class CustomSerializeData : IDisposable
{
    #region Serialize
    private LinkList<string> list = new();

    public void Add(string data) => list.Add(data);

    private bool Null() => list.Empty || string.IsNullOrEmpty(list.Current);

    public string Get(string fallback) => Null() ? fallback : list.Next;

    /// <param name="func">非空时，处理字符串，得到目标类型</param>
    /// <param name="fallback">遇到空值时返回这个默认值</param>
    public T Get<T>(Func<string, T> func, T fallback) => Null() ? fallback : func(list.Next);

    public int Get(int fallback) => Get(int.Parse, fallback);
    public float Get(float fallback) => Get(float.Parse, fallback);

    public void Save(string path)
    {
        EnsureDir(path);

        using var sw = new StreamWriter(path);
        while (!list.Empty)
        {
            LogTool.Message("Archive", list.Current);
            sw.WriteLine(list.Next);
        }
        sw.Flush();
    }

    public void Load(string path)
    {
        EnsureDir(path);
        if (!File.Exists(path))
        {
            using var fs = File.Create(path);
            fs.Flush();
        }

        using var rw = new StreamReader(path);
        while (!rw.EndOfStream)
        {
            var line = rw.ReadLine();
            LogTool.Message("Archive", line);
            Add(line);
        }
    }

    private static void EnsureDir(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
    }
    #endregion

    #region Dispose
    private bool disposed;

    private void Dispose(bool disposing)
    {
        if (disposed) return;
        if (disposing) list = null;
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CustomSerializeData() => Dispose(false);
    #endregion
}
