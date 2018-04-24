﻿using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour {

	[SerializeField] private TextMesh scoreText;

	private void UpdateScore(int _score)
	{
		scoreText.text = "Score: " + _score;
	}

	private void OnEnable()
	{
		LevelProgess.ScoreUpdatedEvent += UpdateScore;
	}

	private void OnDisable()
	{
		LevelProgess.ScoreUpdatedEvent -= UpdateScore;
	}
}