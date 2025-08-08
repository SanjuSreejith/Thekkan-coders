using UnityEngine;
using Unity.Notifications.Android;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [Header("Notification Settings")]
    public string channelId = "rain_alerts";
    public string channelName = "Rain Alerts";
    public string channelDescription = "Notifies when rain is likely";

    private void Awake()
    {
        // Singleton pattern to keep one instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateNotificationChannel();
    }

    private void CreateNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = channelId,
            Name = channelName,
            Importance = Importance.High,
            Description = channelDescription, // <-- Added comma here
            // Custom sound must be added via res/raw in Unity’s Android plugins folder
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public void SendRainNotification(string message)
    {
        var notification = new AndroidNotification
        {
            Title = "☔ Rain Alert!",
            Text = message,
            FireTime = System.DateTime.Now.AddSeconds(2) // Delay to simulate async weather check
        };

        AndroidNotificationCenter.SendNotification(notification, channelId);
    }
}
