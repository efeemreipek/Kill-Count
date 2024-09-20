using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Lean.Pool;

public class EnemyManager : MonoBehaviour
{
    public event Action<int> OnEnemyCountChanged;

    [SerializeField] private AudioClip executeAudio;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private bool canSpawnEnemies;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnRange = 2f;
    [SerializeField] private int enemyCount;

    private List<Enemy> enemyList = new List<Enemy>();
    private int killCount = 0;
    private AudioSource audioSource;
    private LeanGameObjectPool foundPool;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Enemy[] enemyArray;
        enemyArray = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemyArray)
        {
            enemyList.Add(enemy);
        }

        if (PlayerPrefs.HasKey("KillCount"))
        {
            killCount = PlayerPrefs.GetInt("KillCount");
        }
        else
        {
            killCount = 0;
        }

        LeanGameObjectPool.TryFindPoolByPrefab(enemyPrefab, ref foundPool);
        enemyCount = PlayerPrefs.GetInt("Difficulty");
        foundPool.Capacity = enemyCount;
        StartCoroutine(SpawnEnemies());

        Enemy.OnEnemyDied += Enemy_OnEnemyDied;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDied -= Enemy_OnEnemyDied;
    }

    private void Enemy_OnEnemyDied(Enemy obj)
    {
        audioSource.clip = executeAudio;
        audioSource?.Play();
        enemyList.Remove(obj);
        killCount++;
        PlayerPrefs.SetInt("KillCount", killCount);
        OnEnemyCountChanged?.Invoke(killCount);
    }

    private IEnumerator SpawnEnemies()
    {
        while (canSpawnEnemies)
        {
            LeanPool.Spawn(enemyPrefab, RandomPointOnNavMesh(spawnRange), Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);
        }
        yield return null;
    }
    private Vector3 RandomPointOnNavMesh(float radius)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
