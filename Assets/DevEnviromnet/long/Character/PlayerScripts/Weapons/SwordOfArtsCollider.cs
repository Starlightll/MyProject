using System.Collections.Generic;
using UnityEngine;

public class SwordOfArtsCollider : MonoBehaviour
{
    public SwordOfArts _swordOfArts;

    List<GameObject> _colliders = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Enemy"))
        {
            _swordOfArts.hit(collider);
            collider.GetComponent<SpriteRenderer>().color = Color.red;
            _colliders.Add(collider.gameObject);
            Invoke("resetColor", 0.1f);
            Debug.Log("Hit");
        }
    }

    private void resetColor()
    {
        foreach (GameObject enemy in _colliders)
        {
            enemy.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    
}
