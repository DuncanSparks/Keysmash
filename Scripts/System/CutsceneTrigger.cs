using Godot;
using System;

public class CutsceneTrigger : Area2D
{
	//[Export]
	//private bool destroy = true;

	private bool running = false;

	// Refs
	private AnimationPlayer animPlayer;

	// ================================================================
	
	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	// ================================================================

	public void PauseEvent()
	{
		animPlayer.Stop(false);
	}


	public void ResumeEvent()
	{
		animPlayer.Play();
	}

	// ================================================================

	private void StartEvent()
	{
		animPlayer.Play("Cutscene");
	}


	private void EndEvent(string anim_name)
	{
		if (anim_name == "Cutscene")
		{
			Player.State = Player.ST.Ground;
			QueueFree();
		}
	}


	private void BodyEntered(PhysicsBody2D body)
	{
		if (body.IsInGroup("Player") && Player.State == Player.ST.Ground && !running)
		{
			Player.Stop();
			Player.State = Player.ST.NoInput;
			StartEvent();
			running = true;
		}
	}
}
