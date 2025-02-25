using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Vent Tile")]
public class VentTile : TileBase
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject interactionPrefab;
    [Space, SerializeField] private int defaultLevelIndex = 0;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (!go) return false;

        go.hideFlags = HideFlags.DontSave;
        
        if (go.TryGetComponent<Interactable>(out var interactable) && go.TryGetComponent<VentData>(out var data))
        {
            data.levelIndex = defaultLevelIndex;
            
            interactable.Action.RemoveAllListeners();
            interactable.Action.AddListener(data.OnInteract);
        }

        return true;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = sprite;
        tileData.gameObject = interactionPrefab;
    }
}