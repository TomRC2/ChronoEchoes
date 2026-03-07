using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Video;

[RequireComponent(typeof(PlayerController))]
public class TimeRewindAbility : MonoBehaviour
{
    [Header("Rewind Settings")]
    [SerializeField] private float maxRewindTime = 5f;
    [SerializeField] public float rewindCostPerSecond = 1f;
    [SerializeField] public float rewindRegenPerSecond = 0.5f;

    [Header("Video Swap Settings")]
    [SerializeField] private VideoClip normalVideo;
    [SerializeField] private VideoClip reverseVideo;
    [SerializeField] private VideoPlayer videoBackground;

    private float currentRewindEnergy;
    public UnityEvent<float, float> OnRewindEnergyChanged;

    [Header("Input")]
    [SerializeField] private InputActionAsset playerControls;
    private InputAction rewindAction;

    [Header("World Rewind Settings")]
    [SerializeField] private InputAction worldRewindAction;
    private bool isWorldRewinding = false;

    [Header("Post-Processing Effect (URP)")]
    [SerializeField] private Volume globalVolume;
    private ColorAdjustments colorAdjustments;

    private float originalPostExposure, originalContrast, originalHueShift, originalSaturation;
    private Color originalColorFilter;
    [SerializeField] private ParticleSystem rewindParticles;
    private struct TimePoint { public Vector3 position; public Quaternion rotation; }
    private List<TimePoint> timePoints = new List<TimePoint>();
    private bool isRewinding = false;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rewindAction = playerControls.FindActionMap("Player").FindAction("Rewind");
        currentRewindEnergy = maxRewindTime;

        if (globalVolume != null && globalVolume.profile.TryGet(out colorAdjustments))
        {
            originalPostExposure = colorAdjustments.postExposure.value;
            originalContrast = colorAdjustments.contrast.value;
            originalColorFilter = colorAdjustments.colorFilter.value;
            originalHueShift = colorAdjustments.hueShift.value;
            originalSaturation = colorAdjustments.saturation.value;
        }

        if (OnRewindEnergyChanged == null) OnRewindEnergyChanged = new UnityEvent<float, float>();
    }

    private void OnEnable()
    {
        rewindAction.performed += StartRewind; rewindAction.canceled += StopRewind;
        worldRewindAction.Enable();
        worldRewindAction.performed += StartWorldRewind;
        worldRewindAction.canceled += StopWorldRewind;
    }
    private void OnDisable() { rewindAction.performed -= StartRewind; rewindAction.canceled -= StopRewind; }

    private void Update()
    {
        if (isRewinding || isWorldRewinding)
        {
            currentRewindEnergy -= rewindCostPerSecond * Time.deltaTime;
            currentRewindEnergy = Mathf.Max(currentRewindEnergy, 0);

            if (currentRewindEnergy <= 0)
            {
                if (isRewinding) StopRewind(new InputAction.CallbackContext());
                if (isWorldRewinding) StopWorldRewind(new InputAction.CallbackContext());
                return;
            }

            if (isRewinding) Rewind();
        }
        else
        {
            currentRewindEnergy += rewindRegenPerSecond * Time.deltaTime;
            currentRewindEnergy = Mathf.Min(currentRewindEnergy, maxRewindTime);
            Record();
        }
        OnRewindEnergyChanged.Invoke(currentRewindEnergy, maxRewindTime);
    }
    private void StartWorldRewind(InputAction.CallbackContext context)
    {
        if (currentRewindEnergy > 0 && !isRewinding)
        {
            isWorldRewinding = true;

            ObjectRewind[] objects = Object.FindObjectsByType<ObjectRewind>(FindObjectsSortMode.None);
            foreach (var obj in objects) obj.StartRewinding();

            ProjectileRewind[] bullets = Object.FindObjectsByType<ProjectileRewind>(FindObjectsSortMode.None);
            foreach (var bullet in bullets) bullet.StartRewinding();

            ApplyRewindEffect();
        }
    }
    private void StopWorldRewind(InputAction.CallbackContext context)
    {
        isWorldRewinding = false;

        ObjectRewind[] objects = Object.FindObjectsByType<ObjectRewind>(FindObjectsSortMode.None);
        foreach (var obj in objects) obj.StopRewinding();

        ProjectileRewind[] bullets = Object.FindObjectsByType<ProjectileRewind>(FindObjectsSortMode.None);
        foreach (var bullet in bullets) bullet.StopRewinding();

        RestoreNormalEffect();
    }
    private void Record()
    {
        int maxPoints = Mathf.RoundToInt(maxRewindTime / Time.deltaTime);
        if (timePoints.Count > maxPoints) timePoints.RemoveAt(timePoints.Count - 1);
        timePoints.Insert(0, new TimePoint { position = transform.position, rotation = transform.rotation });
    }

    private void Rewind()
    {
        if (timePoints.Count > 0)
        {
            transform.position = timePoints[0].position;
            transform.rotation = timePoints[0].rotation;
            timePoints.RemoveAt(0);
        }
    }

    private void StartRewind(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (currentRewindEnergy > 0 && timePoints.Count > 0)
        {
            isRewinding = true;
            playerController.enabled = false;

            if (rewindParticles != null) rewindParticles.Play();

            if (videoBackground != null && reverseVideo != null)
            {
                videoBackground.clip = reverseVideo;
                videoBackground.Play();
            }

            ObjectRewind[] enemies = Object.FindObjectsByType<ObjectRewind>(FindObjectsSortMode.None);
            foreach (ObjectRewind enemy in enemies) enemy.StartRewinding();

            ProjectileRewind[] bullets = Object.FindObjectsByType<ProjectileRewind>(FindObjectsSortMode.None);
            foreach (ProjectileRewind bullet in bullets) bullet.StartRewinding();

            ApplyRewindEffect();
        }
    }

    private void StopRewind(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        isRewinding = false;
        playerController.enabled = true;

        if (rewindParticles != null) rewindParticles.Stop();

        if (videoBackground != null && normalVideo != null)
        {
            videoBackground.clip = normalVideo;
            videoBackground.Play();
        }

        ObjectRewind[] enemies = Object.FindObjectsByType<ObjectRewind>(FindObjectsSortMode.None);
        foreach (ObjectRewind enemy in enemies) enemy.StopRewinding();

        ProjectileRewind[] bullets = Object.FindObjectsByType<ProjectileRewind>(FindObjectsSortMode.None);
        foreach (ProjectileRewind bullet in bullets) bullet.StopRewinding();

        RestoreNormalEffect();
    }

    private void ApplyRewindEffect()
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.active = true;
            colorAdjustments.postExposure.value = -2f;
            colorAdjustments.contrast.value = -50f;
            colorAdjustments.hueShift.value = 180f;
            colorAdjustments.saturation.value = -100f;
            colorAdjustments.colorFilter.value = Color.magenta;
        }
    }

    private void RestoreNormalEffect()
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = originalPostExposure;
            colorAdjustments.contrast.value = originalContrast;
            colorAdjustments.hueShift.value = originalHueShift;
            colorAdjustments.saturation.value = originalSaturation;
            colorAdjustments.colorFilter.value = originalColorFilter;
        }
    }

    public void RestoreFullEnergy() { currentRewindEnergy = maxRewindTime; OnRewindEnergyChanged.Invoke(currentRewindEnergy, maxRewindTime); }
    public float GetCurrentRewindEnergy() { return currentRewindEnergy; }
    public float GetMaxRewindEnergy() { return maxRewindTime; }

    public void ClearTimePoints()
    {
        if (timePoints != null)
        {
            timePoints.Clear();
            Debug.Log("Historial temporal reiniciado para evitar bucles de muerte.");
        }
    }
}