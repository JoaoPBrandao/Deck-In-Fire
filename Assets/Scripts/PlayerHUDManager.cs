using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUDManager : MonoBehaviour
{
    [SerializeField] private GameObject _interactionText, _victoryOverlay, _defeatOverlay;
    [SerializeField] private Image _extinguisherImage;
    [SerializeField] private TMP_Text _extinguisherQuantityText, _scoreText, _progressText, _livesText;
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
    }

    private void OnMatchFinished(bool result)
    {
        if (result)
        {
            _victoryOverlay.SetActive(true);
        }
        else
        {
            _defeatOverlay.SetActive(true);
        }
    }

    private void OnGameUpdated()
    {
        var manager = GameManager.Instance;
        _scoreText.text = manager.Score.ToString();
        _progressText.text = $"{manager.RemainingTargets}/{manager.LevelTarget}";
        _livesText.text = $"{manager.RemainingMisses}/{manager.LevelMisses}";
    }

    private void OnExtinguisherChanged(ExtinguisherInstance extinguisher)
    {
        if (extinguisher == null)
        {
            _extinguisherImage.sprite = null;
            _extinguisherQuantityText.gameObject.SetActive(false);
            return;
        }
        _extinguisherQuantityText.gameObject.SetActive(true);
        _extinguisherImage.sprite = extinguisher.Definition.Icon;
        _extinguisherQuantityText.text = $"{extinguisher.RemainingUses}/{extinguisher.Definition.Uses}";
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
