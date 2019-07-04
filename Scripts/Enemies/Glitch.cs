using Godot;
using System;

public class Glitch : KinematicBody2D
{
	//[Export]
	//private Godot.Collections.Array<SpriteFrames> enemySprites;

	[Export]
	private Vector2 followOffset = new Vector2(0, 0);

	private Vector2 motion = new Vector2(0, 0);

	private float speed = 50f;
	private bool move = true;
	private bool shake = false;

	// Refs
	private AnimatedSprite spr;
	private Timer timerDie;

	private PackedScene partsDie = GD.Load<PackedScene>("res://Instances/Particles/PartsEnemyDie.tscn");

	// ================================================================

	public float Speed { set => speed = value; }

	// ================================================================
	
	public override void _Ready()
	{
		spr = GetNode<AnimatedSprite>("Sprite");
		timerDie = GetNode<Timer>("TimerDie");

		//spr.Frames = enemySprites[Mathf.RoundToInt((float)GD.RandRange(0, enemySprites.Count))];
	
		Prompts.AddRandomPrompt(Position, this, followOffset);
	}


	public override void _PhysicsProcess(float delta)
	{
		ZIndex = (int)Position.y;
		
		if (move)
		{
			float angle = GetAngleTo(Player.Main.Position);
			motion = new Vector2(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed);
		}
		else
			motion = new Vector2(0, 0);

		if (shake)
			spr.Position = new Vector2(Mathf.RoundToInt((float)GD.RandRange(-2, 2)), Mathf.RoundToInt((float)GD.RandRange(-2, 2)));

		MoveAndSlide(motion);
	}

	// ================================================================

	private void PromptDestroyed()
	{
		move = false;
		shake = true;
		spr.Play("l_hurt");
		timerDie.Start();
	} 


	private void TimerDie()
	{
		var parts = (Particles2D)partsDie.Instance();
		parts.Position = Position;
		parts.Emitting = true;
		GetTree().GetRoot().AddChild(parts);
		QueueFree();
	}
}
