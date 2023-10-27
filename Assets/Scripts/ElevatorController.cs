using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private ObjectData objectData;
    [SerializeField] private AudioSettings audioSettings;
    [SerializeField] public RuntimeValues runtimeValues;
    [SerializeField] public ElevatorDestinations elevatorDestinations;

    private readonly DoorPositions doorPositions = new();

    private float alpha;
    private bool animateDoors;

    // private AudioSource audioSource;
    private bool isOpen;
    private Floor lastActiveFloor;
    private AudioClip openDoorAudio, closeDoorAudio, elevatorMoveAudio;

    public Floor ActiveFloor
    {
        get => runtimeValues.activeFloor;
        set => runtimeValues.activeFloor = value;
    }


    private void Start()
    {
        AssignAudio();
    }

    private void Update()
    {
        SetActiveFloorButtonColor();
        AnimateDoors();

        if (Input.GetKeyDown(KeyCode.E))
            if (isOpen) CloseElevatorDoor();
            else OpenElevatorDoor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController pC)) return;
        pC.transform.parent = transform; // set elevator as player parent (for anim)
        OpenElevatorDoor();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController pC)) return;
        pC.transform.parent = null; // remove player parent
        CloseElevatorDoor();
    }


    private void SetActiveFloorButtonColor()
    {
        var activeFloor = runtimeValues.activeFloor;
        if (activeFloor == lastActiveFloor) return;
        lastActiveFloor = activeFloor;

        var selectedButton = objectData.selectedButton;
        var inactiveButton = objectData.inactiveButton;
        objectData.kitchenButton.material = inactiveButton;
        objectData.restaurantButton.material = inactiveButton;
        objectData.engineButton.material = inactiveButton;

        switch (activeFloor)
        {
            case Floor.Engine:
                objectData.engineButton.material = selectedButton;
                break;
            case Floor.Restaurant:
                objectData.restaurantButton.material = selectedButton;
                break;
            case Floor.Kitchen:
                objectData.kitchenButton.material = selectedButton;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // private void PlayAudio(AudioClip clip, bool interrupt = true, float volume = 1, float pitch = 1)
    // {
    //     if (interrupt) audioSource.Stop();
    //     audioSource.pitch = pitch;
    //     audioSource.volume = volume;
    //     audioSource.clip = clip;
    //     audioSource.Play();
    // }

    private void PlayElevatorMoveAudio()
    {
        var setting = audioSettings.elevatorMoveSettings;
        var elevatorMoveSource = setting.Source;

        elevatorMoveSource.pitch = setting.pitch;
        elevatorMoveSource.volume = setting.volume;
        elevatorMoveSource.Play();
    }

    private void PlayDoorAudio(AudioClip clip, bool interrupt = true)
    {
        var setting = audioSettings.doorAudioSettings;
        var doorAudioSource = setting.Source;

        if (interrupt) doorAudioSource.Stop();
        doorAudioSource.pitch = setting.pitch;
        doorAudioSource.volume = setting.volume;
        doorAudioSource.clip = clip;
        doorAudioSource.Play();
    }


    public IEnumerator MoveElevatorToFloor()
    {
        //TODO if door is open (when move) close door

        CloseElevatorDoor();
        if (isOpen) CloseElevatorDoor();
        yield return new WaitForSeconds(runtimeValues.waitForClosingDoors);
        PlayElevatorMoveAudio();
        yield return new WaitForSeconds(.5f);
        MoveElevator();
        yield return new WaitForSeconds(runtimeValues.elevatorRideDuration);
        OpenElevatorDoor();
    }


    private void MoveElevator()
    {
        var destinations = elevatorDestinations.destinations;
        var destination = ActiveFloor switch
        {
            Floor.Engine => destinations[0].position,
            Floor.Restaurant => destinations[1].position,
            Floor.Kitchen => destinations[2].position,
            _ => throw new ArgumentOutOfRangeException()
        };
        transform.position = destination;
    }


    private void AssignAudio()
    {
        // audioSource = GetComponent<AudioSource>();

        openDoorAudio = audioSettings.audioClips[0];
        closeDoorAudio = audioSettings.audioClips[1];
        // elevatorMoveAudio = audioSettings.audioClips[2];

        // audioSource.clip = openDoorAudio;
    }

    public void CloseElevatorDoor()
    {
        //TODO if door is open (when move) close door
        if (isOpen)
            PlayDoorAudio(closeDoorAudio);
        // PlayAudio(closeDoorAudio, true, audioSettings.openCloseVolume);
        alpha = 1;
        animateDoors = true;
        isOpen = true;
    }

    public void OpenElevatorDoor()
    {
        if (!isOpen)
            PlayDoorAudio(openDoorAudio);
        // PlayAudio(openDoorAudio, true, audioSettings.openCloseVolume);
        alpha = 0;
        animateDoors = true;
        isOpen = false;
    }

    private void AnimateDoors()
    {
        if (!animateDoors) return;
        if (isOpen) //if door is open, alpha is 1, so we must subtract to lerp back
            alpha -= Time.deltaTime * runtimeValues.openSpeed;
        else
            alpha += Time.deltaTime * runtimeValues.openSpeed;

        CheckIfLerpIsMax();
        objectData.door1.transform.rotation = Quaternion.Lerp(
            doorPositions.StartPos, doorPositions.Door1EndPos, alpha);
        objectData.door2.transform.rotation = Quaternion.Lerp(
            doorPositions.StartPos, doorPositions.Door2EndPos, alpha);
    }

    /// <summary>
    ///     checks if alpha is min/max to stop animation and set isOpen state;
    /// </summary>
    private void CheckIfLerpIsMax()
    {
        switch (alpha)
        {
            case <= 0:
                animateDoors = false;
                isOpen = false;
                break;
            case >= 1:
                animateDoors = false;
                isOpen = true;
                break;
        }
    }

    [Serializable]
    public class ObjectData
    {
        public MeshRenderer kitchenButton, restaurantButton, engineButton;
        public Material selectedButton, inactiveButton;
        public GameObject door1, door2;
    }

    [Serializable]
    public class AudioSettings
    {
        [SerializeField] public List<AudioClip> audioClips;
        [SerializeField] public AudioSourceSettings elevatorMoveSettings;
        [SerializeField] public AudioSourceSettings doorAudioSettings;
    }


    [Serializable]
    public class RuntimeValues
    {
        [SerializeField] public Floor activeFloor = Floor.Restaurant;
        [SerializeField] public float elevatorRideDuration = 1f;
        [SerializeField] public float waitForClosingDoors = 1f;
        [SerializeField] public float returnToPlayerCooldown = 1f;
        [SerializeField] public float openSpeed = .9f;
    }

    [Serializable]
    public class ElevatorDestinations
    {
        [SerializeField] public List<Transform> destinations;
    }

    private class DoorPositions
    {
        public Quaternion Door1EndPos { get; } = new(-0.61237f, -0.35355f, -0.35355f, 0.61237f);

        public Quaternion Door2EndPos { get; } = new(-0.35355f, -0.61237f, -0.61237f, 0.35355f);

        public Quaternion StartPos { get; } = new(-0.50f, -0.5f, -0.5f, 0.5f);
    }
}

[Serializable]
public class AudioSourceSettings
{
    [SerializeField] public AudioSource Source;
    [SerializeField] public AudioClip audioClip;
    [SerializeField] [Range(0, 1)] public float volume = 1f;
    [SerializeField] [Range(-3, 3)] public float pitch = 1f;
}