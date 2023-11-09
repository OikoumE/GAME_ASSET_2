using Controllers;
using UnityEngine;

public class DialogueTriggerDrill : DialogueTrigger
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
        if (playerController.hasPickedDrill) return;
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out IPlayer iP)) return;
        if (playerController.hasPickedDrill) return;
        base.OnTriggerExit(other);
    }
}