using UnityEngine;
using unityroom.Api;

/// <summary>
/// UnityRoomëóêMóp
/// </summary>
public class RankingManager : MonoBehaviour
{
    public static RankingManager Instance;

    private void Awake()
    {
         Instance = this;
    }

    public void SendRecordPAmount()
    {
        float sendAmount = (float)GameManager.Instance.pAmount;
        UnityroomApiClient.Instance.SendScore(1, sendAmount, ScoreboardWriteMode.HighScoreDesc);
    }
    public void SendRecordRepeatCount()
    {
        UnityroomApiClient.Instance.SendScore(2, GameManager.Instance.recordRepeatCount, ScoreboardWriteMode.HighScoreDesc);
    }
}