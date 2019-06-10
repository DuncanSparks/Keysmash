using Godot;
using System;

public class Prompt : Node2D
{
	[Export]
	private AudioStream shatterSound;

	private int textPosition = 0;
	private string promptText = string.Empty;
	private string promptProgress;
	private bool finished = false;
	private bool focused = false;
	private bool follower = false;
	private KinematicBody2D followTarget = null;
	private Vector2 followOffset = new Vector2(0, 0);
	private Vector2 shakeOffset = new Vector2(0, 0);

	// Refs
	private PackedScene partsTypeRef = GD.Load<PackedScene>("res://Instances/Particles/PartsType.tscn");
	private PackedScene partsShatterRef = GD.Load<PackedScene>("res://Instances/Particles/PartsBoxShatter.tscn");

	private Label text;
	private Label textProgress;
	private NinePatchRect box;
	private Sprite cs1;
	private Sprite cs2;
	private Sprite cs3;
	private Sprite cs4;
	private Timer timerShake;
	private Prompts promptController;

	private DynamicFont textFont;

	// ================================================================

	public int TextPosition { get => textPosition; set => textPosition = value; }
	public string PromptText { get => promptText; set => promptText = value; }
	public bool Finished { get => finished; set => finished = value; }
	public bool Focused { get => focused; set => focused = value; }
	public bool Follower { get => follower; set => follower = value; }
	public Vector2 FollowOffset { get => followOffset; set => followOffset = value; }
	public KinematicBody2D FollowTarget { get => followTarget; set => followTarget = value; }
	public DynamicFont TextFont { set => textFont = value; }

	// ================================================================

	public override void _Ready()
	{
		
		text = GetNode<Label>("Text");
		textProgress = GetNode<Label>("TextProgress");
		box = GetNode<NinePatchRect>("NinePatchRect");
		cs1 = GetNode<Sprite>("Crosshair");
		cs2 = GetNode<Sprite>("Crosshair2");
		cs3 = GetNode<Sprite>("Crosshair3");
		cs4 = GetNode<Sprite>("Crosshair4");
		timerShake = GetNode<Timer>("TimerShake");
		promptController = GetParent<Prompts>();
		
		promptProgress = promptText;
		text.Text = promptText;

		//box.MarginRight += textFont.GetStringSize(text.Text).x - 45;//text.RectSize.x;
		//text.MarginLeft -= Mathf.Max(0, ((textFont.GetStringSize(text.Text).x) / 2) - 54);
		//box.MarginLeft += Mathf.max
		//box.MarginLeft -= Mathf.max
		//text.MarginLeft -= Mathf.Max(0, textFont.GetStringSize(text.Text).x);
		//text.GetFont

		box.MarginRight += textFont.GetStringSize(text.Text).x - 5;

		float boxWidth = box.MarginRight - box.MarginLeft;
		cs2.Position += new Vector2(boxWidth, 0);
		cs3.Position += new Vector2(0, 28);
		cs4.Position += new Vector2(boxWidth, 28);
	}


	public override void _Process(float delta)
	{
		if (follower)
			Position = followTarget.Position + followOffset + shakeOffset;

		textProgress.Text = promptText.Substring(textPosition);
		cs1.Visible = focused;
		cs2.Visible = focused;
		cs3.Visible = focused;
		cs4.Visible = focused;

		if (finished)
		{
			Shatter();
			promptController.AnyFocused = false;
			QueueFree();
		}
	}

	// ================================================================

	public void Reset()
	{
		textPosition = 0;
		focused = false;
		promptProgress = promptText;
	}


	public void EmitParts()
	{
		var parts = (Particles2D)partsTypeRef.Instance();
		parts.Position = Position + new Vector2(4 + 4.5f * textPosition, 14);
		//parts.Position = new Vector2(textProgress.MarginLeft, 14);
		parts.Emitting = true;
		GetTree().GetRoot().AddChild(parts);
	}


	public void Shake(int amount)
	{
		//switch (Mathf.RoundToInt((float)GD.RandRange(0, 3)))
		switch (Tools.Choose(0, 1, 2, 3))
		{
			case 0:
				shakeOffset = new Vector2(-amount, 0);
				break;
			case 1:
				shakeOffset = new Vector2(amount, 0);
				break;
			case 2:
				shakeOffset = new Vector2(0, -amount);
				break;
			case 3:
				shakeOffset = new Vector2(0, amount);
				break;
		}

		timerShake.Start();
	}

	// ================================================================

	private void EndShake()
	{
		shakeOffset = new Vector2(0, 0);
	}


	private void Shatter()
	{
		Controller.PlaySoundBurst(shatterSound, (float)GD.RandRange(0.8, 1.05));

		var parts = (Particles2D)partsShatterRef.Instance();
		parts.Position = Position + new Vector2((box.MarginLeft + box.MarginRight) / 2f, (box.MarginTop + box.MarginBottom) / 2f);
		
		float boxWidth = (box.MarginRight - box.MarginLeft) / 2f;
		parts.Amount = Mathf.FloorToInt(boxWidth / 2);
		parts.ProcessMaterial.Set("emission_box_extents", new Vector3(boxWidth, 1, 1));
		
		parts.Emitting = true;
		GetTree().GetRoot().AddChild(parts);
	} 
}
