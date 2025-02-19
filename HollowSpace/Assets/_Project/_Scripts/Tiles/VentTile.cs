using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Vent Tile")]
public class VentTile : InteractableTile
{
    [SerializeField] private int levelIndex;
    
    protected override void OnInteract(InteractionAgent agent) {
        Debug.Log(agent.name + " interacted with vent and will be teleported to level " + levelIndex);
        SceneController.Instance.LoadLevelAsync(levelIndex);
    }
}