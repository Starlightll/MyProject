using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TranferMap2 : MonoBehaviour
{
    public string nextSceneName; // Tên của scene tiếp theo
    public GameObject warningUI; // UI cảnh báo cần giết boss trước
    public Collider2D blockCollider; 
    public AudioClip passMap;
    public AudioSource audioSource;

    void Start()
    {
        if (warningUI != null)
        {
            warningUI.SetActive(false); // Ẩn UI khi bắt đầu
        }

        if (blockCollider != null)
        {
            blockCollider.enabled = true; // Kích hoạt collider khi bắt đầu
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("IsBossDefeated: " + BossController.IsBossDefeated);
            if (BossController.IsBossDefeated)
            {
                Debug.Log("Boss is defeated! Transfer to next scene!");
                audioSource.PlayOneShot(passMap);
                StartCoroutine(TransferToNextScene());

            }
            else
            {
                ShowWarning();
            }
        }
    }

    IEnumerator TransferToNextScene()
    {
        yield return new WaitForSeconds(2f); // Đợi 2 giây
        SceneManager.LoadScene(nextSceneName);
    }

    void ShowWarning()
    {
        if (warningUI != null)
        {
            warningUI.SetActive(true);
            Debug.Log("You need to defeat the boss first!");
            Invoke("HideWarning", 2f);
        }

        if (blockCollider != null)
        {
            blockCollider.enabled = true;
        }
    }

    void HideWarning()
    {
        if (warningUI != null)
        {
            warningUI.SetActive(false);
        }

        if (blockCollider != null)
        {
            blockCollider.enabled = false; // Vô hiệu hóa collider để cho phép người chơi tiếp tục di chuyển
        }
    }
}

