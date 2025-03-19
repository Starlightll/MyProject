using System.Collections.Generic;
using UnityEngine;

public class SwordOfArtsCollider : MonoBehaviour
{
    public SwordOfArts _swordOfArts;


    GameObject enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Enemy"))
        {
            enemy = collider.gameObject;
            _swordOfArts.hit(enemy);
            enemy.GetComponent<SpriteRenderer>().color = Color.red;
            Invoke("resetColor", 0.1f);
            Debug.Log("Hit");
        }
    }

    private void resetColor()
    {
        enemy.GetComponent<SpriteRenderer>().color = Color.white;
    }
    
}
