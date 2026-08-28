using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillData))]
public class SkillDataEditor : Editor
{
    private SerializedProperty skillTypeProp;
    private SerializedProperty effectTimeProp;
    private SerializedProperty recastTimeProp;

    private void OnEnable()
    {
        skillTypeProp = serializedObject.FindProperty("skillType");
        effectTimeProp = serializedObject.FindProperty("effectTime");
        recastTimeProp = serializedObject.FindProperty("recastTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SkillData data = (SkillData)target;

        EditorGUILayout.LabelField("スキル設定", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(skillTypeProp);

        EditorGUILayout.LabelField("基本設定", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(effectTimeProp);
        EditorGUILayout.PropertyField(recastTimeProp);

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
        SkillData data = (SkillData)target;
        string currentPath = AssetDatabase.GetAssetPath(data);

        // 実行中やパスが存在しない場合はスキップ
        if (Application.isPlaying || string.IsNullOrEmpty(currentPath)) return;

        string folderPath = Path.GetDirectoryName(currentPath);

        // --- ベース名生成の判定 ---
        // Reinforcementの場合は reinforcementType も名前に含める
        string baseName = $"{data.skillType}Data";

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
