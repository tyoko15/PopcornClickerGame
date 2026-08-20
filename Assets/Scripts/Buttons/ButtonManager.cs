using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    void Start()
    {
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(370f, 150f * transform.childCount + 50f);
        for (int i = 0; i < transform.childCount; i++) transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -100f - i * 150f);            
    }

    void Update()
    {
        
    }
}
