using UnityEngine;

public enum StationType
{
    Any,
    Weapons,
    Shields,
    Engine,
    Bridge,
    Comms
}

public enum CardRequirement
{
    None,
    StationDamaged,
    StationHealthy,
    PassageDamaged
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/CardData")]
public class CardData : ScriptableObject
{
    [Header("Card Info")]
    public string cardName;
    public Sprite cardArt;
    [TextArea] public string cardDescription;

    [Header("Cost & Restrictions")]
    public int energyCost;
    public StationType allowedStation;
    public CardRequirement requirement;
}