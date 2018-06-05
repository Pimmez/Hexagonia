﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Button that calls the VRSwitch Switch method to switch VR Mode.
/// </summary>
[RequireComponent(typeof(Button))]
public class VRModeButton : GazeButton
{
    public static Action InitializedEvent;
        
    private const string EVENT_SYSTEM_NAME = "EventSystem";

    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color pressedColor = Color.yellow;

    private EventSystem[] eventSystems;

    /// <summary>
    /// Called when the button is activated.
    /// </summary>
    protected override void OnTrigger()
    {
        bool _vrState = VRSwitch.Instance.Switch();

        if (_vrState)
        {
            buttonImage.color = pressedColor;
            for (int i = 0; i < eventSystems.Length; i++)
            {
                if (eventSystems[i].transform.name == EVENT_SYSTEM_NAME)
                {
                    eventSystems[i].gameObject.SetActive(false);
                }
            }
        }
        else if (!_vrState)
        {
            buttonImage.color = defaultColor;
            for (int i = 0; i < eventSystems.Length; i++)
            {
                if (eventSystems[i].transform.name == EVENT_SYSTEM_NAME)
                {
                    eventSystems[i].gameObject.SetActive(true);
                }
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        VRSwitch.VRModeSwitchedEvent += OnTrigger;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        VRSwitch.VRModeSwitchedEvent -= OnTrigger;
    }

    private void Start()
    {
        eventSystems = Resources.FindObjectsOfTypeAll<EventSystem>();
        if (InitializedEvent != null)
        {
            InitializedEvent();
        }
    }
}