using UnityEngine;
using TMPro;

public class BannerButtonManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI layerName; // ボタンの名前テキスト
    [SerializeField] GameObject banner;         // UI
    GameObject[] layers;                        // 各UIレイヤー
    RectTransform[] buttons;                    // ボタン

    bool flag;  // UIのONOFF

    // UIのOnOff演出に使用する変数
    bool setFlag;                           // ONOFF演出中フラグ
    [SerializeField] float setTime;         // 演出時間
    float setTimer;                         // 演出タイマー

    void Start()
    {
        // レイヤーとボタンの初期化
        layers = new GameObject[banner.transform.childCount - 2];
        buttons = new RectTransform[transform.childCount];
        // レイヤーとボタンの取得
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i] = banner.transform.GetChild(i + 1).gameObject;
            layers[i].SetActive(false);
            buttons[i] = transform.GetChild(i).GetComponent<RectTransform>();
        }        
        layers[0].SetActive(true);
        layerName.text = $"情報一覧";
        flag = true;
    }

    void Update()
    {
        if (setFlag)
        {
            // OFF
            if (!flag)
            {
                if (setTimer > setTime)
                {
                    setTimer = 0;
                    setFlag = false;
                }
                else
                {
                    setTimer += Time.deltaTime;
                    float x = Mathf.Lerp(200f, -200f, setTimer / setTime);
                    banner.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
                    x = Mathf.Lerp(440f, 40f, setTimer / setTime);
                    GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
                }
            }
            // ON
            else
            {
                if (setTimer > setTime)
                {
                    setTimer = 0;
                    setFlag = false;
                }
                else
                {
                    setTimer += Time.deltaTime;
                    float x = Mathf.Lerp(-200f, 200f, setTimer / setTime);
                    banner.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
                    x = Mathf.Lerp(40f, 440f, setTimer / setTime);
                    GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
                }
            }
        }
    }

    /// <summary>
    /// 切り替えボタン
    /// </summary>
    /// <param name="i"></param>
    public void ClickButton(int i)
    {
        // UIバナーの切り替え
        if (i != 5)
        {
            if (!flag)
            {
                flag = true;
                setFlag = true;
            }

            for (int l = 0; l < layers.Length; l++)
            {
                layers[l].SetActive(false);
                buttons[l].sizeDelta = new Vector2(50f, 200f);
                buttons[l].anchoredPosition = new Vector2(25f, -100f - l * 200f);
            }
            layers[i].SetActive(true);
            buttons[i].sizeDelta = new Vector2(80f, 200f);
            buttons[i].anchoredPosition = new Vector2(40f, -100f - i * 200f);
            switch (i)
            {
                case 0:
                    layerName.text = $"情報一覧";
                    break;
                case 1:
                    layerName.text = $"強化";
                    break;
                case 2:
                    layerName.text = $"自動化";
                    break;
                case 3:
                    layerName.text = $"記録";
                    break;
                case 4:
                    layerName.text = $"設定";
                    break;
            }
        }
        // UIのOnOff
        else
        {
            flag = (!flag) ? true : false;
            setFlag = true;
            string a = (!flag) ? "→" : "←";
            transform.GetChild(transform.childCount - 1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{a}";
        }
    }
}
