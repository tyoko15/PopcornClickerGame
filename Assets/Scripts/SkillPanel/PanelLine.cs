using UnityEngine;

public class PanelLine : MonoBehaviour
{
    public SkillPanelData[] skillPanelData;
    public UILine[] lines;
    public SkillPanel[] skillPanels;

    void Awake()
    {
        skillPanels = new SkillPanel[transform.GetChild(1).childCount];
        for (int i = 0; i < skillPanels.Length; i++) skillPanels[i] = transform.GetChild(1).GetChild(i).GetComponent<SkillPanel>();
        lines = new UILine[transform.GetChild(0).childCount];
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = transform.GetChild(0).GetChild(i).GetComponent<UILine>();
        }
        // Skill Lv.1
        skillPanels[0].data = skillPanelData[0];
        skillPanels[1].data = skillPanelData[3];
        skillPanels[2].data = skillPanelData[3];
        skillPanels[3].data = skillPanelData[6];
        skillPanels[4].data = skillPanelData[6];
        // Skill Lv.2
        skillPanels[5].data = skillPanelData[1];
        skillPanels[6].data = skillPanelData[4];
        skillPanels[7].data = skillPanelData[4];
        skillPanels[8].data = skillPanelData[7];
        skillPanels[9].data = skillPanelData[7];
        // Skill Lv.3
        skillPanels[10].data = skillPanelData[2];
        skillPanels[11].data = skillPanelData[5];
        skillPanels[12].data = skillPanelData[5];
        skillPanels[13].data = skillPanelData[8];
        skillPanels[14].data = skillPanelData[8];
    }

    void Start()
    {
        for (int i = 0; i < skillPanels.Length; i++) skillPanels[i].InitVariable();
        for (int i = 0; i < lines.Length; i++) lines[i].SetLineColor();
    }

    void Update()
    {
        
    }

    public void SetLineColor()
    {
        for (int i = 0; i < lines.Length; i++) lines[i].SetLineColor();
    }
}
