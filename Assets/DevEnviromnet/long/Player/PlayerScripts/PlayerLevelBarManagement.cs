using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelBarManagement : MonoBehaviour
{

    [SerializeField] private PlayerStats _stats;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Image _experienceFill;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       _levelText.text = _stats.level.ToString();
       _experienceFill.fillAmount = _stats.currentExperience / _stats.experienceToNextLevel;
    }
}
