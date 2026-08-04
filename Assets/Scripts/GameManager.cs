using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    // 
    public int pAmount;
    public int score;
    public int caramelRate;
    public int choolateRate;
    public int rainbowRate;
    public int autoMakerCount;
    // Counter
    int regularCount;
    int caramelCount;
    int choolateCount;
    int rainbowCount;

    int currentUpScore;

    [SerializeField] TextMeshProUGUI pAmountText;

    [SerializeField] MainPopcornMaker mainMaker;
    [SerializeField] GameObject autoPopcornMakerPrefab;
    [SerializeField] GameObject autoPopcornMakes;
    List<AutoPopcornMaker> autoMakers = new List<AutoPopcornMaker>(0);
    [SerializeField] GameObject[] points;


    // UI
    [SerializeField] GameObject UI;
    TextMeshProUGUI[] uiTexts = new TextMeshProUGUI[5];

    [SerializeField] GameObject[] buttons;

    // Popcorn
    [SerializeField] Sprite[] popcornSprites;
    [SerializeField] GameObject popcornPrefab;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        uiTexts = new TextMeshProUGUI[5];
        for (int i = 0; i < 5; i++) uiTexts[i] = UI.transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        uiTexts[0].text = $"{score}";
        uiTexts[1].text = $"{caramelRate}%";
        uiTexts[2].text = $"{choolateRate}%";
        uiTexts[3].text = $"{rainbowRate}%";
        uiTexts[4].text = $"{autoMakerCount}‹@";

        pAmountText.text = $"{pAmount.ToString()}p";
    }

    public void OnClick()
    {
        int kind = Lottery();
        GameObject p = Instantiate(popcornPrefab, mainMaker.transform.GetChild(0));
        Sprite s = popcornSprites[kind -1];
        p.GetComponent<Popcorn>().score = currentUpScore;
        p.GetComponent<SpriteRenderer>().sprite = s;
    }

    public void AddAutoMaker()
    {
        Vector2 spawnPos = GetRandomPositionBetweenPoints();
        GameObject a = Instantiate(autoPopcornMakerPrefab, spawnPos, Quaternion.identity);
        a.transform.parent = autoPopcornMakes.transform;
        autoMakers.Add(a.transform.GetComponent<AutoPopcornMaker>());
    }

    public void AutoClick(GameObject auto)
    {
        int kind = Lottery();
        GameObject p = Instantiate(popcornPrefab, auto.transform);
        Sprite s = popcornSprites[kind - 1];
        p.GetComponent<Popcorn>().score = currentUpScore;
        p.GetComponent<SpriteRenderer>().sprite = s;
    }

    public Vector2 GetRandomPositionBetweenPoints()
    {
        if (points == null || points.Length < 2 || points[0] == null || points[1] == null)
        {
            return Vector2.zero;
        }

        Vector2 startPos = points[0].transform.position;
        Vector2 endPos = points[1].transform.position;

        float randomT = Random.Range(0.0f, 1.0f);

        Vector2 randomPosition = Vector2.Lerp(startPos, endPos, randomT);

        return randomPosition;
    }

    /// <summary>
    /// ’Š‘I
    /// </summary>
    /// <returns></returns>
    int Lottery()
    {
        if (Random.Range(0, 100) < rainbowRate)
        {
            pAmount += score * 5;
            currentUpScore = score * 5;
            rainbowCount++;
            return 4;
        }

        if (Random.Range(0, 100) < choolateRate)
        {
            pAmount += score * 3;
            currentUpScore = score * 3;
            choolateCount++;
        }

        if (Random.Range(0, 100) < caramelRate)
        {
            pAmount += score * 2;
            currentUpScore = score * 2;
            caramelCount++;
            return 2;
        }

        pAmount += score;
        currentUpScore = score;
        regularCount++;
        return 1;
    }


}
