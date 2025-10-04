using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Franken;

/// <summary>
/// Ingame Debug控制台
/// </summary>
public partial class DebugConsole : CanvasLayer
{
    private const string ToggleActionName = "debug";

    [Export] private LineEdit commandText;
    [Export] private RichTextLabel historyLog;

    public override void _Ready()
    {
        if (!OS.IsDebugBuild())
        {
            QueueFree();
            return;
        }

        this.ProcessMode = ProcessModeEnum.Always;
        this.Visible = false;

        commandText.TextSubmitted += SendCommand;
    }

    private void SendCommand(string command)
    {
        // 切分字段
        var response = DebugConsoleUtil.Process(command.Split(' '));

        // 显示对话
        historyLog.Text += $"[color=#838383]- {command}[/color]\n";
        historyLog.Text += $"{response}\n";

        commandText.Text = string.Empty;
        // 抢夺Focus实现平滑连续执行
        commandText.ReleaseFocus();
        commandText.GrabFocus();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(ToggleActionName))
        {
            this.Visible = !this.Visible;
            GetTree().Paused = this.Visible;

            // 打开可以直接输入
            if (this.Visible) commandText.GrabFocus();
        }
    }
}
