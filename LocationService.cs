using UnityEngine;
using System.Collections;

public class LocationServiceExample : MonoBehaviour
{
    public static LocationServiceExample Instance;
    public float latitude, longitude;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

   public IEnumerator Start()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location not enabled");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            Debug.Log($"Location: {latitude}, {longitude}");
        }

        Input.location.Stop();
    }
}
