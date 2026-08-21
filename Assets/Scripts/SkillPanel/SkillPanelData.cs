using UnityEngine;

[CreateAssetMenu(fileName = "SkillPanelData", menuName = "Scriptable Objects/SkillPanelData")]
public class SkillPanelData : ScriptableObject
{
    public SkillInfo skillInfo;
    public bool lockFlag = true;
    public float effectTime;
    public float recastTime;
}
