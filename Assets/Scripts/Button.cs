using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeButton
{
    None,
    PlayerLevel,
    ScoreUp,
    Times,
    CaramelRate,
    ChoocolateRate,
    RainbowRate,
}

public enum AutoButton
{
    None,
    Count,
    RecastTime,
    Times,
}

public class Button : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] UpgradeButton upgradeButton = UpgradeButton.None;  // 強化種類
    [SerializeField] AutoButton autoButton = AutoButton.None;           // 自動化種類
    [SerializeField] Kind kind = Kind.Regular;                          // ポップコーンの種類     
    [SerializeField] bool lockFlag;         // 開放あるか
    [SerializeField] int lockPlayerLevel;   // 開放プレイヤーレベル
    [SerializeField] long baseNeedAmount;    // 初期コスト
    [SerializeField] float costMultiplier;  // コスト増加率
    [SerializeField] int limitLevel;        // レベル上限

    // 
    GameManager gameManager;
    int multiple;
    double needAmount;
    bool limitFlag;
    int level = 1;
    Image lockImage;
    TextMeshProUGUI[] texts;
    Image[] images;

    private void OnEnable()
    {
        // GameManagerのイベントに自分の更新関数を「登録」する
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateMultiple += UpdateCostUI;
        }

        UpdateCostUI(1);
    }

    private void OnDisable()
    {
        // オブジェクトが非アクティブ/破棄される時は登録を「解除」する（メモリリーク防止！）
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateMultiple -= UpdateCostUI;
        }
    }

    void Awake()
    {
        InitVariable();
        SetUpData();
    }

    void Update()
    {
        if (lockFlag && GameManager.Instance.playerLevel >= lockPlayerLevel)
        {
            lockFlag = false;
            transform.GetChild(4).gameObject.SetActive(lockFlag);
        }

    }
    
    // 変数の初期化
    void InitVariable()
    {
        gameManager = GameManager.Instance;
        needAmount = baseNeedAmount;

        texts = new TextMeshProUGUI[6];            
        for (int i = 0; i < 4; i++) texts[i] = transform.GetChild(i).GetComponent<TextMeshProUGUI>();
        for (int i = 0; i < 2; i++) texts[i + 4] = transform.GetChild(4).GetChild(i).GetComponent<TextMeshProUGUI>();
        images = new Image[2];
        for (int i = 0; i < images.Length; i++)
        {
            images[i] = transform.GetChild(i + 5).GetComponent<Image>();
            images[i].gameObject.SetActive(false);
        }
    }

    // 設定させたデータを元に初期化
    void SetUpData()
    {
        // upgradeButtonからNameを書き換える
        string name = "NULL";
        if (upgradeButton != UpgradeButton.None)
        {
            gameObject.name = $"{upgradeButton}Button";
            switch (upgradeButton)
            {
                case UpgradeButton.PlayerLevel:
                    name = $"プレイヤー\nレベルアップ";
                    break;
                case UpgradeButton.ScoreUp:
                    name = $"スコアアップ";
                    break;
                case UpgradeButton.Times:
                    name = $"生産量アップ";
                    break;
                case UpgradeButton.CaramelRate:
                    name = $"キャラメル\n確率アップ";
                    break;
                case UpgradeButton.ChoocolateRate:
                    name = $"チョコレート\n確率アップ";
                    break;
                case UpgradeButton.RainbowRate:
                    name = $"レインボー\n確率アップ";
                    break;
            }
        }
        else if (autoButton != AutoButton.None)
        {
            gameObject.name = $"{autoButton}Button";
            switch (kind)
            {
                case Kind.Regular:
                case Kind.Caramel:
                case Kind.Chocolate:
                case Kind.Rainbow:
                    texts[0].GetComponent<RectTransform>().anchoredPosition = new Vector3(-40f, 25f);
                    for (int i = 0; i < images.Length; i++) images[i].gameObject.SetActive(true);                                            
                    images[0].sprite = GameManager.Instance.makerSprites[(int)kind];
                    images[1].sprite = GameManager.Instance.popcornSprites[(int)kind - 1];
                    
                    switch (autoButton)
                    {
                        case AutoButton.Count:
                            name = $"生産マシン追加";
                            break;
                        case AutoButton.RecastTime:
                            name = $"生産マシン\n生産速度アップ";
                            break;
                        case AutoButton.Times:
                            name = $"生産マシン\n生産量アップ";
                            break;
                    }
                    break;
                case Kind.Auto:
                    texts[0].GetComponent<RectTransform>().anchoredPosition = new Vector3(-40f, 25f); 
                    images[0].gameObject.SetActive(true);
                    images[0].sprite = GameManager.Instance.makerSprites[0];
                    switch (autoButton)
                    {
                        case AutoButton.Count:
                            name = $"自動マシン追加";
                            break;
                        case AutoButton.RecastTime:
                            name = $"自動マシン\n生産速度アップ";
                            break;
                        case AutoButton.Times:
                            name = $"自動マシン\n生産量アップ";
                            break;
                    }
                    break;
            }            
        }

        texts[0].text = name;
        texts[4].text = name;

        // レベルとコストのテキストを更新
        UpdateUI();

        // 開放があるか判定
        transform.GetChild(4).gameObject.SetActive(lockFlag);
        texts[5].text = $"PlayerLv.{lockPlayerLevel}";
    }

    // クリック関数
    public void ClickButton()
    {
        if (!limitFlag && !lockFlag)
        {
            if (GameManager.Instance.pAmount >= needAmount)
            {
                AudioManager.Instance.PlayOneShotSE(1);
                int l = level;
                int m = GameManager.Instance.multiple;
                if (level + 1 * GameManager.Instance.multiple >= limitLevel) m = limitLevel - level;
                level += m;
                GameManager.Instance.pAmount -= needAmount;
                UpdataUpgrade();

                if (limitLevel == level)
                {
                    Limit();
                    return;
                }

                needAmount = CalculationNeedAmount();
            }

            UpdateCostUI(0);
        }
    }

    void UpdateCostUI(int m)
    {
        multiple = Mathf.Min(GameManager.Instance.multiple, limitLevel - level);
        needAmount = CalculationNeedAmount();
        UpdateUI();
    }

    double CalculationNeedAmount()
    {
        double need = 0;
        int l = level;
        for (int i = 0; i < multiple; i++)
        {
            need += (double)Mathf.Floor(baseNeedAmount * Mathf.Pow(costMultiplier, l - 1));
            l++;
        }
        return need;
    }

    // 上限時に実行
    void Limit()
    {
        texts[1].text = $"Lv.MAX";
        texts[2].text = $"MAX";
        texts[3].text = $"";
        limitFlag = true;
    }

    // GameManagerを更新
    void UpdataUpgrade()
    {
        int l = level;
        if (upgradeButton != UpgradeButton.None)
        {
            switch (upgradeButton)
            {
                case UpgradeButton.PlayerLevel:
                    GameManager.Instance.playerLevel += 1 * multiple;
                    break;
                case UpgradeButton.ScoreUp:
                    GameManager.Instance.score += 1 * multiple;
                    break;
                case UpgradeButton.Times:
                    GameManager.Instance.times += 1 * multiple;
                    break;
                case UpgradeButton.CaramelRate:
                    GameManager.Instance.caramelRate += 1 * multiple;
                    break;
                case UpgradeButton.ChoocolateRate:
                    GameManager.Instance.chocolateRate += 1 * multiple;
                    break;
                case UpgradeButton.RainbowRate:
                    GameManager.Instance.rainbowRate += 1 * multiple;
                    break;
            }
        }
        else if (autoButton != AutoButton.None)
        {
            switch (autoButton)
            {
                case AutoButton.Count:
                    GameManager.Instance.autoMakerSettings[(int)kind].makerCount += 1 * multiple;
                    for (int i = 0; i < multiple; i++) GameManager.Instance.AddAutoMaker((int)kind);                    
                    break;
                case AutoButton.RecastTime:
                    GameManager.Instance.autoMakerSettings[(int)kind].makerRecastTime -= 0.5f * multiple;
                    break;
                case AutoButton.Times:
                    GameManager.Instance.autoMakerSettings[(int)kind].makerTimes += 1 * multiple;
                    break;
            }
            GameManager.Instance.UpdateAutoMakerSetting((int)kind);
        }

    }

    // Textを更新
    void UpdateUI()
    {
        texts[1].text = $"Lv.{level}\nMAX {limitLevel}";
        texts[2].text = $"{ScoreFormatter.FormatToJapanese(needAmount)}p";
        float before = 0;
        float after = 0;
        string p = "%";
        if (upgradeButton != UpgradeButton.None)
        {
            switch (upgradeButton)
            {
                case UpgradeButton.PlayerLevel:
                    before = GameManager.Instance.playerLevel;
                    after = before + 1 * multiple;
                    p = "Lv.";
                    texts[3].text = $"{p}{before}→{p}{after}";
                    return;                    
                case UpgradeButton.ScoreUp:
                    before = GameManager.Instance.score;
                    p = "p";
                    break;
                case UpgradeButton.Times:
                    before = GameManager.Instance.times;
                    p = "回";
                    break;
                case UpgradeButton.CaramelRate:
                    before = GameManager.Instance.caramelRate;
                    break;
                case UpgradeButton.ChoocolateRate:
                    before = GameManager.Instance.chocolateRate;
                    break;
                case UpgradeButton.RainbowRate:
                    before = GameManager.Instance.rainbowRate;
                    break;
            }
            after = before + 1 * multiple;
        }
        else if (autoButton != AutoButton.None)
        {
            switch (autoButton)
            {
                case AutoButton.Count:
                    before = GameManager.Instance.autoMakerSettings[(int)kind].makerCount;
                    after = before + 1 * multiple;
                    p = "機";
                    break;
                case AutoButton.RecastTime:
                    before = GameManager.Instance.autoMakerSettings[(int)kind].makerRecastTime;
                    after = before - 0.5f * multiple;
                    p = "秒";
                    break;
                case AutoButton.Times:
                    before = GameManager.Instance.autoMakerSettings[(int)kind].makerTimes;
                    after = before + 1 * multiple;
                    p = "回";
                    break;
            }
        }

        texts[3].text = $"{before}{p}→{after}{p}";
    }
}
