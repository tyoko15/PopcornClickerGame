using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
スキルの内容
スキルは、共通して一定時間効果を発動して効果終了後
リキャストタイムがある。

1. 生産固定化
FixedPopcorn
クリックで生産されるポップコーンが()になる。
2. 生産量アップ
TimesUp
生産量が()倍になる。
3. スコア倍率アップ
BonusUp
生産された全ポップコーンのスコアが()倍になる。
4. マシンの分身
MakerOffshoot
稼働中の全マシンの()倍の数になる。
5. マシン稼働速度アップ
MakerSpeedUp
稼働中の全マシンの生産時間を()%短縮。
6. マシン生産量アップ
MakerTimesUp
稼働中の全マシンの生産量が()倍になる。
7. クリティカル
Critical
クリックで生産されたポップコーンに()%で5倍になる。
8. フィーバー
Fever
フィーバータイム内に生産された全ポップコーンスコアの()倍をスキル終了後に得る。
*/
public enum SkillType
{
    None,
    FixedPopcorn,
    TimesUp,
    BonusUp,
    MakerOffshoot,
    MakerSpeedUp,
    MakerTimesUp,
    Critical,
    Fever
}

public enum PanelType
{
    Lv1,
    Lv2,
    Lv3,
    Reinforcement,
    Special,
}

public enum Reinforcement
{
    EffectTime,
    RecastTime
}

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;
    [SerializeField] GameObject layer;
    TextMeshProUGUI[] infoTexts;

    public Sprite[] skillPanelImages;
    public Color[] lineColors;

    // 選択
    bool selectFlag;
    //public Color[] panelColors;
    [SerializeField] Image selectImage;
    [SerializeField] float selectTime;
    float selectTimer;
    [SerializeField] RectTransform viewport;
    [SerializeField] RectTransform content;
    Vector2 targetPos;
    Vector2 targetRect;         // 選択されたスキルパネルの位置
    Vector2 viewportSize;       // Viewportのサイズ
    Vector2 contentSize;        // Contentのサイズ
    Vector2 currentContentRect; // 現在のContentの位置

    // SkillPanel
    PanelLine[] panelLines;
    SkillPanel[] skillPanels;
    PanelLine selectPanelLine;
    SkillPanel selectSkillPanel;
    Image acquisitionButton;
    TextMeshProUGUI acquisitionText;

    bool oneFlag;

    private void Awake()
    {
        Instance = this;
        infoTexts = new TextMeshProUGUI[4];
        for (int i = 0; i < infoTexts.Length; i++) infoTexts[i] = layer.transform.GetChild(1).GetChild(i).GetComponent<TextMeshProUGUI>();
        selectImage.gameObject.SetActive(false);
        panelLines = new PanelLine[9];
        for (int i = 0; i < panelLines.Length; i++) panelLines[i] = layer.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetComponent<PanelLine>();        
        skillPanels = new SkillPanel[15 * 8 + 1];
        for (int i = 0; i < 8; i++) for (int j = 0; j < 15; j++) skillPanels[i * 15 + j] = layer.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i+1).GetChild(1).GetChild(j).GetComponent<SkillPanel>();
        skillPanels[skillPanels.Length - 1] = layer.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<SkillPanel>();
        skillPanels[skillPanels.Length - 1].InitVariable();
        acquisitionButton = layer.transform.GetChild(1).GetChild(5).GetComponent<Image>();
        acquisitionText = layer.transform.GetChild(1).GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        viewportSize = viewport.sizeDelta;
        contentSize = content.sizeDelta;
        content.anchoredPosition = new Vector2(-(contentSize.x * content.localScale.x / 2) + (viewportSize.x / 2),(contentSize.y * content.localScale.y / 2) - (viewportSize.y / 2));

        infoTexts[0].text = $"スキル名";
        infoTexts[1].text = $"";
        infoTexts[2].text = $"";
        infoTexts[3].text = $"";
        acquisitionButton.color = Color.gray;
        acquisitionText.color = Color.gray;
        acquisitionText.text = $"未選択";
    }

    void Update()
    {
        if (selectFlag) PickUpSkillPanel();
        bool specialFlag = false;
        for (int i = 0; i < panelLines.Length - 1; i++)
        {
            if (!panelLines[i + 1].allOpenFlag)
            {
                specialFlag = true;
                break;
            }
        }
        if (!specialFlag && !oneFlag)
        {
            UnlockSpecialPanel();
            oneFlag = true;
        }
    }

    public void SetInfo(SkillPanel panel, Vector2 target)
    {
        SkillPanelData data = panel.data;
        targetPos = target;
        // 名前を代入
        string name = $"";
        switch (data.skillType)
        {
            case SkillType.None:
                if (data.panelType == PanelType.Special) name = $"全開放";
                break;
            case SkillType.FixedPopcorn:
                name = $"生産固定化";
                break;
            case SkillType.TimesUp:
                name = $"生産量アップ";
                break;
            case SkillType.BonusUp:
                name = $"スコア倍率アップ";
                break;
            case SkillType.MakerOffshoot:
                name = $"マシン数アップ";
                break;
            case SkillType.MakerSpeedUp:
                name = $"マシン生産速度\nアップ";
                break;
            case SkillType.MakerTimesUp:
                name = $"マシン生産量\nアップ";
                break;
            case SkillType.Critical:
                name = $"クリティカル";
                break;
            case SkillType.Fever:
                name = $"フィーバー";
                break;
        }
        
        for (int i = 0; i < panelLines.Length; i++)
        {
            for (int j = 0; j < panelLines[i].skillPanels.Length; j++)
            {
                if (panelLines[i].skillPanels[j] == panel)
                {
                    selectPanelLine = panelLines[i];
                }
            }
        }
        
        selectSkillPanel = panel;
        acquisitionButton.color = (selectSkillPanel.state == PanelState.Lock) ? Color.gray : (selectSkillPanel.state == PanelState.UnLock) ? Color.white : Color.gray;
        acquisitionText.color = (selectSkillPanel.state == PanelState.Lock) ? Color.gray : (selectSkillPanel.state == PanelState.UnLock) ? Color.white : Color.yellow;
        acquisitionText.text = (selectSkillPanel.state == PanelState.Lock) ? $"未解放" : (selectSkillPanel.state == PanelState.UnLock) ? $"獲得" : $"獲得済";
        if (selectSkillPanel.state == PanelState.UnLock && data.cost >= GameManager.Instance.pAmount)
        {
            acquisitionButton.color = Color.gray;
            acquisitionText.color = Color.gray;
            acquisitionText.text = "不足";
        }
        infoTexts[0].text = name;
        infoTexts[1].text = $"{ScoreFormatter.FormatToJapanese(data.cost)}p";
        infoTexts[2].text = $"{ScoreFormatter.FormatToJapanese(GameManager.Instance.pAmount)}p";
        infoTexts[3].text = data.infoText;
        targetRect = new Vector2(-contentSize.x * content.localScale.x / 2 + viewport.anchoredPosition.x - target.x * content.localScale.x,
                                  contentSize.y * content.localScale.y / 2 - viewport.anchoredPosition.y - target.y * content.localScale.y);
        currentContentRect = content.anchoredPosition;
        selectImage.gameObject.SetActive(true);
        selectImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(target.x - 60f, target.y + 60f);
        selectFlag = true;
    }

    void UnlockSpecialPanel()
    {
        panelLines[0].UnlockSpecialPanel();
    }


    void PickUpSkillPanel()
    {
        if (selectTimer > selectTime)
        {
            content.anchoredPosition = targetRect;
            selectFlag = false;
            selectTimer = 0;
        }
        else
        {
            selectTimer += Time.deltaTime;
            Vector2 center = new Vector2(Mathf.Lerp(currentContentRect.x, targetRect.x, selectTimer / selectTime), Mathf.Lerp(currentContentRect.y, targetRect.y, selectTimer / selectTime));
            content.anchoredPosition = center;
        }
    }
    public void ResetPosition()
    {
        content.anchoredPosition = new Vector2(-(contentSize.x * content.localScale.x / 2) + (viewportSize.x / 2), (contentSize.y * content.localScale.y / 2) - (viewportSize.y / 2));
    }

    ///
    public void ClickAcquisitionButton()
    {
        if (selectSkillPanel.state == PanelState.UnLock)
        {
            if (selectSkillPanel.data.cost <= GameManager.Instance.pAmount)
            {
                // 獲得の処理
                GameManager.Instance.pAmount -= selectSkillPanel.data.cost;
                selectSkillPanel.state = PanelState.Acquired;

                // SkillListを更新
                if (selectSkillPanel.data.panelType == PanelType.Lv1)
                {
                    SkillDataInfo info = new SkillDataInfo();                   
                    info.skillType = selectSkillPanel.data.skillType;
                    info.level = 1;
                    info.effectTime = selectSkillPanel.data.effectTime;
                    info.effectTimer = 0f;
                    info.recastFlag = false;     // 実験用
                    info.recastTime = selectSkillPanel.data.recastTime;
                    info.recastTimer = 0f;
                    info.infoText = selectSkillPanel.data.infoText;

                    SkillListManager.Instance.RegistrationSkill(info);
                }
                else if (selectSkillPanel.data.panelType == PanelType.Reinforcement)
                {
                    SkillListManager.Instance.UpdateSkill(selectSkillPanel.data);
                }
                else
                {
                    SkillListManager.Instance.UpgradeSkill(selectSkillPanel.data);
                }
                // UIの更新
                if (panelLines[0] == selectPanelLine) selectPanelLine.AcquiredSpecialPanel();
                else selectPanelLine.UpdatePanelLineState(selectSkillPanel);
                SetInfo(selectSkillPanel, targetPos);
            }
        }
    }
}
