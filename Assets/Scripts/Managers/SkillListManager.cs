using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SkillDataInfo
{
    public SkillType skillType;
    public int level;
    public float effectTime;        // 効果時間
    public float effectTimer;
    public bool recastFlag;
    public float recastTime;        // リキャストタイム
    public float recastTimer;
    public string infoText;         // 説明文
}

public class SkillListManager : MonoBehaviour
{
    public static SkillListManager Instance;

    [SerializeField] GameObject layer;

    SkillDataInfo[] skillDatas;
    int registrationNumber;
    GameObject[] skills;
    int selectSkillNumber;

    int activeSkillNumber;

    TextMeshProUGUI[] infoTexts;
    Image activationButton;
    TextMeshProUGUI activationText;

    private void Awake()
    {
        Instance = this;
        skillDatas = new SkillDataInfo[8];
        skills = new GameObject[8];
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i] = layer.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(i).gameObject;
            skills[i].SetActive(false);
        }
        infoTexts = new TextMeshProUGUI[4];
        for (int i = 0; i < infoTexts.Length; i++) infoTexts[i] = layer.transform.GetChild(1).GetChild(i).GetComponent<TextMeshProUGUI>();
        activationButton = layer.transform.GetChild(1).GetChild(4).GetComponent<Image>();
        activationText = layer.transform.GetChild(1).GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        infoTexts[0].text = $"スキル名";
        infoTexts[1].text = $"";
        infoTexts[2].text = $"";
        infoTexts[3].text = $"";
        activationButton.color = Color.gray;
        activationText.color = Color.gray;
        activationText.text = $"未選択";
    }

    void Update()
    {
        UpdateSkillUI();
    }

    public void ClickSkill(int i)
    {
        selectSkillNumber = i;
        string name = $"";
        switch (skillDatas[i].skillType)
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
        infoTexts[0].text = name;
        infoTexts[1].text = $"{skillDatas[i].effectTime}秒";
        infoTexts[2].text = $"{skillDatas[i].recastTime}秒";
        infoTexts[3].text = skillDatas[i].infoText;
    }

    public void ClickActivationButton()
    {
        if (!skillDatas[selectSkillNumber].recastFlag && !GameManager.Instance.activeFlag)
        {
            SkillDataInfo info = skillDatas[selectSkillNumber];
            GameManager.Instance.SetSkill(info.skillType, info.level, info.effectTime, info.infoText);
            activeSkillNumber = selectSkillNumber;
        }
    }

    /// <summary>
    /// 新たにスキルを登録する
    /// </summary>
    /// <param name="data"></param>
    public void RegistrationSkill(SkillDataInfo data)
    {
        skillDatas[registrationNumber] = new SkillDataInfo(); 
        skillDatas[registrationNumber] = data;

        skills[registrationNumber].SetActive(true);
        string name = $"";
        switch (skillDatas[registrationNumber].skillType)
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
        skills[registrationNumber].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        registrationNumber++;
    }

    public void MaxSkill()
    {
        for (int i = 0; i < skillDatas.Length; i++)
        {
            skillDatas[i].effectTime = 60f;
            skillDatas[i].recastTime = 15f;
        }
    }

    /// <summary>
    /// スキルを更新
    /// </summary>
    public void UpdateSkill(SkillPanelData data)
    {
        int selectNumber = 0;
        for (int i = 0; i < skillDatas.Length; i++)
        {
            if (skillDatas[i].skillType == data.skillType)
            {
                selectNumber = i;
                break;
            }
        }

        switch (data.reinforcementType)
        {
            case Reinforcement.EffectTime:
                skillDatas[selectNumber].effectTime += data.effectTime;
                break;
            case Reinforcement.RecastTime:
                skillDatas[selectNumber].recastTime -= data.recastTime;
                break;
        }

        if (selectNumber == selectSkillNumber) ClickSkill(selectNumber);
    }

    public void UpgradeSkill(SkillPanelData data)
    {
        int selectNumber = 0;
        for (int i = 0; i < skillDatas.Length; i++)
        {
            if (skillDatas[i].skillType == data.skillType)
            {
                selectNumber = i;
                break;
            }
        }

        int level = (data.panelType == PanelType.Lv2) ? 2 : 3;
        skillDatas[selectNumber].level = level;
        skillDatas[selectNumber].infoText = data.infoText;

        if (selectNumber == selectSkillNumber) ClickSkill(selectNumber);
    }

    void UpdateSkillUI()
    {
        for (int i = 0; i < skillDatas.Length; i++)
        {
            if (skillDatas[i] == null) break;
            if (skillDatas[i].recastFlag)
            {
                if (skillDatas[i].recastTimer > skillDatas[i].recastTime)
                {                    
                    skillDatas[i].recastTimer = 0f;
                    skillDatas[i].recastFlag = false;
                    skills[i].transform.GetChild(1).GetChild(1).GetComponent<Image>().fillAmount = 1f;
                    skills[i].transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = $"-- 発動可能 --";
                    skills[i].transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.black;
                }
                else
                {
                    skillDatas[i].recastTimer += Time.deltaTime;
                    float amount = Mathf.Lerp(0f, 1f, skillDatas[i].recastTimer / skillDatas[i].recastTime);
                    skills[i].transform.GetChild(1).GetChild(1).GetComponent<Image>().fillAmount = amount;
                    skills[i].transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = $"-- 残り {(skillDatas[i].recastTime - skillDatas[i].recastTimer).ToString("F1")}秒 --";
                    skills[i].transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.white;
                }
            }
            else
            {
                if (!GameManager.Instance.activeFlag)
                {
                    skills[i].transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = $"-- 発動可能 --";
                    skills[i].transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.black;
                }
                else
                {
                    skills[i].transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = $"-- 発動不可 --";
                    skills[i].transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.white;

                    skills[activeSkillNumber].transform.GetChild(1).GetChild(1).GetComponent<Image>().fillAmount = 0f;
                    skills[activeSkillNumber].transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = $"-- 使用中 --";
                }
            }
        }

        if (GameManager.Instance.activeFlag)
        {
            activationButton.color = Color.gray;
            activationText.color = Color.gray;
            activationText.text = $"不可";
        }
        else
        {
            if (skillDatas[selectSkillNumber] != null)
            {
                if (!skillDatas[selectSkillNumber].recastFlag)
                {
                    activationButton.color = Color.white;
                    activationText.color = Color.white;
                    activationText.text = $"発動";
                }
                else
                {
                    activationButton.color = Color.gray;
                    activationText.color = Color.gray;
                    activationText.text = $"不可";
                }

            }

        }

    }

    public void RecastSkill(SkillType skillType)
    {
        for (int i = 0; i < skillDatas.Length; i++)
        {
            if (skillDatas[i] == null) break;
            if (skillDatas[i].skillType == skillType)
            {
                skillDatas[i].recastFlag = true;
            }
        }
    }
}
