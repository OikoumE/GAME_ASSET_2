using Controllers;
using UnityEngine;

//v. 0.3
public class OpenDoor : MonoBehaviour
{
    public enum DoorPosition
    {
        middle,
        left,
        right
    }

    public DoorPosition doorPosition;
    public bool canBeOpened;
    public float openSpeed = 10;
    [SerializeField] [Range(0, 100)] private float allowThroughDoorThreshold;

    private float blendShapeAlpha;
    private bool isOpen;
    private BoxCollider m_collider, triggerBox;
    private SkinnedMeshRenderer m_skinnedMeshRenderer;

    private bool playerInRange;

    public float AllowThroughDoorThreshold
    {
        set => allowThroughDoorThreshold = value;
    }

    public float OpenSpeed
    {
        get => openSpeed;
        set => openSpeed = value;
    }

    private bool Animate { get; set; }

    private void Start()
    {
        m_skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        SetColliders();
    }

    private void Update()
    {
        if (!Animate || !canBeOpened) return;
        if (!isOpen) // 0 == closed
        {
            blendShapeAlpha += Time.deltaTime * openSpeed;
            if (blendShapeAlpha >= allowThroughDoorThreshold)
                m_collider.enabled = false;
            if (blendShapeAlpha >= 100)
            {
                isOpen = true;
                Animate = false;
            }
        }
        else if (isOpen) // 100 == open
        {
            blendShapeAlpha -= Time.deltaTime * openSpeed;
            if (blendShapeAlpha <= 0)
            {
                isOpen = false;
                Animate = false;
            }

            if (blendShapeAlpha <= allowThroughDoorThreshold)
                m_collider.enabled = true;
        }

        SetBlendShape(blendShapeAlpha);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IPlayer iP)) OpenAnimation();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IPlayer iP)) CloseAnimation();
    }

    public void OpenAnimation()
    {
        if (Animate) isOpen = !isOpen;
        Animate = true;
    }

    public void CloseAnimation()
    {
        if (Animate)
            isOpen = true;
        else
            Animate = true;
        m_collider.enabled = true;
    }


    private void SetColliders()
    {
        var boxes = gameObject.GetComponents<BoxCollider>();
        foreach (var box in boxes)
            if (box.isTrigger)
                triggerBox = box;
            else m_collider = box;
    }

    public void SetTriggerBoxActive(bool enable)
    {
        if (triggerBox == null) SetColliders();
        triggerBox.enabled = enable;
    }

    public void SetBlendShape(float value)
    {
        m_skinnedMeshRenderer.SetBlendShapeWeight(0, value);
    }


    public float GetAlpha()
    {
        return blendShapeAlpha;
    }
}