using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayText : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public string text;
    public string nextScene;
    void Start()
    {
        if (textUI != null)
            textUI.text = text;
    }

    public void OnButtonClick()
    {
        SceneManager.LoadScene(nextScene);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
