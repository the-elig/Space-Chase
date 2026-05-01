using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

public class UpgradeCardSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private Image slotImage;
    [SerializeField] private GameObject cancelButton;
    [SerializeField] private GameController gameController;
    [SerializeField] private CardUpgraderUI cardUpgrader;
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

        if (slotImage != null) {
            slotImage.color = filledColor; }

        cardUpgrader.AllowUpgrade();
    }

    public string GetCardName()
    {
        if (currentCard.cardData != null)
        {
            CardData data = currentCard.cardData;
            return data.cardName;
        } else return string.Empty;
    }
    public CardData GetCardUpgrade()
    {
        CardData data = currentCard.cardData;
        return data.upgrade;
    }
    public void OnConfirm()
{
    if (currentCard.cardData != null)
    {
        CardData data = currentCard.cardData;

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

    //StartCoroutine(CloseStationDelay());

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
        cardUpgrader.AllowUpgrade();
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