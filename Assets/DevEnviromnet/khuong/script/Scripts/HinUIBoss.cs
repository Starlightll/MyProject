using UnityEngine;

public class HinUIBoss : MonoBehaviour
{
    public GameObject hpUI;
    public float checkPlayerDistance;
    private Transform player;
    private Transform boss;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        boss = GameObject.FindGameObjectWithTag("BossController")?.transform;
        hpUI.SetActive(false);
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= checkPlayerDistance)
        {
            hpUI.SetActive(true);
        }
        else
        {
            hpUI.SetActive(false);
        }
        if (boss == null)
        {
            hpUI.SetActive(false);
            return;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkPlayerDistance);
    }
}
