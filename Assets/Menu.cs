using System.Collections;
using UnityEditor;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [Tooltip("Which key to press to open game menu")]
    public KeyCode openMenuKey = KeyCode.Escape;

    public GameObject buttonContianer;
    private bool isOpen = true;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (isOpen) return;
        if (Input.GetKeyDown(openMenuKey))
        {
            buttonContianer.SetActive(true);
            LockCursor(isOpen);
        }
    }

    public void LockCursor(bool lockCursor)
    {
        isOpen = !lockCursor;
        if (lockCursor) SetCursorLockMode(CursorLockMode.Locked);
        else SetCursorLockMode(CursorLockMode.None);
    }

    public void SetCursorLockMode(CursorLockMode state)
    {
        Cursor.lockState = state;
        if (state == CursorLockMode.Locked) Cursor.visible = false;
        else Cursor.visible = true;
    }

    public void ExitGame()
    {
        StartCoroutine(TriggerExitGameDelayed());
    }

    private static IEnumerator TriggerExitGameDelayed()
    {
        Debug.Log("EXIT GAME");
        yield return new WaitForSecondsRealtime(1);
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}