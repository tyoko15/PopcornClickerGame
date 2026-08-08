using UnityEngine;
using UnityEngine.UI;

public class MultipleButtonManager : MonoBehaviour
{
    Image[] images = new Image[3];
    int multiple = 1;
    void Awake()
    {
        images = new Image[transform.childCount - 1];
        for (int i = 0; i < images.Length; i++) images[i] = transform.GetChild(i).GetComponent<Image>();
    }

    void Update()
    {
        
    }

    public void MultipleButton(int i)
    {
        if (i == 0) multiple = 1;
        else if (i == 1) multiple = 10;
        else if (i == 2) multiple = 50;
        for (int j = 0; j < images.Length; j++) images[j].color = Color.gray;
        images[i].color = Color.orange;
        GameManager.Instance.Multiple = multiple;
    }
}
