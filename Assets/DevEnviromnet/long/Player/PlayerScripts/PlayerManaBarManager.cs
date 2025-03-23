using UnityEngine;
using UnityEngine.UI;

public class PlayerManaBarManager : MonoBehaviour
{
    [SerializeField] private Image manaFill;
    [SerializeField] private Image manaFillEase;
    [SerializeField] public float fillSpeed = 1f;
    [SerializeField] public float fillEaseSpeed = 0.1f;
    [SerializeField] private PlayerStats _stats;

    public float timeBeforeEase = 1f;
    private float easeTimer = 0f;
    private bool isEasing = false;
     private float previousManaRatio;
    private float currentVelocity;

    public PlayerStats Stats => _stats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Stats.currentMana = Stats.maxMana;
    }

    // Update is called once per frame
    void Update()
    {
        float manaRatio = Stats.currentMana / Stats.maxMana;
        float epsilon = 0.0001f;
        
        if(Mathf.Abs(manaFill.fillAmount - manaRatio) > epsilon)
        {
           if(manaFill.fillAmount < manaRatio)
            {
                manaFill.fillAmount = Mathf.SmoothDamp(
                    manaFill.fillAmount, 
                    manaRatio, 
                    ref currentVelocity, 
                    fillSpeed
                );
            }else{
                manaFill.fillAmount = manaRatio;
            }
            easeTimer = 0f;
            if(manaRatio > previousManaRatio)
            {
                manaFillEase.fillAmount = manaRatio;
            }
        }

        previousManaRatio = manaRatio;
        
        easeTimer += Time.deltaTime;
        if(Mathf.Abs(manaFillEase.fillAmount - manaRatio) > epsilon && easeTimer >= timeBeforeEase) 
        {
            if(manaRatio < manaFillEase.fillAmount)
            {
                manaFillEase.fillAmount = Mathf.SmoothDamp(
                    manaFillEase.fillAmount, 
                    manaRatio, 
                    ref currentVelocity, 
                    fillEaseSpeed
                );
            }
        }
    }
}
