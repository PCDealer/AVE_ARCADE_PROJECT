using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Camera cam;

    public float dashSpeed = 20f;
    public float dashDuration = 0.1f;
    public int maxDashes = 3;
    public float dashCooldown = 2f;
    
    private int currentDashes;
    private float cooldownTimer;
    public bool isDashing;

    Vector2 movement;
    Vector2 mousePos;

    void Start()
    {
        mySR = GetComponent<SpriteRenderer>();
        cleanSprite = mySR.sprite;
        
        currentDashes = maxDashes;
    }

    void Update()
    {
        if (isDashing) return;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        HandleDashCooldown();

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift)) && currentDashes > 0)
        {
            StartCoroutine(Dash());
        }

        if (currentBloodLevel > 0 && Time.time > lastKillTime + cleanUpDelay)
        {
            CleanUpGore();
        }
    }

    private void HandleDashCooldown()
    {
        if (currentDashes < maxDashes)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer >= dashCooldown)
            {
                currentDashes++;
                cooldownTimer = 0f; 
                Debug.Log("Dash Charged! Current: " + currentDashes);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        currentDashes--;

        StartCoroutine(SpawnTrail());

        Vector2 dashDir = movement.sqrMagnitude > 0 ? movement.normalized : (Vector2)transform.up;
        rb.velocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.velocity = Vector2.zero;
    }



    public GameObject ghostPrefab;
    public float ghostSpawnRate = 0.03f;

    private IEnumerator SpawnTrail()
    {
        while (isDashing)
        {
            GameObject currentGhost = Instantiate(ghostPrefab, transform.position, transform.rotation);
        
            currentGhost.transform.localScale = transform.localScale;

            SpriteRenderer ghostSR = currentGhost.GetComponent<SpriteRenderer>();
            SpriteRenderer playerSR = GetComponent<SpriteRenderer>();
            ghostSR.sprite = playerSR.sprite;
        
            ghostSR.color = new Color(0.5f, 0.5f, 1f, 0.5f); 

            yield return new WaitForSeconds(ghostSpawnRate);
        }
    }


    [Header("Gore System")]
    public Sprite cleanSprite;
    public Sprite[] bloodSprites;
    public float cleanUpDelay = 10f;

    private int dashKills = 0;
    private int currentBloodLevel = 0;
    private float lastKillTime;
    private SpriteRenderer mySR;

    public void AddBlood()
    {
        lastKillTime = Time.time;
        
        dashKills++;

        int spriteIndex = Mathf.Clamp(dashKills / 3, 0, bloodSprites.Length - 1);
        mySR.sprite = bloodSprites[spriteIndex];
        
        
        if (currentBloodLevel < bloodSprites.Length)
        {
            currentBloodLevel++;
            UpdatePlayerSprite();
        }

    }

    void CleanUpGore()
{
    
    currentBloodLevel--;
    
    
    lastKillTime = Time.time - (cleanUpDelay - 1f); 

    UpdatePlayerSprite();
}

void UpdatePlayerSprite()
{
    if (currentBloodLevel <= 0)
    {
        mySR.sprite = cleanSprite;
        currentBloodLevel = 0;
    }
    else
    {
        
        mySR.sprite = bloodSprites[currentBloodLevel - 1];
    }
}


    public int GetCurrentDashes() 
    { 
        return currentDashes; 
    }

    public float GetDashChargeProgress() 
    {
        if (currentDashes >= maxDashes) return 0f;

        return cooldownTimer / dashCooldown; 
    }
    
}