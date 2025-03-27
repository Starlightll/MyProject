using UnityEngine;

public class TranferMap2 : MonoBehaviour
{
    public string nextSceneName; // Tên của scene tiếp theo
    public GameObject warningUI; // UI cảnh báo cần giết boss trước
    public BossManager bossManager; // Tham chiếu đến script quản lý boss

    void Start()
    {
        if (warningUI != null)
        {
            warningUI.SetActive(false); // Ẩn UI khi bắt đầu
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (bossManager != null && bossManager.IsBossDefeated)
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                if (warningUI != null)
                {
                    warningUI.SetActive(true);
                    Invoke("HideWarning", 2f); 
                }
            }
        }
    }

    void HideWarning()
    {
        if (warningUI != null)
        {
            warningUI.SetActive(false);
        }
    }
}
