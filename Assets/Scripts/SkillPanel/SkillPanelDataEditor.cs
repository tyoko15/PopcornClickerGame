#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillPanelData))]
public class SkillPanelDataEditor : Editor
{
    private SerializedProperty skillTypeProp;
    private SerializedProperty panelTypeProp;
    private SerializedProperty effectValueProp;
    private SerializedProperty reinforcementTypeProp;
    private SerializedProperty levelProp;
    private SerializedProperty effectTimeProp;
    private SerializedProperty recastTimeProp;
    private SerializedProperty costProp;
    private SerializedProperty infoTextProp;

    private SkillPanelValueMaster valueMaster;

    private void OnEnable()
    {
        skillTypeProp = serializedObject.FindProperty("skillType");
        panelTypeProp = serializedObject.FindProperty("panelType");
        effectValueProp = serializedObject.FindProperty("effectValue");
        reinforcementTypeProp = serializedObject.FindProperty("reinforcementType");
        levelProp = serializedObject.FindProperty("level");
        effectTimeProp = serializedObject.FindProperty("effectTime");
        recastTimeProp = serializedObject.FindProperty("recastTime");
        costProp = serializedObject.FindProperty("cost");
        infoTextProp = serializedObject.FindProperty("infoText");

        valueMaster = AssetDatabase.LoadAssetAtPath<SkillPanelValueMaster>("Assets/Scripts/SkillPanel/PanelData/SkillPanelCostMaster.asset");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SkillPanelData data = (SkillPanelData)target;

        EditorGUILayout.LabelField("基本設定", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(skillTypeProp);
        EditorGUILayout.PropertyField(panelTypeProp);

        // パネルタイプが Reinforcement の場合
        if (panelTypeProp.enumValueIndex == (int)PanelType.Reinforcement)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(levelProp);
            EditorGUILayout.PropertyField(reinforcementTypeProp);
            EditorGUI.indentLevel--;

            data.cost = (data.level == 1) ? valueMaster.costReinforcementLv1 : (data.level == 2) ? valueMaster.costReinforcementLv2 : valueMaster.costReinforcementLv3;
            EditorGUILayout.PropertyField(costProp);
            Reinforcement currentReinforcement = (Reinforcement)reinforcementTypeProp.enumValueIndex;
            EditorGUILayout.LabelField("強化される内容", EditorStyles.boldLabel);

            if (currentReinforcement == Reinforcement.EffectTime)
            {
                data.effectTime = (data.level == 1) ? valueMaster.effectTimeLv1 : (data.level == 2) ? valueMaster.effectTimeLv2 : valueMaster.effectTimeLv3;
                EditorGUILayout.PropertyField(effectTimeProp);                
                data.infoText = $"効果時間を{data.effectTime}秒延長する。";
            }
            else if (currentReinforcement == Reinforcement.RecastTime)
            {
                data.recastTime = (data.level == 1) ? valueMaster.recastTimeLv1 : (data.level == 2) ? valueMaster.recastTimeLv2 : valueMaster.recastTimeLv3;
                EditorGUILayout.PropertyField(recastTimeProp);
                data.infoText = $"再使用までの時間を\n{data.recastTime}秒短縮する。";
            }
        }
        else
        {
            EditorGUILayout.PropertyField(effectValueProp);

            // スキル別に効果説明文を表示
            string infoText = $"NULL";
            switch (data.skillType)
            {
                case SkillType.None:
                    if (data.panelType == PanelType.Special)　infoText = $"全スキルの\n効果時間を60秒\n再利用までの時間を15秒\nにする。";
                    break;
                case SkillType.BonusUp:
                    infoText = $"生産された\n全ポップコーンのスコアが\n{data.effectValue}倍になる。";
                    break;
                case SkillType.Critical:
                    infoText = $"クリックで生産された\nポップコーンが{data.effectValue}%で\n5倍になる。";                    
                    break;
                case SkillType.Fever:
                    infoText = $"フィーバー内に生産された\n全ポップコーンスコアの\n{data.effectValue}倍をスキル終了後に獲得。";
                    break;
                case SkillType.FixedPopcorn:
                    string p = (data.effectValue == 2) ? "キャラメル" : (data.effectValue == 3) ? "チョコレート" : "レインボー";
                    infoText = $"クリックで生産される\nポップコーンが\n{p}になる。";
                    break;
                case SkillType.MakerOffshoot:
                    infoText = $"稼働中の全マシンの\n{data.effectValue}倍の数になる。";
                    break;
                case SkillType.MakerSpeedUp:
                    infoText = $"稼働中の全マシンの\n生産時間を{data.effectValue}%短縮。";
                    break;
                case SkillType.MakerTimesUp:
                    infoText = $"稼働中の全マシンの\n生産量が{data.effectValue}倍になる。";
                    break;
                case SkillType.TimesUp:
                    infoText = $"生産量が{data.effectValue}倍になる。";
                    break;
            }
            data.infoText = infoText;

            EditorGUILayout.PropertyField(infoTextProp);
            data.cost = (data.panelType == PanelType.Lv1) ? valueMaster.costLv1 : (data.panelType == PanelType.Lv2) ? valueMaster.costLv2 : valueMaster.costLv3;
            if (data.panelType == PanelType.Special)
            {
                data.cost = 10000000000;
            }
            EditorGUILayout.PropertyField(costProp); 
            if (panelTypeProp.enumValueIndex == (int)PanelType.Lv1)
            {
                data.effectTime = valueMaster.effectTime;
                data.recastTime = valueMaster.recastTime; 
                EditorGUILayout.LabelField("スキルの初期値", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(effectTimeProp);
                EditorGUILayout.PropertyField(recastTimeProp);
            }
        }
        EditorGUILayout.Space();

        // 値の変更が確定した（ApplyModifiedPropertiesがtrueを返した）場合にリネームを実行
        if (serializedObject.ApplyModifiedProperties())
        {
            EditorUtility.SetDirty(data);
            RenameWithUniqueNumber();
        }
    }

    /// <summary>
    /// 重複しない名前を生成してアセットをリネームする
    /// </summary>
    private void RenameWithUniqueNumber()
    {
        SkillPanelData data = (SkillPanelData)target;
        string currentPath = AssetDatabase.GetAssetPath(data);

        // 実行中やパスが存在しない場合はスキップ
        if (Application.isPlaying || string.IsNullOrEmpty(currentPath)) return;

        string folderPath = Path.GetDirectoryName(currentPath);

        // --- ベース名生成の判定 ---
        // Reinforcementの場合は reinforcementType も名前に含める
        string baseName;
        if (data.panelType == PanelType.Reinforcement)
        {
            baseName = $"{data.skillType}_{data.panelType}_{data.reinforcementType}{data.level}";
        }
        else
        {
            baseName = $"{data.skillType}_{data.panelType}";
        }

        // 重複チェックと連番付与
        int number = 0;
        string targetName = baseName;
        string targetPath = Path.Combine(folderPath, $"{targetName}.asset").Replace("\\", "/");

        while (FileExistsExceptSelf(targetPath, currentPath))
        {
            targetName = $"{baseName}_{number}";
            targetPath = Path.Combine(folderPath, $"{targetName}.asset").Replace("\\", "/");
            number++;
        }

        // 名前が変わる場合のみリネームを実行
        if (data.name != targetName)
        {
            AssetDatabase.RenameAsset(currentPath, targetName);
        }
    }

    /// <summary>
    /// 自分自身を除いて指定パスにファイルが存在するかチェック
    /// </summary>
    private bool FileExistsExceptSelf(string targetPath, string myPath)
    {
        return targetPath != myPath && File.Exists(targetPath);
    }
}
#endif