using UnityEngine;

public class AutoPopcornMaker : MonoBehaviour
{
    GameManager gameManager;
    GameObject spwaner;
    [SerializeField] float time;
    float timer;


    void Start()
    {
        gameManager = GameManager.Instance;
        spwaner = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (timer > time)
        {
            gameManager.AutoClick(spwaner);
            timer = 0;
        }
        else timer += Time.deltaTime;
    }
}
