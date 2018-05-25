using System;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{

    public float JumpForce;

    public float MoveForce;

    public float FallMultiplier = 2.5f;

    public float LowJumpMultiplier = 2f;

    private Rigidbody _rigidbody;

    private bool _inAir;

    private bool _onCloud;

    private float _moveForce;

    private float _jumpForce;

    private int _playerLayer;

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
        _onCloud = false;
        _playerLayer = LayerMask.NameToLayer("Player");

    }

    void Update()
    {
       
        //else if (_rigidbody.velocity.y < 0 && !Input.GetButtonDown("Space"))
        //{
        // //   _rigidbody.velocity += Vector3.up * Physics.gravity.y * (LowJumpMultiplier - 1) * Time.deltaTime;
        //}
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        _jumpForce = Input.GetAxis("Jump") * JumpForce;

        if (Math.Abs(_jumpForce) > 0.1 && _onCloud && Input.GetAxis("Vertical") < 0)
        {
            gameObject.layer = _playerLayer;
        }
        else if (Math.Abs(_jumpForce) > 0.1 && !_inAir)
        {            
            _rigidbody.AddForce(0, _jumpForce, 0, ForceMode.Impulse);
            _inAir = true;
        }

        _moveForce = Input.GetAxis("Horizontal") * MoveForce * Time.fixedDeltaTime;
        _rigidbody.velocity = new Vector3(_moveForce, _rigidbody.velocity.y, _rigidbody.velocity.z);

        if (_rigidbody.velocity.y < 0)
        {
            _rigidbody.velocity += Vector3.up * Physics.gravity.y * (FallMultiplier - 1) * Time.fixedDeltaTime;
        }
        //_rigidbody.AddForce(_moveForce, 0, 0, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Collide with " + col.gameObject.name);
        _inAir = false;
        switch (col.collider.tag)
        {
            case "Earth":
                _onCloud = false;
                gameObject.layer = 0;
                break;
            case "Cloud":
                _onCloud = true; 
                if (col.transform.position.y > transform.position.y)
                    gameObject.layer = _playerLayer;
                else
                {
                    gameObject.layer = 0;
                }
                break;
            case "Water":
                RpcDrown();
                break;
        }
    }

    [ClientRpc]
    private void RpcDrown()
    {
        Debug.Log("Drowned!");
        if (isLocalPlayer)
        {
            var ni = GetComponent<NetworkIdentity>();
            ni.connectionToClient.Disconnect();
        }            
    }
}
