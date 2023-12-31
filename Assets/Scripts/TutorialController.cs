using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject[] _pages;

    private int _currentPage = 0;

    public void OpenTutorial()
    {
        _currentPage = 0;
        gameObject.SetActive(true);
        foreach (var page in _pages)
        {
            page.SetActive(false);
        }
        _pages[_currentPage].SetActive(true);
    }
    void Start()
    {
        OpenTutorial();
    }

    public void AdvancePage()
    {
        _pages[_currentPage].SetActive(false);
        _currentPage++;
        if (_currentPage >= _pages.Length)
        {
            gameObject.SetActive(false);
            GameManager.Instance?.StartGame();
            return;
        }
        _pages[_currentPage].SetActive(true);
    }
}
