using System;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public float JumpForce = 10;

    public float MoveSpeed = 5;

    public float FallMultiplier = 2.5f;

    public float LowJumpMultiplier = 2f;

    private Rigidbody _rb;

    private Vector3 _velocity;

    private bool _inAir;

    private bool _onCloud;

    private bool _facingRight = true;

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

        _rb = GetComponent<Rigidbody>();
        _inAir = false;
        _onCloud = false;
        _playerLayer = LayerMask.NameToLayer("Player");        
    }

    void Update()
    {
        //float _xMov = Input.GetAxis("Horizontal");
        //float _zMov = Input.GetAxis("Jump");

        //Vector3 _movHorizontal = transform.right * _xMov;
        //Vector3 _movVertical = transform.up * _zMov;

        //// Final movement vector
        //_velocity = _movHorizontal * MoveSpeed + _movVertical * JumpForce;

        Flip(Input.GetAxis("Horizontal"));
    }
    
    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // Move();

        _jumpForce = Input.GetAxis("Jump") * JumpForce;

        if (Math.Abs(_jumpForce) > 0.1 && _onCloud && Input.GetAxis("Vertical") < 0)
        {
            gameObject.layer = _playerLayer;
        }
        else if (Math.Abs(_jumpForce) > 0.1 && !_inAir)
        {
            _rb.velocity = new Vector3(_moveForce, 0, 0);
            _rb.AddForce(0, _jumpForce, 0, ForceMode.Impulse);
            _inAir = true;
        }

        _moveForce = Input.GetAxis("Horizontal") * MoveSpeed;
        _rb.velocity = new Vector3(_moveForce, _rb.velocity.y, 0);

        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (FallMultiplier - 1) * Time.fixedDeltaTime;
        }        
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

    private void Move()
    {
       // if (_rb.velocity != Vector3.zero)
        {
            _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
        }
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
