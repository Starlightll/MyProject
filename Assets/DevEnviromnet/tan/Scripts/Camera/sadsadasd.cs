using UnityEngine;

public class sadsadasd : MonoBehaviour
{
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update");
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
