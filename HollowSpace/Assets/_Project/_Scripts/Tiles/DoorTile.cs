using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Tiles/Door Tile")]
public class DoorTile : TileBase
{
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private GameObject interactionPrefab;

    private static Dictionary<Vector3Int, DoorState> doorStates = new();

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (!go) return false;

        if (go.TryGetComponent<Interactable>(out var interactable))
        {
            interactable.Action.RemoveAllListeners();
            interactable.Action.AddListener(_ => OnInteract(position, tilemap));
        }
        
        doorStates[position] = go.GetComponent<DoorState>();

        UpdateTile(position, tilemap);

        return true;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = GetSprite(position);
        tileData.gameObject = interactionPrefab;
    }

    private Sprite GetSprite(Vector3Int position) {
        return doorStates.TryGetValue(position, out DoorState doorState) && doorState.IsOpen ? openedSprite : closedSprite;
    }

    private void OnInteract(Vector3Int position, ITilemap tilemap)
    {
        doorStates.TryGetValue(position, out DoorState doorState);
        
        doorState.ToggleOpen();
        UpdateTile(position, tilemap);
    }

    private void UpdateTile(Vector3Int position, ITilemap tilemap)
    {
        tilemap.RefreshTile(position);
    }
}