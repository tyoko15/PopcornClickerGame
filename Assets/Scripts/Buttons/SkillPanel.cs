using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    public SkillInfo skillInfo;
    void Start()
    {
        InitVariable();
    }

    void Update()
    {
        
    }

    void InitVariable()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = SkillTreeManager.Instance.skillPanelImages[(int)skillInfo.panelType];
    }

    public void ClickPanel(int i)
    {
        transform.GetChild(0).GetComponent<Image>().color = SkillTreeManager.Instance.selectColor;
        SkillTreeManager.Instance.SetInfo(skillInfo, transform.GetComponent<RectTransform>().anchoredPosition);
    }
}
