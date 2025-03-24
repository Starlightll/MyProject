using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    [SerializeField] protected Image healthBar;

    void LateUpdate()
    {
        if (healthBar != null)
        {
            healthBar.transform.rotation = Quaternion.identity;
        }
    }
}
