using System.Threading;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

/*
スキルの内容
スキルは、共通して一定時間効果を発動して効果終了後
リキャストタイムがある。

1. 生産固定
FixedPopcorn
クリック時生産されるポップコーンが固定される。
2. 生産量アップ
TimesUp
クリック時生産量が倍になる。
3. スコア倍率アップ
BonusUp
生産されたポップコーンのスコアに倍率が乗る。
4. クリックに合わせた倍数アップ
RepeatBonus
連打数に合わせた倍率が現在のポップコーンのスコアにかかる。
5. マシン稼働速度アップ
MakerSpeedUp
全稼働中のマシンの生産時間を％短縮。
6. マシン生産量アップ
MakerTimesUp
全稼働中のマシンの生産量が倍になる。
7. クリティカル
Critical
クリック時確率で生産させたポップコーンのスコアに倍率が乗る。
8. フィーバー
Fever
フィーバータイム内に生産されたポップコーンのスコアの倍を計上する。
9. 

*/
public enum SkillType
{
    None,
    FixedPopcorn,
    TimesUp,
    BonusUp,
    RepeatBonus,
    MakerSpeedUp,
    MakerTimesUp,
    Critical,
    Fever
}

public enum PanelType
{
    Reinforcement,
    Lv1,
    Lv2,
    Lv3,
}

[System.Serializable]
public class SkillInfo
{
    public SkillType skillType;
    public PanelType panelType;
    public int cost;
    public string infoText;
}

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;
    [SerializeField] GameObject layer;
    TextMeshProUGUI[] infoTexts;

    public Sprite[] skillPanelImages;

    public Color lockColor;
    public Color selectColor;
    bool selectFlag;
    [SerializeField] float selectTime;
    float selectTimer;
    [SerializeField] RectTransform viewport;
    [SerializeField] RectTransform content;
    Vector2 centerRect;
    Vector2 targetRect;
    Vector2 viewportRect; // ScrollViewのViewport
    Vector2 contentRect;  // ScrollViewのContent
    Vector2 currentContentRent;

    private void Awake()
    {
        Instance = this;
        infoTexts = new TextMeshProUGUI[3];
        for (int i = 0; i < infoTexts.Length; i++) infoTexts[i] = layer.transform.GetChild(1).GetChild(i).GetComponent<TextMeshProUGUI>();
    
    }

    void Start()
    {
        viewportRect = viewport.sizeDelta;
        contentRect = content.sizeDelta;
        content.anchoredPosition = new Vector2(-(contentRect.x * content.localScale.x / 2) + (viewportRect.x / 2),(contentRect.y * content.localScale.y / 2) - (viewportRect.y / 2));
    }

    void Update()
    {
        if (selectFlag) PickUpSkillPanel();
    }

    public void SetInfo(SkillInfo info, Vector2 target)
    {
        // 名前を代入
        string name = $"";
        switch (info.skillType)
        {
            case SkillType.FixedPopcorn:
                name = $"生産固定化";
                break;
            case SkillType.TimesUp:
                name = $"生産量アップ";
                break;
            case SkillType.BonusUp:
                name = $"スコア倍率アップ";
                break;
            case SkillType.RepeatBonus:
                name = $"クリックに合わせた倍数アップ";
                break;
            case SkillType.MakerSpeedUp:
                name = $"マシン稼働速度アップ";
                break;
            case SkillType.MakerTimesUp:
                name = $"マシン生産量アップ";
                break;
            case SkillType.Critical:
                name = $"クリティカル";
                break;
            case SkillType.Fever:
                name = $"フィーバー";
                break;
        }

        infoTexts[0].text = name;
        infoTexts[1].text = $"{ScoreFormatter.FormatToJapanese(info.cost)}p";
        infoTexts[2].text = info.infoText;
        targetRect = target;
        currentContentRent = content.anchoredPosition;
        centerRect = new Vector2(-(contentRect.x * content.localScale.x / 2) + (viewportRect.x / 2), (contentRect.y * content.localScale.y / 2) - (viewportRect.y / 2));
        selectFlag = true;
    }



    void PickUpSkillPanel()
    {

        if (selectTimer > selectTime)
        {
            content.anchoredPosition = centerRect;
            selectFlag = false;
            selectTimer = 0;
        }
        else
        {
            selectTimer += Time.deltaTime;
            Vector2 center = new Vector2(Mathf.Lerp(currentContentRent.x, centerRect.x, selectTimer / selectTime), Mathf.Lerp(currentContentRent.y, centerRect.y, selectTimer / selectTime));
            content.anchoredPosition = center;
        }
    }
    public void ResetPosition()
    {
        content.anchoredPosition = new Vector2(-(contentRect.x * content.localScale.x / 2) + (viewportRect.x / 2), (contentRect.y * content.localScale.y / 2) - (viewportRect.y / 2));
    }
}
