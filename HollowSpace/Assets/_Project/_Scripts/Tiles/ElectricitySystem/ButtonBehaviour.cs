using UnityEngine;
using UnityEngine.Tilemaps;

public class ButtonBehaviour : MonoBehaviour {
    private ITilemap _tilemap;
    private Vector3Int _position;
    private ElectricalWireTile[] _wires;
    
    public void Init(ITilemap tilemap, Vector3Int position, ElectricalWireTile[] wires) {
        _tilemap = tilemap;
        _position = position;
        _wires = wires;
    }

    public void OnInteract(InteractionAgent _) {
        Debug.Log("Button pressed at " + _position);

        // This is a temporary solution until I can fix that annoying bug
        var tempTilemap = GameObject.Find("FloorTileMap").GetComponent<Tilemap>();

        foreach (var wire in _wires) {
            wire.Toggle(tempTilemap, _position);
        }
    }
}