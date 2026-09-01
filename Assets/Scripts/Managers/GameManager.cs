using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

// メーカーの種類
public enum Kind
{
    Auto,
    Regular,
    Caramel,
    Chocolate,
    Rainbow,
}

// メーカーの設定クラス
[System.Serializable]
public class MakerSetting
{
    public Kind kind;
    public int makerCount;
    public float makerRecastTime;
    public int makerTimes;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("強化できる設定")]
    public int playerLevel;     // プレイヤーレベル
    public double totalPAmount; // 通算ポップコーンスコア
    public double pAmount;      // 合計ポップコーンスコア
    public double recordPAmount;// 最大ポップコーンスコア
    public int score;           // 通常スコア
    public int times;           // 生産量
    [Header("抽選倍率設定")]
    public int caramelRate;     // キャラメルの確率
    public int chocolateRate;   // チョコレートの確率
    public int rainbowRate;     // レインボーの確率

    // Counter
    int clickCount;         // クリック回数
    int regularCount;       // レギュラー総数
    int caramelCount;       // キャラメル総数
    int chocolateCount;     // チョコレート総数
    int rainbowCount;       // レインボー総数
    int totalPopcornCount;  // 全ポップコーン数

    // プレイ時間
    int hour;       // 時間
    int minute;     // 分
    float second;   // 秒
    bool timerFlag; // カンストフラグ

    int currentUpScore;     // ポップコーンスコア
    [Header("各メーカー設定")]
    [SerializeField] MainPopcornMaker mainMaker;                        // メインのマシン
    [SerializeField] public Sprite[] makerSprites;                      // マシンのイラスト
    [SerializeField] GameObject autoPopcornMakerPrefab;                 // 自動マシンのプレハブ
    List<AutoPopcornMaker> autoMakers = new List<AutoPopcornMaker>(0);  // 自動マシンの情報リスト
    [SerializeField] GameObject autoPopcornMakerGroup;                  // 自動マシンの親オブジェクト
    public MakerSetting[] autoMakerSettings;                            // 自動マシンの設定
    [SerializeField] GameObject[] points;                               // 自動マシンの出現範囲用座標

    [Header("UI設定")]
    [SerializeField] GameObject UI;                         // UIのGameObject
    [SerializeField] TextMeshProUGUI pAmountText;           // ポップコーンスコアテキスト
    [SerializeField] TextMeshProUGUI instructionText;       // 指示テキスト
    [SerializeField] TextMeshProUGUI versionionText;        // バージョンテキスト
    [SerializeField] GameObject repeatUI;                   // 鬼連打のUI
    TextMeshProUGUI[] repeatTexts;                          // 鬼連打のテキスト
    TextMeshProUGUI[] uiTexts = new TextMeshProUGUI[26];    // 情報一覧のテキスト
    TextMeshProUGUI[] recordTexts = new TextMeshProUGUI[5]; // 記録のテキスト
    [SerializeField] GameObject bottomBannerButtons;
    [SerializeField] GameObject skillUI;
    TextMeshProUGUI[] skillTexts;
    [SerializeField] GameObject[] buttons;                  // 
    [HideInInspector] public int multiple = 1;              // 現在のボタン倍数
    public event Action<int> UpdateMultiple;                // 倍数更新用
    public int Multiple
    {
        get => multiple;
        set
        {
            // 値が変わった時だけ処理する
            if (multiple != value)
            {
                multiple = value;
                // 登録されている全てのボタンスクリプトに一斉通知！
                UpdateMultiple?.Invoke(multiple);
            }
        }
    }
    
    // 情報一覧用
    int totalLimitLevel = 2275;                             // 全強化&自動化ボタンの上限レベルの総数
    [HideInInspector] public int currentTotalLimitLevel;    // 現在の強化&自動化ボタンのレベルの数数

    // Popcorn
    [Header("ポップコーンのイラスト設定")]        
    [SerializeField] public Sprite[] popcornSprites;            // ポップコーンのイラスト   
    [SerializeField] GameObject popcornPrefab;                  // ポップコーンプレハブ
    [HideInInspector] public float popcornForceMultiple = 1f;   // ポップコーン生産勢力

    // 連打
    bool feverFlag;
    bool repeatFlag;                // 連打中フラグ
    int repeatCount;                // 連打回数
    public int recordRepeatCount;   // 最大連打回数
    float repeatTime = 1f;          // 連打継続秒数
    float repeatTimer;              // 連打継続タイマー
    float repeatBonus = 1f;         // 連打ボーナス倍率
    int nextBonusCount;             // ボーナスアップまでに必要な連打回数
    float nextBonus;                // 次の追加ボーナスアップ倍率
    
    // 連打速度
    int currentCPS;     // 現在の連打速度
    int maxCPS;         // 最大連打速度
    Queue<float> clickTimestamps = new Queue<float>();  // 連打速度の記録用

    // 発動スキル
    public bool activeFlag;
    SkillType activeSkillType;
    float activeEffectTime;
    float activeEffectTimer;
    string activeInfoText;
    // 発動スキルの変数
    // BonusUp
    int bonusUpSkill = 1;
    // Critical
    int criticalSkill = 0;
    // Fever
    bool feverSkillFlag;
    int totalFeverPAmount;
    // FixedPopcorn
    int fixedPopcornSkill = 1;
    // MakerOffshoot
    public int makerOffshootSkill = 1;
    // MakerSpeedUp
    public float makerSpeedUpSkill = 1;
    // MakerTimesUp
    public int makerTimesUpSkill = 1;
    // TimesUp
    int timesUpSkill = 1;

    // ゲーム開始の演出
    [SerializeField] Light2D baseLight; // ベースライト
    [SerializeField] Light2D spotLight; // スポットライト
    bool startFlag;     // スタート演出用フラグ
    bool goFlag;        // 開始クリックフラグ
    float startTime = 0.8f; // スタート演出時間
    float startTimer;       // スタート演出タイマー

    // ゲームポーズ
    public bool pauseFlag;
    public int pauseTime = 1;

    // 検証用オートクリッカー(本番では停止)
    [SerializeField] bool autoClickFlag;

    private void Awake()
    {        
        Instance = this;    // インスタンス化
    }


    void Start()
    {
        // 情報一覧テキストの初期化&取得
        uiTexts = new TextMeshProUGUI[26];
        for (int i = 0; i < uiTexts.Length; i++) uiTexts[i] = UI.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(i + 1).GetComponent<TextMeshProUGUI>();
        // 記録テキストの初期化&取得
        recordTexts = new TextMeshProUGUI[UI.transform.GetChild(0).GetChild(4).GetChild(0).GetChild(0).GetChild(0).childCount];
        for (int i = 0; i < UI.transform.GetChild(0).GetChild(4).GetChild(0).GetChild(0).GetChild(0).childCount - 1; i++) recordTexts[i] = UI.transform.GetChild(0).GetChild(4).GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetComponent<TextMeshProUGUI>();
        // 鬼連打テキストの初期化&取得
        repeatTexts = new TextMeshProUGUI[repeatUI.transform.childCount];
        for (int i = 0; i < repeatUI.transform.childCount; i++) repeatTexts[i] = repeatUI.transform.GetChild(i).GetComponent<TextMeshProUGUI>();
        skillTexts = new TextMeshProUGUI[skillUI.transform.childCount];
        for (int i = 0; i < skillTexts.Length; i++) skillTexts[i] = skillUI.transform.GetChild(i).GetComponent<TextMeshProUGUI>();
        // バージョンテキストの更新
        versionionText.text = $"v{Application.version}";

        // ゲーム開始演出の初期化
        instructionText.color = Color.white;    // 指示テキストを白へ変更
        baseLight.intensity = 0.2f;             // ベースライトの明るさを低く
        spotLight.gameObject.SetActive(false);  // スポットライトOFF
        // UI(指示テキスト以外)をOFF
        for (int i = 0; i < UI.transform.childCount - 1; i++) UI.transform.GetChild(i).gameObject.SetActive(false);       
        Camera.main.orthographicSize = 2;                               // メインカメラをズーム
        Camera.main.transform.position = new Vector3(0f, -0.5f, -5f);   // メインカメラ位置調整
    }

    void Update()
    {
        // 開始演出
        if (startFlag)
        {
            goFlag = true;
            GameStart();
            return;
        }

        Timer();        // ゲーム内時間計測用
        UpdateUI();     // UI更新
        Repeat();       // 連打計測
        CPS();          // 連打速度計測
        Skill();

        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.F)) autoClickFlag = true;
        else if (Input.GetKey(KeyCode.F)) autoClickFlag = false;

        // 検証用クリッカー
        // 本番はOFF
        if (!autoClickFlag) return;
        OnClick();
    }

    /// <summary>
    /// ゲーム開始演出関数
    /// </summary>
    void GameStart()
    {
        if (startTimer > startTime)     // ゲーム開始演出終了
        {
            for (int i = 0; i < 4; i++) UI.transform.GetChild(i).gameObject.SetActive(true);            
            baseLight.intensity = 1f;
            spotLight.gameObject.SetActive(false);
            spotLight.pointLightOuterAngle = 45f;
            spotLight.pointLightOuterRadius = 10f;
            versionionText.gameObject.SetActive(false);
            startTimer = 0;
            startFlag = false;
        }
        else if (startTimer > 0.5f)     // 0.5秒からstartTime内の内容
        {
            startTimer += Time.deltaTime;
        }
        else                            // 0秒から0.5秒内の内容
        {
            startTimer += Time.deltaTime;
            Camera.main.orthographicSize = Mathf.Lerp(2f, 5f, startTimer / (startTime - 0.3f));
            Camera.main.transform.position = new Vector3(0f, Mathf.Lerp(-0.5f, 0f, startTimer / (startTime - 0.3f)), -5f);
            spotLight.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// UIテキスト更新
    /// </summary>
    void UpdateUI()
    {
        int u = 0;  // インデックス
        // プレイヤー情報
        uiTexts[u++].text = $"Lv.{playerLevel}";                                                            // プレイヤーレベル
        uiTexts[u++].text = $"{OverallReinforcementRate().ToString("F2")}%";                                // 全強化達成率
        uiTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(totalPAmount)}p";                            // 通算ポップコーンスコア
        uiTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(recordPAmount)}p";                           // 最大ポップコーンスコア
        uiTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(clickCount)}回";                             // 通算クリック数
        uiTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(maxCPS)} 回/秒";                             // 瞬間クリック速度
        uiTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(currentCPS)} 回/秒";                         // 瞬間クリック速度
        uiTexts[u++].text = $"{hour.ToString("00")}:{minute.ToString("00")}:{second.ToString("00")}";       // プレイ時間
        // 効率
        uiTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(CalculateClickExpectedValue())}p";           // 1クリックで稼げる平均スコア
        uiTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(GetAutoMachineCPS())} p/s";                  // 自動マシン平均生産スコア
        // 通常スコア&生産量
        uiTexts[u++].text = $"{score}p";
        uiTexts[u++].text = $"{times}回";
        // 出現確率&倍率
        uiTexts[u++].text = $"{caramelRate}%";      // キャラメル出現確率
        uiTexts[u++].text = $"{chocolateRate}%";    // チョコレート出現確率
        uiTexts[u++].text = $"{rainbowRate}%";      // レインボー出現確率
        uiTexts[u++].text = $"すべて外れると出現";  // レインボー出現確率
        // 自動マシン
        if (makerOffshootSkill != 1)
        {
            for (int i = 0; i < autoMakerSettings.Length; i++)
            {
                uiTexts[i * 2 + u].text = $"<color=#23B500>{autoMakerSettings[i].makerCount * makerOffshootSkill}機</color>";                      // 機数
                uiTexts[i * 2 + u + 1].text = $"{(autoMakerSettings[i].makerRecastTime).ToString("F1")}秒に{autoMakerSettings[i].makerTimes}回";   // 生産時間と生産量
            }
        }
        else if (makerSpeedUpSkill != 1f)
        {
            for (int i = 0; i < autoMakerSettings.Length; i++)
            {
                uiTexts[i * 2 + u].text = $"{autoMakerSettings[i].makerCount}機";                                                                                                                      // 機数
                uiTexts[i * 2 + u + 1].text = $"<color=#23B500>{(autoMakerSettings[i].makerRecastTime * (2f - makerSpeedUpSkill)).ToString("F1")}秒</color>に{autoMakerSettings[i].makerTimes}回";     // 生産時間と生産量
            }
        }
        else if (makerTimesUpSkill != 1)
        {
            for (int i = 0; i < autoMakerSettings.Length; i++)
            {
                uiTexts[i * 2 + u].text = $"{autoMakerSettings[i].makerCount}機";                                                                                                                  // 機数
                uiTexts[i * 2 + u + 1].text = $"{autoMakerSettings[i].makerRecastTime}秒に<color=#23B500>{autoMakerSettings[i].makerTimes * GameManager.Instance.makerTimesUpSkill}回</color>";    // 生産時間と生産量
            }
        }
        else
        {
            for (int i = 0; i < autoMakerSettings.Length; i++)
            {
                uiTexts[i * 2 + u].text = $"{autoMakerSettings[i].makerCount}機";                                                // 機数
                uiTexts[i * 2 + u + 1].text = $"{autoMakerSettings[i].makerRecastTime}秒に{autoMakerSettings[i].makerTimes}回";  // 生産時間と生産量
            }
        }
        // 合計ポップコーンスコア
        pAmountText.text = $"{ScoreFormatter.FormatToJapanese(pAmount)}p";

        u = 1;  // インデックス初期化
        repeatTexts[u++].text = $"{repeatCount.ToString("N0")} <size=25>COMBO</size>!";                                     // 現在の連打数
        repeatTexts[u++].text = $"<size=25>BONUS</size> {repeatBonus.ToString("F1")}x";                                     // 連打ボーナス
        repeatTexts[u++].text = $"次のアップまで\n残り{nextBonusCount}回(+{nextBonus.ToString("F1")}x)";    // 次のボーナスアップまで必要な連打数と次のボーナスアップ
        repeatTexts[u++ + 1].text = $"{recordRepeatCount.ToString("N0")}回";                                // 最大連打数

        u = 1;
        string name = $"";
        switch (activeSkillType)
        {
            case SkillType.None :
                name = $"";
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
                name = $"マシン生産速度アップ";
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
        skillTexts[u++].text = name;
        skillTexts[u++].text = $"{(activeEffectTime-activeEffectTimer).ToString("F1")}秒";
        skillTexts[u++].text = activeInfoText;

        u = 0;  // インデックス初期化
        recordTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(regularCount)}個";        // レギュラー個数
        recordTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(caramelCount)}個";        // キャラメル個数
        recordTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(chocolateCount)}個";      // チョコレート個数
        recordTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(rainbowCount)}個";        // レインボー個数
        recordTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(totalPopcornCount)}個";   // 総ポップコーン個数
    }

    public void Pause(bool flag)
    {
        pauseFlag = flag;
        pauseTime = (!pauseFlag) ? 1 : 0;
    }

    /// <summary>
    /// 全強化達成率の計算関数
    /// </summary>
    /// <returns></returns>
    float OverallReinforcementRate()
    {
        if (currentTotalLimitLevel <= 0) currentTotalLimitLevel = 1;
        
        float r = (float)currentTotalLimitLevel / totalLimitLevel;
        if (currentTotalLimitLevel == 1) r = 0;
        return r * 100;
    }

    /// <summary>
    /// ゲーム内時間の計測関数
    /// </summary>
    void Timer()
    {
        // カンスト対策
        if (hour == 100)
        {
            hour = 99;
            minute = 59;
            second = 59;
            timerFlag = true;
        }
        if (timerFlag) return;

        // タイマー
        second += Time.deltaTime;

        if (second >= 60f)  // minuteの繰り上げ
        {
            second -= 60f;
            minute++;
        }
        if (minute == 60)   // hourの繰り上げ
        {
            minute = 0;
            hour++;
        }
    }

    /// <summary>
    /// 1クリックで稼げる平均スコアの計算関数
    /// </summary>
    /// <returns></returns>
    double CalculateClickExpectedValue()
    {
        // 各確率（% を 0.0~1.0 の少数に変換）
        double pRainbow = rainbowRate / 100.0;
        double pChoco = chocolateRate / 100.0;
        double pCaramel = caramelRate / 100.0;

        // レギュラーの確率（100% から各ポップコーンの合計確率を引いた残り）
        double pSalt = Math.Max(0, 1.0 - (pRainbow + pChoco + pCaramel));

        // 1個あたりの平均倍率
        double averageMultiplier = (pRainbow * 5.0) + (pChoco * 3.0) + (pCaramel * 2.0) + (pSalt * 1.0);

        // 1クリックの期待値 ＝ 基本スコア × 生成個数 × 平均倍率
        double expectedValue = score * times * averageMultiplier;

        return expectedValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    double GetClickEfficiency()
    {
        // 各確率（%表記なら 100 で割って 0.0~1.0 に変換）
        double pCaramel = caramelRate / 100.0;
        double pChoco = chocolateRate / 100.0;
        double pRainbow = rainbowRate / 100.0;

        // レギュラーの確率(残り)
        double pSalt = Math.Max(0, 1.0 - (pCaramel + pChoco + pRainbow));

        // 平均倍率
        double avgMultiplier = (pRainbow * 5.0) + (pChoco * 3.0) + (pCaramel * 2.0) + (pSalt * 1.0);

        // 1タップの期待値 ＝ 通常スコア × 生産量 × 平均倍率
        return score * times * avgMultiplier;
    }

    /// <summary>
    /// 自動マシンが全体で「1秒あたりに稼ぐスコア（CPS）」の計算関数
    /// </summary>
    double GetAutoMachineCPS()
    {
        double totalPopcornPerSecond = 0;

        // 全マシンの1秒あたりポップコーン生成数を合計
        for (int i = 0; i < autoMakerSettings.Length; i++)
        {
            var setting = autoMakerSettings[i];

            // 0除算エラーと機数0のガード
            if (setting.makerRecastTime > 0 && setting.makerCount > 0)
            {
                // 1機あたりの毎秒生成数 ＝ 1回の生成数 ÷ リキャスト時間
                double popcornPerSecPerMachine = (double)setting.makerTimes / setting.makerRecastTime;

                // 全機数の合計毎秒生成数を足し合わせる
                totalPopcornPerSecond += popcornPerSecPerMachine * setting.makerCount;
            }
        }

        // 各ポップコーンの平均期待値スコア（1個あたりの期待値）
        double singlePopcornExpectedValue = GetClickEfficiency() / times;

        // 全マシンの毎秒スコア ＝ 毎秒の合計ポップコーン数 × 1個の期待値スコア
        return totalPopcornPerSecond * singlePopcornExpectedValue;
    }
    
    /// <summary>
    /// 連打速度の計測関数
    /// </summary>
    void CPS()
    {
        float currentTime = Time.time;

        // 1. 今の時刻から「1秒以上前」の古いタイムスタンプを捨てる
        while (clickTimestamps.Count > 0 && clickTimestamps.Peek() < currentTime - 1.0f)
        {
            clickTimestamps.Dequeue();
        }

        // 2. 残っているタイムスタンプの数が「現在1秒間の連打数（CPS）」
        currentCPS = clickTimestamps.Count;

        // 3. 最高記録（ハイスコア）の更新チェック
        if (currentCPS > maxCPS)
        {
            maxCPS = currentCPS;
        }
    }

    /// <summary>
    /// 連打測定関数
    /// </summary>
    void Repeat()
    {
        // 連打継続中
        if (repeatFlag)
        {
            if (recordRepeatCount < repeatCount)    // 最大連打数を更新
            {
                recordRepeatCount = repeatCount;
                RankingManager.Instance.SendRecordRepeatCount();
            }
            if (repeatTimer > repeatTime)           // 連打を解除
            {
                // 初期化
                if (!AudioManager.Instance.bgmSources[0].isPlaying) AudioManager.Instance.PlayBGM(0);
                repeatTimer = 0;
                repeatCount = 0;
                repeatBonus = 1;
                instructionText.text = $"クリックしろ !!";
                instructionText.color = Color.white;
                baseLight.intensity = 1f;
                spotLight.gameObject.SetActive(false);
                spotLight.pointLightOuterAngle = 45f;
                spotLight.pointLightOuterRadius = 10f; 
                popcornForceMultiple = 1f;
                repeatFlag = false;
                feverFlag = false;
            }
            else repeatTimer += Time.deltaTime * pauseTime;
        }

        // 100回未満の連打中
        if (repeatCount < 100)
        {
            repeatBonus = 1f + 0.2f * (repeatCount / 10);
            nextBonusCount = 10 - repeatCount % 10;
            nextBonus = 0.2f;
        }
        // 1000回未満の連打中
        else if (repeatCount < 1000)
        {
            if (!feverFlag) feverFlag = true;
            if (!AudioManager.Instance.bgmSources[1].isPlaying)
            {
                AudioManager.Instance.PlayBGM(1);
                AudioManager.Instance.bgmSources[1].pitch = 0.7f;
            }
            RainbowColorText(instructionText, 0.5f, 0.25f);
            instructionText.text = $"!! FEVER !!";
            RainbowColorText(repeatTexts[2], 0.5f, 0.25f);
            baseLight.intensity = 0.2f;
            spotLight.gameObject.SetActive(true);
            spotLight.pointLightOuterAngle = Mathf.Max(times * 2.4f, 45f);
            spotLight.pointLightOuterRadius = Mathf.Max(times * 0.3f, 10f); 
            popcornForceMultiple = 1 + (Mathf.FloorToInt((repeatCount - 100) / 180f)) * 0.2f;

            repeatBonus = 3f + 0.8f * ((repeatCount - 10) / 90);
            nextBonusCount = 90 - (repeatCount - 10) % 90;
            nextBonus = 0.8f;
        }
        // 1000回以上の連打中
        else
        {            
            // ボーナス倍率ごとにテキストを変更
            if (repeatBonus >= 100)
            {
                instructionText.text = "!! ULTIMATE FEVER !!";
                AudioManager.Instance.bgmSources[1].pitch = 1.2f;
            }
            else if (repeatBonus >= 90)
            {
                instructionText.text = "!! OVERLOAD FEVER !!";
                AudioManager.Instance.bgmSources[1].pitch = 1.15f;
            }
            else if (repeatBonus >= 80)
            {
                instructionText.text = "!! INFINITY FEVER !!";
                AudioManager.Instance.bgmSources[1].pitch = 1.1f;
            }
            else if (repeatBonus >= 70)
            {
                instructionText.text = "!! COSMIC FEVER !!";
                AudioManager.Instance.bgmSources[1].pitch = 1.05f;
            }
            else if (repeatBonus >= 60)
            {
                instructionText.text = "!! EXTREME FEVER !!";
                AudioManager.Instance.bgmSources[1].pitch = 1f;
            }
            else if (repeatBonus >= 50)
            {
                instructionText.text = "!! ULTRA FEVER !!";
                AudioManager.Instance.bgmSources[1].pitch = 0.95f;
            }
            else if (repeatBonus >= 40)
            {
                instructionText.text = "!! GIGA FEVER !!";
                AudioManager.Instance.bgmSources[1].pitch = 0.9f;
            }
            else if (repeatBonus >= 30)
            {
                instructionText.text = "!! MEGA FEVER !!";
                AudioManager.Instance.bgmSources[1].pitch = 0.85f;
            }
            else if (repeatBonus >= 20)
            {
                instructionText.text = "!! HYPER FEVER !!";
                AudioManager.Instance.bgmSources[1].pitch = 0.8f;
            }
            else if (repeatBonus >= 11)
            {
                instructionText.text = "!! SUPER FEVER !!";
                AudioManager.Instance.bgmSources[1].pitch = 0.75f;
            }
            RainbowColorText(instructionText, 1f, 0.1f); 
            RainbowColorText(repeatTexts[2], 1f, 0.1f);
            spotLight.pointLightOuterAngle = Mathf.Max(times * 2.4f, 45f);
            spotLight.pointLightOuterRadius = Mathf.Max(times * 0.3f, 10f); 
            popcornForceMultiple = 2f;
            repeatBonus = 11 + Mathf.FloorToInt((repeatCount - 1000) / 100) * 0.5f;
            nextBonusCount = 100 - (repeatCount - 1000) % 100;
            nextBonus = 0.5f;
        }
    }

    public void SetSkill(SkillType skillType, int level, float effectTime, string infoText)
    {
        activeSkillType = skillType;
        activeEffectTime = effectTime;
        activeInfoText = infoText;

        switch (activeSkillType)
        {
            case SkillType.BonusUp:
                bonusUpSkill = (level == 1) ? 2 : (level == 2) ? 3 : 5;
                break;
            case SkillType.Critical:
                criticalSkill = (level == 1) ? 10 : (level == 2) ? 25 : 50;
                break;
            case SkillType.Fever:
                feverSkillFlag = true;
                break;
            case SkillType.FixedPopcorn:
                fixedPopcornSkill = (level == 1) ? 2 : (level == 2) ? 3 : 4;
                break;
            case SkillType.MakerOffshoot:
                break;
            case SkillType.MakerSpeedUp:
                makerSpeedUpSkill = (level == 1) ? 1.1f : (level == 2) ? 1.25f : 1.5f;
                break;
            case SkillType.MakerTimesUp:
                makerTimesUpSkill = (level == 1) ? 2 : (level == 2) ? 3 : 5;
                break;
            case SkillType.TimesUp:
                timesUpSkill = (level == 1) ? 2 : (level == 2) ? 3 : 5;
                break;
        }

        skillUI.SetActive(true);
        string name = $"";
        switch (activeSkillType)
        {
            case SkillType.None:
                name = $"";
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
                name = $"マシン生産速度アップ";
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
        skillTexts[1].text = name;
        skillTexts[2].text = $"残り {effectTime.ToString("F1")}秒";
        skillTexts[3].text = infoText;

        activeFlag = true;
    }

    void Skill()
    {
        if (!activeFlag) return;
        
        if (activeEffectTimer > activeEffectTime)
        {
            SkillListManager.Instance.RecastSkill(activeSkillType);
            switch (activeSkillType)
            {
                case SkillType.BonusUp:
                    bonusUpSkill = 1;
                    break;
                case SkillType.Critical:
                    criticalSkill = 0;
                    break;
                case SkillType.Fever:
                    pAmount += totalFeverPAmount;
                    totalFeverPAmount = 0;
                    feverSkillFlag = false;
                    break;
                case SkillType.FixedPopcorn:
                    fixedPopcornSkill = 1;
                    break;
                case SkillType.MakerOffshoot:
                    makerOffshootSkill = 1;
                    break;
                case SkillType.MakerSpeedUp:
                    makerSpeedUpSkill = 1f;
                    break;
                case SkillType.MakerTimesUp:
                    makerTimesUpSkill = 1;
                    break;
                case SkillType.TimesUp:
                    timesUpSkill = 1;
                    break;
            }

            activeEffectTimer = 0;
            activeFlag = false;
            skillUI.SetActive(false);
        }
        else
        {
            activeEffectTimer += Time.deltaTime * pauseTime;
        }
    }

    /// <summary>
    /// クリック関数
    /// </summary>
    public void OnClick()
    {
        // 最初のクリック
        if (!goFlag)
        {
            AudioManager.Instance.PlayBGM(0);
            startFlag = true;
        }
        // 生産量に合わせて実行
        for (int i = 0; i < times * timesUpSkill; i++)
        {
            int kind = Lottery();                                                                       // ランダムでポップコーンの種類を決定
            Transform spawnPoint = mainMaker.transform.GetChild(0);                                     // ポップコーンの出現位置を指定
            GameObject p = PopcornPool.Instance.GetPopcorn(spawnPoint.position, spawnPoint.rotation);   // ポップコーンプールからポップコーンを生産
            p.GetComponent<Popcorn>().InitImage(popcornSprites[kind - 1], kind);                        // ポップコーンのイラストを設定
            
        }

        // 連打用
        repeatCount++;
        if (!repeatFlag) repeatFlag = true;
        else repeatTimer = 0;
        // 連打速度用
        clickTimestamps.Enqueue(Time.time);
        clickCount++;
    }
    
    /// <summary>
    /// 自動クリック関数
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="spawner"></param>
    public void AutoClick(int kind, GameObject spawner)
    {
        if (kind == 0) kind = Lottery();                                                            // ランダムでポップコーンの種類を決定
        Transform spawnPoint = spawner.transform;                                                   // ポップコーンの出現位置を指定
        GameObject p = PopcornPool.Instance.GetPopcorn(spawnPoint.position, spawnPoint.rotation);   // ポップコーンプールからポップコーンを生産
        p.GetComponent<Popcorn>().InitImage(popcornSprites[kind - 1], kind);                        // ポップコーンのイラストを設定                                              
    }

    /// <summary>
    /// ポップコーン抽選関数
    /// </summary>
    /// <returns></returns>
    int Lottery()
    {
        totalPopcornCount++;

        switch (fixedPopcornSkill)
        {
            case 2:
                pAmount += (int)(score * repeatBonus * 2 * bonusUpSkill);
                currentUpScore = (int)(score * repeatBonus * 2 * bonusUpSkill);
                totalPAmount += currentUpScore;
                if (feverSkillFlag) totalFeverPAmount += currentUpScore;
                caramelCount++;
                return fixedPopcornSkill;
            case 3:
                pAmount += (int)(score * repeatBonus * 3 * bonusUpSkill);
                currentUpScore = (int)(score * repeatBonus * 3 * bonusUpSkill);
                totalPAmount += currentUpScore;
                if (feverSkillFlag) totalFeverPAmount += currentUpScore;
                chocolateCount++;
                return fixedPopcornSkill;
            case 4:
                pAmount += (int)(score * repeatBonus * 5 * bonusUpSkill);
                currentUpScore = (int)(score * repeatBonus * 5 * bonusUpSkill);
                totalPAmount += currentUpScore;
                if (feverSkillFlag) totalFeverPAmount += currentUpScore;
                rainbowCount++;
                return fixedPopcornSkill;
        }

        int criticalBonus = 1;
        if (criticalSkill == 0) criticalBonus = 1;
        else if (UnityEngine.Random.Range(0, 100) < criticalSkill) criticalBonus = 5;

        if (UnityEngine.Random.Range(0, 100) < rainbowRate)
        {
            pAmount += (int)(score * repeatBonus * 5 * bonusUpSkill * criticalBonus);
            currentUpScore = (int)(score * repeatBonus * 5 * bonusUpSkill * criticalBonus);
            totalPAmount += currentUpScore;
            if (feverSkillFlag) totalFeverPAmount += currentUpScore;
            rainbowCount++;
            return 4;
        }

        if (UnityEngine.Random.Range(0, 100) < chocolateRate)
        {
            pAmount += (int)(score * repeatBonus * 3 * bonusUpSkill * criticalBonus);
            currentUpScore = (int)(score * repeatBonus * 3 * bonusUpSkill * criticalBonus);
            totalPAmount += currentUpScore;
            if (feverSkillFlag) totalFeverPAmount += currentUpScore;
            chocolateCount++;
            return 3;
        }

        if (UnityEngine.Random.Range(0, 100) < caramelRate)
        {
            pAmount += (int)(score * repeatBonus * 2 * bonusUpSkill * criticalBonus);
            currentUpScore = (int)(score * repeatBonus * 2 * bonusUpSkill * criticalBonus);
            totalPAmount += currentUpScore;
            if (feverSkillFlag) totalFeverPAmount += currentUpScore;
            caramelCount++;
            return 2;
        }

        if (recordPAmount <= pAmount)
        {
            recordPAmount = pAmount;
            RankingManager.Instance.SendRecordPAmount();
        }

        pAmount += (int)(score * repeatBonus * bonusUpSkill * criticalBonus);
        currentUpScore = (int)(score * repeatBonus * criticalBonus);
        totalPAmount += currentUpScore;
        if (feverSkillFlag) totalFeverPAmount += currentUpScore;
        regularCount++;
        return 1;
    }

    public void Unlock(UnlockButton unlock)
    {
        bottomBannerButtons.SetActive(true);
        switch (unlock)
        {
            case UnlockButton.Skill:
                bottomBannerButtons.transform.GetChild(0).gameObject.SetActive(true);
                bottomBannerButtons.transform.GetChild(1).gameObject.SetActive(true);
                break;
            case UnlockButton.Room:
                bottomBannerButtons.transform.GetChild(2).gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 自動マシンの生成関数
    /// </summary>
    /// <param name="i"></param>
    public void AddAutoMaker(int i)
    {
        Vector2 spawnPos = GetRandomPositionInArea();                                           // 指定した範囲に生成
        GameObject a = Instantiate(autoPopcornMakerPrefab, spawnPos, Quaternion.identity);      // 自動マシンの生成
        a.transform.parent = autoPopcornMakerGroup.transform;                                   // 親子化設定
        autoMakers.Add(a.transform.GetComponent<AutoPopcornMaker>());                           // 自動マシンのリストに登録
        // リスト登録した所から情報を設定
        autoMakers[autoMakers.Count - 1].kind = (Kind)i;
        autoMakers[autoMakers.Count - 1].recastTime = autoMakerSettings[i].makerRecastTime;
        autoMakers[autoMakers.Count - 1].times = autoMakerSettings[i].makerTimes;
        autoMakers[autoMakers.Count - 1].SetSprite(makerSprites[i]);
    }

    /// <summary>
    /// 自動マシンの設定を更新する関数
    /// </summary>
    /// <param name="i"></param>
    public void UpdateAutoMakerSetting(int i)
    {
        for (int a = 0; a < autoMakers.Count; a++)
        {
            if (autoMakers[a].kind == (Kind)i)
            {
                autoMakers[a].recastTime = autoMakerSettings[i].makerRecastTime;
                autoMakers[a].times = autoMakerSettings[i].makerTimes;
            }
        }
    }

    /// <summary>
    /// 自動マシンの生成位置をランダム指定関数
    /// </summary>
    /// <returns></returns>
    public Vector2 GetRandomPositionInArea()
    {
        // エラーチェック（2つのポイントが設定されているか確認）
        if (points == null || points.Length < 2 || points[0] == null || points[1] == null)
        {
            return Vector2.zero;
        }

        Vector2 pointA = points[0].transform.position;
        Vector2 pointB = points[1].transform.position;

        // 2点からX座標とY座標の最小値（min）・最大値（max）を求める
        float minX = Mathf.Min(pointA.x, pointB.x);
        float maxX = Mathf.Max(pointA.x, pointB.x);
        float minY = Mathf.Min(pointA.y, pointB.y);
        float maxY = Mathf.Max(pointA.y, pointB.y);

        // 四角い範囲内のランダムなX, Y座標を作成
        float randomX = UnityEngine.Random.Range(minX, maxX);
        float randomY = UnityEngine.Random.Range(minY, maxY);

        return new Vector2(randomX, randomY);
    }

    /// <summary>
    /// ゲームリセット
    /// </summary>
    public void ResetButton()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// テキストを虹色に変更する関数
    /// </summary>
    /// <param name="textComponent"></param>
    /// <param name="speed"></param>
    /// <param name="waveWidth"></param>
    public void RainbowColorText(TextMeshProUGUI textComponent, float speed, float waveWidth)
    {
        // メッシュ情報を最新にする
        textComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = textComponent.textInfo;

        int characterCount = textInfo.characterCount;
        if (characterCount == 0) return;

        // 全頂点のカラー配列を取得
        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            // スペースなどの非表示文字はスキップ
            if (!charInfo.isVisible) continue;

            // 文字を構成する4つの頂点のインデックス
            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;
            Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // 時間と文字の位置に応じてHSVカラー（虹色）を計算
            // H (Hue/色相) を 0.0〜1.0 で変化させる
            float hue = (Time.time * speed + i * waveWidth) % 1.0f;
            Color32 rainbowColor = Color.HSVToRGB(hue, 1.0f, 1.0f); // S=1.0(鮮やか), V=1.0(明度Max)

            // 文字の4隅の頂点カラーを虹色に書き換え
            newVertexColors[vertexIndex + 0] = rainbowColor;
            newVertexColors[vertexIndex + 1] = rainbowColor;
            newVertexColors[vertexIndex + 2] = rainbowColor;
            newVertexColors[vertexIndex + 3] = rainbowColor;
        }

        // 変更したカラー情報をメッシュに反映
        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}
