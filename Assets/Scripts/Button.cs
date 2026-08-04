using UnityEngine;
using TMPro;

public enum KindButton
{
    None,
    Regular,
    Caramel,
    Choocolate,
    Rainbow,
    Auto,
}

public class Button : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] KindButton kindButton = KindButton.None;

    public int baseNeedAmount;
    public int needAmount;
    [SerializeField] float costMultiplier;
    [SerializeField] int limitLevel;
    bool limitFlag;
    public int level = 1;

    TextMeshProUGUI aText;
    TextMeshProUGUI lText;
    void Start()
    {
        gameManager = GameManager.Instance;

        aText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        lText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        UpdateUI();

        needAmount = baseNeedAmount;
    }

    void Update()
    {
        
    }
    
    public void ClickButton()
    {
        if (!limitFlag)
        {
            if (gameManager.pAmount >= needAmount)
            {
                level++;
                gameManager.pAmount -= needAmount;
                UpdataLevel();

                if (limitLevel == level)
                {
                    Limit();
                    return;
                }

                needAmount = (int)Mathf.Floor(baseNeedAmount * Mathf.Pow(costMultiplier, level - 1));
            }

            UpdateUI();
        }
    }

    void Limit()
    {
        aText.text = $"MAX";
        lText.text = $"LevelMAX";
        limitFlag = true;
    }

    void UpdataLevel()
    {
        switch (kindButton)
        {
            case KindButton.Regular:
                gameManager.score++;
                break;
            case KindButton.Caramel:
                gameManager.caramelRate++;
                break;
            case KindButton.Choocolate:
                gameManager.choolateRate++;
                break;
            case KindButton.Rainbow:
                gameManager.rainbowRate++;
                break;
            case KindButton.Auto:
                gameManager.autoMakerCount++;
                gameManager.AddAutoMaker();
                break;
        }
    }

    void UpdateUI()
    {
        aText.text = $"{needAmount.ToString()}p";
        lText.text = $"Level{level}";
    }
}
