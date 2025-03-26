using System.Collections.Generic;
using UnityEngine;

public class SwordOfArtsCollider : MonoBehaviour
{
    public SwordOfArts _swordOfArts;

    Collider2D _collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {   
            _swordOfArts.hit(collider, transform.parent);
            DamagePopup.Create(collider.transform.position, _swordOfArts.baseDamage);
        }
    }

    private void resetColor()
    {
        _collider.GetComponent<SpriteRenderer>().color = Color.white;
    }
    
}
