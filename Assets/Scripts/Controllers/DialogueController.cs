using System;
using Dialogue;
using StateMachine;
using Unity.VisualScripting;
using UnityEngine;

namespace Controllers
{
    [RequireComponent(typeof(Collider))]
    public abstract class DialogueController : MonoBehaviour
    {
        [SerializeField] private bool requireSpecificGameState;
        [SerializeField] private GameStateName gameStateName;
        [SerializeField] protected bool enabledPreview;
        [SerializeField] private bool isOneShot;
        [SerializeField] protected DialogueText dialogueText = new();
        protected Collider mCollider;
        protected PlayerController playerController;
        [DoNotSerialize] public DialogueDisplaySystem DisplaySystem { get; set; }

        protected virtual void Start()
        {
            mCollider = GetComponent<Collider>();
            mCollider.isTrigger = true;
        }


        protected virtual void OnTriggerEnter(Collider other)
        {
            if (requireSpecificGameState)
                if (gameStateName != GameStateMachine.Instance.currentStateName)
                    return;
            if (!other.TryGetComponent(out IPlayer iP)) return;
            playerController = iP.GetPlayerController();
            SetDialogueText();
            SetDialogueVisibility(true);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (requireSpecificGameState)
                if (gameStateName != GameStateMachine.Instance.currentStateName)
                    return;
            if (!other.TryGetComponent(out IPlayer iP)) return;
            SetDialogueVisibility(false);
            if (isOneShot)
                mCollider.enabled = false;
        }


        private void OnValidate()
        {
            if (!DisplaySystem) return;

            if (enabledPreview) SetDialogueText();
            SetDialogueVisibility(enabledPreview);
        }


        protected virtual void SetDialogueText()
        {
            DisplaySystem.SetDialogueDisplayText(dialogueText.title, dialogueText.dialogue);
        }

        protected virtual void SetDialogueVisibility(bool show)
        {
            DisplaySystem.SetDialogueDisplayVisibility(show);
        }


        [Serializable]
        public class DialogueText
        {
            [SerializeField] public string title;
            [SerializeField] [TextArea] public string dialogue;
        }
    }
}