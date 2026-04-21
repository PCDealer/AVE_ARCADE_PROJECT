using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { Weak, Fast, Tank, Bomber }
    public EnemyType type;

    [Header("Stats")]
    public float moveSpeed;
    public int bulletHealth = 1; // Enemy health, mungkin di-scrap
    public float knockbackForce = 10f; // Knockback buat musuh tank

    // Player damage
    public float contactDamage = 1f;    
    public float damageRate = 0.5f;     
    private float nextDamageTime;       
    
    //  Bomber
    [Header("Bomber Settings")]
    public float detonateRange = 2.5f;
    public float explosionRadius = 4f;
    [SerializeField] private float swellDuration = 0.75f;
    private bool isDetonating = false;
    private bool hasExploded = false;

    [Header("Audio Settings")]
    public AudioClip armorHitSound;
    [Range(0, 1)] public float sfxVolume = 1f;

    [Header("Technical References")]
    public Transform player;
    public GameObject deathEffect;
    [SerializeField] private float knockbackStunTime = 0.2f;

    private Rigidbody2D rb; 
    private SpriteRenderer sr;
    
    private bool isBeingKnockedBack = false; 



    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>(); 
        sr = GetComponent<SpriteRenderer>();
        SetupEnemyStats();
    }
    


    //  Setup stat musuh
    void SetupEnemyStats()
    {
        if (type == EnemyType.Weak)
        {
            moveSpeed = 7f;
            contactDamage = 1f;
        }
        
        if (type == EnemyType.Fast)
        {
            moveSpeed = 14f;
            contactDamage = 99f; // Instant kill if touched
        }
        
        // Buat jaga-jaga, health dinaikin supaya tank kebal peluru tapi gk kebal dash
        if (type == EnemyType.Tank)
        {
            moveSpeed = 7f;
            bulletHealth = 999;
            contactDamage = 99f; // Instant kill if touched
        }
        
        if (type == EnemyType.Bomber)
        {
            moveSpeed = 10f;
            contactDamage = 1f;
        }
    }



void Update()
    {
        if (player == null)
        {
            return;
        }

        // Cek klo player ada health
        PlayerHealth pHealth = player.GetComponent<PlayerHealth>();
        if (pHealth != null && pHealth.playerHealth <= 0) 
        {
            return; 
        }

        // Logika ai tracking musuh
        if(!isBeingKnockedBack && !isDetonating)
        {
            // Buat jalan ke player
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

            // Buat musuh ngadep ke player
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

            // Khusus buat bomber klo dekat banget ama player, mulai start countdown meledak
            if (type == EnemyType.Bomber && Vector2.Distance(transform.position, player.position) < detonateRange)
            {
                StartCoroutine(BomberSwellRoutine());
            }
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            //  Logika musuh tank supaya kebal peluru
            if (type == EnemyType.Tank)
            {
                Vector2 pushDir = (transform.position - other.transform.position).normalized;
                ApplyKnockback(pushDir);

                if (armorHitSound != null)
                {
                    AudioSource.PlayClipAtPoint(armorHitSound, Camera.main.transform.position, sfxVolume);
                }
                
                Destroy(other.gameObject); 
                StartCoroutine(FlashWhite()); 
                return; 
            }

            // Logika musuh cepat, membuang sistem piercing peluru
            if (type == EnemyType.Fast)
            {
                Destroy(other.gameObject); 
                Die();
                return; 
            }
            
            //  Logika musuh lemah, peluru bisa tembus/piercing
            if (type == EnemyType.Weak)
            {
                Die();
                return;
            }

            // Logika bomber, ditembak langsung meledak
            if (type == EnemyType.Bomber)
            {
                Die(); 
                Bullet b = other.GetComponent<Bullet>();
                if(b != null) b.RegisterHit();
            }
        }

        if (other.CompareTag("Player"))
        {
            //  Cool factor, jika player sering gunain fitur dash-kill
            Player pScript = other.GetComponent<Player>();
            if (pScript != null && pScript.isDashing)
            {
                pScript.AddBlood(); // Makes player bloody!
                Die();
            }
        }
    }


    // Logika tabrakan (Bukan dash-kill)
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player pScript = collision.gameObject.GetComponent<Player>();
            PlayerHealth pHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (pScript != null && pHealth != null)
            {
                // Jika player DASHING, jangan kasih damage (biarkan OnTrigger handle dash kill)
                if (pScript.isDashing) return;

                if (Time.time >= nextDamageTime)
                {
                    pHealth.TakeDamage(contactDamage);
                    nextDamageTime = Time.time + damageRate;
                }
            }
        }
    }



    // Cool factor buat bomber, dia kedap-kedip merah sebelum meledak
    IEnumerator BomberSwellRoutine()
    {
        if (isDetonating) yield break; 
        isDetonating = true;
        
        float timer = 0f;
        Vector3 originalScale = transform.localScale;

        while (timer < swellDuration)
        {
            timer += Time.deltaTime;
            float pulse = 1f + (Mathf.Sin(timer * 25f) * 0.15f);
            float growth = 1f + (timer / swellDuration);
            transform.localScale = originalScale * pulse * growth;
            sr.color = Color.Lerp(Color.white, Color.red, timer / swellDuration);
            yield return null;
        }

        Die();
    }



    //  Knockback buat tank
    void ApplyKnockback(Vector2 direction)
    {
        if (rb != null)
        {
            StopCoroutine(KnockbackRoutine()); 
            StartCoroutine(KnockbackRoutine());
            rb.velocity = Vector2.zero;
            rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }



    //  Knockback buat tank
    IEnumerator KnockbackRoutine()
    {
        isBeingKnockedBack = true;
        yield return new WaitForSeconds(knockbackStunTime); 
        isBeingKnockedBack = false;
        rb.velocity = Vector2.zero; 
    }



    //  Knockback buat tank
    IEnumerator FlashWhite()
    {
        Color oldColor = sr.color;
        sr.color = Color.white; 
        yield return new WaitForSeconds(0.1f);
        sr.color = oldColor; 
    }



    public void Die()
    {
        // Menghindari crash karena chain reaction dari bomber sebelumnya
        if (hasExploded)
        {
            return;
        }
        hasExploded = true;

        ScoreSystem scoreScript = GameObject.FindObjectOfType<ScoreSystem>();
        if (scoreScript != null)
        {
            scoreScript.AddScore(150f);
        }

        if (type == EnemyType.Bomber)
        {
            Explode();
            
            //  Camerashake
            CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
            if(cam != null) cam.StartCoroutine(cam.Shake(0.2f, 0.4f)); 
        }
        
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }



    // Logika ledakan, bisa kena musuh dan player
    void Explode()
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D obj in objectsInRange)
        {
            //  Buat bom kena sesama musuh
            if (obj.gameObject != this.gameObject && obj.CompareTag("Enemy"))
            {
                Enemy otherEnemy = obj.GetComponent<Enemy>();
                if(otherEnemy != null) otherEnemy.Die();
            }

            // Buat bom kena player
            if (obj.CompareTag("Player"))
            {
                PlayerHealth pHealth = obj.GetComponent<PlayerHealth>();
                if (pHealth != null)
                {
                    pHealth.TakeExplosionDamage(25f);
                }
            }
        }
    }
}