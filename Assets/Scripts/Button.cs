using UnityEngine;
using TMPro;

public class Button : MonoBehaviour
{
    GameManager gameManager;
    int pAmount;

    public int needAmount;
    public int level = 1;

    TextMeshProUGUI aText;
    TextMeshProUGUI lText;
    void Start()
    {
        gameManager = GameManager.Instance;
        pAmount = gameManager.pAmount;

        aText.text = $"{needAmount.ToString()}p";
        lText.text = $"{level}";
    }

    void Update()
    {
        
    }
    
    public void ClickButton()
    {
        if (pAmount > needAmount)
        {
            level++;
            pAmount -= needAmount;
        }

        aText.text = $"{needAmount.ToString()}p";
        lText.text = $"{level}";
    }
}
