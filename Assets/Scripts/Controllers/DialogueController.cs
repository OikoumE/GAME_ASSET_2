using System;
using Dialogue;
using StateMachine;
using Unity.VisualScripting;
using UnityEditor;
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

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Handles.Label(transform.position, gameObject.name);
        }
#endif

        protected virtual void OnTriggerExit(Collider other)
        {
            if (requireSpecificGameState)
                if (!GameStateMachine.Instance.IsCurrentState(gameStateName))
                    return;
            if (!other.TryGetComponent(out IPlayer iP)) return;
            SetDialogueVisibility(false);
            if (isOneShot)
                mCollider.enabled = false;
        }


        protected virtual void OnTriggerStay(Collider other)
        {
            if (requireSpecificGameState)
                if (!GameStateMachine.Instance.IsCurrentState(gameStateName))
                    return;
            if (!other.TryGetComponent(out IPlayer iP)) return;
            playerController = iP.GetPlayerController();
            SetDialogueText();
            SetDialogueVisibility(true);
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