using UnityEngine;

public class ScrollRectCenterZoom : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private RectTransform content;   // 拡大縮小するContent
    [SerializeField] private RectTransform viewport;  // ScrollRectのViewport

    [Header("ズーム設定")]
    float currentScale = 1.0f; // 現在のスケール
    [SerializeField] private float zoomSpeed = 0.1f;  // ズーム感度
    [SerializeField] private float minScale = 0.5f;   // 最小倍率
    [SerializeField] private float maxScale = 3.0f;   // 最大倍率

    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll == 0) return;
        if (scroll == 1) currentScale += zoomSpeed;
        else if (scroll == -1) currentScale -= zoomSpeed;

        if (currentScale < minScale) currentScale = minScale;
        if (currentScale > maxScale) currentScale = maxScale;

        content.localScale = new Vector3(currentScale, currentScale, 1f);

        SkillTreeManager.Instance.ResetPosition();
    }
}