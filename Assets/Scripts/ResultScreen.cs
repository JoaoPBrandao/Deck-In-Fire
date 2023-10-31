using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultScreen : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _title, _fire, _errors, _levelScore, _totalScore;

    [SerializeField] private string _victoryTitle, _defeatTitle;
    private bool _victory;

    public void SetupScreen(bool victory)
    {
        _victory = victory;
        _title.text = victory ? _victoryTitle : _defeatTitle;
        var manager = GameManager.Instance;
        _fire.text = manager.RemainingTargets.ToString();
        _errors.text = (manager.LevelMisses - manager.RemainingMisses).ToString();
        _levelScore.text = manager.Score.ToString();
        _totalScore.text = manager.TotalScore.ToString();
        gameObject.SetActive(true);
    }

    public void Continue()
    {
        if (_victory)
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
            {
                SceneManager.LoadScene(nextSceneIndex);
                return;
            }
        }
        SceneManager.LoadScene(0);
    }
}
