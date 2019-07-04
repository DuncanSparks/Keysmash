using Godot;
using System;

public class Title : Node2D
{
	[Export]
	private AudioStream cursorSound;

	private int menuPos = 0;

	// Refs
	private Control options;

	// ================================================================
	
	public override void _Ready()
	{
		options = GetNode<Control>("Options");
	}


	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("sys_up"))
		{
			Controller.PlaySoundBurst(cursorSound, pitch: (float)GD.RandRange(1, 1.1));
			menuPos = Mathf.Wrap(menuPos - 1, 0, 4);
		}
		

		if (Input.IsActionJustPressed("sys_down"))
		{
			Controller.PlaySoundBurst(cursorSound, pitch: (float)GD.RandRange(1, 1.1));
			menuPos = Mathf.Wrap(menuPos + 1, 0, 4);
		}
			

		for (int i = 0; i < 4; i++)
		{
			if (i == menuPos)
				options.GetChild<Label>(i).Modulate = new Color(1, 1, 1);
			else
				options.GetChild<Label>(i).Modulate = new Color(0.85f, 0.85f, 0.85f);
		}
	}

	// ================================================================

	private void SetMenuPos(int pos)
	{
		Controller.PlaySoundBurst(cursorSound, pitch: (float)GD.RandRange(1, 1.1));
		menuPos = pos;
	}
}
