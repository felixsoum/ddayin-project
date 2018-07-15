using UnityEngine;

public class Tile : MonoBehaviour
{
    public int GScore { get; set; }
    public int FScore { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    private MeshRenderer meshRenderer;
    public bool isObstacle;
    private bool isHighlighted;
    private bool isMouseOver;

    private TileManager tileManager;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateColor();
    }

    private void OnMouseEnter()
    {
        isMouseOver = true;
        UpdateColor();
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
        UpdateColor();
    }

    private void OnMouseDown()
    {
        tileManager.OnTileClick(this);
    }

    private void UpdateColor()
    {
        Color color = Color.white;
        if (isObstacle)
        {
            color = Color.red;
        }
        else if (isMouseOver)
        {
            color = Color.yellow;
        }
        else if (isHighlighted)
        {
            color = Color.blue;
        }
        meshRenderer.material.color = color;
    }

    public void Init(TileManager tileManager)
    {
        this.tileManager = tileManager;
    }

    public void SetHighlight(bool value)
    {
        isHighlighted = value;
        UpdateColor();
    }

    public override string ToString()
    {
        return "Tile(" + X + "," + Y + ")";
    }
}
