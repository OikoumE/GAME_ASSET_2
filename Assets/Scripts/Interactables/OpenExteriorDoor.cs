using Controllers;
using StateMachine;
using UnityEngine;

namespace Interactables
{
    public class OpenExteriorDoor : OpenDoor
    {
        protected override void OnTriggerEnter(Collider other)
        {
            state = GameStateMachine.Instance.currentStateName;
            canBeOpened = state != GameStateName.WireGameState;
            if (!other.TryGetComponent(out IPlayer iP)) return;
            if (canBeOpened)
                OpenAnimation();
        }

        protected override void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IPlayer iP)) return;
            if (canBeOpened)
                CloseAnimation();
        }
    }
}