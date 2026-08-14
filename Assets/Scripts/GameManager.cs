using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public enum Kind
{
    Auto,
    Regular,
    Caramel,
    Chocolate,
    Rainbow,
}

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
    public double totalPAmount;      // 合計ポップコーン数
    public double pAmount;      // 合計ポップコーン数
    public double recordPAmount;// 合計ポップコーン数
    public int score;           // 通常スコア
    public int times;           // 生産量
    [Header("抽選倍率設定")]
    public int caramelRate;     // キャラメルの確率
    public int chocolateRate;   // チョコレートの確率
    public int rainbowRate;     // レインボーの確率

    [Header("各マシンの設定")]
    public MakerSetting[] autoMakerSettings;

    // Counter
    int clickCount;
    int regularCount;
    int caramelCount;
    int chocolateCount;
    int rainbowCount;
    int totalPopcornCount;

    // プレイ時間
    int hour;
    int minute;
    float second;
    bool timerFlag;

    int currentUpScore;
    [Header("各メーカー設定")]
    [SerializeField] MainPopcornMaker mainMaker;
    [SerializeField] GameObject autoPopcornMakerPrefab;
    [SerializeField] GameObject autoPopcornMakes;
    List<AutoPopcornMaker> autoMakers = new List<AutoPopcornMaker>(0);
    [SerializeField] GameObject[] points;

    // UI
    [Header("UI設定")]
    [SerializeField] GameObject UI;
    [SerializeField] TextMeshProUGUI pAmountText;
    [SerializeField] TextMeshProUGUI instructionText;
    [SerializeField] TextMeshProUGUI versionionText;
    [SerializeField] GameObject repeatUI;
    TextMeshProUGUI[] repeatTexts;
    TextMeshProUGUI[] uiTexts = new TextMeshProUGUI[25];
    TextMeshProUGUI[] recordTexts = new TextMeshProUGUI[5];
    [SerializeField] GameObject[] buttons;
    [HideInInspector] public int multiple = 1;
    public event Action<int> UpdateMultiple;
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
    int totalLimitLevel = 2275;
    public int currentTotalLimitLevel;

    // Popcorn
    [Header("ポップコーンのイラスト設定")]
    [SerializeField] public Sprite[] popcornSprites;
    [SerializeField] GameObject popcornPrefab;
    public float popcornForceMultiple = 1f;
    [Header("メーカーのイラスト設定")]
    [SerializeField] public Sprite[] makerSprites;

    // 連打
    
    bool repeatFlag;
    int repeatCount;
    public int recordRepeatCount;
    float repeatTime = 1f;
    float repeatTimer;
    float repeatBonus = 1f;
    int nextBonusCount;
    float nextBonus;

    // 連打速度
    int currentCPS;
    int maxCPS;
    Queue<float> clickTimestamps = new Queue<float>();

    // ゲーム開始の演出
    [SerializeField] Light2D baseLight;
    [SerializeField] Light2D spotLight;
    bool startFlag;
    bool goFlag;
    float startTime = 0.8f;
    float startTimer;

    // 検証用オートクリッカー
    float autoTime = 0.01f;
    float autoTimer;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {        
        uiTexts = new TextMeshProUGUI[25];
        for (int i = 0; i < uiTexts.Length; i++) uiTexts[i] = UI.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(i + 1).GetComponent<TextMeshProUGUI>();
        repeatTexts = new TextMeshProUGUI[repeatUI.transform.childCount];
        for (int i = 0; i < repeatUI.transform.childCount; i++) repeatTexts[i] = repeatUI.transform.GetChild(i).GetComponent<TextMeshProUGUI>();

        versionionText.text = $"v{Application.version}";

        // ゲーム開始の演出
        instructionText.color = Color.white;
        baseLight.intensity = 0.2f;
        spotLight.gameObject.SetActive(false);
        for (int i = 0; i < UI.transform.childCount - 1; i++) UI.transform.GetChild(i).gameObject.SetActive(false);
        recordTexts = new TextMeshProUGUI[UI.transform.GetChild(0).GetChild(4).GetChild(0).GetChild(0).GetChild(0).childCount];
        for (int i = 0; i < UI.transform.GetChild(0).GetChild(4).GetChild(0).GetChild(0).GetChild(0).childCount - 1; i++) recordTexts[i] = UI.transform.GetChild(0).GetChild(4).GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetComponent<TextMeshProUGUI>();
        Camera.main.orthographicSize = 2;
        Camera.main.transform.position = new Vector3(0f, -0.5f, -5f);
    }

    void Update()
    {
        if (startFlag)
        {
            goFlag = true;
            GameStart();
            return;
        }

        Timer();

        UpdateUI();
        Repeat();
        CPS();

        //if (autoTimer > autoTime)
        //{
        //    autoTimer = 0;
        //    OnClick();
        //}
        //else autoTimer += Time.deltaTime;
    }

    void GameStart()
    {
        if (startTimer > startTime)
        {
            for (int i = 0; i < UI.transform.childCount - 1; i++) UI.transform.GetChild(i).gameObject.SetActive(true);
            instructionText.color = Color.white;
            baseLight.intensity = 1f;
            spotLight.gameObject.SetActive(false);
            startTimer = 0;
            startFlag = false;
        }
        else if (startTimer > 0.5f)
        {
            startTimer += Time.deltaTime;
        }
        else
        {
            startTimer += Time.deltaTime;
            instructionText.color = Color.black;
            Camera.main.orthographicSize = Mathf.Lerp(2f, 5f, startTimer / (startTime - 0.3f));
            Camera.main.transform.position = new Vector3(0f, Mathf.Lerp(-0.5f, 0f, startTimer / (startTime - 0.3f)), -5f);
            spotLight.gameObject.SetActive(true);
        }
    }

    void UpdateUI()
    {
        int u = 0;
        // プレイヤー情報
        uiTexts[u++].text = $"Lv.{playerLevel}";
        uiTexts[u++].text = $"{OverallReinforcementRate().ToString("F2")}%";
        uiTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(totalPAmount)}p";
        uiTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(recordPAmount)}p";
        uiTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(clickCount)}回";
        uiTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(maxCPS)} 回/秒";
        uiTexts[u++].text = $"{hour.ToString("00")}:{minute.ToString("00")}:{second.ToString("00")}";
        // 効率
        uiTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(CalculateClickExpectedValue())}p";
        uiTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(GetAutoMachineCPS())} p/s";
        // 通常スコア&生産量
        uiTexts[u++].text = $"{score}p";
        uiTexts[u++].text = $"{times}回";
        // 出現確率&倍率
        uiTexts[u++].text = $"{caramelRate}%";
        uiTexts[u++].text = $"{chocolateRate}%";
        uiTexts[u++].text = $"{rainbowRate}%";
        uiTexts[u++].text = $"すべて外れると出現";
        // 自動マシン
        for (int i = 0; i < autoMakerSettings.Length; i++)
        {
            uiTexts[i * 2 + u].text = $"{autoMakerSettings[i].makerCount}機";
            uiTexts[i * 2 + u + 1].text = $"{autoMakerSettings[i].makerRecastTime}秒に{autoMakerSettings[i].makerTimes}回";
        }
        pAmountText.text = $"{ScoreFormatter.FormatToJapanese(pAmount)}p";

        u = 1;
        repeatTexts[u++].text = $"{repeatCount.ToString("N0")} COMBO!";
        repeatTexts[u++].text = $"BONUS {repeatBonus.ToString("F1")}x";
        repeatTexts[u++].text = $"次のアップまで\n残り{nextBonusCount}回(+{nextBonus.ToString("F1")}x)";
        repeatTexts[u++ + 1].text = $"{recordRepeatCount.ToString("N0")}回";

        u = 0;
        recordTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(regularCount)}個";
        recordTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(caramelCount)}個";
        recordTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(chocolateCount)}個";
        recordTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(rainbowCount)}個";
        recordTexts[u++].text = $"{ScoreFormatter.FormatToJapanese(totalPopcornCount)}個";
    }

    float OverallReinforcementRate()
    {
        if (currentTotalLimitLevel <= 0) currentTotalLimitLevel = 1;
        
        float r = (float)currentTotalLimitLevel / totalLimitLevel;
        if (currentTotalLimitLevel == 1) r = 0;
        return r * 100;
    }

    void Timer()
    {

        if (hour == 99 && minute == 59 && second > 60f) timerFlag = true;
        if (timerFlag) return;
        second += Time.deltaTime;

        if (second >= 60f)
        {
            second -= 60f;
            minute++;
        }
        if (minute == 60)
        {
            minute = 0;
            hour++;
        }

    }
    double CalculateClickExpectedValue()
    {
        // 各確率（% を 0.0~1.0 の少数に変換）
        double pRainbow = rainbowRate / 100.0;   // 1  -> 0.01
        double pChoco = chocolateRate / 100.0;   // 10 -> 0.10
        double pCaramel = caramelRate / 100.0;   // 20 -> 0.20

        // 塩の確率（100% からレアポップコーンの合計確率を引いた残り）
        double pSalt = Math.Max(0, 1.0 - (pRainbow + pChoco + pCaramel));

        // 1個あたりの平均倍率
        double averageMultiplier = (pRainbow * 5.0) + (pChoco * 3.0) + (pCaramel * 2.0) + (pSalt * 1.0);

        // 1クリックの期待値 ＝ 基本スコア × 生成個数 × 平均倍率
        double expectedValue = score * times * averageMultiplier;

        return expectedValue;
    }

    double GetClickEfficiency()
    {
        // 各確率（%表記なら 100 で割って 0.0~1.0 に変換）
        double pCaramel = caramelRate / 100.0;
        double pChoco = chocolateRate / 100.0;
        double pRainbow = rainbowRate / 100.0;

        // 塩の確率（残り）
        double pSalt = Math.Max(0, 1.0 - (pCaramel + pChoco + pRainbow));

        // 平均倍率
        double avgMultiplier = (pRainbow * 5.0) + (pChoco * 3.0) + (pCaramel * 2.0) + (pSalt * 1.0);

        // 1タップの期待値 ＝ 1個のスコア × 1回の個数 × 平均倍率
        return score * times * avgMultiplier;
    }

    /// <summary>
    /// 自動マシンが全体で「1秒あたりに稼ぐスコア（CPS）」を計算
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

    void Repeat()
    {
        if (repeatFlag)
        {
            if (recordRepeatCount < repeatCount)
            {
                recordRepeatCount = repeatCount;
                RankingManager.Instance.SendRecordRepeatCount();
            }
            if (repeatTimer > repeatTime)
            {
                if (!AudioManager.Instance.bgmSources[0].isPlaying) AudioManager.Instance.PlayBGM(0);
                repeatTimer = 0;
                repeatCount = 0;
                repeatBonus = 1;
                instructionText.text = $"クリックしろ !!";
                instructionText.color = Color.white;
                baseLight.intensity = 1f;
                spotLight.gameObject.SetActive(false);
                popcornForceMultiple = 1f;
                repeatFlag = false;
            }
            else repeatTimer += Time.deltaTime;
        }

        if (repeatCount < 100)
        {
            repeatBonus = 1f + 0.2f * (repeatCount / 10);
            nextBonusCount = 10 - repeatCount % 10;
            nextBonus = 0.2f;
        }
        else if (repeatCount < 1000)
        {
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
            popcornForceMultiple = 1 + (Mathf.FloorToInt((repeatCount - 100) / 180f)) * 0.2f;

            repeatBonus = 3f + 0.8f * ((repeatCount - 10) / 90);
            nextBonusCount = 90 - (repeatCount - 10) % 90;
            nextBonus = 0.8f;
        }
        else
        {
            //if (!AudioManager.Instance.bgmSources[2].isPlaying) AudioManager.Instance.PlayBGM(2);
            RainbowColorText(instructionText, 1f, 0.1f);
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
            RainbowColorText(repeatTexts[2], 1f, 0.1f);
            popcornForceMultiple = 2f;
            repeatBonus = 11 + Mathf.FloorToInt((repeatCount - 1000) / 100) * 0.5f;
            nextBonusCount = 100 - (repeatCount - 1000) % 100;
            nextBonus = 0.5f;
        }
    }

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

    public void OnClick()
    {
        if (!goFlag)
        {
            AudioManager.Instance.PlayBGM(0);
            startFlag = true;
        }
        for (int i = 0; i < times; i++)
        {
            int kind = Lottery();
            //GameObject p = Instantiate(popcornPrefab, mainMaker.transform.GetChild(0));
            Transform spawnPoint = mainMaker.transform.GetChild(0);
            GameObject p = PopcornPool.Instance.GetPopcorn(spawnPoint.position, spawnPoint.rotation);
            Sprite s = popcornSprites[kind - 1];
            p.GetComponent<Popcorn>().score = currentUpScore;
            p.GetComponent<SpriteRenderer>().sprite = s;
        }
        repeatCount++;
        if (!repeatFlag) repeatFlag = true;
        else repeatTimer = 0;
        clickTimestamps.Enqueue(Time.time);
        clickCount++;
    }

    public void AddAutoMaker(int i)
    {
        Vector2 spawnPos = GetRandomPositionInArea();
        GameObject a = Instantiate(autoPopcornMakerPrefab, spawnPos, Quaternion.identity);
        a.transform.parent = autoPopcornMakes.transform;
        autoMakers.Add(a.transform.GetComponent<AutoPopcornMaker>());
        autoMakers[autoMakers.Count - 1].kind = (Kind)i;
        autoMakers[autoMakers.Count - 1].recastTime = autoMakerSettings[i].makerRecastTime;
        autoMakers[autoMakers.Count - 1].times = autoMakerSettings[i].makerTimes;
        autoMakers[autoMakers.Count - 1].SetSprite(makerSprites[i]);
    }

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

    public void AutoClick(int kind, GameObject spawner)
    {
        if (kind == 0) kind = Lottery();
        //GameObject p = Instantiate(popcornPrefab, spawner.transform);
        Transform spawnPoint = spawner.transform;
        GameObject p = PopcornPool.Instance.GetPopcorn(spawnPoint.position, spawnPoint.rotation);
        Sprite s = popcornSprites[kind - 1];
        p.GetComponent<Popcorn>().score = currentUpScore;
        p.GetComponent<SpriteRenderer>().sprite = s;
    }

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
    /// 抽選
    /// </summary>
    /// <returns></returns>
    int Lottery()
    {
        totalPopcornCount++;        
        if (UnityEngine.Random.Range(0, 100) < rainbowRate)
        {
            pAmount += (int)(score * repeatBonus * 5);
            currentUpScore = (int)(score * repeatBonus * 5);
            totalPAmount += currentUpScore;
            rainbowCount++;
            return 4;
        }

        if (UnityEngine.Random.Range(0, 100) < chocolateRate)
        {
            pAmount += (int)(score * repeatBonus * 3);
            currentUpScore = (int)(score * repeatBonus * 3);
            totalPAmount += currentUpScore;
            chocolateCount++;
            return 3;
        }

        if (UnityEngine.Random.Range(0, 100) < caramelRate)
        {
            pAmount += (int)(score * repeatBonus * 2);
            currentUpScore = (int)(score * repeatBonus * 2);
            totalPAmount += currentUpScore;
            caramelCount++;
            return 2;
        }

        if (recordPAmount <= pAmount)
        {
            recordPAmount = pAmount;
            RankingManager.Instance.SendRecordPAmount();
        }

        pAmount += (int)(score * repeatBonus);
        currentUpScore = (int)(score * repeatBonus);
        totalPAmount += currentUpScore;
        regularCount++;
        return 1;
    }

    public void ResetButton()
    {
        SceneManager.LoadScene(0);
    }

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
