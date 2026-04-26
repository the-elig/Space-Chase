using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    private Canvas canvas;
    private Image imageComponent;
    [SerializeField] private bool instantiateVisual = true;
    private VisualCardsHandler visualHandler;
    private Vector3 offset;

    [Header("Movement")]
    [SerializeField] private float moveSpeedLimit = 50;

    [Header("Selection")]
    public bool selected;
    public float selectionOffset = 50;
    private float pointerDownTime;
    private float pointerUpTime;

    [Header("Visual")]
    [SerializeField] private GameObject cardVisualPrefab;
    [HideInInspector] public CardVisual cardVisual;

    [Header("States")]
    public bool isHovering;
    public bool isDragging;
    [HideInInspector] public bool wasDragged;

    // === ZOOM VARIABLES ===
    private bool isZoomed = false;
    private bool justZoomed = false;
    private float lastClickTime = -1f;
    // ======================

    [Header("Events")]
    [HideInInspector] public UnityEvent<Card> PointerEnterEvent;
    [HideInInspector] public UnityEvent<Card> PointerExitEvent;
    [HideInInspector] public UnityEvent<Card, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<Card> PointerDownEvent;
    [HideInInspector] public UnityEvent<Card> BeginDragEvent;
    [HideInInspector] public UnityEvent<Card> EndDragEvent;
    [HideInInspector] public UnityEvent<Card, bool> SelectEvent;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();

        if (!instantiateVisual)
            return;

        Transform visualParent = VisualCardsHandler.instance != null ?
            VisualCardsHandler.instance.transform : transform;

        cardVisual = Instantiate(cardVisualPrefab, visualParent).GetComponent<CardVisual>();
        cardVisual.Initialize(this);
    }

    void Update()
{
    if (isDragging)
    {
        Vector2 localPoint;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            Input.mousePosition,
            null,
            out localPoint
        );
        transform.localPosition = Vector2.Lerp(
            transform.localPosition,
            localPoint,
            moveSpeedLimit * Time.deltaTime
        );
    }

    if (isZoomed && Input.GetMouseButtonDown(0) && !justZoomed)
        DismissZoom();
    justZoomed = false;
}

    // === ZOOM METHODS ===
    void ZoomCard()
    {
        if (cardVisual == null) return;
        isZoomed = true;
        justZoomed = true;
        Canvas cardCanvas = cardVisual.GetComponent<Canvas>();
        if (cardCanvas != null) { cardCanvas.overrideSorting = true; cardCanvas.sortingOrder = 100; }
        cardVisual.transform.DOLocalMove(Vector3.zero, 0.3f).SetEase(Ease.OutBack);
        cardVisual.transform.DOScale(2f, 0.3f).SetEase(Ease.OutBack);
    }

    void DismissZoom()
    {
        if (cardVisual == null) return;
        isZoomed = false;
        Canvas cardCanvas = cardVisual.GetComponent<Canvas>();
        if (cardCanvas != null) { cardCanvas.overrideSorting = false; cardCanvas.sortingOrder = 0; }
        cardVisual.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }
    // ====================

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (transform.parent.GetComponent<RoomCardSlot>() != null)
            return;
        BeginDragEvent.Invoke(this);
        isDragging = true;
        imageComponent.raycastTarget = false;
        wasDragged = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent.Invoke(this);
        isDragging = false;
        imageComponent.raycastTarget = true;

        StartCoroutine(FrameWait());

        IEnumerator FrameWait()
        {
            yield return new WaitForEndOfFrame();
            wasDragged = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent.Invoke(this);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent.Invoke(this);
        isHovering = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        PointerDownEvent.Invoke(this);
        pointerDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        pointerUpTime = Time.time;
        PointerUpEvent.Invoke(this, pointerUpTime - pointerDownTime > .2f);

        if (pointerUpTime - pointerDownTime > .2f)
            return;

        if (wasDragged)
            return;

        if (Time.time - lastClickTime <= 0.3f)
        {
            if (transform.parent.GetComponent<RoomCardSlot>() == null)
                ZoomCard();
            lastClickTime = -1f;
            return;
        }
        lastClickTime = Time.time;

        selected = !selected;
        SelectEvent.Invoke(this, selected);

        if (selected)
            transform.localPosition += new Vector3(0, selectionOffset, 0);
        else
            transform.localPosition = Vector3.zero;
    }

    public void Deselect()
    {
        if (selected)
        {
            selected = false;
            transform.localPosition = Vector3.zero;
        }
    }

    public int SiblingAmount()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;
    }

    public int ParentIndex()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
    }

    public float NormalizedPosition()
    {
        return transform.parent.CompareTag("Slot") ? ExtensionMethods.Remap((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1) : 0;
    }

    private void OnDestroy()
    {
        if (cardVisual != null)
            Destroy(cardVisual.gameObject);
    }
}