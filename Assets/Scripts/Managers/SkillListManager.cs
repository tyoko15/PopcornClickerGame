using TMPro;
using UnityEngine;

public class SkillListManager : MonoBehaviour
{
    public static SkillListManager Instance;

    [SerializeField] GameObject layer;
    GameObject[] skills;
    TextMeshProUGUI[] infoTexts;

    private void Awake()
    {
        Instance = this;
        skills = new GameObject[8];
        for (int i = 0; i < skills.Length; i++)
        {
            //skills[i] = layer.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetComponent<GameObject>();
        }
        infoTexts = new TextMeshProUGUI[4];
        //for (int i = 0; i < infoTexts.Length; i++) layer.transform.GetChild(1).GetChild(i).GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        //infoTexts[0].text = $"スキル名";
        //infoTexts[1].text = $"";
        //infoTexts[1].text = $"";
    }

    void Update()
    {
        
    }

    public void ClickSkill(int i)
    {
        infoTexts[0].text = $"スキル名";
        infoTexts[1].text = $"";
        infoTexts[1].text = $"";
    }
}
