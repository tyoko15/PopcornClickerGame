using UnityEngine;
using UnityEngine.UI;

public class AutoPopcornMaker : MonoBehaviour
{
    GameManager gameManager;
    GameObject spwaner;
    [SerializeField] float time;
    float timer;

    Image gauge;
    void Start()
    {
        gameManager = GameManager.Instance;
        spwaner = transform.GetChild(0).gameObject;
        gauge = transform.GetChild(1).GetChild(1).GetComponent<Image>();
    }

    void Update()
    {
        if (timer > time)
        {
            gameManager.AutoClick(spwaner);
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
            gauge.fillAmount = Mathf.InverseLerp(0f, time, timer);
        }
    }
}
