using System;
using System.Collections.Generic;
using Elevator;
using StateMachine;
using UnityEngine;

namespace Controllers
{
    public class LightController : MonoBehaviour
    {
        [SerializeField] private GameObject kitchenLightsGo, restaurantLightsGo, dockingLightsGo, engineLightsGo;
        private Light[] dockingLights;

        private void Start()
        {
            dockingLights = dockingLightsGo.GetComponentsInChildren<Light>();
        }

        private void Update()
        {
        }

        public static void SetLightsArrayEnabled(IEnumerable<Light> lights, bool enable)
        {
            foreach (var mLight in lights) mLight.enabled = enable;
        }

        public void SetAllLightsEnabled(bool enable)
        {
            engineLightsGo.SetActive(enable);
            kitchenLightsGo.SetActive(enable);
            restaurantLightsGo.SetActive(enable);
            SetLightsArrayEnabled(dockingLights, enable);
        }

        public void SetFloorLightsActive(Floor floor)
        {
            if (GameStateMachine.Instance.currentStateName == GameStateName.WireGameState) return;
            switch (floor)
            {
                case Floor.Engine:
                    engineLightsGo.SetActive(true);
                    kitchenLightsGo.SetActive(false);
                    restaurantLightsGo.SetActive(false);
                    SetLightsArrayEnabled(dockingLights, false);
                    break;
                case Floor.Restaurant:
                    engineLightsGo.SetActive(false);
                    kitchenLightsGo.SetActive(false);
                    restaurantLightsGo.SetActive(true);
                    SetLightsArrayEnabled(dockingLights, true);
                    break;
                case Floor.Kitchen:
                    engineLightsGo.SetActive(false);
                    kitchenLightsGo.SetActive(true);
                    restaurantLightsGo.SetActive(false);
                    SetLightsArrayEnabled(dockingLights, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}