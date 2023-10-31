using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class PlayerHUDManager : MonoBehaviour
{
    [Header("Match progress")] [SerializeField]
    private GameObject _playerHUD;
    [SerializeField] private TMP_Text _livesText, _firesText, _progressText;

    [Header("Interaction")] [SerializeField]
    private GameObject _interactionText;
    
    [Header("Extinguisher Info")]
    [SerializeField] private Image _extinguisherImage;
    [SerializeField] private Image _extinguisherQuantityBar;
    
    [Header("Match ending")]
    [SerializeField] private ResultScreen _resultScreen;

    private PlayerCharacter _player;
    void Start()
    {
        _player = FindObjectOfType<PlayerCharacter>();
        _player.OnExtinguisherChanged.AddListener(OnExtinguisherChanged);
        _player.OnExtinguisherUsed.AddListener(OnExtinguisherChanged);
        _player.OnBeginInteract.AddListener(OnBeginInteract);
        _player.OnEndInteract.AddListener(OnEndInteract);
        
        GameManager.Instance.OnGameUpdated.AddListener(OnGameUpdated);
        GameManager.Instance.OnMatchFinished.AddListener(OnMatchFinished);
        OnGameUpdated();
        OnExtinguisherChanged(null);
    }

    private void OnMatchFinished(bool result)
    {
        _playerHUD.SetActive(false);
        _resultScreen.SetupScreen(result);
    }

    private void OnGameUpdated()
    {
        var manager = GameManager.Instance;
        _firesText.text = $"X {manager.FireAmount}";
        _progressText.text = $"{manager.RemainingTargets}/{manager.LevelTarget}";
        _livesText.text = $"{manager.RemainingMisses}/{manager.LevelMisses}";
    }

    private void OnExtinguisherChanged(ExtinguisherInstance extinguisher)
    {
        if (extinguisher == null)
        {
            _extinguisherQuantityBar.fillAmount = 0;
            _extinguisherImage.gameObject.SetActive(false);
            return;
        }
        _extinguisherImage.gameObject.SetActive(true);
        _extinguisherImage.sprite = extinguisher.Definition.Icon;
        _extinguisherQuantityBar.fillAmount = (float)extinguisher.RemainingUses/extinguisher.Definition.Uses;
    }

    private void OnBeginInteract()
    {
        _interactionText.SetActive(true);
    }

    private void OnEndInteract()
    {
        _interactionText.SetActive(false);
    }
    
}
