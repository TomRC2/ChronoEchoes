using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class TimeRewindAbility : MonoBehaviour
{
    [Header("Rewind Settings")]
    [SerializeField] private float maxRewindTime = 5f;

    [Header("Input")]
    [SerializeField] private InputActionAsset playerControls;
    private InputAction rewindAction;

    private struct TimePoint
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    private List<TimePoint> timePoints = new List<TimePoint>();
    private bool isRewinding = false;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rewindAction = playerControls.FindActionMap("Player").FindAction("Rewind");
    }
    private void Update()
    {
        if (isRewinding)
        {
            Rewind();
        }
        else
        {
            Record();
        }
    }

    private void Record()
    {
        if (timePoints.Count > Mathf.Round(maxRewindTime / Time.deltaTime))
        {
            timePoints.RemoveAt(timePoints.Count - 1);
        }
        timePoints.Insert(0, new TimePoint { position = transform.position, rotation = transform.rotation });
    }

    private void Rewind()
    {
        if (timePoints.Count > 0)
        {
            TimePoint timePoint = timePoints[0];
            transform.position = timePoint.position;
            transform.rotation = timePoint.rotation;
            timePoints.RemoveAt(0);
        }
        else
        {
            isRewinding = false;
        }
    }

    private void StartRewind(InputAction.CallbackContext context)
    {
        isRewinding = true;
        playerController.enabled = false;
    }

    private void StopRewind(InputAction.CallbackContext context)
    {
        isRewinding = false;
        playerController.enabled = true;
    }
}