using UnityEngine;

public class PlayerLevelController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private PlayerController _playerController;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_stats.currentExperience >= _stats.experienceToNextLevel)
        {
            _stats.LevelUp();
            _stats.currentExperience = 0;
        }
    }
}
