using System.Collections;
using Controllers;
using Elevator;
using UnityEngine;

namespace Interactables
{
    public class ReplaceFuseInteract : Interactable
    {
        [SerializeField] private AudioSourceSettings audioSourceSettings;
        private BoxCollider boxCollider;
        private FuseAnimator brokenFuse, insertFuse;


        private FuseboxController fuseboxController;
        private bool hasRemovedFuse, hasReplacedFuse;

        public bool CompletelyDone { get; private set; }

        private void Start()
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        protected override void Update()
        {
            if (CompletelyDone) return;
            if (brokenFuse && brokenFuse.Done)
                hasRemovedFuse = true;
            if (!insertFuse || !insertFuse.Done) return;
            StartCoroutine(CloseFuseBoxDoor());
            CompletelyDone = true;
        }

        private IEnumerator CloseFuseBoxDoor()
        {
            yield return new WaitForSeconds(1);
            fuseboxController.animateDoor = true;
            yield return new WaitForSeconds(1.5f);
            var pC = fuseboxController.GetPlayerController;
            fuseboxController.Interact(pC, audioSourceSettings);
        }

        public override void Interact(FuseboxController fC)
        {
            fuseboxController = fC;
            if (!brokenFuse) brokenFuse = fuseboxController.FuseToRemove.GetComponent<FuseAnimator>();
            if (!insertFuse) insertFuse = fuseboxController.FuseToInsert.GetComponent<FuseAnimator>();
            if (!hasRemovedFuse && !hasReplacedFuse)
            {
                brokenFuse.StartAnimation();
                return;
            }

            if (hasReplacedFuse || !hasRemovedFuse) return;
            hasReplacedFuse = true;
            fuseboxController.FuseToInsert.gameObject.SetActive(true);
            insertFuse.StartAnimation();
            boxCollider.enabled = false;
            isInteractable = false;
        }
    }
}