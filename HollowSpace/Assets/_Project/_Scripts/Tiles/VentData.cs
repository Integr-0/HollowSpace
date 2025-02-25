using UnityEngine;

public class VentData : MonoBehaviour
{
    public int levelIndex = 0;
    
    public void OnInteract(InteractionAgent agent) {
        Debug.Log(agent.name + " interacted with vent and will be teleported to level " + levelIndex);
        SceneController.Instance.LoadLevelAsync(levelIndex);
    }
}