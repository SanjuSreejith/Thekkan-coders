// Create a new script called MenuScoreDisplay.cs
using TMPro;
using UnityEngine;

public class MenuScoreDisplay : MonoBehaviour
{
    public TMP_Text scoreText;

    void Start()
    {
        // Load the saved score
        int savedScore = PlayerPrefs.GetInt("PlayerScore", 0);

        // Display it
        scoreText.text = "Your Score: " + savedScore;
    }

    // Optional: Add a method to reset the score
    public void ResetScore()
    {
        PlayerPrefs.SetInt("PlayerScore", 0);
        scoreText.text = "Your Score: 0";
    }
}