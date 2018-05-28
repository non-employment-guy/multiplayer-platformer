using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool ShieldBuffActive;

    public bool FreezeBuffActive;

    public bool DamageBuffActive;

    public int EnemiesMaxCount = 4;

    public List<Enemy> Enemies;

    public List<Player> Players;

    void Start()
    {
        Players = new List<Player>();        
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Pause()
    {
        Time.timeScale = Time.timeScale > 0 ? 0 : 1;
    }

    public void FreezeEnemies(float freezeTime)
    {
        foreach (var enemy in Enemies)
        {
            StartCoroutine(FreezeEnemy(enemy, freezeTime));
        }
    }

    private IEnumerator FreezeEnemy(Enemy enemy, float freezeTime)
    {
        var previousState = enemy.State;
        enemy.State = EnemyState.Freeze;
        yield return new WaitForSeconds(freezeTime);
        enemy.State = previousState;
    }
}