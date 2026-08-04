using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int pAmount;
    public int score;
    public int caramelRate;
    public int choolateRate;
    public int rainbowRate;
    [SerializeField] TextMeshProUGUI pAmountText;

    [SerializeField] MainPopcornMaker mainMaker;
    [SerializeField] GameObject autoPopcornMakerPrefab;
    List<AutoPopcornMaker> autoMakers;

    [SerializeField] GameObject[] buttons;
    TextMeshProUGUI[] levelTexts;
    TextMeshProUGUI[] amountTexts;

    // Popcorn
    [SerializeField] Sprite[] popcornSprites;
    [SerializeField] GameObject popcornPrefab;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        pAmountText.text = $"{pAmount.ToString()}p";

    }

    void Update()
    {
        
    }

    public void OnClick()
    {
        int kind = Lottery();
        GameObject p = Instantiate(popcornPrefab, mainMaker.transform.GetChild(0));
        Sprite s = popcornSprites[kind -1];
        p.GetComponent<SpriteRenderer>().sprite = s;

        pAmountText.text = $"{pAmount.ToString()}p";
    }

    int Lottery()
    {
        int rand = Random.Range(0, 100); // 0 ～ 99 のランダム値

        // ① レインボー（5倍）の判定（例: rainbowRateが5なら、0~4の5%で当選）
        if (rand < rainbowRate)
        {
            pAmount += score * 5;
            return 4;
        }

        // ② チョコレート（3倍）の判定
        if (rand < rainbowRate + choolateRate)
        {
            pAmount += score * 3;
            return 3;
        }

        // ③ キャラメル（2倍）の判定
        if (rand < rainbowRate + choolateRate + caramelRate)
        {
            pAmount += score * 2;
            return 2;
        }

        // ④ 外れ（通常ポップコーン）
        pAmount += score;
        return 1;
    }
}
