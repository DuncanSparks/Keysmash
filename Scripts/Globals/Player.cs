//#define DEBUG_DRAW

using Godot;
using System;

public class Player : KinematicBody2D
{
	public static Player Main;

	Player()
	{
		Main = this;
	}

	// ================================================================

	//[Export]
	//private float jumpHeight = 0f;
	
	private float speed = 0f;
	private Vector2 velocity = new Vector2(0, 0);
	private Vector2 floor;
	//private float acceleration = 0f;

	private float axisX;
	private float axisY;
	//p/rivate float dashStrength = 1f;
	
	public enum ST {Ground, Air, Dash, NoInput};
	private ST state = ST.Ground;

	public enum Direction {UL, DL, UR, DR};
	private Direction dir = Direction.DR;
	
	private const float speedCap = 120f;
	private const float dashStrength = 450f;
	private readonly Vector2 dash = new Vector2(dashStrength, dashStrength);
	private const float dashDecay = 57f;
	private const float acceleration = 400f;

	// Refs
	private AnimatedSprite spr;
	//private CollisionShape2D coll;
	private Timer timerDash;
	private Particles2D partsRun;
	private Camera2D playerCamera;
	private AnimationPlayer animPlayer;

	// ================================================================

	public static ST State { get => Player.Main.state; set => Player.Main.state = value; }
	public static Camera2D PlayerCamera { get => Player.Main.playerCamera; }

	// ================================================================

	public override void _Ready()
	{
		spr = GetNode<AnimatedSprite>("Sprite");
		timerDash = GetNode<Timer>("TimerDash");
		partsRun = GetNode<Particles2D>("PartsRun");
		playerCamera = GetNode<Camera2D>("Camera2D");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}


	public override void _Process(float delta)
	{
		partsRun.Emitting = Mathf.Abs(axisX) > 0.5 || Mathf.Abs(axisY) > 0.5;

		#if DEBUG_DRAW
			Update();
		#endif
	}


	public override void _PhysicsProcess(float delta)
	{
		ZIndex = (int)Position.y;

		axisX = Input.GetActionStrength("player_moveright") - Input.GetActionStrength("player_moveleft");
		axisY = Input.GetActionStrength("player_movedown") - Input.GetActionStrength("player_moveup");

		//if (axisX != 0 && axisY != 0)
		//acceleration = Mathf.Clamp(acceleration + (axisX != 0 || axisY != 0 ? 1f : -1f) * delta, 0, 1);

		switch (state)
		{
			case ST.Ground:
			{
				velocity.x = axisX * speedCap;
				velocity.y = axisY * speedCap;


				if (Input.IsActionJustPressed("player_dash"))
				{
					velocity = dash * new Vector2(axisX, axisY).Normalized();
					state = ST.Dash;
					timerDash.Start();
				}

				ChangeDirection(ref axisX, ref axisY);
				Animation();
				/* if (Input.IsActionJustPressed("player_moveleft"))
					animPlayer.Play("FlipLeft");

				if (Input.IsActionJustPressed("player_moveright"))
					animPlayer.Play("FlipRight"); */

				break;
			}

			case ST.Dash:
			{
				velocity *= dashDecay * delta;

				break;
			}
		}

		MoveAndSlide(velocity);
		//MoveAndSlide(velocity * acceleration);
		//GD.Print(Performance.GetMonitor(Performance.Monitor.TimeFps));
	}


#if DEBUG_DRAW
	public override void _Draw()
	{
		DrawLine(new Vector2(0, 0), new Vector2(Input.GetActionStrength("player_moveright") - Input.GetActionStrength("player_moveleft"), Input.GetActionStrength("player_movedown") - Input.GetActionStrength("player_moveup")) * 80f, new Color(1, 1, 1, 1), 5);
	}
#endif

	// ================================================================

	private void ChangeDirection(ref float xAxis, ref float yAxis) // Refactor this??
	{
		if (xAxis < 0)
		{
			if (yAxis < 0)
				dir = Direction.UL;
			else if (yAxis > 0)
				dir = Direction.DL;
			else
			{
				if (dir == Direction.UR)
					dir = Direction.UL;
				else if (dir == Direction.DR)
					dir = Direction.DL;
			}
		}
		else if (xAxis > 0)
		{
			if (yAxis < 0)
				dir = Direction.UR;
			else if (yAxis > 0)
				dir = Direction.DR;
			else
			{
				if (dir == Direction.UL)
					dir = Direction.UR;
				else if (dir == Direction.DL)
					dir = Direction.DR;
			}
		}
		else
		{
			if (yAxis < 0)
			{
				if (dir == Direction.DL)
					dir = Direction.UL;
				else if (dir == Direction.DR)
					dir = Direction.UR;
			}
			else if (yAxis > 0)
			{
				if (dir == Direction.UL)
					dir = Direction.DL;
				else if (dir == Direction.UR)
					dir = Direction.DR;
			}
		}
	}


	private void Animation()
	{
		switch (dir)
		{
			/* case Direction.UL:
				spr.Play("ul");
				break;
			case Direction.DL:
			{
				spr.SpeedScale = Mathf.Max(-axisX, axisY) < 0.5 ? Mathf.Max(-axisX, axisY) : 1;
				spr.Play(axisX != 0 || axisY != 0 ? Mathf.Max(-axisX, axisY) >= 0.5 ? "dl_run" : "dl_walk" : "dl");
				break;
			}
			case Direction.UR:
				spr.Play("ur");
				break;
			case Direction.DR:
			{
				spr.SpeedScale = Mathf.Max(axisX, axisY) < 0.5 ? Mathf.Max(axisX, axisY) : 1;
				spr.Play(axisX != 0 || axisY != 0 ? Mathf.Max(axisX, axisY) >= 0.5 ? "dr_run" : "dr_walk" : "dr");
				break;
			} */

			case Direction.UL:
			{
				spr.SpeedScale = Mathf.Max(-axisX, axisY) < 0.5 ? Mathf.Max(-axisX, axisY) : 1;
				spr.Play(state == ST.Dash ? "dl_slide" : axisX != 0 || axisY != 0 ? Mathf.Max(-axisX, axisY) >= 0.5 ? "dl_run" : "dl_walk" : "dl");
				break;
			}
			case Direction.DL:
			{
				spr.SpeedScale = Mathf.Max(-axisX, axisY) < 0.5 ? Mathf.Max(-axisX, axisY) : 1;
				spr.Play(state == ST.Dash ? "dl_slide" : axisX != 0 || axisY != 0 ? Mathf.Max(-axisX, axisY) >= 0.5 ? "dl_run" : "dl_walk" : "dl");
				break;
			}
			case Direction.UR:
			{
				spr.SpeedScale = Mathf.Max(axisX, axisY) < 0.5 ? Mathf.Max(axisX, axisY) : 1;
				spr.Play(state == ST.Dash ? "dr_slide" : axisX != 0 || axisY != 0 ? Mathf.Max(axisX, axisY) >= 0.5 ? "dr_run" : "dr_walk" : "dr");
				break;
			}
			case Direction.DR:
			{
				spr.SpeedScale = Mathf.Max(axisX, axisY) < 0.5 ? Mathf.Max(axisX, axisY) : 1;
				spr.Play(state == ST.Dash ? "dr_slide" : axisX != 0 || axisY != 0 ? Mathf.Max(axisX, axisY) >= 0.5 ? "dr_run" : "dr_walk" : "dr");
				break;
			}
				
		}
	}


	private void TimerDashTimeout()
	{
		state = ST.Ground;
	}
}
