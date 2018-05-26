using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public float RadiusCheck;

    private Enemy parent;

    // Use this for initialization
    void Start()
    {
        parent = transform.GetComponentInParent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        var colliders = Physics.OverlapSphere(transform.position, RadiusCheck);
        if (colliders.Length == 0)
        {
            parent.JumpValue = 1;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
     //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(transform.position, RadiusCheck);
    }
}