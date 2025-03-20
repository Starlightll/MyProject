using UnityEngine;

public class AbilityStone : MonoBehaviour
{
    [SerializeField] private Skill _skillToUnlock;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<SkillManager>().UnlockSkill(_skillToUnlock);
            Destroy(gameObject);
        }
    }
}
