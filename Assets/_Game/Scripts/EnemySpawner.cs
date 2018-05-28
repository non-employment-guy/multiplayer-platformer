using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject EnemyPrefab;

    public float SpawnDelay;

    private float _currentDelayTime;

    public override void OnStartServer()
    {
        GameManager.Instance.Enemies.Clear();
        _currentDelayTime = 0;
    }

    void Update()
    {
        if (!isServer) return;
        if (_currentDelayTime <= 0)
        {
            SpawnEnemy();
            _currentDelayTime = SpawnDelay;
        }
        else
        {
            _currentDelayTime -= Time.deltaTime;
        }
    }
    
    public void SpawnEnemy()
    {
        if (GameManager.Instance.Enemies.Count >= GameManager.Instance.EnemiesMaxCount) return;
        var enemy = Instantiate(EnemyPrefab, transform.position, EnemyPrefab.transform.rotation);
        GameManager.Instance.Enemies.Add(enemy.GetComponent<Enemy>());
        NetworkServer.Spawn(enemy);
        
    }
}