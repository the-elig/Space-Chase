using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

public class RoomCardSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Room")]
    public string roomName = "Weapons";

    [Header("References")]
    [SerializeField] private Image slotImage;
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private GameObject cancelButton;
    [SerializeField] private GameController gameController;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text damagedText;
    [SerializeField] private TMP_Text normalText;

    [Header("Slot Visuals")]
    [SerializeField] private Color emptyColor = new Color(1, 1, 1, 0.3f);
    [SerializeField] private Color hoverColor = new Color(1, 1, 1, 0.6f);
    [SerializeField] private Color filledColor = new Color(1, 1, 1, 0.0f);

    [Header("Events")]
    public UnityEvent<Card> OnCardConfirmed;

    private Card currentCard = null;
    private Transform cardOriginalParent;
    private int cardOriginalSiblingIndex;
    private GameObject cardOriginalSlot;
    private HorizontalCardHolder cardHolder;

    [HideInInspector] public bool openedFromPassage = false;
    [HideInInspector] public PassageInteractable currentPassage = null;

    void Awake()
{
    if (confirmButton != null) confirmButton.SetActive(true);
    if (cancelButton != null) cancelButton.SetActive(true);
    if (messageText != null) messageText.gameObject.SetActive(false);
    if (damagedText != null) damagedText.gameObject.SetActive(false);
    if (normalText != null) normalText.gameObject.SetActive(false);

    cardHolder = FindObjectOfType<HorizontalCardHolder>();
    if (gameController == null)
        gameController = FindObjectOfType<GameController>();
}

    public void UpdateStationMessage(bool isDamaged)
    {
        if (damagedText != null)
            damagedText.gameObject.SetActive(isDamaged);
        if (normalText != null)
            normalText.gameObject.SetActive(!isDamaged);
    }

    public void HideStationMessages()
    {
        if (damagedText != null)
            damagedText.gameObject.SetActive(false);
        if (normalText != null)
            normalText.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (currentCard != null) return;

        Card droppedCard = eventData.pointerDrag?.GetComponent<Card>();
        if (droppedCard == null) return;

        AcceptCard(droppedCard);
    }

    void AcceptCard(Card card)
    {
        currentCard = card;
        cardOriginalParent = card.transform.parent;
        cardOriginalSiblingIndex = card.transform.GetSiblingIndex();
        cardOriginalSlot = card.transform.parent.gameObject;

        if (cardHolder != null)
            cardHolder.RemoveCard(card);

        if (card.cardVisual != null)
            card.cardVisual.pauseFollow = true;

        card.transform.SetParent(transform);
        card.transform.localPosition = Vector3.zero;

        if (card.cardVisual != null)
        {
            card.cardVisual.transform.DOMove(transform.position, 0.2f).SetEase(Ease.OutBack);
            card.cardVisual.transform.DOScale(1f, 0.2f);
            card.cardVisual.transform.DORotate(Vector3.zero, 0.2f);

            Transform tiltParent = card.cardVisual.transform.Find("ShakeParent/TiltParent");
            if (tiltParent != null)
                tiltParent.DORotate(Vector3.zero, 0.2f);
        }

        if (slotImage != null)
            slotImage.color = filledColor;
    }

    public void OnConfirm()
{
    Debug.Log("Card: " + currentCard.cardData?.cardName + " Room: " + gameController._currentRoom + " Requirement: " + currentCard.cardData?.requirement);
    if (currentCard == null) return;

    if (currentCard.cardData != null)
    {
        CardData data = currentCard.cardData;

        // check energy cost
        if (gameController._energy < data.energyCost)
        {
            StartCoroutine(ShowMessage("Not enough energy!"));
            OnCancel();
            return;
        }

        // check station type restriction
        if (data.allowedStation != StationType.Any)
        {
            string currentRoom = gameController._currentRoom.ToString().ToLower();
            string requiredStation = data.allowedStation.ToString().ToLower();

            if (currentRoom != requiredStation)
            {
                StartCoroutine(ShowMessage("This card can't be used here!"));
                OnCancel();
                return;
            }
        }

        // check station healthy requirement
        if (data.requirement == CardRequirement.StationHealthy)
        {
            string currentRoomName = gameController._currentRoom.ToString();
            bool isDamaged = gameController._damagedRooms.Exists(r =>
                r.ToLower() == currentRoomName.ToLower());

            if (isDamaged)
            {
                StartCoroutine(ShowMessage("This station is damaged! Repair it first."));
                OnCancel();
                return;
            }
        }

        // check station damaged requirement
        if (data.requirement == CardRequirement.StationDamaged)
        {
            Debug.Log("openedFromPassage = " + openedFromPassage + " | currentPassage = " + currentPassage);
            if (openedFromPassage)
            {
                // passage repair - always valid
            }
            else
            {
                string currentRoomName = gameController._currentRoom.ToString();
                bool isDamaged = gameController._damagedRooms.Exists(r =>
                    r.ToLower() == currentRoomName.ToLower());

                if (!isDamaged)
                {
                    StartCoroutine(ShowMessage("This station isn't damaged!"));
                    OnCancel();
                    return;
                }
            }
        }

        // deduct energy
        gameController._energy -= data.energyCost;
    }

    ExecuteConfirm();
}

    private void ExecuteConfirm()
{
    OnCardConfirmed?.Invoke(currentCard);

    Card cardToDestroy = currentCard;
    currentCard = null;

    if (slotImage != null)
        slotImage.color = emptyColor;

    if (cardToDestroy.cardData != null)
    {
        // if it was a repair card, fix the room or passage
        if (cardToDestroy.cardData.requirement == CardRequirement.StationDamaged)
        {
            if (openedFromPassage && currentPassage != null)
            {
                currentPassage.RepairPassage();
                openedFromPassage = false;
                currentPassage = null;
            }
            else
            {
                // remove from damaged rooms list
                string currentRoomName = gameController._currentRoom.ToString();
                gameController._damagedRooms.RemoveAll(r =>
                    r.ToLower() == currentRoomName.ToLower());

                // find room by ID and repair it
                int currentRoomId = -1;
                switch (gameController._currentRoom)
                {
                    case GameController.PlayerLocation.comms: currentRoomId = 0; break;
                    case GameController.PlayerLocation.engine: currentRoomId = 1; break;
                    case GameController.PlayerLocation.weapons: currentRoomId = 2; break;
                    case GameController.PlayerLocation.bridge: currentRoomId = 3; break;
                    case GameController.PlayerLocation.shields: currentRoomId = 4; break;
                }

                RoomController[] rooms = FindObjectsOfType<RoomController>();
                foreach (RoomController room in rooms)
                {
                    if (room.id == currentRoomId)
                    {
                        room.damaged = false;
                        room.warning.SetActive(false);
                        break;
                    }
                }
            }
        }
        // if it was a station card, reduce turns left
        else if (cardToDestroy.cardData.requirement == CardRequirement.StationHealthy &&
                 cardToDestroy.cardData.allowedStation != StationType.Any)
        {
            gameController._turnsLeft--;
        }
    }

    StartCoroutine(CloseStationDelay());

    if (cardToDestroy.cardVisual != null)
    {
        cardToDestroy.cardVisual.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(cardToDestroy.cardVisual.gameObject);
            Destroy(cardToDestroy.gameObject);
            if (cardOriginalSlot != null)
                Destroy(cardOriginalSlot);
        });
    }
    else
    {
        Destroy(cardToDestroy.gameObject);
        if (cardOriginalSlot != null)
            Destroy(cardOriginalSlot);
    }
}

    IEnumerator ShowMessage(string message, float duration = 2f)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(duration);
            messageText.gameObject.SetActive(false);
        }
    }

    IEnumerator CloseStationDelay()
{
    yield return new WaitForSeconds(0.4f);
    openedFromPassage = false;
    currentPassage = null;
    GameObject stationPanel = GameObject.Find("StationPanel");
    if (stationPanel != null)
        stationPanel.SetActive(false);
}

    public void OnCancel()
    {
        if (currentCard == null) return;

        Card cardToReturn = currentCard;
        currentCard = null;

        cardToReturn.transform.SetParent(cardOriginalParent);
        cardToReturn.transform.SetSiblingIndex(cardOriginalSiblingIndex);
        cardToReturn.transform.localPosition = Vector3.zero;

        if (cardHolder != null)
            cardHolder.AddCard(cardToReturn);

        if (slotImage != null)
            slotImage.color = emptyColor;

        StartCoroutine(CancelAnimation(cardToReturn));
    }

    IEnumerator CancelAnimation(Card card)
    {
        if (card.cardVisual == null) yield break;

        Transform visual = card.cardVisual.transform;
        DOTween.Kill(visual, true);

        Vector3 startPos = visual.position;
        Vector3 throwTarget = startPos + new Vector3(-400f, 0f, 0f);

        Sequence seq = DOTween.Sequence();
        seq.Append(visual.DOMove(throwTarget, 1.45f).SetEase(Ease.OutCubic));
        seq.Join(visual.DORotate(new Vector3(0f, 0f, -360f), 1.45f, RotateMode.FastBeyond360));
        seq.Append(visual.DOMove(card.transform.position, 0.35f).SetEase(Ease.InOutQuad));
        seq.Join(visual.DORotate(Vector3.zero, 0.35f).SetEase(Ease.OutBack));
        seq.OnComplete(() => card.cardVisual.pauseFollow = false);

        yield return seq.WaitForCompletion();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentCard != null) return;
        if (slotImage != null)
            slotImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentCard != null) return;
        if (slotImage != null)
            slotImage.color = emptyColor;
    }
}