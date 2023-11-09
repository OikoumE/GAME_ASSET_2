using System;
using Controllers;
using TMPro;
using UnityEngine;

public abstract class DialogueController : MonoBehaviour
{
    [SerializeField] protected bool isOneShot;
    [SerializeField] protected UiElements uiElements = new();
    [SerializeField] protected DialogueText dialogueText = new();
    protected bool hasShot;

    protected PlayerController playerController;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (hasShot && isOneShot) return;
        if (!other.TryGetComponent(out IPlayer iP)) return;
        playerController = iP.GetPlayerController();
        SetDialogueText();
        SetDialogueVisibility(true);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out IPlayer iP)) return;
        SetDialogueVisibility(false);
        hasShot = true;
    }

    public virtual void SetDialogueText()
    {
        uiElements.titleTmp.text = dialogueText.title;
        uiElements.dialogueTmp.text = dialogueText.dialogue;
    }

    public void SetDialogueVisibility(bool show)
    {
        uiElements.dialogueCanvas.gameObject.SetActive(show);
    }

    [Serializable]
    public class UiElements
    {
        [SerializeField] public TMP_Text titleTmp, dialogueTmp;
        [SerializeField] public Canvas dialogueCanvas;
    }


    [Serializable]
    public class DialogueText
    {
        [SerializeField] public string title;
        [SerializeField] [TextArea] public string dialogue;
    }
}