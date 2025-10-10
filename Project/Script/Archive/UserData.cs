using System.Collections.Generic;

namespace Franken;

/// <summary>
/// 存档数据
/// </summary>
public class UserData
{
    public static UserData Current => Archive.Data[Archive.Current];

    public List<string> ActorBodyParts { get; set; }

    public List<ActorBodyData> ActorBodies { get; set; }
}