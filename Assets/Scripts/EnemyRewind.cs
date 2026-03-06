using System.Collections.Generic;
using UnityEngine;

public class EnemyRewind : MonoBehaviour
{
    private struct TimePoint
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    private List<TimePoint> timePoints = new List<TimePoint>();
    private EnemyController enemyController;
    private bool isRewinding = false;
    private float maxRewindTime;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        TimeRewindAbility playerRewind = GameObject.FindWithTag("Player").GetComponent<TimeRewindAbility>();
        if (playerRewind != null) maxRewindTime = playerRewind.GetMaxRewindEnergy();
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
        int maxPoints = Mathf.RoundToInt(maxRewindTime / Time.fixedDeltaTime);
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

    public void StartRewinding()
    {
        isRewinding = true;
        if (enemyController != null) enemyController.enabled = false;
    }

    public void StopRewinding()
    {
        isRewinding = false;
        if (enemyController != null) enemyController.enabled = true;
    }
}