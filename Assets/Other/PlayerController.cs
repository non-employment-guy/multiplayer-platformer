using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 1)]
public class PlayerController : NetworkBehaviour
{
    public GameObject BulletPrefab;

    public Transform BulletSpawn;

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.position = Vector3.Lerp(transform.position, x * ( transform.position + Vector3.forward), Time.deltaTime);
    }

    [Command]
    void CmdFire()
    {
        var bullet = Instantiate(BulletPrefab, BulletSpawn.position, BulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

        NetworkServer.Spawn(bullet);

        Destroy(bullet, 2.0f);
    }
}