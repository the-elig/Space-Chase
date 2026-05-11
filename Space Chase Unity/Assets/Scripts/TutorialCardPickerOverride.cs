using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCardPickerOverride : MonoBehaviour
{
    private CardPickerUI cardPickerUI;
    private CardData repairCard;
    private CardData commsCard;
    private bool overrideActive = true;

    public void Initialize(CardPickerUI picker, CardData repair, CardData comms)
    {
        cardPickerUI = picker;
        repairCard = repair;
        commsCard = comms;
    }

    public void OpenTutorialCardPicker()
    {
        if (!overrideActive)
        {
            cardPickerUI.OpenCardPicker();
            return;
        }

        Deck deck = cardPickerUI.GetDeck();
        if (deck == null)
        {
            cardPickerUI.OpenCardPicker();
            return;
        }

        List<CardData> originalCards = new List<CardData>(deck.cards);
        deck.cards = new List<CardData> { repairCard, commsCard };
        cardPickerUI.OpenCardPicker();
        StartCoroutine(RestoreDeck(deck, originalCards));
    }

    private IEnumerator RestoreDeck(Deck deck, List<CardData> original)
    {
        yield return null;
        deck.cards = original;
    }

    public void DisableOverride() => overrideActive = false;
}