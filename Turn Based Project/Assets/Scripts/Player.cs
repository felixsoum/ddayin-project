using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5;
    [SerializeField]
    private float distanceToTile = 0.5f;
    public List<Tile> Path { get; set; }
    public Tile CurrentTile { get; set; }

    public void Init()
    {
        var pos = transform.position;
        pos.x = CurrentTile.transform.position.x;
        pos.z = CurrentTile.transform.position.z;
    }

    private void Update()
    {
        if (Path == null || Path.Count == 0)
        {
            return;
        }

        CurrentTile = Path[Path.Count - 1];

        var playerPos = transform.position;
        var tilePos = CurrentTile.transform.position;

        transform.position = Vector3.Lerp(transform.position, CurrentTile.transform.position, Time.deltaTime * moveSpeed);
        if (Vector3.Distance(playerPos, tilePos) < distanceToTile)
        {
            Path.RemoveAt(Path.Count - 1);
        }
    }
}
