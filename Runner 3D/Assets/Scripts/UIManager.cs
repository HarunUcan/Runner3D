using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("UI Manager is null");

            return _instance;
        }
    }

    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private List<Image> _hearthImgs;
    [SerializeField] private GameObject _gameOverMenu;


    private void Awake()
    {
        _instance = this;
    }
    private void Update()
    {
    }
    public void UpdateScore(int score)
    {
        _scoreText.text = $"SCORE:{score}";
    }
    public void UpdateLives(int lives)
    {
        for (int i = 0; i < _hearthImgs.Count - lives; i++)
        {
            _hearthImgs[i].color = Color.black;
        }
    }
    public void GameOver()
    {
        Time.timeScale = 0;
    }

    public void ChangeScoreStyle()
    {
        _scoreText.color = new Color32(0, 150, 25, 255);
        _scoreText.transform.DOScale(1.2f, .5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
    public void StartGame()
    {
        Time.timeScale = 1;
    }
    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SampleScene");
    }
    public void ShowGameOverMenu()
    {
        _gameOverMenu.SetActive(true);
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}