using Godot;

namespace Franken;

public static class AnimUtil
{
    public static void PlayAnim(this AnimationPlayer player, string name)
    {
        foreach (var key in player.GetAnimationLibraryList())
        {
            var lib = player.GetAnimationLibrary(key);
            if (!lib.HasAnimation(name)) continue;

            player.Play($"{key}/{name}");
            break;
        }
    }
}