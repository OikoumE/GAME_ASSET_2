using UnityEngine;

public class FuseboxController : MonoBehaviour
{
    //Lets piss off leif with shitty code

    public GameObject fuseboxLid;
    public GameObject fuseToInsert;
    public float rot;
    public float maxRotation = 90;
    public bool animate;
    private bool isFuseboxOpen;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isFuseboxOpen == false) animate = !animate;
        if (animate) OpenFusebox();

        fuseboxLid.transform.rotation = Quaternion.Euler(0, rot, 0);
    }

    private void OpenFusebox()
    {
        // Open the fusebox lid 

        if (rot >= -maxRotation)
            rot -= 1;
        else if (rot <= -maxRotation)
            isFuseboxOpen = true;
    }

    private void InsertFuse()
    {
        // Check if there is a fuse to insert.
        if (fuseToInsert != null)
            // Insert the fuse
            fuseToInsert.SetActive(true);
    }

    private void CloseFusebox()
    {
        // Close the fusebox
        fuseboxLid.SetActive(false);
        isFuseboxOpen = false;
    }
}