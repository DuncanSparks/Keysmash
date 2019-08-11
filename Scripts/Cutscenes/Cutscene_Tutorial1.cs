using Godot;
using System;

public class Cutscene_Tutorial1 : Cutscene
{
	[Export]
	private AudioStream slapSound;

	[Export]
	private Texture tutorialGraphic;

	[Export]
	private NodePath glitch1;

	[Export]
	private NodePath glitch2;

	private PackedScene glitchRef = GD.Load<PackedScene>("res://Instances/Enemies/Glitch.tscn");
	private PackedScene tutorialRef = GD.Load<PackedScene>("res://Instances/TutorialControl.tscn");
	private PackedScene tutorialRef2 = GD.Load<PackedScene>("res://Instances/TutorialDash.tscn");

	private Glitch g1;
	private Glitch g2;

	// ================================================================
	
	private void Event_SpawnGlitches()
	{
		g1 = (Glitch)glitchRef.Instance();
		g1.NoAi = true;
		g1.Position = Player.Main.Position + new Vector2(300, -10);
		g1.Motion = new Vector2(-220f, 0);
		GetTree().GetRoot().AddChild(g1);
		g1.PlayAnimation("l_walk");

		g2 = (Glitch)glitchRef.Instance();
		g2.NoAi = true;
		g2.Position = Player.Main.Position + new Vector2(300, 10);
		g2.Motion = new Vector2(-220f, 0);
		GetTree().GetRoot().AddChild(g2);
		g2.PlayAnimation("l_walk");
	}


	private void Event_DestroyGlitches()
	{
		g1.QueueFree();
		g2.QueueFree();
	}


	private void Event_PlayerDizzy()
	{
		Player.PlayAnimation("Spin");
		Player.PlaySpriteAnimation("dr_dizzy");
	}


	private void Event_PlayerAnim(string anim)
	{
		Player.PlaySpriteAnimation(anim);
	}


	private void Event_PunchSound()
	{
		Controller.PlaySoundBurst(slapSound);
	}


	private void Event_ShowMoveTutorial()
	{
		var tut = (CanvasLayer)tutorialRef.Instance();
		//tut.Pos = new Vector2(50, 50);
		//tut.Spr = tutorialGraphic;
		//tut.Text = "MOVE";
		//tut.Duration = 8f;
		//tut.TextOffset = new Vector2(0, 40);
		GetTree().GetRoot().AddChild(tut);
	}


	private void Event_LockCamera()
	{
		Player.PlayerCamera.Current = false;
		GetNode<Glitch>(glitch1).NoAi = false;
		GetNode<Glitch>(glitch2).NoAi = false;
		var tut = (CanvasLayer)tutorialRef2.Instance();
		GetTree().GetRoot().AddChild(tut);
	}
}
