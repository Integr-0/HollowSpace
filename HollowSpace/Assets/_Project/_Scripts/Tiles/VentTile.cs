using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Vent Tile")]
public class VentTile : TileBase
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject interactionPrefab;
    [FormerlySerializedAs("sceneIndex")] [SerializeField] private int levelIndex;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (!go) return false;
        
        if (go.TryGetComponent<Interactable>(out var interactable))
        {
            interactable.Action.RemoveAllListeners();
            interactable.Action.AddListener(OnInteract);
        }

        return true;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = sprite;
        tileData.gameObject = interactionPrefab;
    }

    private void OnInteract(InteractionAgent agent)
    {
        Debug.Log(agent.name + " interacted with vent and will be teleported to level " + levelIndex);
        SceneController.Instance.LoadLevelAsync(levelIndex);
    }
}