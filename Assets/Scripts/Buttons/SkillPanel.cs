using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    public SkillInfo skillInfo;
    TextMeshProUGUI nameText;
    [SerializeField] Color selectColor;
    void Start()
    {
        nameText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        InitVariable();
    }

    void Update()
    {
        
    }

    void InitVariable()
    {
        nameText.text = skillInfo.skillName;
    }

    public void ClickPanel(int i)
    {
        transform.GetComponent<Image>().color = selectColor;
        SkillTreeManager.Instance.SetInfo(skillInfo);
    }
}
