using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public float JumpForce = 10;

    public float MoveSpeed = 5;

    public float FallMultiplier = 2.5f;

    public float LowJumpMultiplier = 2f;

    private bool _isDying = false;

    private float _moveForce;

    private float _jumpForce;

    private Rigidbody _rb;

    // private Vector3 _velocity;

    private bool _inAir;

    private bool _onCloud;

    private bool _facingRight = true;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _inAir = false;
        _onCloud = false;        
    }

    void Update()
    {
        Flip(Input.GetAxis("Horizontal"));
    }

    void FixedUpdate()
    {       

        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (FallMultiplier - 1) * Time.fixedDeltaTime;
        }

        if (_isDying) return;

        _jumpForce = Input.GetAxis("Jump") * JumpForce;

        if (Math.Abs(_jumpForce) > 0.1 && _onCloud && Input.GetAxis("Vertical") < 0)
        {
           // gameObject.layer = _playerLayer;
        }
        else if (Math.Abs(_jumpForce) > 0.1 && !_inAir)
        {
            _rb.velocity = new Vector3(_moveForce, 0, 0);
            _rb.AddForce(0, _jumpForce, 0, ForceMode.Impulse);
            _inAir = true;
        }

        _moveForce = Input.GetAxis("Horizontal") * MoveSpeed;
        _rb.velocity = new Vector3(_moveForce, _rb.velocity.y, 0);


    }

    private void Flip(float inputValue)
    {
        if (inputValue > 0.2f)
        {
            if (_facingRight) return;
            transform.eulerAngles += new Vector3(0, 180, 0);
            _facingRight = !_facingRight;
        }
        else if (inputValue < -0.2f)
        {
            if (!_facingRight) return;
            transform.eulerAngles -= new Vector3(0, -180, 0);
            _facingRight = !_facingRight;
        }
    }
}
