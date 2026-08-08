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
    public long pAmount;         // 合計ポップコーン数
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
    [SerializeField] GameObject repeatUI;
    TextMeshProUGUI[] repeatTexts;
    TextMeshProUGUI[] uiTexts = new TextMeshProUGUI[16];
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
    // Popcorn
    [Header("ポップコーンのイラスト設定")]
    [SerializeField] public Sprite[] popcornSprites;
    [SerializeField] GameObject popcornPrefab;
    [Header("メーカーのイラスト設定")]
    [SerializeField] public Sprite[] makerSprites;

    // 連打
    bool repeatFlag;
    int repeatCount;
    int recordRepeatCount;
    float repeatTime = 1f;
    float repeatTimer;
    float repeatBonus = 1f;
    int nextBonusCount;
    float nextBonus;

    // ゲーム開始の演出
    [SerializeField] Light2D baseLight;
    [SerializeField] Light2D spotLight;
    bool startFlag;
    bool goFlag;
    float startTime = 0.8f;
    float startTimer;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {        
        uiTexts = new TextMeshProUGUI[16];
        for (int i = 0; i < uiTexts.Length; i++) uiTexts[i] = UI.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(i + 1).GetComponent<TextMeshProUGUI>();
        repeatTexts = new TextMeshProUGUI[repeatUI.transform.childCount];
        for (int i = 0; i < repeatUI.transform.childCount; i++) repeatTexts[i] = repeatUI.transform.GetChild(i).GetComponent<TextMeshProUGUI>();

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

        UpdateUI();
        Repeat();
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
        uiTexts[0].text = $"{score}p";
        uiTexts[1].text = $"{times}回";
        uiTexts[2].text = $"{caramelRate}%";
        uiTexts[3].text = $"{chocolateRate}%";
        uiTexts[4].text = $"{rainbowRate}%";
        uiTexts[5].text = $"すべて外れると出現";
        for (int i = 0; i < autoMakerSettings.Length; i++)
        {
            uiTexts[i * 2 + 6].text = $"{autoMakerSettings[i].makerCount}機";
            uiTexts[i * 2 + 7].text = $"{autoMakerSettings[i].makerRecastTime}秒に{autoMakerSettings[i].makerTimes}回";
        }
        pAmountText.text = $"{pAmount.ToString("N0")}p";

        repeatTexts[1].text = $"{repeatCount.ToString("N0")} COMBO!";
        repeatTexts[2].text = $"BONUS {repeatBonus.ToString("F1")}x";
        repeatTexts[3].text = $"次のアップまで\n残り{nextBonusCount}回(+{nextBonus.ToString("F1")}x)";
        repeatTexts[5].text = $"{recordRepeatCount.ToString("N0")}回";

        recordTexts[0].text = $"{regularCount}個";
        recordTexts[1].text = $"{caramelCount}個";
        recordTexts[2].text = $"{chocolateCount}個";
        recordTexts[3].text = $"{rainbowCount}個";
        recordTexts[4].text = $"{clickCount}回";
    }

    void Repeat()
    {
        if (repeatFlag)
        {
            if (recordRepeatCount < repeatCount) recordRepeatCount = repeatCount;
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
            if (!AudioManager.Instance.bgmSources[1].isPlaying) AudioManager.Instance.PlayBGM(1);
            RainbowColorText(instructionText, 0.5f, 0.25f);
            instructionText.text = $"!! FEVER !!";
            RainbowColorText(repeatTexts[2], 0.5f, 0.25f);
            baseLight.intensity = 0.2f;
            spotLight.gameObject.SetActive(true);

            repeatBonus = 3f + 0.8f * ((repeatCount - 100) / 90);
            nextBonusCount = 90 - (repeatCount - 100) % 90;
            nextBonus = 0.8f;
        }
        else
        {
            if (!AudioManager.Instance.bgmSources[2].isPlaying) AudioManager.Instance.PlayBGM(2);
            RainbowColorText(instructionText, 1f, 0.1f);
            instructionText.text = $"!! SUPER FEVER !!";
            RainbowColorText(repeatTexts[2], 1f, 0.1f);
            repeatBonus = 11;
            nextBonusCount = 0;
            nextBonus = 0f;
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
        if (UnityEngine.Random.Range(0, 100) < rainbowRate)
        {
            pAmount += (int)(score * repeatBonus * 5);
            currentUpScore = (int)(score * repeatBonus * 5);
            rainbowCount++;
            return 4;
        }

        if (UnityEngine.Random.Range(0, 100) < chocolateRate)
        {
            pAmount += (int)(score * repeatBonus * 3);
            currentUpScore = (int)(score * repeatBonus * 3);
            chocolateCount++;
            return 3;
        }

        if (UnityEngine.Random.Range(0, 100) < caramelRate)
        {
            pAmount += (int)(score * repeatBonus * 2);
            currentUpScore = (int)(score * repeatBonus * 2);
            caramelCount++;
            return 2;
        }

        pAmount += (int)(score * repeatBonus);
        currentUpScore = (int)(score * repeatBonus);
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
