using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Vent Tile")]
public class VentTile : TileBase
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject interactionPrefab;
    [SerializeField] private int sceneIndex;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (!go) return false;

        Debug.Log(position);
        go.transform.position = position - new Vector3(0.5f, 0, 0);
        
        var interactable = go.GetComponent<Interactable>();
        if (interactable != null)
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
        Debug.Log(agent.name + " interacted with vent and will be teleported to scene " + sceneIndex);
    }
}