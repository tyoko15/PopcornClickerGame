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

    public void UpdatePanelLineState(SkillPanel panel)
    {
        int number = 0;
        for (int i = 0; i < skillPanels.Length; i++) if (panel == skillPanels[i]) number = i;
        // ŽŸ‚Ìƒpƒlƒ‹‚Ì‰ðœ‚·‚é
        switch (number) 
        {
            case 0:
                if (skillPanels[1].state == PanelState.Lock) skillPanels[1].state = PanelState.UnLock;
                if (skillPanels[3].state == PanelState.Lock) skillPanels[3].state = PanelState.UnLock;
                break;
            case 1:
                if (skillPanels[2].state == PanelState.Lock) skillPanels[2].state = PanelState.UnLock;
                break;
            case 2:
            case 4:
                if (skillPanels[5].state == PanelState.Lock) skillPanels[5].state = PanelState.UnLock;
                break;
            case 3:
                if (skillPanels[4].state == PanelState.Lock) skillPanels[4].state = PanelState.UnLock;
                break;
            case 5:
                if (skillPanels[6].state == PanelState.Lock) skillPanels[6].state = PanelState.UnLock;
                if (skillPanels[8].state == PanelState.Lock) skillPanels[8].state = PanelState.UnLock;
                break;
            case 6:
                if (skillPanels[7].state == PanelState.Lock) skillPanels[7].state = PanelState.UnLock;
                break;
            case 7:
            case 9:
                if (skillPanels[10].state == PanelState.Lock) skillPanels[10].state = PanelState.UnLock;
                break;
            case 8:
                if (skillPanels[9].state == PanelState.Lock) skillPanels[9].state = PanelState.UnLock;
                break;
            case 10:
                if (skillPanels[11].state == PanelState.Lock) skillPanels[11].state = PanelState.UnLock;
                if (skillPanels[13].state == PanelState.Lock) skillPanels[13].state = PanelState.UnLock;                
                break;
            case 11:
                if (skillPanels[12].state == PanelState.Lock) skillPanels[12].state = PanelState.UnLock;
                break;
            case 12:
                break;
            case 13:
                if (skillPanels[14].state == PanelState.Lock) skillPanels[14].state = PanelState.UnLock;
                break;
            case 14:
                break;
        }

        for (int i = 0; i < skillPanels.Length; i++) skillPanels[i].Lock();
        SetLineColor();
    }
}
