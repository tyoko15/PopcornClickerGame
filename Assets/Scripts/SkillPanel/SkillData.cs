using UnityEngine;
[CreateAssetMenu(fileName = "SkillPanelData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    public SkillDataInfo info;
    public SkillType skillType;
    public float effectTime;        // 効果時間
    public float recastTime;        // リキャストタイム
}
