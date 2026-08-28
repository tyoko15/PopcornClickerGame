using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UILine : MaskableGraphic
{
    [SerializeField] private RectTransform startTarget;
    [SerializeField] private RectTransform endTarget;
    public SkillPanel startPanel;
    public SkillPanel endPanel;

    [SerializeField] private float thickness = 5f;


    // 白テクスチャの正解の参照方法（自分自身を呼ばず親クラスの処理を利用する）
    public override Texture mainTexture => s_WhiteTexture;

    public void SetTargets(RectTransform start, RectTransform end)
    {
        startTarget = start;
        endTarget = end;
        RefreshLine();
    }

    public void RefreshLine()
    {
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (startTarget == null || endTarget == null) return;

        RectTransform myRect = rectTransform;
        Vector2 startPos = myRect.InverseTransformPoint(startTarget.position);
        Vector2 endPos = myRect.InverseTransformPoint(endTarget.position);

        Vector2 dir = (endPos - startPos).normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x) * (thickness * 0.5f);

        UIVertex[] verts = new UIVertex[4];
        for (int i = 0; i < 4; i++)
        {
            verts[i] = UIVertex.simpleVert;
            verts[i].color = color;
            verts[i].uv0 = Vector2.zero;
        }

        verts[0].position = startPos - normal;
        verts[1].position = startPos + normal;
        verts[2].position = endPos + normal;
        verts[3].position = endPos - normal;

        vh.AddUIVertexQuad(verts);
    }

    public void SetLineColor()
    {
        if (endTarget == null)
        {
            color = SkillTreeManager.Instance.lineColors[0];
            return;
        }

        if (startPanel == null && endPanel == null)
        {
            startPanel = startTarget.gameObject.GetComponent<SkillPanel>();
            endPanel = endTarget.gameObject.GetComponent<SkillPanel>();
        }

        if (startPanel.state == PanelState.Acquired && endPanel.state == PanelState.UnLock)
        {
            color = SkillTreeManager.Instance.lineColors[1];
        }
        else if (startPanel.state == PanelState.Acquired && endPanel.state == PanelState.Acquired)
        {
            color = SkillTreeManager.Instance.lineColors[2];
        }
        else color = SkillTreeManager.Instance.lineColors[0];
    } 
}