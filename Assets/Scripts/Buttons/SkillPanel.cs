using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    [SerializeField] SkillPanelData skillPanelData;
    void Start()
    {
        InitVariable();
    }

    void Update()
    {
        
    }

    void InitVariable()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = SkillTreeManager.Instance.skillPanelImages[(int)skillPanelData.skillInfo.skillType];
        if (skillPanelData.skillInfo.panelType == PanelType.Lv1) skillPanelData.lockFlag = false;
        else skillPanelData.lockFlag = true;
        Lock(skillPanelData.lockFlag);
    }

    public void Lock(bool flag)
    {
        skillPanelData.lockFlag = flag;
        transform.GetChild(0).GetComponent<Image>().color = (!skillPanelData.lockFlag) ? Color.white : SkillTreeManager.Instance.lockColor;
    }

    public void ClickPanel(int i)
    {
        if (skillPanelData.lockFlag) return;
        transform.GetChild(0).GetComponent<Image>().color = SkillTreeManager.Instance.selectColor;
        SkillTreeManager.Instance.SetInfo(skillPanelData.skillInfo, transform.GetComponent<RectTransform>().anchoredPosition);
    }
}
