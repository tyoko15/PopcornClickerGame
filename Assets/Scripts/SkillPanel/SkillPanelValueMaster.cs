using UnityEngine;

[CreateAssetMenu(fileName = "SkillCostMaster", menuName = "Scriptable Objects/SkillPanelCostMaster")]
public class SkillPanelValueMaster : ScriptableObject
{
    [Header("レベル別コスト設定")]
    public int costLv1 = 1000000;
    public int costLv2 = 50000000;
    public int costLv3 = 1000000000;
    public int costReinforcementLv1 = 1500000;
    public int costReinforcementLv2 = 75000000;
    public int costReinforcementLv3 = 1500000000;
    [Header("効果時間とリキャストタイムの初期値")]
    public float effectTime = 5f;
    public float recastTime = 60f;
    [Header("レベル別効果時間設定")]
    public float effectTimeLv1 = 0.5f;
    public float effectTimeLv2 = 1f;
    public float effectTimeLv3 = 3.5f;
    [Header("レベル別リキャストタイム設定")]
    public float recastTimeLv1 = 2.5f;
    public float recastTimeLv2 = 4f;
    public float recastTimeLv3 = 8.5f;
}
