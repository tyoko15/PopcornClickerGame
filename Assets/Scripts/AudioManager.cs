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
        Instance = this;

        bgmSources = new AudioSource[transform.GetChild(0).childCount];
        seSources = new AudioSource[transform.GetChild(1).childCount];
        //for (int i = 0; i < bgmSources.Length; i++) bgmSources[i] = transform.GetChild(0).GetChild(i).GetComponent<AudioSource>();
        bgmSources = transform.GetChild(0).GetComponentsInChildren<AudioSource>();
        for (int i = 0; i < seSources.Length; i++) seSources[i] = transform.GetChild(1).GetChild(i).GetComponent<AudioSource>();
    }

    public void SetAudioValue(int audio)
    {
        if (audio == 0) for (int i = 0; i < bgmSources.Length; i++) bgmSources[i].volume = bgmSlider.value;
        else if (audio == 1) for (int i = 0; i < seSources.Length; i++) seSources[i].volume = seSlider.value;
    }

    public void PlayBGM(int i)
    {
        for (int g = 0; g < bgmSources.Length; g++) bgmSources[g].Stop();
        bgmSources[i].Play();
    }

    public void PlayOneShotSE(int i)
    {
        seSources[i].PlayOneShot(seSources[i].clip);
    }
}
