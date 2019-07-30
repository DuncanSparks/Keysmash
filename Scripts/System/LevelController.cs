using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using LevelList = System.Collections.Generic.List<System.Tuple<int, System.Collections.Generic.List<System.Tuple<string, Godot.Vector2>>>>;

public class LevelController : Node2D
{
	[Export(PropertyHint.File, "*.txt")]
	private string infoFile = string.Empty;

	//private List<List<Tuple<string, Vector2>>> levelData = new List();
	private LevelList levelData = new LevelList();

	private int levelProgress = 0;
	private int levelSize = 0;
	private bool stopped = false;
	private int waveProgress = 0;
	private int waveSize = 1;

	private Regex indexRegex = new Regex(@"\[(\d+)\]");
	private Regex lineRegex = new Regex(@"(\w+)\s+(\d+)\s+(\d+)");

	// Enemy references
	private PackedScene enemyGlitch = GD.Load<PackedScene>("res://Instances/Enemies/Glitch.tscn");

	// ================================================================

	public int WaveProgress { get => waveProgress; set => waveProgress = value; }

	// ================================================================
	
	public override void _Ready()
	{
		ReadInfoFile();

		//PrintLevelData();
	}


	public override void _Process(float delta)
	{
		if (!stopped)
		{
			if (levelProgress < levelSize && Player.PlayerCamera.GetCameraPosition().x >= levelData[levelProgress].Item1)
			{
				stopped = true;
				Player.PlayerCamera.Current = false;
				var currentEnemies = levelData[levelProgress].Item2;
				waveProgress = 0;
				waveSize = currentEnemies.Count;
				SpawnEnemies(ref currentEnemies);
				levelProgress++;
			}
		}
		else
		{
			if (waveProgress >= waveSize)
			{
				stopped = false;
				Player.PlayerCamera.Current = true;
			}
		}
	}

	// ================================================================

	
	private void ReadInfoFile()
	{
		File file = new File();
		try
		{
			file.Open(infoFile, (int)File.ModeFlags.Read);
			bool read = false;
			var currentList = new List<Tuple<string, Vector2>>();
			int currentCoord = 0;

			while (!file.EofReached())
			{
				string line = file.GetLine();
				if (line.Length > 0)
				{
					switch (line[0])
					{
						case '[':
						{
							var currentLine = indexRegex.Match(line);
							currentCoord = Int32.Parse(currentLine.Groups[1].ToString());
							levelSize++;
							break;
						}

						case '}':
						{
							read = false;
							levelData.Add(new Tuple<int, List<Tuple<string, Vector2>>>(currentCoord, currentList));
							break;
						}

						case '{':
						{
							read = true;
							currentList.Clear();
							break;
						}

						default:
						{
							if (read)
							{
								var currentLine = lineRegex.Match(line);
								currentList.Add(new Tuple<string, Vector2>(currentLine.Groups[1].ToString(), new Vector2(Int32.Parse(currentLine.Groups[2].ToString()), Int32.Parse(currentLine.Groups[3].ToString()))));
							}

							break;
						}
					}
				}
			}
		}
		finally
		{
			if (file.IsOpen())
				file.Close();
		}
	}


	private void SpawnEnemies(ref List<Tuple<string, Vector2>> enemies)
	{
		foreach (var enemy in enemies)
		{
			switch (enemy.Item1)
			{
				case "Glitch":
				{
					var inst = (Glitch)enemyGlitch.Instance();
					inst.Position = enemy.Item2;
					GetTree().GetRoot().AddChild(inst);
					break;
				}

				// Other enemy types

				default:
				{
					var inst = (Glitch)enemyGlitch.Instance();
					inst.Position = enemy.Item2;
					GetTree().GetRoot().AddChild(inst);
					break;
				}
			}
		}
	}


	#if DEBUG
	private void PrintLevelData()
	{
		foreach (var pair in levelData)
		{
			string s = $"Spawn at {pair.Item1}:\n";
			foreach (var enemy in pair.Item2)
			{
				s += $"{enemy.Item1}({enemy.Item2.x}, {enemy.Item2.y})\n";
			} 

			GD.Print(s);
		}
	}
	#endif
	
}
