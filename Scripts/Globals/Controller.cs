using Godot;
using System;
//using System.Collections.Generic;

public class Controller : Node
{
	private static Controller Main;

	Controller()
	{
		Main = this;
	}

	// ================================================================

	private static readonly string[] colors = {"#82bb44", "#bb4444", "#bbb344", "#448bbb", "#6a44bb", "#808080"};
	private int screenColorIndex = -1;

	private bool showHealthbar = false;

	// Refs
	private PackedScene soundBurstRef = GD.Load<PackedScene>("res://Instances/SoundBurst.tscn");
	private PackedScene dialogueRef = GD.Load<PackedScene>("res://Instances/Dialogue.tscn");

	private ColorRect overlay;
	private StaticBody2D walls;

	// ================================================================

	public static bool ShowHealthbar { get => Controller.Main.showHealthbar; set => Controller.Main.showHealthbar = value; }

	// ================================================================
	
	public override void _Ready()
	{
		GD.Randomize();

		overlay = GetNode<CanvasLayer>("CanvasLayer").GetNode<ColorRect>("ColorRect");
		walls = GetNode<StaticBody2D>("PlayerWalls");

		var m = (ShaderMaterial)overlay.Material;
		int color = Mathf.RoundToInt((float)GD.RandRange(0, colors.Length - 1));
		m.SetShaderParam("color", new Color(colors[color]));
		screenColorIndex = color;
		//m.SetShaderParam("color", new Color(Tools.Choose(colors)));
	}


	public override void _Process(float delta)
	{
		//GD.Print(GetViewport().CanvasTransform.origin);
		walls.Position = new Vector2(-GetViewport().CanvasTransform.origin.x - 40, 0);

		if (Input.IsActionJustPressed("sys_fullscreen"))
			OS.WindowFullscreen = !OS.WindowFullscreen;

		if (Input.IsActionJustPressed("sys_color"))
		{
			screenColorIndex = Mathf.Wrap(++screenColorIndex, 0, colors.Length);
			var m = (ShaderMaterial)overlay.Material;
			m.SetShaderParam("color", new Color(colors[screenColorIndex]));
		}
	}

	// ================================================================

	public static void Dialogue(string textFile, int set, Vector2 position, Node connection = null, string connectionMethod = "")
	{
		var dlg = (Dialogue)Controller.Main.dialogueRef.Instance();
		//dlg.Position = new Vector2(Player.PlayerCamera.GetCameraPosition().x + position.x - 160, position.y);
		dlg.SetBoxPosition(position);
		dlg.TextFile = textFile;
		dlg.TextSet = set;
		Controller.Main.GetTree().GetRoot().AddChild(dlg);
		if (connection != null)
			dlg.Connect("dialogue_ended", connection, connectionMethod);

		dlg.Start();
	}


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
