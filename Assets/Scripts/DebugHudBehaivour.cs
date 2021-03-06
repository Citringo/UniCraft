﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DebugHudBehaivour : BaseBehaviour {

	PlayerEntity player;

	bool IsDebugMode = false;

	const string DebugMessageFormat = "Unicraft {0}\n"
									+ "(C)2018 Xeltica\n"
									+ "\n"
									+ "FPS:{1}\n"
									+ "\n"
									+ "Pp:{2} Pv:{3} Pg:{4}\n"
									+ "Hand:{5}\n"
									+ "\n"
									+ "Day:{6} Time:{7}\n"
									+ "\n";

	private Text text;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		player = GameMaster.Instance.Player;

		if (player != null)
		{
			text.text = string.Format(DebugMessageFormat, GameMaster.Version, 1 / Time.deltaTime, player.transform.position, player.Velocity, player.IsGrounded, UniCraft.BlockIdInHand, Daylight.Day, Daylight.GameTime);
		
			// Looking Block Debug
			if (!player.LookingBlock.Equals(default(LocationInfo)))
				text.text += $"Looking At {player.LookingBlock.BlockId} @ {player.LookingBlock.Location}";
		}
	}
}
