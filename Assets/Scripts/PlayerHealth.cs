using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Stats")]
    public float playerHealth;
    public float maxHealth = 10f;
    
    [Header("UI References")]
    public Image healthFill;

    [Header("Settings")]
    public float damageCooldown = 0.5f; 
    private float lastDamageTime;
    private bool isDead = false;

    public GameObject deathEffect;



    void Start()
    {
        playerHealth = maxHealth;
    }



    void Update()
    {
        playerHealth = Mathf.Clamp(playerHealth, 0, maxHealth);

        if (playerHealth <= 0 && !isDead)
        {
            Die();
        }
    }



    public void TakeDamage(float amount)
    {
        playerHealth -= amount;
        if(healthFill != null)
        {
            StartCoroutine(HitFlashUI());
        }
    }



    public void TakeExplosionDamage(float amount)
    {
        playerHealth -= amount;
        if(healthFill != null) 
        {
            StartCoroutine(HitFlashUI());
        }

        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if(cam != null)
        {
            cam.StartCoroutine(cam.Shake(0.3f, 0.6f));
        }
    }
    


    IEnumerator HitFlashUI()
    {
        Color originalColor = Color.white;
        healthFill.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        healthFill.color = originalColor;
    }



    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemyScript = collision.gameObject.GetComponent<Enemy>();
            if (enemyScript == null) 
            {
                return;
            }

            if (enemyScript.type == Enemy.EnemyType.Fast || enemyScript.type == Enemy.EnemyType.Tank)
            {
                Player pMove = GetComponent<Player>();
                if (pMove != null && !pMove.isDashing)
                {
                    playerHealth = 0;
                }
            }
            else
            {
                if (Time.time > lastDamageTime + damageCooldown)
                {
                    playerHealth--;
                    lastDamageTime = Time.time;
                    if(healthFill != null) StartCoroutine(HitFlashUI());
                }
            }
        }
    }

    

    public void Die()
    {
        isDead = true; 
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        // Instead of Destroy, you might want to show a Game Over screen first
        gameObject.SetActive(false); 
        Debug.Log("Player has been consumed by the horde.");
    }
}