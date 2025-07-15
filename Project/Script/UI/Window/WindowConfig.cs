using Godot;

namespace Franken;

[GlobalClass]
public partial class WindowConfig : Resource
{
    [Export]
    public int ID { get; set; }
}