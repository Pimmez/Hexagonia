﻿using UnityEngine;
using System.Collections;
using System;

public class ResourceValue : MonoBehaviour
{

	public static ResourceValue Instance { get { return GetInstance(); } }

    #region Instance
    private static ResourceValue instance;

    private static ResourceValue GetInstance()
    {
        if(instance == null)
        {
            instance = FindObjectOfType<ResourceValue>();
        }
        return instance;
    }
    #endregion

    public float Value { get { return resourceValue; } set { resourceValue = value; } }

	private const float MAXVALUE = 1f; 
	private const float MINVALUE = 0f; 

	private float resourceValue;

	[Tooltip("Wait for seconds(timeBetweenCoroutines), A higher number increases the wait time.")]
	[SerializeField] private float timeBetweenCoroutines = 1f;
	[SerializeField] private float resouceIncreaseOnPickup = 0.3f;
	[SerializeField] private float increaseSpeed = 0.5f;
	[SerializeField] private float decreaseSpeed = 0.5f;
	[SerializeField] private float maxValue = 5;

	private float targetValue;
	private Coroutine coroutineIncrease, coroutineDecrease;

	private void Awake()
	{
		resourceValue = 0;
		ResourceBarUI.Instance.UpdateResourceBar();
	}

	private void OnScoreUpdated(int _score)
	{
        if(coroutineDecrease == null)
        {
            float _newValue = targetValue + resouceIncreaseOnPickup;
            targetValue = Mathf.Clamp(_newValue, 0, maxValue);
        }
        else
        {
            targetValue = resourceValue + resouceIncreaseOnPickup;
        }


        StartIncreaseCoroutine(targetValue);
	}

	private void StartIncreaseCoroutine(float _targetValue)
    {
		if(coroutineIncrease != null)
		{
			StopCoroutine(coroutineIncrease);
		}
		if (coroutineDecrease != null)
		{
			StopCoroutine(coroutineDecrease);
		}

		coroutineIncrease = StartCoroutine(IncreaseToTargetValueOverTime(_targetValue, () => {
            StartDecreaseCoroutine();
        }));
	}

	private void StartDecreaseCoroutine()
	{
		if (coroutineDecrease != null)
		{
			StopCoroutine(coroutineDecrease);
		}

		coroutineDecrease = StartCoroutine(DecreaseToZeroOverTime());
	}

	private IEnumerator IncreaseToTargetValueOverTime(float _targetValue, Action onCompleted = null)
	{
		while (resourceValue < _targetValue)
		{
			resourceValue += increaseSpeed * Time.deltaTime;
			ResourceBarUI.Instance.UpdateResourceBar();
			yield return null;
		}

		resourceValue = _targetValue;
		yield return new WaitForSeconds(timeBetweenCoroutines);

		coroutineIncrease = null; 
		if(onCompleted != null)
		{
			onCompleted();
		}
	}

	private IEnumerator DecreaseToZeroOverTime(Action onCompleted = null)
	{
		while (resourceValue > MINVALUE)
		{
			resourceValue -= decreaseSpeed * Time.deltaTime;
			ResourceBarUI.Instance.UpdateResourceBar();
			yield return null;
		}

		coroutineDecrease = null;
		if (onCompleted != null)
		{
			onCompleted();
		}
	}

	private void StopResources()
	{
		if(coroutineDecrease != null)
		{
			StopCoroutine(coroutineDecrease);
		}
		if(coroutineIncrease != null)
		{
			StopCoroutine(coroutineIncrease);
		}
	}

	private void OnEnable()
    {
        LevelProgess.ScoreUpdatedEvent += OnScoreUpdated;
		Player.PlayerDiedEvent += StopResources;
    }

    private void OnDisable()
    {
        LevelProgess.ScoreUpdatedEvent -= OnScoreUpdated;
		Player.PlayerDiedEvent -= StopResources;
	}
}