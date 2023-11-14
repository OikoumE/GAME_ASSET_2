using Controllers;
using StateMachine;
using UnityEngine;

namespace Dialogue
{
    public class DialogueTriggerReturnToShuttle : DialogueTrigger
    {
        protected override void OnTriggerStay(Collider other)
        {
            var state = GameStateMachine.Instance.currentStateName;

            if (!other.TryGetComponent(out IPlayer iP)) return;
            playerController = iP.GetPlayerController();

            if (state != GameStateName.WireGameState) return;
            base.OnTriggerStay(other);
        }
    }
}