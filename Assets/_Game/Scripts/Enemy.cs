using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Enemy : NetworkBehaviour
{
    public EnemyState State = EnemyState.Idle;

    public Transform Target;

    public float SearchingRadius = 5f;

    public float ChasingRaduis = 10f;

    public float ShootingRadius = 7f;

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

    private bool _isDying = false;

    private float _moveForce;

    private float _jumpForce;

    private int _playerLayer;

    private float _idleTimer = 2f;

    private int _moveVal = 0;




    public Transform GunPivot;

    public GameObject BulletPrefab;

    public float Damage = 25f;

    public float FireForce;

    public float FireCooldown = 1f;

    private float _currentCooldown;

    private float _rndTime = 3f;

    private float _currentRndTime;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _inAir = false;
        _onCloud = false;
        _playerLayer = LayerMask.NameToLayer("Player");

        State = EnemyState.Idle;
    }

    [ServerCallback]
    void Update()
    {
        Flip(_moveVal);
    }

    [ServerCallback]
    void FixedUpdate()
    {
        switch (State)
        {
            case EnemyState.Idle:
                if (_idleTimer <= 0)
                    State = EnemyState.Patrol;
                else
                    _idleTimer -= Time.deltaTime;
                _currentRndTime = _rndTime;
                break;
            case EnemyState.Patrol:
                _idleTimer = 2f;
                _moveVal = 1;
                _currentRndTime -= Time.deltaTime;
                if (_currentRndTime <= 0)
                {
                    Flip(_moveVal * -1);
                    _currentRndTime = _rndTime;
                }
                FindPlayer();
                break;
            case EnemyState.Chase:
                var direction = Target.position.x - transform.position.x;
                if (direction >= 0)
                    _moveVal = 1;
                else if (direction < 0)
                    _moveVal = -1;
                var dist = Vector3.Distance(Target.position, transform.position);
                if (dist <= ShootingRadius)
                {
                    State = EnemyState.Shoot;
                }
                else if (dist >= ChasingRaduis)
                {
                    Target = null;
                    _moveVal = 0;
                    State = EnemyState.Idle;                    
                }
                break;
            case EnemyState.Shoot:
                if (Target == null)
                {
                    State = EnemyState.Idle;
                    return;
                }
                dist = Vector3.Distance(Target.position, transform.position);
                if (dist > ShootingRadius)
                {
                    State = EnemyState.Chase;
                    return;
                }
                _moveVal = 0;

                if (_currentCooldown > 0f)
                {
                    _currentCooldown -= Time.deltaTime;
                    return;
                }

                Fire();
                break;
            case EnemyState.Freeze:
                _moveVal = 0;
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

    private void FindPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, SearchingRadius);
        foreach (var col in colliders) //todo: переделать этот кусок говна в нормальный поиск
        {
            if (!col.name.Contains("Player")) continue;
            Target = col.transform;
            State = EnemyState.Chase;
            break;
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

    public void Fire()
    {
        var bullet = Instantiate(BulletPrefab, GunPivot.position, GunPivot.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(GunPivot.forward * FireForce, ForceMode.Impulse);
        var bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.Damage = Damage;        
        _currentCooldown = FireCooldown;
    }

    [ServerCallback]
    void OnTriggerEnter(Collider col)
    {
        //Debug.Log("OBSTACLECHECK triggered with " + col.name); //todo: change obstacle finding to raycast
        switch (col.tag)
        {
            case "Box":
                JumpValue = 1;
                break;
            case "Water":
                RpcDrown();
                break;
        }
        //Destroy(gameObject);
    }

    [ServerCallback]
    private void RpcDrown()
    {
        _isDying = true;
        Debug.Log("Drowned!");
        FallMultiplier = 0.001f;
        _rb.velocity = Vector3.zero;
        StartCoroutine(StartDrowning(1f));
    }


    public IEnumerator StartDrowning(float time)
    {
        yield return new WaitForSeconds(time);
        GameManager.Instance.Enemies.Remove(this);
        Destroy(gameObject);
    }

    [ServerCallback]
    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("Collide with " + col.gameObject.name);
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
        }
    }
}

public enum EnemyState
{
    Idle,
    Freeze,
    Patrol,
    Chase,
    Shoot
}