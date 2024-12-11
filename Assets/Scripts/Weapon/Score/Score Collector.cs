using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreCollector : MonoBehaviour
{
    public static ScoreCollector Instance; // Singleton instance for global access

    [Header("Score UI")]
    public TextMeshProUGUI scoreText; // Reference to the TextMeshPro UI

    [Header("Score Settings")]
    public int maxScore; // Maximum score for display purposes
    public int currentScore; // Tracks the current score

    private void Awake()
    {
        // Set up Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Try to find and assign the scoreText in case it's not assigned
        if (scoreText == null)
        {
            scoreText = FindObjectOfType<TextMeshProUGUI>(); // Find the first TextMeshProUGUI in the scene
        }

        if (scoreText != null)
        {
            UpdateScoreUI(); // Initialize the score display if scoreText is found
        }
        else
        {
            Debug.LogWarning("scoreText not found in the scene.");
        }
    }

    public void AddScore(int amount)
    {
        currentScore += amount; // Increment the score
        UpdateScoreUI(); // Update the UI
    }

    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore + "/" + maxScore;
        }
    }

    public void Update()
    {
        if (currentScore == maxScore)
        {
            sceneManager.Instance.LoadScene(sceneManager.Scene.WinScreen);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            currentScore = 0;
        }
    }
}