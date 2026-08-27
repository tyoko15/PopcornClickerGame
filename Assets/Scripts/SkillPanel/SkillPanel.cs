using UnityEngine;
using UnityEngine.UI;

public enum PanelState
{
    Lock,
    UnLock,
    Acquired,
}

public class SkillPanel : MonoBehaviour
{
    public SkillPanelData data;
    public bool lockFlag;
    public PanelState state;
    Image[] images;

    public void InitVariable()
    {
        name = data.name;
        images = new Image[transform.childCount];
        for (int i = 0; i < images.Length; i++) images[i] = transform.GetChild(i).GetComponent<Image>();
        images[0].sprite = SkillTreeManager.Instance.skillPanelImages[(int)data.panelType];
        if (data.panelType == PanelType.Lv1)
        {
            state = PanelState.UnLock;
            lockFlag = false;
        }
        else lockFlag = true;
        Lock(lockFlag);
    }

    public void Lock(bool flag)
    {
        lockFlag = flag;
        images[0].color = (!lockFlag) ? Color.white : SkillTreeManager.Instance.lockColor;
    }

    public void ClickPanel()
    {
        Vector2 my = transform.GetComponent<RectTransform>().anchoredPosition;
        SkillTreeManager.Instance.SetInfo(this, my);
    }
}
