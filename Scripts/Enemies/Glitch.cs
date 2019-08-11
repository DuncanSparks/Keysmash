using Godot;
using System;

public class Glitch : KinematicBody2D
{
	//[Export]
	//private Godot.Collections.Array<SpriteFrames> enemySprites;

	[Export]
	private Vector2 followOffset = new Vector2(0, 0);

	[Export]
	private float speed = 50f;

	[Export]
	private AudioStream deathSound;

	[Export]
	private bool noAi = false;

	private Vector2 motion = new Vector2(0, 0);

	
	private bool move = true;
	private bool stunnable = true;
	private bool stunned = false;
	private bool shake = false;

	// Refs
	private AnimatedSprite spr;
	private AnimationPlayer animPlayer;
	private Timer timerStunBuffer;
	private Timer timerStun;
	private Timer timerStunCooldown;
	private Timer timerDie;

	private CollisionShape2D coll;

	private PackedScene partsDie = GD.Load<PackedScene>("res://Instances/Particles/PartsEnemyDie.tscn");

	// ================================================================

	public float Speed { set => speed = value; }
	public bool NoAi { set => noAi = value; }
	public Vector2 Motion { set => motion = value; }

	// ================================================================
	
	public override void _Ready()
	{
		spr = GetNode<AnimatedSprite>("Sprite");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		timerStunBuffer = GetNode<Timer>("TimerStunBuffer");
		timerStun = GetNode<Timer>("TimerStun");
		timerStunCooldown = GetNode<Timer>("TimerStunCooldown");
		timerDie = GetNode<Timer>("TimerDie");

		coll = GetNode<CollisionShape2D>("CollisionShape2D");

		//spr.Frames = enemySprites[Mathf.RoundToInt((float)GD.RandRange(0, enemySprites.Count))];
	}


	public override void _PhysicsProcess(float delta)
	{
		ZIndex = (int)Position.y;
		
		if (!noAi)
		{
			if (move)
			{
				float angle = GetAngleTo(Player.Main.Position);
				motion = new Vector2(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed);
			}
			else
				motion = new Vector2(0, 0);

			if (shake)
				spr.Position = new Vector2(Mathf.RoundToInt((float)GD.RandRange(-2, 2)), Mathf.RoundToInt((float)GD.RandRange(-2, 2)));
		
			Animate();
		}

		MoveAndSlide(motion);
	}

	// ================================================================

	public void SpawnPrompt()
	{
		Prompts.AddRandomPrompt(Position, this, followOffset);
	}


	public void PlayAnimation(string anim)
	{
		spr.Play(anim);
	}


	public void Stun()
	{
		move = false;
		stunned = true;
		coll.Disabled = true;
		//spr.Play("l_dizzy");
		animPlayer.Play("Spin");
		timerStun.Start();
	}

	// ================================================================

	private void Animate()
	{
		if (move)
			spr.Play(motion.x < 0 ? "l_walk" : "r_walk");
		else
			spr.Play(motion.x < 0 ? "l" : "r");
	
		if (stunned)
			spr.Play(motion.x < 0 ? "l_dizzy" : "r_dizzy");

		if (shake)
			spr.Play(motion.x < 0 ? "l_hurt" : "r_hurt");
	}


	private void HBEntered(Area2D area)
	{
		if (!stunned && stunnable && area.IsInGroup("PlayerHB") && Player.State == Player.ST.Dash)
			timerStunBuffer.Start();
	}


	private void PromptDestroyed()
	{
		move = false;
		shake = true;
		spr.Play(motion.x < 0 ? "l_hurt" : "r_hurt");
		timerDie.Start();
	}


	private void StunEnd()
	{
		move = true;
		coll.Disabled = false;
		spr.Play(motion.x < 0 ? "l" : "r");
		stunnable = false;
		stunned = false;
		animPlayer.Play("Stunbar");
		timerStunCooldown.Start();
	}


	private void StunCooldown()
	{
		stunnable = true;
	}


	private void TimerDie()
	{
		Controller.PlaySoundBurst(deathSound, pitch: (float)GD.RandRange(0.9, 1.1));
		var parts = (Particles2D)partsDie.Instance();
		parts.Position = Position;
		parts.Emitting = true;
		GetTree().GetRoot().GetNode<Node2D>("Stage").GetNode<LevelController>("LevelController").WaveProgress++;
		GetTree().GetRoot().AddChild(parts);
		QueueFree();
	}
}
