using UnityEngine;
using System.Collections;

public class RainAlertSystem : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(CheckRainAndNotify());
    }

    IEnumerator CheckRainAndNotify()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("Location services not enabled by user.");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0 || Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogWarning("Unable to get location.");
            yield break;
        }

        float latitude = Input.location.lastData.latitude;
        float longitude = Input.location.lastData.longitude;

        Debug.Log($"Location: {latitude}, {longitude}");

        // Fake rain chance (replace later with API)
        bool rainLikely = Random.value > 0.5f;

        if (rainLikely)
        {
            NotificationManager.Instance.SendRainNotification(
                $"Rain is expected soon at your location ({latitude:F2}, {longitude:F2})."
            );
        }
        else
        {
            Debug.Log("No rain expected right now.");
        }

        Input.location.Stop();
    }
}
