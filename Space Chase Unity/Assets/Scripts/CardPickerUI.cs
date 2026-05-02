using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPickerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameController gameController;
    [SerializeField] private Deck deck;
    [SerializeField] private GameObject cardPickerPanel;
    [SerializeField] private CanvasController canvas;
    [SerializeField] private Image card1Image;
    [SerializeField] private Image card2Image;
    [SerializeField] private Image card1Highlight;
    [SerializeField] private Image card2Highlight;
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private TMP_Text messageText;

    private CardData selectedCard = null;
    private CardData option1 = null;
    private CardData option2 = null;
    private HorizontalCardHolder cardHolder;
    private GameObject stationPanel;

    void Start()
    {
        cardHolder = FindObjectOfType<HorizontalCardHolder>(true);
        if (gameController == null)
            gameController = FindObjectOfType<GameController>();

        card1Highlight.gameObject.SetActive(false);
        card2Highlight.gameObject.SetActive(false);
        confirmButton.SetActive(false);
    }

    public void OpenCardPicker()
    {
        if (gameController._energy < 1)
        {
            if (messageText != null)
            {
                messageText.text = "Not enough energy!";
                messageText.gameObject.SetActive(true);
                StartCoroutine(HideMessage());
            }
            return;
        }

        // find StationPanel even if inactive
        if (stationPanel == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>(true);
            if (canvas != null)
                stationPanel = canvas.transform.Find("StationPanel")?.gameObject;
        }

        // enable StationPanel so cards work
        if (stationPanel != null)
        {
            stationPanel.SetActive(true);
            canvas.UIBackground(true);

            // hide station specific elements
            GameObject weaponSlot = stationPanel.transform.Find("WeaponSlot")?.gameObject;
            if (weaponSlot != null) weaponSlot.SetActive(false);
        }

        // get 2 random cards from deck
        List<CardData> shuffled = new List<CardData>(deck.cards);
        for (int i = 0; i < shuffled.Count; i++)
        {
            CardData temp = shuffled[i];
            int randomIndex = Random.Range(i, shuffled.Count);
            shuffled[i] = shuffled[randomIndex];
            shuffled[randomIndex] = temp;
        }

        option1 = shuffled[0];
        option2 = shuffled[1];

        card1Image.sprite = option1.cardArt;
        card2Image.sprite = option2.cardArt;

        selectedCard = null;
        card1Highlight.gameObject.SetActive(false);
        card2Highlight.gameObject.SetActive(false);
        confirmButton.SetActive(false);

        cardPickerPanel.SetActive(true);
    }

    public void SelectCard1()
    {
        selectedCard = option1;
        card1Highlight.gameObject.SetActive(true);
        card2Highlight.gameObject.SetActive(false);
        confirmButton.SetActive(true);
    }

    public void SelectCard2()
    {
        selectedCard = option2;
        card2Highlight.gameObject.SetActive(true);
        card1Highlight.gameObject.SetActive(false);
        confirmButton.SetActive(true);
    }

    public void ConfirmSelection()
    {
        if (selectedCard == null) return;
        if (cardHolder == null)
        {
            cardHolder = FindObjectOfType<HorizontalCardHolder>(true);
            if (cardHolder == null) return;
        }

        // deduct energy
        gameController._energy -= 1;

        // add card to hand
        cardHolder.AddCardToHand(selectedCard);

        StartCoroutine(CloseAfterDelay());
    }

    IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(0.4f);
        canvas.UIBackground(false);
        cardPickerPanel.SetActive(false);

        // restore hidden elements then close station panel
        if (stationPanel != null)
        {
            
            GameObject weaponSlot = stationPanel.transform.Find("WeaponSlot")?.gameObject;
            if (weaponSlot != null) weaponSlot.SetActive(true);

            stationPanel.SetActive(false);
        }

    }

    IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(2f);
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }
    public void CloseCardPicker()
{
    if (cardPickerPanel.activeSelf)
    {
        cardPickerPanel.SetActive(false);
        
        if (stationPanel != null)
        {
            GameObject weaponSlot = stationPanel.transform.Find("WeaponSlot")?.gameObject;
            if (weaponSlot != null) weaponSlot.SetActive(true);

            canvas.UIBackground(false);
            stationPanel.SetActive(false);
        }
    }
}
}