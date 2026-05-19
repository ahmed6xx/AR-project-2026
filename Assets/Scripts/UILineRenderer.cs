using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    public Vector2 from;
    public Vector2 to;
    public float thickness = 6f;
    public GameObject headObject;
    public Sprite sprite;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (sprite != null)
            material = new Material(Shader.Find("UI/Default"));
    }

    protected override void UpdateMaterial()
    {
        base.UpdateMaterial();
        if (sprite != null)
            canvasRenderer.SetTexture(sprite.texture);
    }

    void OnDestroy()
    {
        if (headObject != null) Destroy(headObject);
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 dir = (to - from).normalized;
        Vector2 perp = new Vector2(-dir.y, dir.x) * (thickness * 0.5f);

        Vector2 localFrom = ScreenToLocal(from);
        Vector2 localTo = ScreenToLocal(to);

        Vector2 p = new Vector2(-dir.y, dir.x) * (thickness * 0.5f);

        // Recalculate perp in local space
        Vector2 localDir = (localTo - localFrom).normalized;
        Vector2 localPerp = new Vector2(-localDir.y, localDir.x) * (thickness * 0.5f);

        Color32 c = color;

        // UV coords to stretch the sprite along the line
        UIVertex v0 = new UIVertex(); v0.color = c; v0.position = localFrom - localPerp; v0.uv0 = new Vector2(0, 0);
        UIVertex v1 = new UIVertex(); v1.color = c; v1.position = localFrom + localPerp; v1.uv0 = new Vector2(0, 1);
        UIVertex v2 = new UIVertex(); v2.color = c; v2.position = localTo + localPerp; v2.uv0 = new Vector2(1, 1);
        UIVertex v3 = new UIVertex(); v3.color = c; v3.position = localTo - localPerp; v3.uv0 = new Vector2(1, 0);

        vh.AddVert(v0);
        vh.AddVert(v1);
        vh.AddVert(v2);
        vh.AddVert(v3);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    Vector2 ScreenToLocal(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, screenPos, null, out Vector2 local);
        return local;
    }
}