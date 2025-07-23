// 这份文件通过xlsx生成，请务必不要更改！！！！！

using Godot;
using System.Linq;
using System.Collections.Generic;

namespace Franken;

public partial class CSV
{
    public static void LoadAll()
    {
        ActorBodyPart.Load();
    }

    public class ActorBodyPart
    {
        public static List<ActorBodyPart> Data { get; private set; }

        private static Dictionary<string, ActorBodyPart> dict;
        public static ActorBodyPart Get(string key) =>
            dict.TryGetValue(key, out var value) ? value : null;

        private ActorBodyPart(string[] data)
        {
            Name = Util.GetString(data[0]);
            Qual = Util.GetEnum<Franken.ActorBodyPart.Quality>(data[1]);
            Comp = Util.GetEnum<Franken.ActorBodyPart.Component>(data[2]);
            Active = Util.GetString(data[3]);
            Passive = Util.GetString(data[4]);
            Hp = Util.GetString(data[5]);
            San = Util.GetString(data[6]);
            Mp = Util.GetString(data[7]);
            Pt = Util.GetString(data[8]);
            Atk = Util.GetString(data[9]);
            Def = Util.GetString(data[10]);
            Agi = Util.GetString(data[11]);
        }

        public string Name { get; init; }
        public Franken.ActorBodyPart.Quality Qual { get; init; }
        public Franken.ActorBodyPart.Component Comp { get; init; }
        public string Active { get; init; }
        public string Passive { get; init; }
        public string Hp { get; init; }
        public string San { get; init; }
        public string Mp { get; init; }
        public string Pt { get; init; }
        public string Atk { get; init; }
        public string Def { get; init; }
        public string Agi { get; init; }

        public static void Load()
        {
            if (Data != null) return;
            Data = [];
            using var fa = FileAccess.Open("res://Assets/Config/CSV/ActorBodyPart.txt", FileAccess.ModeFlags.Read);
            while (!fa.EofReached()) Data.Add(new(fa.GetCsvLine("\t")));
            dict = Data.ToDictionary(data => data.Name);
        }
    }

}