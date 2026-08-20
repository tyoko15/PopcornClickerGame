using TMPro;
using UnityEngine;

[System.Serializable]
public class SkillInfo
{
    public string skillName;
    public int cost;
    public string infoText;
}

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;
    [SerializeField] GameObject layer;
    TextMeshProUGUI[] infoTexts;
    private void Awake()
    {
        Instance = this;
        infoTexts = new TextMeshProUGUI[3];
        for (int i = 0; i < infoTexts.Length; i++) infoTexts[i] = layer.transform.GetChild(1).GetChild(i).GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetInfo(SkillInfo info)
    {
        infoTexts[0].text = info.skillName;
        infoTexts[1].text = $"{ScoreFormatter.FormatToJapanese(info.cost)}p";
        infoTexts[2].text = info.infoText;
    }
}
