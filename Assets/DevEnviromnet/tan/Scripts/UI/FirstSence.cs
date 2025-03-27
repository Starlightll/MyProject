using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstSence : MonoBehaviour
{
    public string playSence;
    public void OnButtonClick()
    {
        SceneManager.LoadScene(playSence);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void IntroScene()
    {
        SceneManager.LoadScene("IntroSence");

    }
    public void GamePlayScene()
    {
        SceneManager.LoadScene("GamePlaySence");
    }
}
