using UnityEngine;
using UnityEngine.UI;

public enum Audio
{
    BGM,
    SE,
}


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource[] bgmSources;
    public AudioSource[] seSources;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider seSlider;
    void Awake()
    {
        Instance = this;    // インスタンス化

        // オーディオソース初期化&取得
        bgmSources = new AudioSource[transform.GetChild(0).childCount];
        seSources = new AudioSource[transform.GetChild(1).childCount];
        bgmSources = transform.GetChild(0).GetComponentsInChildren<AudioSource>();
        seSources = transform.GetChild(1).GetComponentsInChildren<AudioSource>();
    }

    /// <summary>
    /// 音量調整
    /// </summary>
    /// <param name="audio"></param>
    public void SetAudioValue(int audio)
    {
        if (audio == 0) for (int i = 0; i < bgmSources.Length; i++) bgmSources[i].volume = bgmSlider.value;
        else if (audio == 1) for (int i = 0; i < seSources.Length; i++) seSources[i].volume = seSlider.value;
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="i"></param>
    public void PlayBGM(int i)
    {
        for (int g = 0; g < bgmSources.Length; g++) bgmSources[g].Stop();
        bgmSources[i].Play();
    }

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="i"></param>
    public void PlayOneShotSE(int i)
    {
        seSources[i].PlayOneShot(seSources[i].clip);
    }
}
