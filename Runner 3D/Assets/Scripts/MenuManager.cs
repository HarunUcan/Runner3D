using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 1;

        JsonSaveLoad jsonSaveLoad = new JsonSaveLoad();
        jsonSaveLoad.LoadData();
    }
    void Start()
    {
        Button playBtn = GameObject.Find("PlayBtn").GetComponent<Button>();
        TMP_Text highScoreText = GameObject.Find("HighScoreText").GetComponent<TMP_Text>();

        playBtn.transform.DOScale(1.15f, .5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        highScoreText.text = $"High Score:{PlayerStats.highScore}";
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void QuitGame()
    {
        UIManager.QuitGame();
    }
}
