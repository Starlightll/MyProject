using UnityEngine;

public class NavigationPanelController : MonoBehaviour
{
    public GameObject SkillPanel; // Kéo thả Panel vào đây trong Inspector


    void Start()
    {
        SkillPanel.SetActive(false); // Ẩn menu
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) // Khi nhấn phím K
        {
            ToggleMenu();
        }
        if (SkillPanel.activeSelf && Input.GetMouseButtonDown(0)) // Nhấn chuột
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(SkillPanel.GetComponent<RectTransform>(), Input.mousePosition))
        {
            SkillPanel.SetActive(false);
        }
    }
    }

    void ToggleMenu()
    {
        if (SkillPanel != null)
        {
            SkillPanel.SetActive(!SkillPanel.activeSelf); // Bật/Tắt menu
        }
    }
}
