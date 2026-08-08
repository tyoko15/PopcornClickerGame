using UnityEngine;
using UnityEngine.UI;

public class AutoPopcornMaker : MonoBehaviour
{
    GameManager gameManager;
    GameObject spwaner;
    public Kind kind;
    public float recastTime;
    float recastTimer;
    public int times;

    Image gauge;
    void Start()
    {
        gameManager = GameManager.Instance;
        spwaner = transform.GetChild(0).gameObject;
        gauge = transform.GetChild(1).GetChild(1).GetComponent<Image>();
    }

    void Update()
    {
        if (recastTimer > recastTime)
        {
            for (int i = 0; i < times; i++) gameManager.AutoClick((int)kind, spwaner);
            recastTimer = 0;
        }
        else
        {
            recastTimer += Time.deltaTime;
            gauge.fillAmount = Mathf.InverseLerp(0f, recastTime, recastTimer);
        }
    }

    public void SetSprite(Sprite sprite) => GetComponent<SpriteRenderer>().sprite = sprite;

}
