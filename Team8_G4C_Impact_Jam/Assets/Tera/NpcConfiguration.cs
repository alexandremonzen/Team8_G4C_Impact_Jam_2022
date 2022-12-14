using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NpcConfiguration : MonoBehaviour, IInteractable
{

    [Header("Quest")]
    [SerializeField] private ItemType _requiredItem;
    [SerializeField] private LanguageKnowledgeType _languageType;
    [SerializeField] private GameObject _itemToSpawn;
    [SerializeField] private GameObject _itemToGive;
    private bool _itemGiven;

    [Header("UI")]
    [SerializeField] private Dialogue _activeDialogue;
    [SerializeField] private GameObject _dialogueUI;
    [SerializeField] private TextMeshProUGUI _titleUI;
    [SerializeField] private TextMeshProUGUI _textUI;
    [SerializeField] private Image _titleImageUI;
    [SerializeField] private Image _ImageText;
    [SerializeField] private Sprite[] _titleImages;
    [SerializeField] private TMP_FontAsset[] _fonts;
    [SerializeField] private float _timeToDeactivate = 0;
    [SerializeField] private Sprite[] _animation;
    [SerializeField] private bool AnimationActive;

    [Header("Translate")]
    [SerializeField] private bool _isTranslated;
    [SerializeField] private bool _CheckTranslation = true;
    [SerializeField] private bool _isText = true;

    [Header("Change NPC")]
    [SerializeField] private GameObject _oldNPC;
    [SerializeField] private GameObject _newNPC;

    private bool _inDialogue;
    private bool _active;
    private PlayerHoldItem _actualPlayerHoldItem;

    public SpriteRenderer NpcSprite;

    private void Awake()
    {
        _inDialogue = false;
        _itemGiven = false;
    }

    private void OnEnable()
    {
        
    }

    private void SelfDeactive()
    {
        _dialogueUI.SetActive(false);
    }

    public void Update()
    {
        if(!AnimationActive && _animation.Length != 0)
            AnimationPlayer(_animation);
        if (GameObject.FindWithTag("Player").transform.position.y > transform.position.y-0.2f)
            NpcSprite.sortingOrder = 21;
        else
            NpcSprite.sortingOrder = 0;
    }

    public void Interact(PlayerInteraction playerInteraction)
    {
        _actualPlayerHoldItem = playerInteraction.PlayerHoldItem;

        if (_CheckTranslation)
            _isTranslated = playerInteraction.PlayerLanguageKnowledge.SearchForLanguageKnowledges(_languageType);

        playerInteraction.PlayerMovement.RemoveAllMovement();
        if (!_inDialogue)
        {
            StartDialogue();
            return;
        }

        if (_inDialogue && !_active)
        {
            switch (_activeDialogue.AfterOption)
            {
                case Dialogue.AfterD.End:
                    _inDialogue = false;
                    playerInteraction.PlayerMovement.ReturnAllMovement();
                    _dialogueUI.GetComponent<Animator>().SetBool("Active", _inDialogue);
                    break;

                case Dialogue.AfterD.EndReplace:
                    _inDialogue = false;
                    if (_isTranslated)
                        _activeDialogue = _activeDialogue.ReplaceDialogue;
                    _dialogueUI.GetComponent<Animator>().SetBool("Active", _inDialogue);
                    playerInteraction.PlayerMovement.ReturnAllMovement();
                    break;

                case Dialogue.AfterD.NextReplace:
                    if (_isTranslated)
                    {
                        _activeDialogue = _activeDialogue.ReplaceDialogue;
                        StartDialogue();
                    }
                    else
                    {
                        _inDialogue = false;
                        _dialogueUI.GetComponent<Animator>().SetBool("Active", _inDialogue);
                        playerInteraction.PlayerMovement.ReturnAllMovement();
                    }
                    break;

                case Dialogue.AfterD.NextReplaceIfComplete:
                    if ((playerInteraction.PlayerHoldItem.ActualHoldingItem && _isTranslated))
                    {
                        if (playerInteraction.PlayerHoldItem.ActualHoldingItem.ItemType == _requiredItem)
                        {
                            _activeDialogue = _activeDialogue.ReplaceDialogue;
                            playerInteraction.PlayerHoldItem.DisappearActualItem();
                            StartDialogue();
                        }
                        else
                        {
                            _inDialogue = false;
                            playerInteraction.PlayerMovement.ReturnAllMovement();
                            _dialogueUI.GetComponent<Animator>().SetBool("Active", _inDialogue);
                        }
                    }
                    else
                    {
                        _inDialogue = false;
                        _dialogueUI.GetComponent<Animator>().SetBool("Active", _inDialogue);
                        playerInteraction.PlayerMovement.ReturnAllMovement();
                    }
                    break;
            }

            return;
        }

        if (_active)
        {
            StopAllCoroutines();
            _active = false;
            _textUI.text = _activeDialogue.Text;
        }
    }

    private IEnumerator AnimationPlayer(Sprite[] anim)
    {
        AnimationActive = true;
        for (int i = 1; i < anim.Length; ++i)
        {
            yield return new WaitForSecondsRealtime(0.75f);
            _ImageText.sprite = anim[i];
            yield return null;
        }
        AnimationActive = false;
        yield return null;
    }
    private IEnumerator TypeSentence(string sentence)
    {
        _active = true;
        char[] array = sentence.ToCharArray();
        _textUI.text = array[0].ToString();
        for (int i = 1; i < array.Length; ++i)
        {
            yield return new WaitForSecondsRealtime(0.05f);
            _textUI.text += array[i];
            yield return null;
        }
        _active = false;
        yield return null;
    }

    public void StartDialogue()
    {
        _textUI.enabled = false;
        if(_ImageText) _ImageText.enabled = true;
        if (_timeToDeactivate > 0)
        {
            Invoke("SelfDeactive", _timeToDeactivate);
        }
        
        _inDialogue = true;
        _dialogueUI.GetComponent<Animator>().SetBool("Active", _inDialogue);
        switch (_activeDialogue.Name)
        {
            case Dialogue.Character.Player:
                _titleUI.text = "Parrot";
                _titleImageUI.sprite = _titleImages[0];
                _textUI.font = _fonts[0];
                break;
            case Dialogue.Character.Cat:
                _titleUI.text = _languageType.ToString();
                _titleImageUI.sprite = _titleImages[1];
                if (!_isTranslated)
                    _textUI.font = _fonts[1];
                else
                    _textUI.font = _fonts[0];
                break;
            case Dialogue.Character.Dog:
                _titleUI.text = _languageType.ToString();
                _titleImageUI.sprite = _titleImages[2];
                if (!_isTranslated)
                    _textUI.font = _fonts[2];
                else
                    _textUI.font = _fonts[0];
                break;
            case Dialogue.Character.UntranslatedCat:
                _titleUI.text = _languageType.ToString();
                _titleImageUI.sprite = _titleImages[1];
                _textUI.font = _fonts[1];
                break;
            case Dialogue.Character.UntranslatedDog:
                _titleUI.text = _languageType.ToString();
                _titleImageUI.sprite = _titleImages[2];
                _textUI.font = _fonts[2];
                break;
        }

        if (_activeDialogue.ActivateObject)
            _itemToSpawn.SetActive(true);

        if (_activeDialogue.HasItemToGive)
        {
            if (!_itemGiven)
            {
                _itemToGive.SetActive(true);
                _itemGiven = true;
            }
        }

        if(_activeDialogue.ChangeNPC)
        {
            _oldNPC.SetActive(false);
            _newNPC.SetActive(true);
        }

        if(_activeDialogue.GiveSpecialItem)
        {
            _actualPlayerHoldItem.HasDeliveredCatCeviche = true;
        }

        if (_isText == true || _activeDialogue.Name == Dialogue.Character.UntranslatedCat || _activeDialogue.Name == Dialogue.Character.UntranslatedDog || !_isTranslated)
        {
            _textUI.enabled = true;
            _ImageText.enabled = false;
            StartCoroutine(TypeSentence(_activeDialogue.Text));
        }
        else
        {
            _ImageText.sprite = _activeDialogue.Animation[0];
            _animation = _activeDialogue.Animation;
        }
    }
}
