using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUDManager : MonoBehaviour
{
    [SerializeField] private GameObject _interactionText;
    [SerializeField] private Image _extinguisherImage;
    [SerializeField] private TMP_Text _extinguisherQuantityText;
    private PlayerCharacter _player;
    void Start()
    {
        _player = FindObjectOfType<PlayerCharacter>();
        _player.OnExtinguisherChanged.AddListener(OnExtinguisherChanged);
        _player.OnBeginInteract.AddListener(OnBeginInteract);
        _player.OnEndInteract.AddListener(OnEndInteract);
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
