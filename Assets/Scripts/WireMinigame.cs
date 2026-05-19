using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class WireMinigame : MonoBehaviour
{
    [Header("UI References")]
    public GameObject minigamePanel;
    public RectTransform[] leftNodes;
    public RectTransform[] rightNodes;

    [Header("Colors")]
    public Color[] wireColors = new Color[]
    {
        Color.red, Color.yellow, Color.blue, Color.green
    };

    [Header("Visual")]
    public Sprite wireSprite;        // Wire_in sprite for the line
    public float lineThickness = 6f;

    [Header("References")]
    public GameManager gameManager;

    // Internal
    private int draggingIndex = -1;
    private Vector2 dragCurrentPos;
    private Dictionary<int, int> connections = new Dictionary<int, int>();
    private int[] correctMapping;
    private List<UILineRenderer> drawnLines = new List<UILineRenderer>();

    void Start()
    {
        minigamePanel.SetActive(false);
        correctMapping = new int[] { 0, 1, 2, 3 };
        ShuffleArray(correctMapping);
    }

    // ─── OPEN / CLOSE ────────────────────────────────────────────

    public void OpenMinigame()
    {
        connections.Clear();
        foreach (var line in drawnLines)
            if (line != null) Destroy(line.gameObject);
        drawnLines.Clear();

        correctMapping = new int[] { 0, 1, 2, 3 };
        ShuffleArray(correctMapping);

        // Color left nodes
        for (int i = 0; i < leftNodes.Length; i++)
        {
            Image img = leftNodes[i].GetComponent<Image>();
            if (img != null) img.color = wireColors[i];
        }

        // Color right nodes
        for (int i = 0; i < rightNodes.Length; i++)
        {
            for (int j = 0; j < correctMapping.Length; j++)
            {
                if (correctMapping[j] == i)
                {
                    Image img = rightNodes[i].GetComponent<Image>();
                    if (img != null) img.color = wireColors[j];
                    break;
                }
            }
        }

        minigamePanel.SetActive(true);
    }

    public void CloseMinigame()
    {
        minigamePanel.SetActive(false);
    }

    // ─── CALLED BY EVENT TRIGGERS ────────────────────────────────

    public void BeginDragNode0() { StartDrag(0); }
    public void BeginDragNode1() { StartDrag(1); }
    public void BeginDragNode2() { StartDrag(2); }
    public void BeginDragNode3() { StartDrag(3); }

    public void EndDragNode0() { EndDrag(0); }
    public void EndDragNode1() { EndDrag(1); }
    public void EndDragNode2() { EndDrag(2); }
    public void EndDragNode3() { EndDrag(3); }

    public void BeginDragRight0() { StartDragFromRight(0); }
    public void BeginDragRight1() { StartDragFromRight(1); }
    public void BeginDragRight2() { StartDragFromRight(2); }
    public void BeginDragRight3() { StartDragFromRight(3); }

    // ─── DRAG LOGIC ──────────────────────────────────────────────

    void StartDrag(int index)
    {
        if (connections.ContainsKey(index))
            connections.Remove(index);

        draggingIndex = index;
        dragCurrentPos = leftNodes[index].position;
        RedrawLines();
    }

    void StartDragFromRight(int rightIndex)
    {
        int leftIndex = -1;
        foreach (var kv in connections)
        {
            if (kv.Value == rightIndex) { leftIndex = kv.Key; break; }
        }

        if (leftIndex < 0) return;

        connections.Remove(leftIndex);
        draggingIndex = leftIndex;
        dragCurrentPos = rightNodes[rightIndex].position;
        RedrawLines();
    }

    void EndDrag(int index)
    {
        if (draggingIndex < 0) return;

        for (int i = 0; i < rightNodes.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rightNodes[i], dragCurrentPos))
            {
                List<int> toRemove = new List<int>();
                foreach (var kv in connections)
                    if (kv.Value == i) toRemove.Add(kv.Key);
                foreach (var k in toRemove) connections.Remove(k);

                connections[draggingIndex] = i;
                break;
            }
        }

        draggingIndex = -1;
        RedrawLines();
        CheckWin();
    }

    public void OnDrag(BaseEventData eventData)
    {
        if (draggingIndex < 0) return;
        PointerEventData ped = eventData as PointerEventData;
        if (ped != null) dragCurrentPos = ped.position;
        RedrawLines();
    }

    void Update()
    {
        if (draggingIndex < 0) return;
        dragCurrentPos = Input.mousePosition;
        RedrawLines();
    }

    // ─── DRAW LINES ──────────────────────────────────────────────

    void RedrawLines()
    {
        foreach (var line in drawnLines)
            if (line != null) Destroy(line.gameObject);
        drawnLines.Clear();

        foreach (var kv in connections)
            DrawLine(leftNodes[kv.Key].position, rightNodes[kv.Value].position, wireColors[kv.Key]);

        if (draggingIndex >= 0)
            DrawLine(leftNodes[draggingIndex].position, dragCurrentPos, wireColors[draggingIndex]);
    }

    void DrawLine(Vector2 from, Vector2 to, Color color)
    {
        GameObject go = new GameObject("Line");
        go.transform.SetParent(minigamePanel.transform, false);
        go.transform.SetAsFirstSibling();
        go.AddComponent<CanvasRenderer>();

        UILineRenderer lr = go.AddComponent<UILineRenderer>();
        lr.color = color;
        lr.from = from;
        lr.to = to;
        lr.thickness = lineThickness;
        if (wireSprite != null) lr.sprite = wireSprite;
        drawnLines.Add(lr);
    }

    // ─── WIN CHECK ───────────────────────────────────────────────

    void CheckWin()
    {
        if (connections.Count < 4) return;

        bool allCorrect = true;
        for (int i = 0; i < 4; i++)
        {
            if (!connections.ContainsKey(i) || connections[i] != correctMapping[i])
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            Debug.Log("Wires matched! Electricity fixed!");
            CloseMinigame();
            gameManager.ResolveCurrentIncident();
        }
    }

    // ─── HELPERS ─────────────────────────────────────────────────

    void ShuffleArray(int[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = arr[i]; arr[i] = arr[j]; arr[j] = tmp;
        }
    }
}