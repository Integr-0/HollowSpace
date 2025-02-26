using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Electricity/Button Tile")]
public class ButtonTile : TileBase {
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject prefab;
    
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = sprite;
        tileData.gameObject = prefab;
    }
    
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
        if (!go) return false;
        
        if (go.TryGetComponent<Interactable>(out var interactable) && go.TryGetComponent<ButtonBehaviour>(out var button)) {
            button.Init(tilemap, position, GetNeighbouringWireTiles(tilemap, position));
            
            interactable.Action.RemoveAllListeners();
            interactable.Action.AddListener(button.OnInteract);
        }

        return true;
    }

    private static ElectricalWireTile[] GetNeighbouringWireTiles(ITilemap tilemap, Vector3Int position) {
        List<ElectricalWireTile> tiles = new();
        for (int yd = -1; yd <= 1; yd++)
        for (int xd = -1; xd <= 1; xd++) {
            Vector3Int pos = new(position.x + xd, position.y + yd, position.z);
            if (!IsWire(tilemap, pos, out var wire)) continue;
            
            tiles.Add(wire);
        }
        
        return tiles.ToArray();
    }
    private static bool IsWire(ITilemap tilemap, Vector3Int position, out ElectricalWireTile wire) {
        if (tilemap.GetTile(position) is ElectricalWireTile wireTile) {
            wire = wireTile;
            return true;
        }

        wire = null;
        return false;
    }
}