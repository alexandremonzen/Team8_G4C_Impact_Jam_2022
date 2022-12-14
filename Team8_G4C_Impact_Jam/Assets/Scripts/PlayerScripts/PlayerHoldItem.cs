using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerHoldItem : MonoBehaviour
{
    [SerializeField] private Transform _holdItemOffset;
    [SerializeField] private Transform _dropItemOffset;
    private Item _actualHoldingItem;
    private bool _canTakeItem;
    private bool _canDropItem;

    private PlayerInputActions _playerInputActions;

    public bool HasDeliveredCatCeviche = false;

    #region Getters & Setters
    public Item ActualHoldingItem { get => _actualHoldingItem; }
    #endregion

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        HasDeliveredCatCeviche = false;
        _canTakeItem = true;
        _canDropItem = true;
    }

    private void OnEnable()
    {
        _playerInputActions.PlayerHoldItem.Enable();
        _playerInputActions.PlayerHoldItem.DropItem.performed += DropItem;
    }

    private void OnDisable()
    {
        _playerInputActions.PlayerHoldItem.DropItem.performed -= DropItem;
        _playerInputActions.PlayerHoldItem.Disable();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        NpcConfiguration npcConfiguration = col.GetComponent<NpcConfiguration>();
        if(npcConfiguration)
        {
            _canDropItem = false;
        }

        if (_canTakeItem)
        {
            if (!_actualHoldingItem)
            {
                Item item = col.GetComponent<Item>();
                if (item)
                {
                    item.transform.position = _holdItemOffset.position;
                    item.transform.parent = _holdItemOffset.transform;
                    _actualHoldingItem = item;
                    _canTakeItem = false;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        NpcConfiguration npcConfiguration = col.GetComponent<NpcConfiguration>();
        if(npcConfiguration)
        {
            _canDropItem = true;
        }
    }

    private void DropItem(InputAction.CallbackContext obj)
    {
        if (_canDropItem)
        {
            if (_actualHoldingItem)
            {
                _actualHoldingItem.transform.parent = null;
                _actualHoldingItem.transform.position = _dropItemOffset.position;
                _actualHoldingItem = null;
                StartCoroutine(SetCantTakeItemTrue());
            }
        }
    }

    public void DisappearActualItem()
    {
        if (_actualHoldingItem)
        {
            _actualHoldingItem.gameObject.SetActive(false);
            _actualHoldingItem.transform.parent = null;
            _actualHoldingItem = null;
            _canTakeItem = true;
        }
    }

    private IEnumerator SetCantTakeItemTrue()
    {
        yield return new WaitForSeconds(0.1f);
        _canTakeItem = true;
    }
}
