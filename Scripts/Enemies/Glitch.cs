using Godot;
using System;

public class Glitch : KinematicBody2D
{
	//[Export]
	//private Godot.Collections.Array<SpriteFrames> enemySprites;

	[Export]
	private Vector2 followOffset = new Vector2(0, 0);

	// Refs
	private AnimatedSprite spr;

	// ================================================================
	
	public override void _Ready()
	{
		spr = GetNode<AnimatedSprite>("Sprite");

		//spr.Frames = enemySprites[Mathf.RoundToInt((float)GD.RandRange(0, enemySprites.Count))];
	
		Prompts.AddRandomPrompt(Position, this, followOffset);
	}
}
