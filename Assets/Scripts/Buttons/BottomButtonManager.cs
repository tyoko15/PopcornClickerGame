using TMPro;
using UnityEngine;

public class BottomButtonManager: MonoBehaviour
{
    [SerializeField] GameObject ui;
    [SerializeField] GameObject bottomBanner;
    GameObject[] commonUIs;
    GameObject[] layers;
    TextMeshProUGUI nameText;

    [SerializeField] GameObject otherUI;

    void Start()
    {
        commonUIs = new GameObject[3];
        for (int i = 0; i < commonUIs.Length; i++) commonUIs[i] = bottomBanner.transform.GetChild(i).gameObject;
        layers = new GameObject[3];
        for (int i = 0; i < layers.Length; i++) layers[i] = bottomBanner.transform.GetChild(i + 3).gameObject;
        nameText = bottomBanner.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        ClickReturn();
    }
   
    public void ClickButton(int i)
    {
        GameManager.Instance.Pause(true);
        bottomBanner.SetActive(true);
        for (int c = 0; c < commonUIs.Length; c++) commonUIs[c].SetActive(true);
        for (int c = 0; c < layers.Length; c++) layers[c].SetActive(false);
        switch (i)
        {
            case 0:
                nameText.text = $"スキルツリー";
                break;
            case 1:
                nameText.text = $"スキル一覧";
                break;
            case 2:
                nameText.text = $"部屋情報";
                break;
        }
        layers[i].SetActive(true);
        otherUI.SetActive(false);
    }

    public void ClickReturn()
    {
        bottomBanner.SetActive(false);
        for (int i = 0; i < commonUIs.Length; i++) commonUIs[i].SetActive(false);
        for (int i = 0; i < layers.Length; i++) layers[i].SetActive(false);
        GameManager.Instance.Pause(false);
        otherUI.SetActive(true);
    }
}
