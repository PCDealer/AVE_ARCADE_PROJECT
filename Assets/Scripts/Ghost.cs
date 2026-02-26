using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    public float ghostLifetime = 0.5f;
    private float timer;
    private SpriteRenderer sr;
    private Color color;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;
        timer = ghostLifetime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        float alpha = timer / ghostLifetime;
        sr.color = new Color(color.r, color.g, color.b, alpha);

        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}