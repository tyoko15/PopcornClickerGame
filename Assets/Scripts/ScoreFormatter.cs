using System;
using UnityEngine;

public class ScoreFormatter : MonoBehaviour
{
    // 日本の4桁ごとの単位リスト（無量大数まで対応）
    private static readonly string[] JapaneseUnits = new string[]
    {
        "", "万", "億", "兆", "京", "垓", "秭", "穣", "溝", "澗", "正", "載", "極", "恒河沙", "阿僧祇", "那由他", "不可思議", "無量大数"
        // 万億兆京垓秭穣溝澗正載極恒河沙阿僧祇那由他不可思議無量大数
    };

    /// <summary>
    /// double型の数値を「億」「兆」「京」などの日本語単位付き文字列に変換する
    /// </summary>
    public static string FormatToJapanese(double amount)
    {
        // 無限大（Infinity）や計算エラーのガード
        if (double.IsInfinity(amount) || double.IsNaN(amount))
        {
            return "MAX";
        }

        // 1億（1e8 = 100,000,000）未満は通常のカンマ区切りで表示
        // ※ もし1万から表示したい場合は「1e8」を「1e4」に変更してください
        if (amount < 1e8)
        {
            return amount.ToString("N0");
        }

        int unitIndex = 0;
        double temp = amount;

        // 4桁（10,000）で割りながら単位を1つずつ進める
        while (temp >= 10000 && unitIndex < JapaneseUnits.Length - 1)
        {
            temp /= 10000;
            unitIndex++;
        }

        // 無量大数（約10の68乗）すら超えてしまった場合は指数表記（e表記）にする
        if (temp >= 10000 && unitIndex == JapaneseUnits.Length - 1)
        {
            return amount.ToString("e2");
        }

        // 小数点以下2桁まで表示（例: 1.23億 p / 45.67兆 p）
        return $"{temp:F2} {JapaneseUnits[unitIndex]}";
    }
}