using System;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{

    public float JumpForce;

    public float MoveForce;

    private Rigidbody _rigidbody;

    private bool _inAir;

    private float _moveForce;

    private float _jumpForce;    

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer)
        {
            var camObject = GameObject.FindGameObjectWithTag("MainCamera");
            camObject.GetComponent<PlayerFollow>().TargeTransform = transform;
        }

        _rigidbody = GetComponent<Rigidbody>();
        _inAir = false;

    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        _jumpForce = Input.GetAxis("Jump") * JumpForce;
        if (Math.Abs(_jumpForce) > 0.1 && !_inAir)
        {
            //  _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForce(0, _jumpForce, 0, ForceMode.Acceleration);

            _inAir = true;
        }

        _moveForce = Input.GetAxis("Horizontal") * MoveForce * Time.deltaTime;
        _rigidbody.velocity = new Vector3(_moveForce, _rigidbody.velocity.y, _rigidbody.velocity.z);
        //_rigidbody.AddForce(_moveForce, 0, 0, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Collide with " + col.gameObject.name);
        _inAir = false;
    }
}
