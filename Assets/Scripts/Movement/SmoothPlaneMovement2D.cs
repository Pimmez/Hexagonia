﻿using UnityEngine;

public class SmoothPlaneMovement2D : MonoBehaviour
{

    [SerializeField] [Range(0, 20)] private float speed = 0.3f;

    private void UpdateTargetPosition(Vector3 _targetPosition)
    {
        Vector2 _delta = _targetPosition - transform.position;
        Vector2 _direction = _delta.normalized;
        float _distance = Vector2.Distance(_targetPosition, transform.position);
        float deltaSpeed = speed * Time.deltaTime;

        Vector3 positionIncrement = _direction * (_distance * deltaSpeed);
        transform.position += positionIncrement;
    }

    private void OnEnable()
    {
        LookPositionOnPlane.LookPositionUpdatedEvent += UpdateTargetPosition;
    }

    private void OnDisable()
    {
        LookPositionOnPlane.LookPositionUpdatedEvent -= UpdateTargetPosition;
    }

}