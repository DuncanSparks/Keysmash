using Godot;
using System;
using System.Collections.Generic;

public class Prompts : Node
{
	public static Prompts Main;

	Prompts()
	{
		Main = this;
	}

	[Export]
	private string promptFile = string.Empty;

	[Export]
	private AudioStream typeSound;

	[Export]
	private AudioStream cancelSound;

	// ================================================================

	private bool anyFocused = false;
	private int focusedIndex = 0;
	private List<string> promptsList = new List<string>();

	private Dictionary<string, string> keyCodes = new Dictionary<string, string>()
	{
		{"key_a", "A"},
		{"key_b", "B"},
		{"key_c", "C"},
		{"key_d", "D"},
		{"key_e", "E"},
		{"key_f", "F"},
		{"key_g", "G"},
		{"key_h", "H"},
		{"key_i", "I"},
		{"key_j", "J"},
		{"key_k", "K"},
		{"key_l", "L"},
		{"key_m", "M"},
		{"key_n", "N"},
		{"key_o", "O"},
		{"key_p", "P"},
		{"key_q", "Q"},
		{"key_r", "R"},
		{"key_s", "S"},
		{"key_t", "T"},
		{"key_u", "U"},
		{"key_v", "V"},
		{"key_w", "W"},
		{"key_x", "X"},
		{"key_y", "Y"},
		{"key_z", "Z"},
		{"key_1", "1"},
		{"key_2", "2"},
		{"key_3", "3"},
		{"key_4", "4"},
		{"key_5", "5"},
		{"key_6", "6"},
		{"key_7", "7"},
		{"key_8", "8"},
		{"key_9", "9"},
		{"key_0", "0"},
		{"key_comma", ","},
		{"key_period", "."},
		{"key_question", "?"},
		{"key_exclamation", "!"},
		{"key_hash", "#"},
		{"key_dollarsign", "$"},
		{"key_percent", "%"},
		{"key_ampersand", "&"},
		{"key_openparen", "("},
		{"key_closeparen", ")"},
		{"key_hyphen", "-"},
		{"key_plus", "+"},
		{"key_equal", "="},
		{"key_slash", "/"},
		{"key_apostrophe", "'"},
		{"key_quote", "\""},
		{"key_colon", ":"},
		{"key_semicolon", ";"},
	};

	// Refs
	private PackedScene promptRef = GD.Load<PackedScene>("res://Instances/Prompt.tscn");
	private DynamicFont textFont = GD.Load<DynamicFont>("res://Fonts/TextFont.tres");

	private AudioStreamPlayer soundType;

	// ================================================================

	public bool AnyFocused { get => Prompts.Main.anyFocused; set => Prompts.Main.anyFocused = value; }
	public int FocusedIndex { get => Prompts.Main.focusedIndex; set => Prompts.Main.focusedIndex = value; }
	public List<string> PromptsList { get => Prompts.Main.promptsList; }

	// ================================================================

	public override void _Ready()
	{
		File file = new File();
		try
		{
			file.Open(promptFile, (int)File.ModeFlags.Read);
			while (!file.EofReached())
			{
				string line = file.GetLine();
				promptsList.Add(line);
			}
		}
		finally
		{
			if (file.IsOpen())
				file.Close();
		}
	}


	public override void _Process(float delta)
	{
		// Debug
		if (Input.IsActionJustPressed("debug_1"))
			AddPrompt(PromptsList[Mathf.RoundToInt((float)GD.RandRange(0, PromptsList.Count - 1))], new Vector2((int)GD.RandRange(0, 300), (int)GD.RandRange(0, 180)), null, new Vector2(0, 0));

		if (Input.IsActionJustPressed("cancel_prompt") && anyFocused)
		{
			Controller.PlaySoundBurst(cancelSound, 0.5f, 1.5f);
			GetChild<Prompt>(focusedIndex).Reset();
			anyFocused = false;
			return;
		}

		if (Input.IsActionJustPressed("select_up") && anyFocused)
		{
			GetChild<Prompt>(focusedIndex).Reset();
			focusedIndex = Mathf.Wrap(focusedIndex - 1, 0, GetChildren().Count);
			GetChild<Prompt>(focusedIndex).Focused = true;
			return;
		}

		if (Input.IsActionJustPressed("select_down") && anyFocused)
		{
			GetChild<Prompt>(focusedIndex).Reset();
			focusedIndex = Mathf.Wrap(focusedIndex + 1, 0, GetChildren().Count);
			GetChild<Prompt>(focusedIndex).Focused = true;
			return;
		}

		if (anyFocused)
		{
			var p = GetChild<Prompt>(focusedIndex);
			foreach (var action in keyCodes)
			{
				if (Input.IsActionJustPressed(action.Key) && p.PromptText[p.TextPosition].ToString().ToUpper() == action.Value)
				{
					IncrementPosition(ref p);
					return;
				}
			}
		}
		else
		{
			for (int i = 0; i < GetChildren().Count; i++)
			{
				var p = GetChild<Prompt>(i);
				foreach (var action in keyCodes)
				{
					if (Input.IsActionJustPressed(action.Key) && p.PromptText[p.TextPosition].ToString().ToUpper() == action.Value)
					{
						p.Focused = true;
						IncrementPosition(ref p);
						focusedIndex = i;
						anyFocused = true;
						return;
					}	
				}
			}
		}
	}

	// ================================================================

	public static void AddPrompt(string text, Vector2 position, KinematicBody2D followTarget, Vector2 followOffset)
	{
		var promptInst = (Prompt)Prompts.Main.promptRef.Instance();
		promptInst.PromptText = text;
		promptInst.Position = position;
		promptInst.TextFont = Prompts.Main.textFont;

		if (followTarget != null)
		{
			promptInst.FollowTarget = followTarget;
			promptInst.Follower = true;
			promptInst.FollowOffset = followOffset;
		}

		promptInst.Connect("Destroyed", followTarget, "PromptDestroyed");

		Prompts.Main.AddChild(promptInst);
	}


	public static void IncrementPosition(ref Prompt prompt)
	{
		Controller.PlaySoundBurst(Prompts.Main.typeSound, 0.2f, (float)GD.RandRange(1d, 1.2));
		prompt.EmitParts();
		prompt.Shake(Tools.Choose(1, 2));

		prompt.TextPosition++;
		if (prompt.TextPosition >= prompt.PromptText.Length)
		{
			prompt.Finished = true;
			return;
		}

		if (!prompt.Finished)
			while (prompt.PromptText[prompt.TextPosition] == ' ')
				prompt.TextPosition++;
	}


	public static void AddRandomPrompt(Vector2 position, KinematicBody2D followTarget, Vector2 followOffset)
	{
		AddPrompt(Prompts.Main.PromptsList[Mathf.RoundToInt((float)GD.RandRange(0, Prompts.Main.PromptsList.Count - 1))], position, followTarget, followOffset);
	}
}
