using UnityEngine;

//v. 0.3
public class OpenDoor : MonoBehaviour
{
    public bool canBeOpened;
    public float openSpeed = 10;

    private bool animate;
    private float blendShapeAlpha;
    private bool isOpen;
    private BoxCollider m_collider, triggerBox;
    private SkinnedMeshRenderer m_skinnedMeshRenderer;

    private bool playerInRange;

    private void Start()
    {
        m_skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        var boxes = gameObject.GetComponents<BoxCollider>();
        foreach (var box in boxes)
            if (!box.isTrigger)
                m_collider = box;
    }

    private void Update()
    {
        if (!animate || !canBeOpened) return;
        if (!isOpen) // 0 == closed
        {
            blendShapeAlpha += Time.deltaTime * openSpeed;
            if (blendShapeAlpha >= 100)
            {
                isOpen = true;
                animate = false;
                m_collider.enabled = false;
            }
        }
        else if (isOpen) // 100 == open
        {
            blendShapeAlpha -= Time.deltaTime * openSpeed;
            if (blendShapeAlpha <= 0)
            {
                isOpen = false;
                animate = false;
                m_collider.enabled = true;
            }
        }

        m_skinnedMeshRenderer.SetBlendShapeWeight(0, blendShapeAlpha);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IPlayer iP))
        {
            if (animate) isOpen = !isOpen;
            animate = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IPlayer iP))
        {
            if (animate)
                isOpen = true;
            else
                animate = true;
            m_collider.enabled = true;
        }
    }
}