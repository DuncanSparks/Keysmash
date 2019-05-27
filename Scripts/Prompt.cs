using Godot;
using System;

public class Prompt : Node2D
{
	private int textPosition = 0;
	private string promptText = string.Empty;
	private string promptProgress;
	private bool finished = false;
	private bool focused = false;

	// Refs
	private Label text;
	private Label textProgress;
	private NinePatchRect box;
	private Sprite arrow;
	private Prompts promptController;

	private DynamicFont textFont;

	// ================================================================

	public int TextPosition { get => textPosition; set => textPosition = value; }
	public string PromptText { get => promptText; set => promptText = value; }
	public bool Finished { get => finished; set => finished = value; }
	public bool Focused { get => focused; set => focused = value; }
	public DynamicFont TextFont { set => textFont = value; }

	// ================================================================

	public override void _Ready()
	{
		text = GetNode<Label>("Text");
		textProgress = GetNode<Label>("TextProgress");
		box = GetNode<NinePatchRect>("NinePatchRect");
		arrow = GetNode<Sprite>("Arrow");
		promptController = GetParent<Prompts>();

		promptProgress = promptText;
		text.Text = promptText;

		box.MarginRight += textFont.GetStringSize(text.Text).x - 45;//text.RectSize.x;
		//text.MarginLeft -= Mathf.Max(0, ((textFont.GetStringSize(text.Text).x) / 2) - 54);
		//box.MarginLeft += Mathf.max
		//box.MarginLeft -= Mathf.max
		//text.MarginLeft -= Mathf.Max(0, textFont.GetStringSize(text.Text).x);
		//text.GetFont
	}


	public override void _Process(float delta)
	{
		textProgress.Text = promptText.Substring(textPosition);
		arrow.Visible = focused;

		if (finished)
		{
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
}
