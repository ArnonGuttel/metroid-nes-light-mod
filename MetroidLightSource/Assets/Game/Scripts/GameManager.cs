using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


public class GameManager : MonoBehaviour
{
    #region Inspector

    [SerializeField] private TextMeshProUGUI TimerFrame;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameWonUI;
    [SerializeField] private GameObject gamePausedUI;
    [SerializeField] private float Timer;
    [SerializeField] private PlayableDirector TimeLine;
    [SerializeField] private GameObject helpPanel;

    #endregion

    #region Fields

    private static GameManager _shared;
    [HideInInspector] public List<GameObject> DeadEnemies;
    private bool _resetFlag;
    private bool _startTimer;
    private bool _isPaused;
    private bool _freezeGameMovement = true;
    private bool _TimeLineRunning = true;
    
    #endregion

    #region Events

    public static event Action OpenHiddenDoor;
    public static event Action PlayerDead;
    public static event Action GameWon;
    public static event Action EnemyKilled;

    public static void KeyTaken()
    {
        OpenHiddenDoor?.Invoke();
    }

    public static void InvokePlayerDead()
    {
        PlayerDead?.Invoke();
    }

    public static void InvokeGameWon()
    {
        GameWon?.Invoke();
    }

    public static void InvokeEnemyKilled()
    {
        EnemyKilled?.Invoke();
    }

    private void hideTimer()
    {
        _startTimer = false;
        TimerFrame.gameObject.SetActive(false);
    }

    private void activeGameWonUI()
    {
        gameWonUI.SetActive(true);
        GetComponent<AudioSource>().Stop();
    }

    private void activeGameOverUI()
    {
        gameOverUI.SetActive(true);
    }

    private void startTimer()
    {
        TimerFrame.gameObject.SetActive(true);
        _startTimer = true;
    }
    

    #endregion

    #region MonoBehaviour

    private void Start()
    {
        _shared = this;
    }

    private void Awake()
    {
        OpenHiddenDoor += startTimer;
        PlayerDead += activeGameOverUI;
        GameWon += hideTimer;
        GameWon += activeGameWonUI;
    }

    private void OnDestroy()
    {
        OpenHiddenDoor -= startTimer;
        PlayerDead -= activeGameOverUI;
        GameWon -= hideTimer;
        GameWon -= activeGameWonUI;
    }

    private void Update()
    {
        if (_TimeLineRunning)
        {
            if (TimeLine.state == PlayState.Paused)
            {
                _freezeGameMovement = false;
                GetComponent<AudioSource>().Play(0);
                _TimeLineRunning = false;
                TimeLine.gameObject.SetActive(false);
            }
        }

        if (_startTimer)
            runTimer();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameOverUI.gameObject.activeInHierarchy || gameWonUI.gameObject.activeInHierarchy)
                return;
            if (_isPaused)
                resumeGame();
            else
                pauseGame();
        }

        if (Input.GetKey(KeyCode.Q))
            helpPanel.SetActive(true);
        else
            helpPanel.SetActive(false);
    }

    #endregion

    #region StaticMethods

    public static void addToDeadEnemies(GameObject enemy)
        // whenever an Enemy die it will be added to the deadEnemies list 
    {
        _shared.DeadEnemies.Add(enemy);
    }

    public static void resetEnemies()
        // Once the player will reach to a respawn point we will reActive every dead enemy.
        // We will set the respawns limit to every boundary change. 
    {
        if (_shared.DeadEnemies.Count == 0 || _shared._resetFlag == false)
            return;
        foreach (var enemy in _shared.DeadEnemies.ToList())
        {
            enemy.gameObject.SetActive(true);
            _shared.DeadEnemies.Remove(enemy);
        }

        _shared._resetFlag = false;
    }

    public static void boundrayChange()
    {
        _shared._resetFlag = true;
    }

    public static bool FreezeGameMovement => _shared._freezeGameMovement;

    #endregion

    #region Methods

    public void QuitGame()
    {
        Application.Quit();
    }

    public void playAgain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameUpdate");
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("OpenScreen");
        Time.timeScale = 1;
    }

    public void resumeGame()
    {
        gamePausedUI.SetActive(false);
        Time.timeScale = 1;
        _isPaused = false;
    }

    private void pauseGame()
    {
        gamePausedUI.SetActive(true);
        Time.timeScale = 0;
        _isPaused = true;
    }

    private void runTimer()
    {
        Timer = Timer - Time.deltaTime;
        double timeLeft = Math.Round(Timer, 2);
        TimerFrame.text = timeLeft.ToString();
        if (Timer <= 0)
        {
            TimerFrame.text = "0";
            _startTimer = false;
            InvokePlayerDead();
        }
    }

    #endregion


}