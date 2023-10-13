using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject[] _pages;

    private int _currentPage = 0;
    // Start is called before the first frame update
    void Start()
    {
        _pages[_currentPage].SetActive(true);
    }

    public void AdvancePage()
    {
        _pages[_currentPage].SetActive(false);
        _currentPage++;
        if (_currentPage >= _pages.Length)
        {
            gameObject.SetActive(false);
            GameManager.Instance.StartGame();
            return;
        }
        _pages[_currentPage].SetActive(true);
    }
}
