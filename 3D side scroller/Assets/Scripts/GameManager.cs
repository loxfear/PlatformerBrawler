using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using InControl;
using MultiplayerBasicExample;

public class GameManager : MonoBehaviour {
    public float PlayersReady;
    public float PlayersAlive;
    public bool loaded;
    public float player1Score;
    public float player2Score;
    public float player3Score;
    public float player4Score;
    private float winningPlayer;
    private PlayerManager playerManager;

    // Use this for initialization
    void Start ()
    {
        GameObject playerManagerObj = GameObject.Find("PlayerManager");
        playerManager = playerManagerObj.GetComponent<PlayerManager>();
        DontDestroyOnLoad(transform.parent);
        loaded = false;
    }

    public void StartGame()
    {
        if (PlayersReady > 1 && !loaded)
        {
            Debug.Log("All Ready");
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("scene", LoadSceneMode.Single);
            loaded = true;
        }
        
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded");
        playerManager.RespawnPlayers();
        playerManager.joinRoom = false;
        PlayersReady = 0;


    }

    // when the game has ended and a winner is found
    public void GameEnd()
    {
        
        //find out who won
       winningPlayer = playerManager.FindWinningPlayer();
        if(winningPlayer == 0)
        {
            player1Score += 1;
        }
        if (winningPlayer == 1)
        {
            player2Score += 1;
        }
        if (winningPlayer == 2)
        {
            player3Score += 1;
        }
        if (winningPlayer == 3)
        {
            player4Score += 1;
        }

        //reset values
        playerManager.ResetPlayerVariables();

        // load "readyscreen"
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("PlayersReplay", LoadSceneMode.Single);
        loaded = false;

    }
    // Update is called once per frame
    void Update ()
    {

	}

    void OnGUI()
    {
        const float h = 22.0f;
        var y = 10.0f;

        y += h;
        GUI.Label(new Rect(10, y, 300, y + h), "P1 score: " + player1Score);
        y += h;
        GUI.Label(new Rect(10, y, 300, y + h), "P2 score: " + player2Score);
        y += h;
        GUI.Label(new Rect(10, y, 300, y + h), "P3 score: " + player3Score);
        y += h;
        GUI.Label(new Rect(10, y, 300, y + h), "P4 score: " + player4Score);
    }
}
