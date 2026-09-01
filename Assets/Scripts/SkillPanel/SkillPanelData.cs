using UnityEngine;

[CreateAssetMenu(fileName = "SkillPanelData", menuName = "Scriptable Objects/SkillPanelData")]
public class SkillPanelData : ScriptableObject
{
    public SkillType skillType;                 // スキルの種類
    public PanelType panelType;                 // パネルの種類
    public int effectValue;                     // 効果の強さ
    public Reinforcement reinforcementType;     // 強化の種類
    [Range(1, 3)] public int level = 1;         // 強化のレベル
    public double cost;                            // 獲得費用 
    [TextArea(3, 10)] public string infoText;                     // 情報文
    public float effectTime;                    // 効果時間
    public float recastTime;                    // リキャストタイム
}

