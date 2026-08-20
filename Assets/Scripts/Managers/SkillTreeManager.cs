using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    bool selectFlag;
    [SerializeField] float selectTime;
    float selectTimer;
    [SerializeField] RectTransform viewport;
    [SerializeField] RectTransform content;
    Vector2 centerRect;
    Vector2 targetRect;
    Vector2 viewportRect; // ScrollView‚ÌViewport
    Vector2 contentRect;  // ScrollView‚ÌContent
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
        content.anchoredPosition = new Vector2(-(contentRect.x / 2) + (viewportRect.x / 2),(contentRect.y / 2) - (viewportRect.y / 2));
    }

    void Update()
    {
        if (selectFlag) PickUpSkillPanel();
    }

    public void SetInfo(SkillInfo info, Vector2 target)
    {
        infoTexts[0].text = info.skillName;
        infoTexts[1].text = $"{ScoreFormatter.FormatToJapanese(info.cost)}p";
        infoTexts[2].text = info.infoText;
        targetRect = target;
        currentContentRent = content.anchoredPosition;
        centerRect = new Vector2(-(contentRect.x / 2) + (viewportRect.x / 2), (contentRect.y / 2) - (viewportRect.y / 2));
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
}
