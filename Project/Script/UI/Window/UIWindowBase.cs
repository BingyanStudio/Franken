using Godot;
using System;
using System.Threading.Tasks;

namespace Franken;

public abstract partial class UIWindowBase : Control
{
    [Export]
    protected WindowConfig Config { get; private set; }

    public event Action OnInit;
    public event Action OnUninit;

    private AnimationPlayer animPlayer;

    public virtual void Setup()
    {
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        GetRef();
    }

    public async Task Show(bool playAnim)
    {
        if (playAnim) await PlayAnim("show");
        Init();
    }

    public async Task Hide(bool playAnim)
    {
        if (playAnim) await PlayAnim("hide");
        Uninit();
    }

    protected virtual void Init() => OnInit?.Invoke();
    protected virtual void Uninit() => OnUninit?.Invoke();

    protected virtual void GetRef() { }

    private async Task PlayAnim(string anim)
    {
        if (animPlayer == null || !animPlayer.HasAnimation(anim)) return;

        animPlayer.Play(anim);
        await Task.Delay((int)(animPlayer.CurrentAnimationLength * 1000));
    }
}
