using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    private int playerScore;
    public Text scoreText;

    private int playerMove;
    public Text movesText;

    public void GetScore(int point)
    {
        playerScore += point;
        scoreText.text = playerScore.ToString();
    }

    public void AddMove()
    {
        playerMove++;
        movesText.text = playerMove.ToString();

        StartCoroutine(WaitMove());
    }

    private IEnumerator WaitMove()
    {
        yield return new WaitForSeconds(.8f);

        Tile.canMove = true;
    }
}
