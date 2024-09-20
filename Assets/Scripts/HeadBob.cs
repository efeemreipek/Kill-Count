using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private bool _enable = true;
    [SerializeField, Range(0, 0.1f)] private float _amplitude = 0.015f;
    [SerializeField, Range(0, 30)] private float _frequency = 10.0f;
    [SerializeField] private Transform _camera = null;
    [SerializeField] private Transform _cameraHolder = null;
    [SerializeField] private float walkAmplitude = 0.001f;
    [SerializeField] private float sprintAmplitude = 0.002f;
    [SerializeField] private float walkFrequency = 10f;
    [SerializeField] private float sprintFrequency = 15f;

    private float _toggleSpeed = 3.0f;
    private float _walkSpeed;
    private Vector3 _startPos;
    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _startPos = _camera.localPosition;
    }

    void Update()
    {
        if (!_enable) return;

        _walkSpeed = PlayerController.Instance.GetWalkSpeed() * PlayerController.Instance.GetWalkSpeed();
        if (_controller.velocity.sqrMagnitude > _walkSpeed + 0.1f)
        {
            _amplitude = sprintAmplitude;
            _frequency = sprintFrequency;
        }
        else
        {
            _amplitude = walkAmplitude;
            _frequency = walkFrequency;
        } 

        CheckMotion();
        _camera.LookAt(FocusTarget());
        
    }
    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
        pos.x += Mathf.Cos(Time.time * _frequency / 2) * _amplitude * 2;
        return pos;
    }
    private void CheckMotion()
    {
        float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;
        ResetPosition();
        if (speed < _toggleSpeed) return;
        if (!_controller.isGrounded) return;

        PlayMotion(FootStepMotion());
    }
    private void PlayMotion(Vector3 motion)
    {
        _camera.localPosition += motion;
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + _cameraHolder.localPosition.y, transform.position.z);
        pos += _cameraHolder.forward * 15.0f;
        return pos;
    }
    private void ResetPosition()
    {
        if (_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, 1 * Time.deltaTime);
    }
}
