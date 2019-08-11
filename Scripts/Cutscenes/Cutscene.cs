using Godot;
using System;


public class Cutscene : AnimationPlayer
{
	[Export(PropertyHint.File, "*.txt")]
	protected string textFile = string.Empty;

	// ================================================================
	
	protected void Resume()
	{
		GetParent<CutsceneTrigger>().ResumeEvent();
	}


	protected void Event_Dialogue(int set, Vector2 position)
	{
		GetParent<CutsceneTrigger>().PauseEvent();
		Controller.Dialogue(textFile, set, position, this, nameof(Resume));
	}
}
