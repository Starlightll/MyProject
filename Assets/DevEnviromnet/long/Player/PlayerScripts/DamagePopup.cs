using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{

    [SerializeField] private TextMeshPro _text;
    private float disappearTimer;
    private Color textColor;
    private const float DISAPPEAR_TIMER_MAX = 1f;
    private Vector3 moveVector;
    private int sortingOrder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Awake() {
        _text = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;
        if(disappearTimer > DISAPPEAR_TIMER_MAX * .5f){
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }else{
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }
        disappearTimer -= Time.deltaTime;
        if(disappearTimer < 0){
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            _text.color = textColor;
            if(textColor.a < 0){
                Destroy(gameObject);
            }
        }
    }

    public static DamagePopup Create(Vector3 position, float damageAmount, bool isCritical){
        Vector2 randomPosition = new Vector2(position.x + Random.Range(-2f, 2f), position.y + Random.Range(-.5f, .5f));
        position = randomPosition;
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);
        
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, isCritical);
    
        return damagePopup;
        
    }

    private void Setup(float damageAmount, bool isCritical){
        _text.SetText(damageAmount.ToString());
        
        if(!isCritical){
            _text.fontSize = Random.Range(20, 30);
            textColor = new Color(0f, 1f, 0.57f);
        }else{
            _text.fontSize = Random.Range(30, 40);
            textColor = new Color(1f, 0f, 0.27f);
        }
        disappearTimer = DISAPPEAR_TIMER_MAX;
        _text.color = textColor;
        sortingOrder++;
        _text.sortingOrder = sortingOrder;
        moveVector = new Vector3(Random.Range(-1f, 1f), 1) * 60f;
    }

    
}
