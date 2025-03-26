using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraPath : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 8f;
    public float waitTime = 2f;   

    public CinemachineCamera playerFollowCam;
    public CinemachineCamera mapOverviewCam;

    public Transform triggerPoint; 
    public Transform player; 
    public float triggerDistance = 1f;

    private GateController gate;

    private bool hasTriggered = false;

    public GameObject wall;

    private void Start()
    {
        gate= FindAnyObjectByType<GateController>();
    }


    void Update()
    {
        if (!hasTriggered && Vector3.Distance(player.position, triggerPoint.position) < triggerDistance)
        {
            hasTriggered = true;
            StartCoroutine(MoveAlongPath());
        }
    }

    IEnumerator MoveAlongPath()
    {
        mapOverviewCam.gameObject.SetActive(true);
        playerFollowCam.gameObject.SetActive(false);


        for (int i = 0; i < waypoints.Length; i++)
        {
            Transform target = waypoints[i];

            while (Vector3.Distance(transform.position, target.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);
        }

        gate.CloseGate();


        yield return new WaitUntil(() => gate.IsClosed());


        playerFollowCam.gameObject.SetActive(true);
        mapOverviewCam.gameObject.SetActive(false);
        GameObject.Destroy(wall);
    }
}
