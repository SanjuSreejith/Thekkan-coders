using System.Collections;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ElephantRiddleGame : MonoBehaviour
{
    
    [System.Serializable]
    public class Riddle
    {
        public string question_en;
        public string answer;
        public float timeLimit = 30f;
    }

    public Riddle[] riddles;
    public TMP_Text riddleText;
    public TMP_InputField answerInput;
    public TMP_Text feedbackText;
    public TMP_Text pointsText;
    public TMP_Text timerText;
    public TMP_Text elephantSoundText;
    public Button submitButton;
    public Button nextButton;

    public AudioSource audioSource;
    public AudioClip askClip;
    public AudioClip correctClip;
    public AudioClip wrongClip;
    public AudioClip timeUpClip;

    public GameObject winPanel;
    public Button mainMenuButton;

    private int currentRiddleIndex = 0;
    private int points = 0;
    private bool gameEnded = false;
    private float timeRemaining;
    private bool timerIsRunning = false;
    private Coroutine timerCoroutine;

    // Mocking lines for wrong answers
    private string[] mockingLines = new string[]
    {
        " Elephant says: My cousin answered faster than that… and he's a tortoise!",
        " Elephant says: That answer was so wrong, my trunk curled in embarrassment.",
        " Elephant says: If that's your best guess, I'm packing my bags!",
        " Elephant says: I'd clap, but I'm too busy laughing.",
        " Elephant says: Are you even trying?",
        " Elephant says: Wow… just wow… that was creative… and wrong.",
        " Elephant says: My pet mouse could do better… and I'm afraid of him.",
        " Elephant says: If wrong answers were gold, you'd be rich by now.",
        " Elephant says: You sure you're awake?",
        " Elephant says: I… I have no words for that guess.",
        " Elephant says: I'll pretend I didn't hear that.",
        " Elephant says: I think my ears just quit their job.",
        " Elephant says: That killed me… and not in the good way.",
        " Elephant says: Are you auditioning for clown of the year?",
        " Elephant says: Wrong! Please stop hurting my brain."
    };

    // Multiple "ask" lines
    private string[] askLines = new string[]
    {
        " Elephant says: PHROOO! Here's your riddle!",
        " Elephant says: I hope you're smarter than you look… here's a riddle.",
        " Elephant says: Pay attention, this one's tricky!",
        " Elephant says: Let's see if your brain still works…",
        " Elephant says: Okay genius, answer this!",
        " Elephant says: Don't embarrass yourself…",
        " Elephant says: If you get this wrong, I'll cry.",
        " Elephant says: Here comes the challenge!",
        " Elephant says: Think fast, human!",
        " Elephant says: Riddle time!"
    };

    void Start()
    {
        submitButton.onClick.AddListener(CheckAnswer);
        nextButton.onClick.AddListener(NextRiddle);
        mainMenuButton.onClick.AddListener(LoadMainMenu);

        winPanel.SetActive(false);
        nextButton.gameObject.SetActive(false);

        // Initialize riddles
        riddles = new Riddle[]
        {
            new Riddle{ question_en = "What has keys but can't open locks?", answer = "piano", timeLimit = 25f },
            new Riddle{ question_en = "What has to be broken before you can use it?", answer = "egg", timeLimit = 20f },
            new Riddle{ question_en = "I'm tall when I'm young, short when I'm old. What am I?", answer = "candle", timeLimit = 25f },
            new Riddle{ question_en = "What gets wetter the more it dries?", answer = "towel", timeLimit = 20f },
            new Riddle{ question_en = "I have branches but no fruit, trunk, or leaves. What am I?", answer = "bank", timeLimit = 30f },
            new Riddle{ question_en = "The more of me you take, the more you leave behind. What am I?", answer = "footsteps", timeLimit = 35f },
            new Riddle{ question_en = "I'm not alive, but I can grow. I have no mouth, but water kills me. What am I?", answer = "fire", timeLimit = 30f },
            new Riddle{ question_en = "I fly without wings, I cry without eyes. What am I?", answer = "cloud", timeLimit = 25f },
            new Riddle{ question_en = "What has a face and two hands but no arms or legs?", answer = "clock", timeLimit = 20f },
            new Riddle{ question_en = "The more you take from me, the bigger I get. What am I?", answer = "hole", timeLimit = 25f },
            new Riddle{ question_en = "What comes down but never goes up?", answer = "rain", timeLimit = 20f },
            new Riddle{ question_en = "What belongs to you but is used more by others?", answer = "name", timeLimit = 25f },
            new Riddle{ question_en = "I'm always in front of you but can't be seen. What am I?", answer = "future", timeLimit = 30f },
            new Riddle{ question_en = "I have cities but no houses, forests but no trees, and water but no fish. What am I?", answer = "map", timeLimit = 35f },
            new Riddle{ question_en = "I'm light as a feather, but even the world's strongest man can't hold me for long. What am I?", answer = "breath", timeLimit = 30f },
            new Riddle{ question_en = "What can travel around the world while staying in the same spot?", answer = "stamp", timeLimit = 25f },
            new Riddle{ question_en = "What has many teeth but can't bite?", answer = "comb", timeLimit = 20f },
            new Riddle{ question_en = "If you drop me I crack, but if you smile at me I smile back. What am I?", answer = "mirror", timeLimit = 25f },
            new Riddle{ question_en = "I'm full of holes but I can hold water. What am I?", answer = "sponge", timeLimit = 20f },
            new Riddle{ question_en = "What can you catch but not throw?", answer = "cold", timeLimit = 15f },
            new Riddle{ question_en = "What has one eye but can't see?", answer = "needle", timeLimit = 20f },
            new Riddle{ question_en = "What kind of coat is always wet when you put it on?", answer = "paint", timeLimit = 25f },
            new Riddle{ question_en = "What gets sharper the more you use it?", answer = "brain", timeLimit = 20f },
            new Riddle{ question_en = "What comes once in a minute, twice in a moment, but never in a thousand years?", answer = "m", timeLimit = 35f },
            new Riddle{ question_en = "What's always running but never moves?", answer = "time", timeLimit = 20f },
            new Riddle{ question_en = "I'm always in bed but never sleep. What am I?", answer = "river", timeLimit = 25f },
            new Riddle{ question_en = "The more of me you see, the less you see. What am I?", answer = "darkness", timeLimit = 30f },
            new Riddle{ question_en = "I'm easy to lift but hard to throw. What am I?", answer = "feather", timeLimit = 20f },
            new Riddle{ question_en = "I speak without a mouth and hear without ears. What am I?", answer = "echo", timeLimit = 25f },
            new Riddle{ question_en = "What has a head, a tail, but no body?", answer = "coin", timeLimit = 30f },
            new Riddle{ question_en = "What can you break without touching it?", answer = "promise", timeLimit = 25f },
            new Riddle{ question_en = "What goes up and down but doesn't move?", answer = "stairs", timeLimit = 25f },
            new Riddle{ question_en = "What has hands but can't clap?", answer = "clock", timeLimit = 20f },
            new Riddle{ question_en = "What can you hold in your right hand but not in your left?", answer = "left elbow", timeLimit = 35f },
            new Riddle{ question_en = "What gets bigger when more is taken away?", answer = "hole", timeLimit = 25f },
            new Riddle{ question_en = "What has words but never speaks?", answer = "book", timeLimit = 20f },
            new Riddle{ question_en = "What has a neck but no head?", answer = "bottle", timeLimit = 25f },
            new Riddle{ question_en = "What can run but can't walk?", answer = "river", timeLimit = 20f },
            new Riddle{ question_en = "What has a thumb and four fingers but isn't alive?", answer = "glove", timeLimit = 25f },
            new Riddle{ question_en = "What is always in front of you but can't be seen?", answer = "future", timeLimit = 30f },
                new Riddle{ question_en = "What has to be broken before you can use it?", answer = "egg", timeLimit = 20f },
    new Riddle{ question_en = "I'm tall when I'm young, short when I'm old. What am I?", answer = "candle", timeLimit = 25f },
    
    // Add these new ones:
    new Riddle{ question_en = "What can fill a room but takes up no space?", answer = "light", timeLimit = 30f },
    new Riddle{ question_en = "What has legs but doesn't walk?", answer = "table", timeLimit = 20f },
    new Riddle{ question_en = "What has a bottom at the top?", answer = "leg", timeLimit = 25f },
    new Riddle{ question_en = "What goes through cities and fields but never moves?", answer = "road", timeLimit = 25f },
    new Riddle{ question_en = "What has many needles but doesn't sew?", answer = "pine", timeLimit = 30f },
    new Riddle{ question_en = "What gets whiter the dirtier it gets?", answer = "blackboard", timeLimit = 35f },
    new Riddle{ question_en = "What has hands but can't clap?", answer = "clock", timeLimit = 20f },
    new Riddle{ question_en = "What can you catch but not throw?", answer = "cold", timeLimit = 15f },
    new Riddle{ question_en = "What has a head and a tail but no body?", answer = "coin", timeLimit = 20f },
    new Riddle{ question_en = "What has words but never speaks?", answer = "book", timeLimit = 20f },
    new Riddle{ question_en = "What has a neck but no head?", answer = "bottle", timeLimit = 25f },
    new Riddle{ question_en = "What can run but can't walk?", answer = "river", timeLimit = 20f },
    new Riddle{ question_en = "What has a thumb and four fingers but isn't alive?", answer = "glove", timeLimit = 25f },
    new Riddle{ question_en = "What is always in front of you but can't be seen?", answer = "future", timeLimit = 30f },
    new Riddle{ question_en = "What can you break without touching it?", answer = "promise", timeLimit = 25f },
    new Riddle{ question_en = "What goes up and down but doesn't move?", answer = "stairs", timeLimit = 25f },
    new Riddle{ question_en = "What can you hold in your right hand but not in your left?", answer = "left elbow", timeLimit = 35f },
    new Riddle{ question_en = "What gets bigger when more is taken away?", answer = "hole", timeLimit = 25f },
    new Riddle{ question_en = "What has a face that doesn't frown, hands that don't wave, but moves at a steady pace?", answer = "clock", timeLimit = 30f },
    new Riddle{ question_en = "What can travel around the world while staying in a corner?", answer = "stamp", timeLimit = 25f },
    new Riddle{ question_en = "What has a bed but never sleeps, has a mouth but never eats?", answer = "river", timeLimit = 30f },
    new Riddle{ question_en = "What has a ring but no finger?", answer = "phone", timeLimit = 20f },
    new Riddle{ question_en = "What has a foot but no legs?", answer = "snail", timeLimit = 25f },
    new Riddle{ question_en = "What has a head, can't think, and travels all over the world?", answer = "nail", timeLimit = 30f },
    new Riddle{ question_en = "What can you keep after giving it to someone?", answer = "word", timeLimit = 25f },
    new Riddle{ question_en = "What has a spine but no bones?", answer = "book", timeLimit = 20f },
    new Riddle{ question_en = "What has a tongue but can't talk?", answer = "shoe", timeLimit = 25f },
    new Riddle{ question_en = "What has teeth but can't bite?", answer = "comb", timeLimit = 20f },
    new Riddle{ question_en = "What has a single eye but can't see?", answer = "needle", timeLimit = 20f },
    new Riddle{ question_en = "What gets wet while drying?", answer = "towel", timeLimit = 20f },
    new Riddle{ question_en = "What has a bark but no bite?", answer = "tree", timeLimit = 20f },
    new Riddle{ question_en = "What has a heart that doesn't beat?", answer = "artichoke", timeLimit = 35f },
    new Riddle{ question_en = "What has a face but no head, hands but no arms?", answer = "clock", timeLimit = 25f },
    new Riddle{ question_en = "What can you serve but never eat?", answer = "tennis ball", timeLimit = 30f },
    new Riddle{ question_en = "What has a golden head and golden tail but no body?", answer = "coin", timeLimit = 25f },
    new Riddle{ question_en = "What has a neck but no head, two arms but no hands?", answer = "bottle", timeLimit = 30f },
    new Riddle{ question_en = "What is so fragile that saying its name breaks it?", answer = "silence", timeLimit = 35f },
    new Riddle{ question_en = "What can go up a chimney down but can't go down a chimney up?", answer = "umbrella", timeLimit = 40f },
    new Riddle{ question_en = "What has a face but no eyes, hands but no fingers?", answer = "clock", timeLimit = 25f },
    new Riddle{ question_en = "What is full of holes but still holds water?", answer = "sponge", timeLimit = 20f },
    new Riddle{ question_en = "What has a thumb and fingers but isn't alive?", answer = "glove", timeLimit = 25f },
    new Riddle{ question_en = "What has a head like a cat, feet like a cat, but isn't a cat?", answer = "kitten", timeLimit = 30f },
    new Riddle{ question_en = "What is always old and sometimes new, never sad sometimes blue, never empty sometimes full, never pushes always pulls?", answer = "moon", timeLimit = 45f },
    new Riddle{ question_en = "What can you make but can't see?", answer = "noise", timeLimit = 25f },
    new Riddle{ question_en = "What has a foot on each side and one in the middle?", answer = "compass", timeLimit = 35f },
    new Riddle{ question_en = "What has a mouth but never speaks, runs but never walks?", answer = "river", timeLimit = 30f },
    new Riddle{ question_en = "What has a tail and head but no body?", answer = "coin", timeLimit = 20f },
    new Riddle{ question_en = "What has a face but no eyes, can run but has no legs?", answer = "clock", timeLimit = 30f },
    new Riddle{ question_en = "What has a bed but never sleeps, can run but never walks?", answer = "river", timeLimit = 30f },
    new Riddle{ question_en = "What has a head like a nail but can't be hammered?", answer = "match", timeLimit = 35f },
    new Riddle{ question_en = "What has a tongue but can't taste?", answer = "shoe", timeLimit = 25f },
    new Riddle{ question_en = "What has a face but no expression?", answer = "clock", timeLimit = 25f },
    new Riddle{ question_en = "What has a spine but no bones, leaves but no branches?", answer = "book", timeLimit = 30f },
    new Riddle{ question_en = "What has a neck but no throat?", answer = "bottle", timeLimit = 25f },
    new Riddle{ question_en = "What has a foot but no toes?", answer = "snail", timeLimit = 25f },
    new Riddle{ question_en = "What has a face that never frowns, hands that never wave?", answer = "clock", timeLimit = 30f },
    new Riddle{ question_en = "What has a mouth but never eats, a bed but never sleeps?", answer = "river", timeLimit = 30f },
    new Riddle{ question_en = "What has a thumb but is not alive?", answer = "glove", timeLimit = 25f },
    new Riddle{ question_en = "What has a head like a cat, feet like a cat, tail like a cat, but isn't a cat?", answer = "kitten", timeLimit = 35f },
    new Riddle{ question_en = "What has a golden head, golden tail, but no body?", answer = "coin", timeLimit = 25f },
    new Riddle{ question_en = "What has a face but no head, hands but no arms, tells but doesn't talk?", answer = "clock", timeLimit = 35f },
    new Riddle{ question_en = "What has a bed but doesn't sleep, a mouth but doesn't eat?", answer = "river", timeLimit = 30f },
    new Riddle{ question_en = "What has a spine but no bones, pages but no leaves?", answer = "book", timeLimit = 30f },
    new Riddle{ question_en = "What has a neck but no head, arms but no hands?", answer = "bottle", timeLimit = 30f },
    new Riddle{ question_en = "What has a foot but no legs, can move but can't walk?", answer = "snail", timeLimit = 30f },
    new Riddle{ question_en = "What has a face that never changes, hands that never wave?", answer = "clock", timeLimit = 30f },
    new Riddle{ question_en = "What has a mouth but never speaks, runs but never walks, has a bed but never sleeps?", answer = "river", timeLimit = 35f },
    new Riddle{ question_en = "What has a thumb but no fingers?", answer = "mitten", timeLimit = 30f },
    new Riddle{ question_en = "What has a head like a cat, walks like a cat, but isn't a cat?", answer = "lion", timeLimit = 35f },
    new Riddle{ question_en = "What has a golden head, silver tail, but no body?", answer = "coin", timeLimit = 25f },
    new Riddle{ question_en = "What has a face but no eyes, hands but no arms, tells but doesn't speak?", answer = "clock", timeLimit = 35f },
    new Riddle{ question_en = "What has a bed but never sleeps, runs but never walks, has a mouth but never eats?", answer = "river", timeLimit = 40f },
    new Riddle{ question_en = "What has a spine but no bones, can tell a story but can't speak?", answer = "book", timeLimit = 35f },
    new Riddle{ question_en = "What has a neck but no head, arms but no hands, can hold liquid but can't drink?", answer = "bottle", timeLimit = 35f },
    new Riddle{ question_en = "What has a foot but no legs, carries its home on its back?", answer = "snail", timeLimit = 30f },
    new Riddle{ question_en = "What has a face that never ages, hands that never tire?", answer = "clock", timeLimit = 30f },
    new Riddle{ question_en = "What has a mouth but never talks, runs but never walks, has a head but never thinks?", answer = "river", timeLimit = 35f },
    new Riddle{ question_en = "What has a thumb but no fingers, keeps your hands warm in winter?", answer = "mitten", timeLimit = 30f },
    new Riddle{ question_en = "What has a head like a cat, walks like a cat, roars like a lion?", answer = "lion", timeLimit = 35f },
    new Riddle{ question_en = "What has a golden head, silver tail, is worth money but isn't alive?", answer = "coin", timeLimit = 25f },
    new Riddle{ question_en = "What has a face but no eyes, hands but no arms, runs but never walks?", answer = "clock", timeLimit = 35f },
    new Riddle{ question_en = "What has a bed but never sleeps, runs but never walks, has a mouth but never eats?", answer = "river", timeLimit = 40f },
    new Riddle{ question_en = "What has a spine but no bones, can tell stories but can't speak?", answer = "book", timeLimit = 35f },
    new Riddle{ question_en = "What has a neck but no head, arms but no hands, can hold water but can't drink?", answer = "bottle", timeLimit = 35f },
    new Riddle{ question_en = "What has a foot but no legs, leaves a trail of slime?", answer = "snail", timeLimit = 30f },
    new Riddle{ question_en = "What has a face that never changes, hands that never stop?", answer = "clock", timeLimit = 30f },
    new Riddle{ question_en = "What has a mouth but never speaks, runs but never walks, has a bed but never sleeps?", answer = "river", timeLimit = 35f },
    new Riddle{ question_en = "What has a thumb but no fingers, comes in pairs?", answer = "mitten", timeLimit = 30f },
    new Riddle{ question_en = "What has a head like a cat, walks like a cat, is king of the jungle?", answer = "lion", timeLimit = 35f },
    new Riddle{ question_en = "What has a golden head, silver tail, jingles in your pocket?", answer = "coin", timeLimit = 25f },
    new Riddle{ question_en = "What has a face but no eyes, hands but no arms, tells time but can't speak?", answer = "clock", timeLimit = 35f },
    new Riddle{ question_en = "What has a bed but never sleeps, runs but never walks, has a mouth but never eats?", answer = "river", timeLimit = 40f },
    new Riddle{ question_en = "What has a spine but no bones, can be read but can't think?", answer = "book", timeLimit = 35f },
    new Riddle{ question_en = "What has a neck but no head, arms but no hands, can be glass or plastic?", answer = "bottle", timeLimit = 35f },
    new Riddle{ question_en = "What has a foot but no legs, carries its house on its back?", answer = "snail", timeLimit = 30f },
    new Riddle{ question_en = "What has a face that never ages, hands that never stop moving?", answer = "clock", timeLimit = 30f },
    new Riddle{ question_en = "What has a mouth but never speaks, runs but never walks, has a bed but never sleeps?", answer = "river", timeLimit = 35f },
    new Riddle{ question_en = "What has a thumb but no fingers, keeps you warm in winter?", answer = "mitten", timeLimit = 30f },
    new Riddle{ question_en = "What has a head like a cat, walks like a cat, is called the king?", answer = "lion", timeLimit = 35f },
    new Riddle{ question_en = "What has a golden head, silver tail, is money but not paper?", answer = "coin", timeLimit = 25f },
    new Riddle{ question_en = "What has a face but no eyes, hands but no arms, tells time but can't talk?", answer = "clock", timeLimit = 35f },
    new Riddle{ question_en = "What has a bed but never sleeps, runs but never walks, has a mouth but never eats?", answer = "river", timeLimit = 40f },
    new Riddle{ question_en = "What has a spine but no bones, can tell stories but can't speak?", answer = "book", timeLimit = 35f },
    new Riddle{ question_en = "What has a neck but no head, arms but no hands, can hold liquid but can't drink?", answer = "bottle", timeLimit = 35f },
    new Riddle{ question_en = "What has a foot but no legs, moves slowly and leaves a trail?", answer = "snail", timeLimit = 30f },
    new Riddle{ question_en = "What has a face that never changes, hands that never stop?", answer = "clock", timeLimit = 30f },
    new Riddle{ question_en = "What has a mouth but never speaks, runs but never walks, has a bed but never sleeps?", answer = "river", timeLimit = 35f },
    new Riddle{ question_en = "What has a thumb but no fingers, keeps hands warm in winter?", answer = "mitten", timeLimit = 30f },
    new Riddle{ question_en = "What has a head like a cat, walks like a cat, is the king of beasts?", answer = "lion", timeLimit = 35f },
    new Riddle{ question_en = "What has a golden head, silver tail, makes noise in your pocket?", answer = "coin", timeLimit = 25f }
        };

        // Shuffle the riddles array
        ShuffleRiddles();
        DisplayRiddle();
    }

    void ShuffleRiddles()
    {
        // Fisher-Yates shuffle algorithm
        for (int i = 0; i < riddles.Length - 1; i++)
        {
            int randomIndex = Random.Range(i, riddles.Length);
            Riddle temp = riddles[i];
            riddles[i] = riddles[randomIndex];
            riddles[randomIndex] = temp;
        }
    }

    void DisplayRiddle()
    {
        if (gameEnded) return;

        // Stop any existing timer
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        Riddle current = riddles[currentRiddleIndex];
        riddleText.text = current.question_en;
        answerInput.text = "";
        feedbackText.text = "";
        pointsText.text = "Score: " + points;
        nextButton.gameObject.SetActive(false);

        // Start the timer
        timeRemaining = current.timeLimit;
        timerCoroutine = StartCoroutine(TimerCountdown());

        PlaySound("ask");

    }

    IEnumerator TimerCountdown()
    {
        timerIsRunning = true;
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();
            yield return null;
        }

        // Time's up!
        timerIsRunning = false;
        TimeUp();
    }

    void UpdateTimerDisplay()
    {
        timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString();
    }

    void TimeUp()
    {
        if (gameEnded) return;

        points -= 10;
    
        pointsText.text = "Score: " + points;
        feedbackText.text = "Time's up! The answer was: " + riddles[currentRiddleIndex].answer;
        PlaySound("timeup");
        if(points <=-25)
        { EndGame(); }

        // Automatically move to next question after a short delay
        StartCoroutine(AutoNextQuestion());
    }
    IEnumerator AutoNextQuestion()
    {
        // Wait for 2 seconds before auto-advancing
        yield return new WaitForSeconds(2f);
        NextRiddle();
    }

    void PlaySound(string type)
    {
        switch (type)
        {
            case "ask":
                elephantSoundText.text = askLines[Random.Range(0, askLines.Length)];
                if (askClip != null) audioSource.PlayOneShot(askClip);
                break;
            case "correct":
                elephantSoundText.text = "🐘 Elephant says: You got it right! +20 points!";
                if (correctClip != null) audioSource.PlayOneShot(correctClip);
                break;
            case "wrong":
                string randomMock = mockingLines[Random.Range(0, mockingLines.Length)];
                elephantSoundText.text = randomMock + " -5 points!";
                if (wrongClip != null) audioSource.PlayOneShot(wrongClip);
                break;
            case "timeup":
                elephantSoundText.text = "⏰ Elephant says: Too slow! -10 points!";
                if (timeUpClip != null) audioSource.PlayOneShot(timeUpClip);
                break;
        }
    }

    void CheckAnswer()
    {
        if (gameEnded || !timerIsRunning) return;

        string userAnswer = answerInput.text.Trim().ToLower();
        string correctAnswer = riddles[currentRiddleIndex].answer.ToLower();

        if (userAnswer == correctAnswer)
        {
            // Stop the timer
            timerIsRunning = false;
            StopCoroutine(timerCoroutine);

            points += 20;
            feedbackText.text = "Correct! +20 points!";
            PlaySound("correct");
            nextButton.gameObject.SetActive(true);
        }
        
        else
        {
            points -= 5;
          
            feedbackText.text = "Wrong! -5 points! Try again.";
            PlaySound("wrong");
        }
        if (points <= -25)
        { EndGame(); }

        pointsText.text = "Score: " + points;
    }
    void NextRiddle()
    {
        if (gameEnded) return;

        currentRiddleIndex++;
        if (currentRiddleIndex >= riddles.Length)
        {
            currentRiddleIndex = 0;
            ShuffleRiddles();
            EndGame(); // End game if we've gone through all questions
            return;
        }

        DisplayRiddle();
    }
    void EndGame()
    {
        gameEnded = true;
        winPanel.SetActive(true);
        SaveScore();
        nextButton.interactable = false;
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        SaveScore();
    }
    // Add these new methods to your ElephantRiddleGame class
    public void SaveScore()
    {
        // 1. Get the previously saved score (default to 0 if none exists)
        int previousScore = PlayerPrefs.GetInt("PlayerScore", 0);

        // 2. Add the current score to the previous one
        int newTotalScore = previousScore + points;

        // 3. Save the accumulated score
        PlayerPrefs.SetInt("PlayerScore", newTotalScore);
        PlayerPrefs.Save();

        Debug.Log($"Saved score: {points} added to previous {previousScore} = {newTotalScore}");
    }



}
