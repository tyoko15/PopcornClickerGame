using UnityEngine;
using unityroom.Api;

public class RankingManager : MonoBehaviour
{
    public static RankingManager Instance;

    private void Awake()
    {
         Instance = this;
    }

    public void SendRecordPAmount()
    {
        UnityroomApiClient.Instance.SendScore(1, GameManager.Instance.recordPAmount, ScoreboardWriteMode.HighScoreDesc);
    }
    public void SendRecordRepeatCount()
    {
        UnityroomApiClient.Instance.SendScore(2, GameManager.Instance.recordRepeatCount, ScoreboardWriteMode.HighScoreDesc);
    }
}