using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeatherChecker : MonoBehaviour
{
    // The Api is removed due to security Reasons
    private string apiKey = "";

    public IEnumerator CheckWeather(float lat, float lon)
    {
        string url = $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid={apiKey}&units=metric";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;

            // Quick & simple check for rain in forecast
            if (json.ToLower().Contains("rain"))
            {
                Debug.Log("🌧 Rain is likely soon!");
                NotificationManager.Instance.SendRainNotification(
                    $"Rain is expected soon at your location ({lat:F2}, {lon:F2})."
                );
            }
            else
            {
                Debug.Log("☀ No rain expected in the near future.");
            }
        }
        else
        {
            Debug.LogError($"Weather API Error: {www.error}");
        }
    }
}
