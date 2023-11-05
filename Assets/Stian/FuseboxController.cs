using Controllers;
using Interactables;
using UnityEngine;

public class FuseboxController : Interactable
{
    //Lets piss off leif with shitty code

    public GameObject fuseboxLid;
    public GameObject fuseToInsert;
    public float rot;
    public float maxRotation = 90, startRotation = 20;
    public bool animate;
    private bool isFuseboxOpen;
    public override void Interact(PlayerController playerController)
    {
        Debug.Log("interacted");
        animate = !animate;

    }
    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.E)) animate = !animate;
        if (animate)
        {
            OpenFusebox();
            CloseFusebox();
        }


        fuseboxLid.transform.localRotation = Quaternion.Euler(0, 0, rot);
    }
    private void Start()
    {
        rot = startRotation;
    }
    private void OpenFusebox()
    {
        // Open the fusebox lid 
        if (isFuseboxOpen) return;
        if (rot >= -maxRotation)
            rot -= 1;
        else if (rot <= -maxRotation)
        { isFuseboxOpen = true; animate = false; }
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
        if (!isFuseboxOpen) return;
        if (rot <= 0)
            rot += 1;
        else if (rot >= 0)
        { isFuseboxOpen = false; animate = false; }

    }
}