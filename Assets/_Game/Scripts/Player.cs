using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public float JumpForce = 10;

    public float MoveSpeed = 5;

    public float FallMultiplier = 2.5f;

    public float LowJumpMultiplier = 2f;

    public int Kills;

    public Transform GunPivot;

    public GameObject BulletPrefab;

    public GameObject Flash;

    public float Damage = 25f;

    public float FireForce = 50f;

    public float FireCooldown = 0.5f;

    private Rigidbody _rb;   

    private bool _inAir;

    private bool _onCloud;

    private bool _facingRight = true;

    private bool _isDying = false;

    private float _moveForce;

    private float _jumpForce;

    private int _playerLayer;

    private Health _healthComp;

    private float _currentCooldown;

    private float _vertical;

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;        
    }

    // Use this for initialization
    void Start()
    {
        if (localPlayerAuthority)
        {
            _healthComp = GetComponent<Health>();            
        }

        if (isLocalPlayer)
        {
            var camObject = GameObject.FindGameObjectWithTag("MainCamera");
            camObject.GetComponent<PlayerFollow>().TargeTransform = transform;

            var uiManager = camObject.GetComponentInChildren<UIManager>();
            uiManager.LocalPlayer = this;
            uiManager.PlayerHealth = _healthComp;
        }

        _rb = GetComponent<Rigidbody>();
        _inAir = false;
        _onCloud = false;
        _playerLayer = LayerMask.NameToLayer("Player");     
        GameManager.Instance.Players.Add(this);        
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        Flip(Input.GetAxis("Horizontal"));

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            CmdFire();
        }
    }
    
    void FixedUpdate()
    {        
        if (!isLocalPlayer) return;

        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (FallMultiplier - 1) * Time.fixedDeltaTime;
        }

        if (_isDying) return;

        _jumpForce = Input.GetAxis("Jump") * JumpForce;

        _vertical = Input.GetAxis("Vertical");

        if (Math.Abs(_jumpForce) > 0.1 && !_inAir && _vertical >= 0)
        {
            _rb.velocity = new Vector3(_moveForce, 0, 0);
            _rb.AddForce(0, _jumpForce, 0, ForceMode.Impulse);
            _inAir = true;
        }

        _moveForce = Input.GetAxis("Horizontal") * MoveSpeed;
        _rb.velocity = new Vector3(_moveForce, _rb.velocity.y, 0);

              
    }

    [Command]
    public void CmdJumpDown(GameObject obj)
    {
        obj.layer = _playerLayer;
    }

    [ClientRpc]
    private void RpcJumpDown()
    {
        gameObject.layer = _playerLayer;
    }

    [Command]
    void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            BulletPrefab,
            GunPivot.position,
            GunPivot.rotation);

        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * FireForce;
        NetworkServer.Spawn(bullet);
        var bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.Damage = Damage;
        bulletComp.GunOwner = this;
       
        _currentCooldown = FireCooldown;
        StartCoroutine(ShowFlash(0.001f));
    }

    private IEnumerator ShowFlash(float f)
    {
        Flash.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(f);
        Flash.GetComponent<MeshRenderer>().enabled = false;
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

    void OnCollisionStay(Collision col)
    {
        switch (col.collider.tag)
        {
            case "Cloud":
                if (Math.Abs(_jumpForce) > 0.1 && _onCloud && _vertical < 0)
                {
                    if (isServer)
                        RpcJumpDown();
                    else
                    {
                        gameObject.layer = _playerLayer;
                        CmdJumpDown(gameObject);
                    }
                }
                break;
        }
    }

    void OnCollisionEnter(Collision col)
    {
       //Debug.Log("Collide with " + col.gameObject.name);
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
        }
    }

    void OnTriggerEnter(Collider col)
    {
        
        switch (col.tag)
        {
            case "Buff":
                var bf = col.GetComponent<BuffObject>();
                if (bf.IsActive)
                    TakeBuff(bf.Use());
                break;
            case "Water":
                if (!isServer)
                    return;
                RpcDrown();
                break;
        }
    }

    private void TakeBuff(Buff buff)
    {
        _healthComp.Buffs.Add(buff);
        switch (buff.Type)
        {
            case BuffType.Shield:
            case BuffType.Damage:
                _healthComp.ApplyBuff(buff);
                break;
            case BuffType.Freeze:
                GameManager.Instance.FreezeEnemies(buff.Duration);
                break;
        }
    }

    [ClientRpc]
    private void RpcDrown()
    {
        if (!isLocalPlayer) return;
        _isDying = true;
        Debug.Log("Drowned!");
        FallMultiplier = 0.001f;
        _rb.velocity = Vector3.zero;
        StartCoroutine(StartDrowning(1f));
    }

    public IEnumerator StartDrowning(float time)
    {
        yield return new WaitForSeconds(time);
        if (isServer)
        {            
            NetworkManager.singleton.StopHost();
        }
        else if (isClient)
        {
            GameManager.Instance.Players.Remove(this);
            NetworkManager.singleton.StopClient();            
        }
        Destroy(gameObject);
    }
}
