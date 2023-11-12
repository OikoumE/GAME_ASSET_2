using System;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

namespace Elevator
{
    public class ElevatorPreventOutsideInteraction : MonoBehaviour
    {
        [SerializeField] private List<ElevatorButton> elevatorButtons;

        private void Start()
        {
            if (elevatorButtons.Count == 0)
                throw new Exception("NO elevatorButtons SET!!");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController pC)) return;
            SetElevatorButtonEnabled(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController pC)) return;
            SetElevatorButtonEnabled(false);
        }

        private void SetElevatorButtonEnabled(bool enable)
        {
            foreach (var elevatorButton in elevatorButtons) elevatorButton.isPlayerInsideElevator = enable;
        }
    }
}