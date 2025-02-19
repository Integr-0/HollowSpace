using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ManualPower : MonoBehaviour {
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Vector3Int position;

    [ContextMenu("Power Tile")]
    public void Power() {
        var tile = tilemap.GetTile<ElectricalWireTile>(position);

        if (!tile) {
            Debug.LogWarning("Tile at position " + position + " is not an ElectricalWireTile");
        }
        
        tile.Toggle(tilemap, position);
    }
}
