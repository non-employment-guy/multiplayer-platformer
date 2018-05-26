using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage;

    public float LifeTime = 2f;

    void Start()
    {
        Destroy(gameObject, LifeTime);
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Bullet triggered with " + col.name);
        switch (col.tag)
        {
            case "Box":
                Destroy(col.gameObject);
                break;
        }
        Destroy(gameObject);
    }
}