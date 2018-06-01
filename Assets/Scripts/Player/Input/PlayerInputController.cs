﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public static PlayerInputController Instance { get { return GetInstance(); } }
    public static bool State { get { return state; } }

    #region Singleton
    private static PlayerInputController instance;

    private static PlayerInputController GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<PlayerInputController>();
        }
        return instance;
    }
    #endregion

    /// <summary>
    /// Parameters: TargetPosition
    /// </summary>
    public static Action<Vector3> InputEvent;

    private static bool state;

    [SerializeField] List<PlayerInputBase> playerInputs;
    [SerializeField] private PlayerInputType mobileVRPlayerInputType = PlayerInputType.Tilt;
    [SerializeField] private PlayerInputType editorVRPlayerInputType = PlayerInputType.Look;
    [SerializeField] private PlayerInputType nonVRPlayerInputType = PlayerInputType.Drag;

    private Coroutine inputUpdateCoroutine;
    private PlayerInputBase currentPlayerInputBase;

    public void SetState(bool _enabled)
    {
        state = _enabled;
        bool _previousState = state;

        if (state == _previousState) { return; }

        if (state)
        {
            UpdateInput();
        }
        else
        {
            StopCurrentInput();
        }
    }

    private IEnumerator InputUpdate()
    {
        while (true)
        {
            if (InputEvent != null)
            {
                InputEvent(currentPlayerInputBase.TargetPosition);
            }

            yield return null;
        }
    }

    private void UpdateInput()
    {
        if(!state) { return; }

        if(VRSwitch.VRState)
        {
            if(Application.isEditor)
            {
                StartInput(editorVRPlayerInputType);
            }
            else
            {
                StartInput(mobileVRPlayerInputType);
            }
        }
        else
        {
            StartInput(nonVRPlayerInputType);
        }
    }

    private void StartInput(PlayerInputType _playerInputType)
    {

        PlayerInputBase _playerInputBase = playerInputs.Find(x => x.PlayerInputType == _playerInputType);

        if(_playerInputBase == null)
        {
            Debug.LogError("No PlayerInput of type " + _playerInputType, gameObject);
            return;
        }

        StopCurrentInput();

        _playerInputBase.Activate();
        currentPlayerInputBase = _playerInputBase;

        inputUpdateCoroutine = StartCoroutine(InputUpdate());
    }

    private void StopCurrentInput()
    {
        if(inputUpdateCoroutine != null)
        {
            StopCoroutine(inputUpdateCoroutine);
            inputUpdateCoroutine = null;
        }

        if(currentPlayerInputBase != null)
        {
            currentPlayerInputBase.Deactivate();
        }
    }

    private void Start()
    {
        SetState(true);
        UpdateInput();
    }

    private void OnSceneSwitchCompleted(Scenes? _previousScene, Scenes _nextScene)
    {
        SetState(VRSwitch.VRState);
        UpdateInput();
    }

    private void OnEnable()
    {
        VRSwitch.SwitchedEvent += UpdateInput;
        SceneLoader.SceneSwitchCompletedEvent += OnSceneSwitchCompleted;
    }

    private void OnDisable()
    {
        VRSwitch.SwitchedEvent -= UpdateInput;
        SceneLoader.SceneSwitchCompletedEvent -= OnSceneSwitchCompleted;
    }
}