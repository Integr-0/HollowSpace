using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class FlashlightBattery : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Interactable>().Action.AddListener(InsertBattery);
    }
    
    private void InsertBattery(InteractionAgent agent)
    {
        if (!agent.TryGetComponent<Flashlight>(out var flashlight)) {
            Debug.LogWarning("No flashlight found on the agent.", agent);
            return;
        }
        
        if (!flashlight.InsertBattery()) {
            Debug.LogWarning("Flashlight already has the maximum amount of batteries.", flashlight);
            return;
        }
        
        Destroy(gameObject);
    }
}
