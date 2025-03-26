using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{

    [SerializeField] private TextMeshPro _text;

    private float disappearTimer;
    private Color textColor;
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
        float moveYSpeed = Random.Range(15f, 20f);
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;
        disappearTimer -= Time.deltaTime;
        if(disappearTimer < 0){
            Destroy(gameObject);

        }
    }

    public static DamagePopup Create(Vector3 position, float damageAmount){
        Vector2 randomPosition = new Vector2(position.x + Random.Range(-2f, 2f), position.y + Random.Range(-.5f, .5f));
        position = randomPosition;
        Transform damagePopupTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);
        
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount);
        

        return damagePopup;
        
    }

    private void Setup(float damageAmount){
        _text.SetText(damageAmount.ToString());
        disappearTimer = 1f;
    }

    
}
