using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Dialogue : Node2D
{
	[Signal]
	public delegate void dialogue_ended();

	[Export]
	private AudioStream textSound;

	[Export]
	private AudioStream pageSound;

	private List<string> text = new List<string>();
	private int textPage = 0;

	private string textFile = string.Empty;
	private int textSet = -1;

	private int disp = 0;
	private int pageLength = 0;
	private bool roll = false;
	private bool buffer = false;

	private Regex setRegex = new Regex(@"^\[(\d+)\]$");

	// Refs
	private Label label;
	private AnimationPlayer animPlayer;

	// ================================================================

	public string TextFile { set => textFile = value; }
	public int TextSet { set => textSet = value; }

	// ================================================================

	public override void _Ready()
	{
		label = GetNode<CanvasLayer>("CanvasLayer").GetNode<Label>("Text");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}


	public override void _Process(float delta)
	{
		label.Text = text[textPage];
		label.VisibleCharacters = disp;

		if (Input.IsActionJustPressed("sys_select") && !buffer)
		{
			if (disp >= text[textPage].Length)
				AdvancePage();
			else
			{
				disp = text[textPage].Length;
				buffer = true;
				GetNode<Timer>("TimerBuffer").Start();
			}
		}
	}

	// ================================================================

	public void SetBoxPosition(Vector2 pos)
	{
		GetNode<CanvasLayer>("CanvasLayer").Offset = pos;
	}


	public void Start()
	{
		LoadTextFromFile();
		animPlayer.Play("Start");
	}

	// ================================================================

	private void LoadTextFromFile()
	{
		File file = new File();
		try
		{
			file.Open(textFile, (int)File.ModeFlags.Read);
			bool read = false;
			while (!file.EofReached())
			{
				string line = file.GetLine();

				if (line.Length > 0 && line[0] == '#' && read)
				{
					read = false;
					break;
				}

				if (read)
				{
					text.Add(line.StripEdges());
					continue;
				}

				if (line.Length > 0 && line[0] == '[')
				{
					var lineMatch = setRegex.Match(line);
					if (lineMatch.Groups[1].ToString().ToInt() == textSet)
						read = true;
				}
			}

		}
		finally
		{
			if (file.IsOpen())
				file.Close();
		}
	}


	private void Start2()
	{
		pageLength = text[textPage].Replace(" ", "").Length;
		roll = true;
		GetNode<Timer>("TimerRollText").Start();
	}


	private void AdvancePage()
	{
		if (textPage < text.Count - 1)
		{
			textPage++;
			disp = 0;
			pageLength = text[textPage].Replace(" ", "").Length;
			roll = true;
			GetNode<Timer>("TimerRollText").Start();
			buffer = true;
			GetNode<Timer>("TimerBuffer").Start();
		}
		else
		{
			//EmitSignal(nameof(dialogue_ended));
			// play animation
			animPlayer.Play("End");
		}
	}


	private void RollText()
	{
		if (roll && disp < text[textPage].Length)
		{
			if (disp < pageLength)
				Controller.PlaySoundBurst(textSound, -4f, (float)GD.RandRange(0.98, 1.02));
			
			disp++;
		}
		else
			GetNode<Timer>("TimerRollText").Stop();
	}


	private void ResetBuffer()
	{
		buffer = false;
	}


	private void End()
	{
		//GD.Print("TEST");
		EmitSignal(nameof(dialogue_ended));
		QueueFree();
	}
}
