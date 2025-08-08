using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class MultiWoodManager : MonoBehaviour
{
    [System.Serializable]
    public class WoodSlot
    {
        public string woodName;
        public GameObject woodObject;
        public ParticleSystem smokeParticle;

        [HideInInspector] public Vector3 originalPosition;
        [HideInInspector] public bool isSmoking = false;
        [HideInInspector] public bool isPresent = true;
        [HideInInspector] public float timer = 0f;
        [HideInInspector] public float targetTime = 0f;
        [HideInInspector] public State currentState = State.WaitingToSmoke;
    }

    public enum State { WaitingToSmoke, Smoking, WaitingToReplace }

    [Header("Game Settings")]
    public WoodSlot[] woods;
    public float minSmokeDelay = 15f;
    public float maxSmokeDelay = 35f;
    public float maxSmokeDuration = 40f;
    public float maxReplaceTime = 30f;
    public int minWoodsToSurvive = 5;

    [Header("UI")]
    public TextMeshProUGUI taskText;
    public TextMeshProUGUI alertText;
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    private Queue<WoodSlot> smokingQueue = new Queue<WoodSlot>();
    private int score = 0;
    private bool isGameOver = false;

    void Start()
    {
        gameOverPanel.SetActive(false);

        foreach (var wood in woods)
        {
            wood.originalPosition = wood.woodObject.transform.position;
            StartWaitingToSmoke(wood);
        }

        UpdateTaskText();
        UpdateScoreUI();
    }

    void Update()
    {
        if (isGameOver) return;

        foreach (var wood in woods)
        {
            wood.timer += Time.deltaTime;

            switch (wood.currentState)
            {
                case State.WaitingToSmoke:
                    if (wood.timer >= wood.targetTime)
                        StartSmoking(wood);
                    break;

                case State.Smoking:
                    if (wood.timer >= maxSmokeDuration)
                        TriggerGameOver();
                    break;

                case State.WaitingToReplace:
                    if (wood.timer >= maxReplaceTime)
                        TriggerGameOver();
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveAnySmokingWood();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddNewWood();
        }
    }

    // ---------- STATE CONTROLS ----------
    void StartWaitingToSmoke(WoodSlot wood)
    {
        wood.currentState = State.WaitingToSmoke;
        wood.timer = 0f;
        wood.targetTime = Random.Range(minSmokeDelay, maxSmokeDelay);
    }

    void StartSmoking(WoodSlot wood)
    {
        wood.currentState = State.Smoking;
        wood.isSmoking = true;
        wood.smokeParticle.Play();
        wood.timer = 0f;

        if (!smokingQueue.Contains(wood))
            smokingQueue.Enqueue(wood);

        UpdateTaskText();
    }

    public void RemoveSmokingWood(WoodSlot wood)
    {
        if (!wood.isSmoking) return;

        wood.isSmoking = false;
        wood.smokeParticle.Stop();
        wood.woodObject.SetActive(false); // Hide it
        wood.isPresent = false;

        smokingQueue = new Queue<WoodSlot>(smokingQueue.Where(w => w != wood));

        score++;
        UpdateScoreUI();
        CheckWoodCount();

        wood.currentState = State.WaitingToReplace;
        wood.timer = 0f;

        UpdateTaskText();
    }

    public void ReplaceWood(WoodSlot wood)
    {
        if (wood.isPresent) return;

        wood.woodObject.transform.position = wood.originalPosition;
        wood.woodObject.SetActive(true); // Show again
        wood.isPresent = true;

        StartWaitingToSmoke(wood);
        StartCoroutine(ShowNewWoodAlert(wood.woodName));
    }

    public void RemoveAnySmokingWood()
    {
        foreach (var wood in woods)
        {
            if (wood.isSmoking)
            {
                RemoveSmokingWood(wood);
                break;
            }
        }
    }

    public void AddNewWood()
    {
        foreach (var wood in woods)
        {
            if (!wood.isPresent)
            {
                ReplaceWood(wood);
                return;
            }
        }
    }

    void CheckWoodCount()
    {
        int count = woods.Count(w => w.isPresent);
        if (count < minWoodsToSurvive)
        {
            TriggerGameOver();
        }
    }

    void UpdateTaskText()
    {
        if (smokingQueue.Count > 0)
        {
            taskText.text = "⚠ " + smokingQueue.Peek().woodName + " is smoking! Remove it!";
        }
        else
        {
            taskText.text = "✅ All woods are safe.";
        }
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }
    private void SaveScore()
    {
        // 1. Get the previously saved score (default to 0 if none exists)
        int previousScore = PlayerPrefs.GetInt("PlayerScore", 0);

        // 2. Add the current score to the previous one
        int newTotalScore = previousScore + score;

        // 3. Save the accumulated score
        PlayerPrefs.SetInt("PlayerScore", newTotalScore);
        PlayerPrefs.Save();

        Debug.Log($"Saved score: {score} added to previous {previousScore} = {newTotalScore}");
    }
    System.Collections.IEnumerator ShowNewWoodAlert(string woodName)
    {
        alertText.gameObject.SetActive(true);
        alertText.text = "🪵 New wood added: " + woodName;
        yield return new WaitForSeconds(1f);
        alertText.gameObject.SetActive(false);
        UpdateTaskText();
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        finalScoreText.text = "Final Score: " + score;
        SaveScore(); // Save the score when game is over
        gameOverPanel.SetActive(true);
    }
}
