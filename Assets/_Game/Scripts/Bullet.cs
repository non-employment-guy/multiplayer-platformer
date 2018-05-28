using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    public float Damage;

    public Player GunOwner;

    public float LifeTime = 2f;

    public GameObject HitParticles;

    void Start()
    {
        Destroy(gameObject, LifeTime);
    }
    
    void OnTriggerEnter(Collider col)
    {
   //     Debug.Log("Bullet triggered with " + col.name);
        switch (col.tag)
        {
            case "Box":
                Destroy(col.gameObject);
                Destroy(gameObject);
                break;
            case "Enemy":
            case "Player":
                var health = col.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(Damage);
                    if (health.HealthValue <= 0)
                    {
                        if (GunOwner != null)
                        {
                            GunOwner.Kills++;                           
                        }
                    }
                    if (HitParticles != null)
                        Instantiate(HitParticles, col.transform.position, Quaternion.identity);
                }
                Destroy(gameObject);
                break;
        }
    }
}