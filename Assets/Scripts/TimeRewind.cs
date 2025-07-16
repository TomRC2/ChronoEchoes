using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


[RequireComponent(typeof(PlayerController))]
public class TimeRewindAbility : MonoBehaviour
{
    [Header("Rewind Settings")]
    [SerializeField] private float maxRewindTime = 5f;
    [SerializeField] public float rewindCostPerSecond = 1f;
    [SerializeField] public float rewindRegenPerSecond = 0.5f;

    private float currentRewindEnergy;

    [Header("Input")]
    [SerializeField] private InputActionAsset playerControls;
    private InputAction rewindAction;

    [Header("Post-Processing Effect (URP)")]
    [SerializeField] private Volume globalVolume;
    private ColorAdjustments colorAdjustments;

    private float originalPostExposure;
    private float originalContrast;
    private Color originalColorFilter;
    private float originalHueShift;
    private float originalSaturation;

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

        currentRewindEnergy = maxRewindTime;

        if (globalVolume != null && globalVolume.profile != null)
        {
            if (!globalVolume.profile.TryGet(out colorAdjustments))
            {
                Debug.LogError("TimeRewindAbility: No se pudo obtener el ColorAdjustments del Volume Profile. Asegúrate de que está añadido al perfil.");
            }

            if (colorAdjustments != null)
            {
                colorAdjustments.postExposure.overrideState = true;
                colorAdjustments.contrast.overrideState = true;
                colorAdjustments.colorFilter.overrideState = true;
                colorAdjustments.hueShift.overrideState = true;
                colorAdjustments.saturation.overrideState = true;

                originalPostExposure = colorAdjustments.postExposure.value;
                originalContrast = colorAdjustments.contrast.value;
                originalColorFilter = colorAdjustments.colorFilter.value;
                originalHueShift = colorAdjustments.hueShift.value;
                originalSaturation = colorAdjustments.saturation.value;
            }
        }
        else
        {
            Debug.LogError("TimeRewindAbility: Global Volume o su Profile no asignados en el Inspector.");
        }
    }

    private void OnEnable()
    {
        rewindAction.performed += StartRewind;
        rewindAction.canceled += StopRewind;
    }

    private void OnDisable()
    {
        rewindAction.performed -= StartRewind;
        rewindAction.canceled -= StopRewind;
    }

    private void Update()
    {
        if (isRewinding)
        {
            currentRewindEnergy -= rewindCostPerSecond * Time.deltaTime;
            currentRewindEnergy = Mathf.Max(currentRewindEnergy, 0);

            if (currentRewindEnergy <= 0 || timePoints.Count == 0)
            {
                isRewinding = false;
                StopRewind(new InputAction.CallbackContext());
                return;
            }

            Rewind();
        }
        else
        {
            currentRewindEnergy += rewindRegenPerSecond * Time.deltaTime;
            currentRewindEnergy = Mathf.Min(currentRewindEnergy, maxRewindTime);

            Record();
        }
    }

    private void Record()
    {
        int maxPoints = Mathf.RoundToInt(maxRewindTime / Time.deltaTime);
        if (timePoints.Count > maxPoints)
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
    }

    private void StartRewind(InputAction.CallbackContext context)
    {
        if (currentRewindEnergy > 0 && timePoints.Count > 0)
        {
            isRewinding = true;
            playerController.enabled = false;
            ApplyRewindEffect();
        }
    }

    private void StopRewind(InputAction.CallbackContext context)
    {
        isRewinding = false;
        playerController.enabled = true;
        RestoreNormalEffect();
    }

    private void ApplyRewindEffect()
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.active = true;

            colorAdjustments.postExposure.overrideState = true;
            colorAdjustments.postExposure.value = -2f;

            colorAdjustments.contrast.overrideState = true;
            colorAdjustments.contrast.value = -50f;

            colorAdjustments.hueShift.overrideState = true;
            colorAdjustments.hueShift.value = 180f;

            colorAdjustments.saturation.overrideState = true;
            colorAdjustments.saturation.value = -100f;

            colorAdjustments.colorFilter.overrideState = true;
            colorAdjustments.colorFilter.value = Color.magenta;
        }
        else
        {
            Debug.LogWarning("ColorAdjustments is null when trying to apply rewind effect. Check globalVolume assignment and profile.");
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

    public float GetCurrentRewindEnergy()
    {
        return currentRewindEnergy;
    }

    public float GetMaxRewindEnergy()
    {
        return maxRewindTime;
    }
}