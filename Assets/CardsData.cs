using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Cards
{
    public Sprite frontImage;
    public string CardName;
}

public class CardsData : MonoBehaviour
{
    [Tooltip("The default image for the back of all cards.")]
    public Sprite cardBackImage; // This line is new

    [Tooltip("A list of all unique cards in the game.")]
    public List<Cards> cards;
}