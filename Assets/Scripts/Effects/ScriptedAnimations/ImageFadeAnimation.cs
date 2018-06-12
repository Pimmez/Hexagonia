﻿using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fades an images in or out.
/// </summary>
public class ImageFadeAnimation : LerpAnimation
{
    [SerializeField] private Image image;

    public override void StartAnimation(Action animationStoppedEvent = null)
    {
        StartValue = image.color.a;
        base.StartAnimation(animationStoppedEvent);
    }

    protected override void Apply(float _value)
    {
        Color tempColor = image.color;
        tempColor.a = _value;
        image.color = tempColor;
    }
}
