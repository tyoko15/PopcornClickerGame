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
    [SerializeField] int baseNeedAmount;    // 初期コスト
    [SerializeField] float costMultiplier;  // コスト増加率
    [SerializeField] int limitLevel;        // レベル上限

    // 
    GameManager gameManager;
    int needAmount;
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
        if (lockFlag && gameManager.playerLevel >= lockPlayerLevel)
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
                    images[0].sprite = gameManager.makerSprites[(int)kind];
                    images[1].sprite = gameManager.popcornSprites[(int)kind - 1];
                    
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
                    images[0].sprite = gameManager.makerSprites[0];
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
            if (gameManager.pAmount >= needAmount)
            {
                AudioManager.Instance.PlayOneShotSE(1);
                int l = level;
                int m = GameManager.Instance.multiple;
                if (level + 1 * GameManager.Instance.multiple >= limitLevel) m = limitLevel - level;
                level += m;
                gameManager.pAmount -= needAmount;
                UpdataUpgrade();

                if (limitLevel == level)
                {
                    Limit();
                    return;
                }

                needAmount = CalculationNeedAmount();
            }

            UpdateUI();
        }
    }

    void UpdateCostUI(int m)
    {
        needAmount = CalculationNeedAmount();
        UpdateUI();
    }

    int CalculationNeedAmount()
    {
        int need = 0;
        int l = level;
        int m = GameManager.Instance.multiple;
        if (level + 1 * GameManager.Instance.multiple >= limitLevel) m = limitLevel - level;
        for (int i = 0; i < m; i++)
        {
            need += (int)Mathf.Floor(baseNeedAmount * Mathf.Pow(costMultiplier, l - 1));
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
        int m = GameManager.Instance.multiple;
        if (l + 1 * GameManager.Instance.multiple >= limitLevel) m = limitLevel - l;
        if (upgradeButton != UpgradeButton.None)
        {
            switch (upgradeButton)
            {
                case UpgradeButton.PlayerLevel:
                    gameManager.playerLevel += 1 * m;
                    break;
                case UpgradeButton.ScoreUp:
                    gameManager.score += 1 * m;
                    break;
                case UpgradeButton.Times:
                    gameManager.times += 1 * m;
                    break;
                case UpgradeButton.CaramelRate:
                    gameManager.caramelRate += 1 * m;
                    break;
                case UpgradeButton.ChoocolateRate:
                    gameManager.chocolateRate += 1 * m;
                    break;
                case UpgradeButton.RainbowRate:
                    gameManager.rainbowRate += 1 * m;
                    break;
            }
        }
        else if (autoButton != AutoButton.None)
        {
            switch (autoButton)
            {
                case AutoButton.Count:
                    gameManager.autoMakerSettings[(int)kind].makerCount += 1 * m;
                    for (int i = 0; i < m; i++) gameManager.AddAutoMaker((int)kind);                    
                    break;
                case AutoButton.RecastTime:
                    gameManager.autoMakerSettings[(int)kind].makerRecastTime -= 0.5f * m;
                    break;
                case AutoButton.Times:
                    gameManager.autoMakerSettings[(int)kind].makerTimes += 1 * m;
                    break;
            }
            gameManager.UpdateAutoMakerSetting((int)kind);
        }

    }

    // Textを更新
    void UpdateUI()
    {
        texts[1].text = $"Lv.{level}";
        texts[2].text = $"{needAmount.ToString("N0")}p";
        float before = 0;
        float after = 0;
        string p = "%";
        if (upgradeButton != UpgradeButton.None)
        {
            switch (upgradeButton)
            {
                case UpgradeButton.PlayerLevel:
                    before = gameManager.playerLevel;
                    after = before + 1 * gameManager.multiple;
                    if (level + 1 * gameManager.multiple >= limitLevel) after = before + 1 * limitLevel - level;                    
                    p = "Lv.";
                    texts[3].text = $"{p}{before}→{p}{after}";
                    return;                    
                case UpgradeButton.ScoreUp:
                    before = gameManager.score;
                    p = "p";
                    break;
                case UpgradeButton.Times:
                    before = gameManager.times;
                    p = "回";
                    break;
                case UpgradeButton.CaramelRate:
                    before = gameManager.caramelRate;
                    break;
                case UpgradeButton.ChoocolateRate:
                    before = gameManager.chocolateRate;
                    break;
                case UpgradeButton.RainbowRate:
                    before = gameManager.rainbowRate;
                    break;
            }
            after = before + 1 * gameManager.multiple;
            if (level + 1 * gameManager.multiple >= limitLevel) after = before + 1 * limitLevel - level;
        }
        else if (autoButton != AutoButton.None)
        {
            switch (autoButton)
            {
                case AutoButton.Count:
                    before = gameManager.autoMakerSettings[(int)kind].makerCount;
                    after = before + 1 * gameManager.multiple;
                    if (level + 1 * gameManager.multiple >= limitLevel) after = before + 1 * limitLevel - level;
                    p = "機";
                    break;
                case AutoButton.RecastTime:
                    before = gameManager.autoMakerSettings[(int)kind].makerRecastTime;
                    after = before - 0.5f * gameManager.multiple;
                    if (level + 1 * gameManager.multiple >= limitLevel) after = before - 0.5f * limitLevel - level;
                    p = "秒";
                    break;
                case AutoButton.Times:
                    before = gameManager.autoMakerSettings[(int)kind].makerTimes;
                    after = before + 1 * gameManager.multiple;
                    if (level + 1 * gameManager.multiple >= limitLevel) after = before + 1 * limitLevel - level;
                    p = "回";
                    break;
            }
        }
        
        texts[3].text = $"{before}{p}→{after}{p}";
    }
}
