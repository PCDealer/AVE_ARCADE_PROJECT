using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnRate = 2f;
    public float spawnDistance = 10f;
    
    [Header("Timers")]
    public float startDelay = 3.0f;
    private float timeSinceLastSpawn;
    
    private bool playerIsDead = false;
    private bool canSpawn = false;

    void Start()
    {
        Invoke("EnableSpawning", startDelay);
    }

    void EnableSpawning() => canSpawn = true;

    void Update()
    {
        if (!canSpawn || playerIsDead)
        {
            return;
        }

        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnRate)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            
            if (player == null)
            {
                TriggerGameOver();
                return;
            }

            SpawnEnemy();
            timeSinceLastSpawn = 0f;
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPosition = (Vector2)transform.position + (Random.insideUnitCircle.normalized * spawnDistance);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    void TriggerGameOver()
    {
        playerIsDead = true;
        StartCoroutine(KillAllEnemiesRoutine());
    }

    IEnumerator KillAllEnemiesRoutine()
    {
        yield return new WaitForSeconds(1f);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Enemy eScript = enemy.GetComponent<Enemy>();
            if (eScript != null)
            {
                eScript.Die();
            }
            else
            {
                Destroy(enemy);
            }
        }

        Time.timeScale = 0.3f; 
        
        yield return new WaitForSecondsRealtime(2f);

        Time.timeScale = 1f;
        SceneManager.LoadScene("DeathScene");
    }
}