using Godot;
using System;
using System.Collections.Generic;

public class Controller : Node
{
	private static Controller Main;

	Controller()
	{
		Main = this;
	}

	// ================================================================

	private static readonly string[] colors = {"#82bb44", "#bb4444", "#bbb344", "#448bbb", "#6a44bb", "#808080"};

	// Refs
	private PackedScene soundBurstRef = GD.Load<PackedScene>("res://Instances/SoundBurst.tscn");

	private ColorRect overlay;

	// ================================================================
	
	public override void _Ready()
	{
		GD.Randomize();

		overlay = GetNode<CanvasLayer>("CanvasLayer").GetNode<ColorRect>("ColorRect");
	
		var m = (ShaderMaterial)overlay.Material;
		m.SetShaderParam("color", new Color(Tools.Choose(colors)));
	}


	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("sys_fullscreen"))
			OS.WindowFullscreen = !OS.WindowFullscreen;
	}

	// ================================================================

	public static void PlaySoundBurst(AudioStream sound, float volume = 0f, float pitch = 1f)
	{
		var sb = (AudioStreamPlayer)Controller.Main.soundBurstRef.Instance();
		sb.Stream = sound;
		sb.VolumeDb = volume;
		sb.PitchScale = pitch;
		sb.Play();
		Controller.Main.GetTree().GetRoot().AddChild(sb);
	}
}
