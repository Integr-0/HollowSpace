using UnityEngine;
using UnityEngine.Tilemaps;

public class TileInteraction : MonoBehaviour {
    private ITilemap _tilemap;
    private TileBase _tile;
    private Vector3Int _position;
    
    public void Initialize(ITilemap tilemap, TileBase tile, Vector3Int position) {
        _tilemap = tilemap;
        _tile = tile;
        _position = position;
    }
    
    public void Refresh() {
        _tilemap.RefreshTile(_position);
    }
}