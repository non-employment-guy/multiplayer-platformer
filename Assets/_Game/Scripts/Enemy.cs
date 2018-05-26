using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyState State = EnemyState.Idle;

    public Transform Target;

    public float ChasingRaduis = 100f;

    public float JumpForce = 10;

    public float JumpValue = 0;

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

    private float _idleTimer = 2f;

    private int _moveVal = 0;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _inAir = false;
        _onCloud = false;
        _playerLayer = LayerMask.NameToLayer("Player");

        State = EnemyState.Idle;
    }

    void Update()
    {
        Flip(Input.GetAxis("Horizontal"));
    }

    void FixedUpdate()
    {
        switch (State)
        {
            case EnemyState.Idle:
                if (_idleTimer <= 0)
                    State = EnemyState.Patrol;
                else
                    _idleTimer -= Time.deltaTime;               
                break;
            case EnemyState.Patrol:
                _idleTimer = 2f;
                _moveVal = 1;
                break;
            case EnemyState.Chase:
                var dist = Vector3.Distance(Target.position, transform.position);
                if (dist >= ChasingRaduis)
                {
                    Target = null;
                    _moveVal = 0;
                    State = EnemyState.Idle;                    
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _jumpForce = JumpValue * JumpForce;

        if (Math.Abs(_jumpForce) > 0.1 && _onCloud && Input.GetAxis("Vertical") < 0)
        {
            gameObject.layer = _playerLayer;
        }
        else if (_jumpForce > 0.1 && !_inAir)
        {
            _rb.velocity = new Vector3(_moveForce, 0, 0);
            _rb.AddForce(0, _jumpForce, 0, ForceMode.Impulse);
            _inAir = true;
        }

        _moveForce = _moveVal * MoveSpeed;
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

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("OBSTACLECHECK triggered with " + col.name);
        switch (col.tag)
        {
            case "Box":
                JumpValue = 1;
                break;
        }
        //Destroy(gameObject);
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Collide with " + col.gameObject.name);
        _inAir = false;
        JumpValue = 0;
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
                //RpcDrown();
                break;
        }
    }
}

public enum EnemyState
{
    Idle,
    Patrol,
    Chase
}