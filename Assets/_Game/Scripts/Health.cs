using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    public Image HealthBar;

    public List<Buff> Buffs;

    public GameObject DestroyParticles;

    private float _maxHealthValue;

    [SyncVar(hook = "OnChangeHealth")]
    public float HealthValue = 100f;

    private float _shieldModifier = 1f;

    void Start()
    {
        _maxHealthValue = HealthValue;
        Buffs = new List<Buff>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        foreach (var buff in Buffs.ToList())
        {
            buff.Duration -= Time.deltaTime;
            if (buff.Duration <= 0)
            {
                Buffs.Remove(buff);
                RemoveBuff(buff);
            }
        }

        GameManager.Instance.ShieldBuffActive = Buffs.Exists(b => b.Type == BuffType.Shield);
        GameManager.Instance.FreezeBuffActive = Buffs.Exists(b => b.Type == BuffType.Freeze);
        GameManager.Instance.DamageBuffActive = Buffs.Exists(b => b.Type == BuffType.Damage);
    }

    public void TakeDamage(float dmgValue)
    {
        if (!isServer)
            return;

        dmgValue /= _shieldModifier;

        HealthValue -= dmgValue;

        if (HealthValue <= 0)
        {
            RpcDie();
        }
    }

    public void OnChangeHealth(float health)
    {
        HealthBar.fillAmount = health / _maxHealthValue;
        if (tag == "Player" && isLocalPlayer)
        {
            HealthValue = health;
        }
    }

    public void ApplyBuff(Buff buff)
    {
        switch (buff.Type)
        {
            case BuffType.Shield:
                _shieldModifier = 2f;
                break;
            case BuffType.Freeze:
                break;
            case BuffType.Damage:
                GetComponent<Player>().Damage += buff.Value;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RemoveBuff(Buff buff)
    {
        if (Buffs.Exists(b => b.Type == buff.Type)) return;
        switch (buff.Type)
        {
            case BuffType.Shield:
                _shieldModifier = 1f;
                break;
            case BuffType.Freeze:
                break;
            case BuffType.Damage:
                GetComponent<Player>().Damage -= buff.Value;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [ClientRpc]
    private void RpcDie()
    {
        switch (tag)
        {
            case "Enemy":
                GameManager.Instance.Enemies.Remove(GetComponent<Enemy>());
                Destroy(gameObject);
                break;
            case "Player":
                if (!isLocalPlayer) return;
                if (isServer)
                {
                    NetworkManager.singleton.StopHost();
                }
                else if (isClient)
                {
                    NetworkManager.singleton.StopClient();
                }
                break;
        }
        if (DestroyParticles != null)
        {
            Instantiate(DestroyParticles, transform.position, Quaternion.identity);
        }
    }
}