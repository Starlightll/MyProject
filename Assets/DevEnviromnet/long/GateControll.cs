using UnityEngine;

public class GateControll : MonoBehaviour
{
    public bool isOpen = false;
    public GameObject pointUp;
    public GameObject pointDown;
    public GameObject gate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 GateEndPoint = new Vector2(gate.transform.position.x, gate.transform.position.y - gate.transform.localScale.y  / 2);
        if(isOpen)
        {
            if(GateEndPoint.y <= pointUp.transform.position.y)
            {
                gate.transform.position = new Vector3(gate.transform.position.x, gate.transform.position.y + 0.1f, gate.transform.position.z);
            }
        }
        else
        {
            if (GateEndPoint.y >= pointDown.transform.position.y)
            {
                gate.transform.position = new Vector3(gate.transform.position.x, gate.transform.position.y - 0.1f, gate.transform.position.z);
            }
        }
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 GateEndPoint = new Vector2(gate.transform.position.x, gate.transform.position.y - gate.transform.localScale.y  / 2);
        Gizmos.DrawSphere(GateEndPoint, 0.1f);
        Gizmos.DrawSphere(pointUp.transform.position, 0.1f);
        Gizmos.DrawSphere(pointDown.transform.position, 0.1f);
    }
}
