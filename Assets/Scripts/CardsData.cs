using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // Makes the class visible in the Inspector for editing
public class Cards
{
    public Sprite frontImage;   // The front image of the card
    public string CardName;     // The name/label of the card
}

public class CardsData : MonoBehaviour
{
    [Tooltip("The default image for the back of all cards.")]
    public Sprite cardBackImage; // Shared back image used for every card

    [Tooltip("A list of all unique cards in the game.")]
    public List<Cards> cards;    // Collection of all available cards
}
