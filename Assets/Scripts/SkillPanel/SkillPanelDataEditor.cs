using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillPanelData))]
public class SkillPanelDataEditor : Editor
{
    private SerializedProperty skillTypeProp;
    private SerializedProperty panelTypeProp;
    private SerializedProperty reinforcementTypeProp;
    private SerializedProperty levelProp;
    private SerializedProperty effectTimeProp;
    private SerializedProperty recastTimeProp;
    private SerializedProperty costProp;
    private SerializedProperty infoTextProp;

    private void OnEnable()
    {
        skillTypeProp = serializedObject.FindProperty("skillType");
        panelTypeProp = serializedObject.FindProperty("panelType");
        reinforcementTypeProp = serializedObject.FindProperty("reinforcementType");
        levelProp = serializedObject.FindProperty("level");
        effectTimeProp = serializedObject.FindProperty("effectTime");
        recastTimeProp = serializedObject.FindProperty("recastTime");
        costProp = serializedObject.FindProperty("cost");
        infoTextProp = serializedObject.FindProperty("infoText");
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

            EditorGUILayout.PropertyField(costProp);

            Reinforcement currentReinforcement = (Reinforcement)reinforcementTypeProp.enumValueIndex;
            EditorGUILayout.LabelField("強化される内容", EditorStyles.boldLabel);

            if (currentReinforcement == Reinforcement.EffectTime)
            {
                EditorGUILayout.PropertyField(effectTimeProp);                
                data.infoText = $"効果時間を{data.effectTime}秒延長する。";
            }
            else if (currentReinforcement == Reinforcement.RecastTime)
            {
                EditorGUILayout.PropertyField(recastTimeProp);
                data.infoText = $"再使用までの時間を\n{data.recastTime}秒短縮する。";
            }
        }
        else
        {
            EditorGUILayout.PropertyField(costProp);
            EditorGUILayout.PropertyField(infoTextProp);
            if (panelTypeProp.enumValueIndex == (int)PanelType.Lv1)
            {
                EditorGUILayout.LabelField("スキルの初期値", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(effectTimeProp);
                EditorGUILayout.PropertyField(recastTimeProp);
            }
        }
        EditorGUILayout.Space();

        // 値の変更が確定した（ApplyModifiedPropertiesがtrueを返した）場合にリネームを実行
        if (serializedObject.ApplyModifiedProperties())
        {
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