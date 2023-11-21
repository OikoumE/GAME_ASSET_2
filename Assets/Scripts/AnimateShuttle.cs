using UnityEngine;

public class AnimateShuttle : MonoBehaviour
{
    [SerializeField] private Vector3 startPos, stopPos = new(-7.09000015f, 2.004555f, 74.69292f);
    [SerializeField] private float lerpSpeed = 1;
    [SerializeField] private bool doLerp, isDone;
    [SerializeField] private AudioSource audioSource;

    private Vector3 fromPos, toPos;
    private float lerpAlpha;

    private void Start()
    {
        startPos = transform.position;
        fromPos = startPos;
        toPos = stopPos;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!doLerp) return;
        if (isDone) return;
        lerpAlpha += Time.deltaTime * lerpSpeed;
        transform.position = Vector3.Lerp(fromPos, toPos, lerpAlpha);
        if (lerpAlpha <= 1) return;
        isDone = true;
        doLerp = false;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(startPos, 1f);
        Gizmos.DrawWireSphere(stopPos, 1f);
    }
#endif

    public void Play()
    {
        doLerp = true;
        audioSource.Play();
    }
}