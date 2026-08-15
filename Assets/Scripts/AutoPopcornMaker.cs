using UnityEngine;
using UnityEngine.UI;

public class AutoPopcornMaker : MonoBehaviour
{
    GameObject spwaner;         // 生成位置
    public Kind kind;           // ポップコーンの種類
    public float recastTime;    // 生産時間
    float recastTimer;          // 生産時間タイマー
    public int times;           // 生産量

    Image gauge;
    void Start()
    {
        spwaner = transform.GetChild(0).gameObject;
        gauge = transform.GetChild(1).GetChild(1).GetComponent<Image>();
    }

    void Update()
    {
        // 生産
        if (recastTimer > recastTime)
        {
            for (int i = 0; i < times; i++) GameManager.Instance.AutoClick((int)kind, spwaner);
            recastTimer = 0;
        }
        else
        {
            recastTimer += Time.deltaTime;
            gauge.fillAmount = Mathf.InverseLerp(0f, recastTime, recastTimer);
        }
    }

    // マシンのイラスト編集
    public void SetSprite(Sprite sprite) => GetComponent<SpriteRenderer>().sprite = sprite;

}
