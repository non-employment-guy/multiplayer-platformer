using System;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public Transform GunPivot;

    public GameObject BulletPrefab;

    public float Damage = 25f;

    public float FireForce;

    public float FireCooldown = 1f;

    private float _currentCooldown;

    void Update()
    {
        if (_currentCooldown > 0f)
        {
            _currentCooldown -= Time.deltaTime;
            return;
        }

        if (Math.Abs(Input.GetAxis("Fire1")) > 0.001f)
        {
            Fire();
        }
    }

    public void Fire()
    {
        var bullet = Instantiate(BulletPrefab, GunPivot.position, GunPivot.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(GunPivot.forward * FireForce, ForceMode.Impulse);
        _currentCooldown = FireCooldown;
    }
}