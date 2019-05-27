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
	private ColorRect overlay;

	// ================================================================
	
	public override void _Ready()
	{
		GD.Randomize();

		overlay = GetNode<CanvasLayer>("CanvasLayer").GetNode<ColorRect>("ColorRect");
	
		var m = (ShaderMaterial)overlay.Material;
		m.SetShaderParam("color", new Color(Tools.Choose(colors)));
	}
}