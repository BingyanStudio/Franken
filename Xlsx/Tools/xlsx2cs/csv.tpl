// 这份文件通过xlsx生成，请务必不要更改！！！！！

using Godot;
using System.Linq;
using System.Collections.Generic;

namespace Franken;

public partial class CSV
{
    public static void LoadAll()
    {
        {{- range .}}
        {{.Name}}.Load();
        {{- end}}
    }
    {{- println}}
    {{- range .}}
    public partial class {{.Name}}
    {
        static {{.Name}}() => Load();

        public static List<{{.Name}}> Data { get; private set; }

        private static Dictionary<{{- range .Fields}}{{.Type}}{{- break}}{{- end}}, {{.Name}}> dict;
        public static {{.Name}} Get({{- range .Fields}}{{.Type}}{{- break}}{{- end}} key) =>
            dict.TryGetValue(key, out var value) ? value : null;

        private {{.Name}}(string[] data)
        {
            {{- range $index, $field := .Fields}}
            {{- if eq $field.Type "string"}}
            {{$field.Identifier}} = Util.GetString(data[{{$index}}]);
            {{- else if eq $field.Type "int"}}
            {{$field.Identifier}} = Util.GetInt(data[{{$index}}]);
            {{- else if eq $field.Type "float"}}
            {{$field.Identifier}} = Util.GetInt(data[{{$index}}]);
            {{- else if HasPrefix $field.Type "List"}}
            {{$field.Identifier}} = Util.Get{{$field.Type}}(data[{{$index}}]);
            {{- else}}
            {{$field.Identifier}} = Util.GetEnum<{{$field.Type}}>(data[{{$index}}]);
            {{- end}}
            {{- end}}
        }
        {{- println}}
        {{- range .Fields}}
        public {{.Type}} {{.Identifier}} { get; set; }
        {{- end}}

        public static void Load()
        {
            if (Data != null) return;
            Data = [];
            using var fa = FileAccess.Open("{{.Path}}", FileAccess.ModeFlags.Read);
            while (!fa.EofReached()) {
                var tokens = fa.GetCsvLine("\t");
                if (tokens.All(string.IsNullOrEmpty)) continue;
                Data.Add(new(tokens));
            }
            dict = Data.ToDictionary(data => data.{{- range .Fields}}{{.Identifier}}{{- break}}{{- end}});
        }
    }
    {{- println}}
    {{- end}}
}