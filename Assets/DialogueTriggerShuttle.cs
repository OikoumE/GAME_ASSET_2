using Controllers;
using UnityEngine;

public class DialogueTriggerShuttle : DialogueTrigger
{
    private void Start()
    {
    }

    private void Update()
    {
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IPlayer iP)) return;
        playerController = iP.GetPlayerController();
        if (playerController.hasPickedFuse && playerController.hasReadShuttleTablet) return;
        base.OnTriggerEnter(other);
    }
}