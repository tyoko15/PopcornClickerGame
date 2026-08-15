using UnityEngine;
using UnityEngine.UI;

public class MultipleButtonManager : MonoBehaviour
{
    Image[] images = new Image[3];  // ボタンのイメージ
    int multiple = 1;               // 倍数

    void Awake()
    {
        // ボタンのイメージ初期化&取得
        images = new Image[transform.childCount - 1];
        for (int i = 0; i < images.Length; i++) images[i] = transform.GetChild(i).GetComponent<Image>();
    }

    /// <summary>
    /// 倍数ボタン
    /// </summary>
    /// <param name="i"></param>
    public void MultipleButton(int i)
    {
        if (i == 0) multiple = 1;
        else if (i == 1) multiple = 5;
        else if (i == 2) multiple = 10;
        for (int j = 0; j < images.Length; j++) images[j].color = Color.gray;
        images[i].color = Color.orange;
        GameManager.Instance.Multiple = multiple;
    }
}
