using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    #region Singleton
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Tile.canMove = true;
    //    DontDestroyOnLoad(gameObject);
    }
    #endregion

    public float inputDelay = .8f;

    private int playerScore;
    public Text scoreText;

    private int playerMove;
    public Text movesText;

    private int combo;
    public Text comboText;
    public Text comboMultiplierText;

    public float timer;
    public Text timerText;

    public GameObject pausePanel;
    public Text title;

    public GameObject totalScore;
    public Text totalScoreText;

    public GameObject resumeButton;

    private bool isGameEnded = false;

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = "Time Left : " + (int)timer;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                title.text = "Paused";

                if (!pausePanel.activeSelf)
                {
                    Time.timeScale = 0;
                    pausePanel.SetActive(true);
                }
                else
                {
                    Resume();
                }
            }
        }
        else
        {
            if (!isGameEnded)
            {
                isGameEnded = true;
                Tile.canMove = false;

                Time.timeScale = 0;

                title.text = "Game Over";

                resumeButton.SetActive(false);
                totalScore.SetActive(true);
                totalScoreText.text = playerScore.ToString();

                pausePanel.SetActive(true);
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Application.");
        Application.Quit();
    }

    public void GetScore(int point)
    {
        playerScore += Mathf.RoundToInt(point * ComboMultiplier());
        scoreText.text = playerScore.ToString();
    }

    public void AddMove()
    {
        playerMove++;
        movesText.text = "Total Moves : " + playerMove.ToString();

        StartCoroutine(WaitMove());
    }

    private IEnumerator WaitMove()
    {
        yield return new WaitForSeconds(inputDelay);

        Tile.canMove = true;
    }

    public void GetCombo()
    {
        combo++;
        comboText.enabled = true;
        comboText.text = combo.ToString() + " Combos!";
    }

    public void ResetCombo()
    {
        combo = 0;
        comboText.enabled = false;
        comboText.text = combo.ToString() + " Combo";
        ComboMultiplier();
    }

    private float ComboMultiplier()
    {
        float multiplier;

        if (combo < 3) multiplier = 1f;
        else if (combo < 5) multiplier = 1.1f;
        else if (combo < 15) multiplier = 1.25f;
        else if (combo < 25) multiplier = 1.5f;
        else if (combo < 30) multiplier = 1.75f;
        else if (combo < 40) multiplier = 2f;
        else multiplier = 2.5f;

        comboMultiplierText.text = multiplier.ToString() + "x";

        return multiplier;
    }
}
