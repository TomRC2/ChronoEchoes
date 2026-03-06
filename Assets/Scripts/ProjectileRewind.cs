using System.Collections.Generic;
using UnityEngine;

public class ProjectileRewind : MonoBehaviour
{
    private struct TimePoint
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    private List<TimePoint> timePoints = new List<TimePoint>();
    private bool isRewinding = false;
    private float maxRewindTime = 5f;

    private MonoBehaviour bulletScript;

    private void Awake()
    {
        bulletScript = (MonoBehaviour)GetComponent("Projectile") ?? (MonoBehaviour)GetComponent("EnemyBullet");

        TimeRewindAbility player = GameObject.FindWithTag("Player")?.GetComponent<TimeRewindAbility>();
        if (player != null) maxRewindTime = player.GetMaxRewindEnergy();
    }

    private void FixedUpdate()
    {
        if (isRewinding)
            Rewind();
        else
            Record();
    }

    private void Record()
    {
        timePoints.Insert(0, new TimePoint { position = transform.position, rotation = transform.rotation });

        int maxPoints = Mathf.RoundToInt(maxRewindTime / Time.fixedDeltaTime);
        if (timePoints.Count > maxPoints)
        {
            timePoints.RemoveAt(timePoints.Count - 1);
        }
    }

    private void Rewind()
    {
        if (timePoints.Count > 0)
        {
            transform.position = timePoints[0].position;
            transform.rotation = timePoints[0].rotation;
            timePoints.RemoveAt(0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartRewinding()
    {
        isRewinding = true;
        if (bulletScript != null) bulletScript.enabled = false;
    }

    public void StopRewinding()
    {
        isRewinding = false;
        if (bulletScript != null) bulletScript.enabled = true;
    }
}