using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 2f;
    public int pierceCount = 1;



    void Start()
    {
        Destroy(gameObject, lifeTime);
    }



    public void RegisterHit()
    {
        pierceCount--;

        if (pierceCount <= 0)
        {
            Destroy(gameObject);
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        Destroy(gameObject);
    }
}