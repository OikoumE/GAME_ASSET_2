using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueDisplaySystem : MonoBehaviour
    {
        [SerializeField] private GameObject dialogueTriggersParent;
        public bool updateTriggerList;
        [SerializeField] private DialogueTrigger[] dialogueTriggers;
        private Image dialoguePanel;
        private TMP_Text titleTmp, dialogueTmp;

        private void Start()
        {
            GetUiComponents();
            GetAllTriggers();
            SetAllTriggersDisplaySystem();

            SetDialogueDisplayText("", "");
            SetDialogueDisplayVisibility(false);
        }


        private void OnValidate()
        {
            updateTriggerList = false;
            GetUiComponents();
            GetAllTriggers();
            SetAllTriggersDisplaySystem();
        }

        private void SetAllTriggersDisplaySystem()
        {
            foreach (var dialogueTrigger in dialogueTriggers) dialogueTrigger.DisplaySystem = this;
        }

        private void GetAllTriggers()
        {
            dialogueTriggers = dialogueTriggersParent.GetComponentsInChildren<DialogueTrigger>(true);
        }

        private void GetUiComponents()
        {
            var textComponents = GetComponentsInChildren<TMP_Text>();
            (titleTmp, dialogueTmp) = (textComponents[0], textComponents[1]);
            dialoguePanel = GetComponentInChildren<Image>();
        }

        public void SetDialogueDisplayText(string title, string dialogue)
        {
            titleTmp.text = title;
            dialogueTmp.text = dialogue;
        }

        public void SetDialogueDisplayVisibility(bool enable)
        {
            titleTmp.enabled = enable;
            dialogueTmp.enabled = enable;
            dialoguePanel.enabled = enable;
        }
    }
}