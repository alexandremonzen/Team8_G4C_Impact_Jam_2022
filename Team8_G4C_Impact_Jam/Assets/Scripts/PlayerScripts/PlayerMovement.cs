using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerMovement : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private float _moveSpeed;

    private Rigidbody _rigidbody;
    private PlayerInputActions _playerInputActions;
    private InputAction _movementAction;

    private Vector2 _movementVector;
    private Vector2 _finalMovementVector;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInputActions = new PlayerInputActions();
        _movementAction = _playerInputActions.PlayerMovement.Movement;

    }

    private void OnEnable()
    {
        _playerInputActions.PlayerMovement.Enable();
        _movementAction.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.PlayerMovement.Disable();
        _movementAction.Disable();
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        HandleMovementPhysics();
    }

    private void HandleInput()
    {
        _movementVector = _movementAction.ReadValue<Vector2>();
    }

    private void HandleMovementPhysics()
    {
        _rigidbody.velocity = new Vector3(_movementVector.x * _moveSpeed, _rigidbody.velocity.y, _movementVector.y * _moveSpeed);
    }
}