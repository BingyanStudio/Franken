// 这份文件通过xlsx生成，请务必不要更改！！！！！

using Godot;
using System.Linq;
using System.Collections.Generic;

namespace Franken;

public partial class CSV
{
    public static void LoadAll()
    {
        ActiveConf.Load();
        ActorBodyPart.Load();
        PassiveConf.Load();
    }

    public partial class ActiveConf
    {
        static ActiveConf() => Load();

        public static List<ActiveConf> Data { get; private set; }

        private static Dictionary<string, ActiveConf> dict;
        public static ActiveConf Get(string key) =>
            dict.TryGetValue(key, out var value) ? value : null;

        private ActiveConf(string[] data)
        {
            ID = Util.GetString(data[0]);
            Name = Util.GetString(data[1]);
            Desc = Util.GetString(data[2]);
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }

        public static void Load()
        {
            if (Data != null) return;
            Data = [];
            using var fa = FileAccess.Open("res://Assets/Config/CSV/ActiveConf.txt", FileAccess.ModeFlags.Read);
            while (!fa.EofReached()) {
                var tokens = fa.GetCsvLine("\t");
                if (tokens.All(string.IsNullOrEmpty)) continue;
                Data.Add(new(tokens));
            }
            dict = Data.ToDictionary(data => data.ID);
        }
    }

    public partial class ActorBodyPart
    {
        static ActorBodyPart() => Load();

        public static List<ActorBodyPart> Data { get; private set; }

        private static Dictionary<string, ActorBodyPart> dict;
        public static ActorBodyPart Get(string key) =>
            dict.TryGetValue(key, out var value) ? value : null;

        private ActorBodyPart(string[] data)
        {
            ID = Util.GetString(data[0]);
            Name = Util.GetString(data[1]);
            Qual = Util.GetEnum<Quality>(data[2]);
            Comp = Util.GetEnum<Component>(data[3]);
            Active = Util.GetList<string>(data[4]);
            Passive = Util.GetList<string>(data[5]);
            Hp = Util.GetString(data[6]);
            San = Util.GetString(data[7]);
            Cmp = Util.GetString(data[8]);
            Pti = Util.GetString(data[9]);
            Pth = Util.GetString(data[10]);
            Def = Util.GetString(data[11]);
            Agi = Util.GetString(data[12]);
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public Quality Qual { get; set; }
        public Component Comp { get; set; }
        public List<string> Active { get; set; }
        public List<string> Passive { get; set; }
        public string Hp { get; set; }
        public string San { get; set; }
        public string Cmp { get; set; }
        public string Pti { get; set; }
        public string Pth { get; set; }
        public string Def { get; set; }
        public string Agi { get; set; }

        public static void Load()
        {
            if (Data != null) return;
            Data = [];
            using var fa = FileAccess.Open("res://Assets/Config/CSV/ActorBodyPart.txt", FileAccess.ModeFlags.Read);
            while (!fa.EofReached()) {
                var tokens = fa.GetCsvLine("\t");
                if (tokens.All(string.IsNullOrEmpty)) continue;
                Data.Add(new(tokens));
            }
            dict = Data.ToDictionary(data => data.ID);
        }
    }

    public partial class PassiveConf
    {
        static PassiveConf() => Load();

        public static List<PassiveConf> Data { get; private set; }

        private static Dictionary<string, PassiveConf> dict;
        public static PassiveConf Get(string key) =>
            dict.TryGetValue(key, out var value) ? value : null;

        private PassiveConf(string[] data)
        {
            ID = Util.GetString(data[0]);
            Name = Util.GetString(data[1]);
            Desc = Util.GetString(data[2]);
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }

        public static void Load()
        {
            if (Data != null) return;
            Data = [];
            using var fa = FileAccess.Open("res://Assets/Config/CSV/PassiveConf.txt", FileAccess.ModeFlags.Read);
            while (!fa.EofReached()) {
                var tokens = fa.GetCsvLine("\t");
                if (tokens.All(string.IsNullOrEmpty)) continue;
                Data.Add(new(tokens));
            }
            dict = Data.ToDictionary(data => data.ID);
        }
    }

}