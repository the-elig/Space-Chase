using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeck", menuName = "Cards/Deck")]
public class Deck : ScriptableObject
{
    public List<CardData> cards;
}
