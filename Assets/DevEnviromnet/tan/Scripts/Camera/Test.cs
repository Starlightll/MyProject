using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 4, LayerMask.GetMask("Player"));
        if (hits != null)
        {
            Debug.Log("Enter collider");
        }
    }

    private void OnTriggerEnter2D(Collider2D collier)
    {
        Debug.Log("OnTriggerEnter2D");
    }
    
}
