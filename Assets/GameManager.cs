using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("How long the flip animation should take in seconds.")]
    public float flipDuration = 0.4f;

    [Header("Object References")]
    [SerializeField] private ResponsiveGridLayout gridSettings;
    [SerializeField] private Transform cardHolder;
    [SerializeField] private GameObject cardPrefab;

    [Header("UI References")]
    [SerializeField] private TMP_Text gridSizeTxt;
    [SerializeField] private TMP_Text scoreTxt;
    [SerializeField] private TMP_Text turnsTxt;
    [SerializeField] private TMP_Text matchesTxt;

    private CardsData cardsData;

    // Track states
    private Dictionary<GameObject, bool> cardFaceUp = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> cardMatched = new Dictionary<GameObject, bool>();

    // Track current open pair
    private List<GameObject> openPair = new List<GameObject>();

    private int score, turns, matches, comboCount;

    private int currentPairCount;

    void Start()
    {
        cardsData = GetComponent<CardsData>();
        DistributeCards(2, 3); 
    }

    public void DistributeCards(int rows, int columns)
    {
        ResetGame();
        gridSizeTxt.text = "GRID : " + rows + "X" + columns;
        gridSettings.columnNos = columns;
        currentPairCount = 0;
        int totalCards = rows * columns;
        currentPairCount = totalCards / 2;

        if (cardsData.cards.Count < currentPairCount)
        {
            Debug.LogError("Not enough unique cards in CardsData to create the grid!");
            return;
        }

        // pick unique cards
        List<Cards> availableCards = new List<Cards>(cardsData.cards);
        List<Cards> selectedCards = new List<Cards>();
        for (int i = 0; i <currentPairCount; i++)
        {
            int randomIndex = Random.Range(0, availableCards.Count);
            selectedCards.Add(availableCards[randomIndex]);
            availableCards.RemoveAt(randomIndex);
        }

        // double them
        List<Cards> finalDeck = new List<Cards>();
        finalDeck.AddRange(selectedCards);
        finalDeck.AddRange(selectedCards);

        // shuffle
        finalDeck = finalDeck.OrderBy(x => System.Guid.NewGuid()).ToList();

        // instantiate cards
        for (int i = 0; i < totalCards; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardHolder);
            newCard.name = finalDeck[i].CardName;

            Image cardImage = newCard.GetComponent<Image>();
            if (cardImage != null && cardsData.cardBackImage != null)
                cardImage.sprite = cardsData.cardBackImage;

            Button cardButton = newCard.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(() => OnCardClicked(newCard));
            }

            cardFaceUp[newCard] = false;
            cardMatched[newCard] = false;
        }
    }

    void ResetGame()
    {
        foreach (Transform child in cardHolder)
        {
            Destroy(child.gameObject);
        }
        cardFaceUp.Clear();
        cardMatched.Clear();
        openPair.Clear();

        score = 0;
        turns = 0;
        matches = 0;
        comboCount = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        turnsTxt.text = turns.ToString();
        scoreTxt.text = score.ToString();
        matchesTxt.text = matches.ToString();
    }

    public void OnCardClicked(GameObject cardObj)
    {
        if (cardMatched[cardObj] || cardFaceUp[cardObj]) return;

        // Flip card up
        StartCoroutine(FlipCoroutine(cardObj, GetFrontSprite(cardObj.name), true));

        // Add to open pair
        openPair.Add(cardObj);

        if (openPair.Count == 2)
        {
            turns++;
            StartCoroutine(CheckMatch(openPair[0], openPair[1]));
            openPair.Clear(); 
        }
    }

    private IEnumerator CheckMatch(GameObject c1, GameObject c2)
    {
        yield return new WaitForSeconds(flipDuration); // let flip animations finish

        if (c1 == null || c2 == null) yield break;

        if (c1.name == c2.name)
        {
            
            cardMatched[c1] = true;
            cardMatched[c2] = true;
            matches++;
            SoundManager.instance.PlaySFX("match");
            if(matches >= currentPairCount)
            {
                Matchfinished();
            }
            score += 10;
            comboCount++;
        }
        else
        {
            comboCount = 0;
            StartCoroutine(FlipCoroutine(c1, cardsData.cardBackImage, false));
            StartCoroutine(FlipCoroutine(c2, cardsData.cardBackImage, false));
        }

        UpdateUI();
    }

    private Sprite GetFrontSprite(string cardName)
    {
        Cards cardInfo = cardsData.cards.Find(c => c.CardName == cardName);
        if (cardInfo != null)
            return cardInfo.frontImage;

        Debug.LogWarning("No front sprite for card: " + cardName);
        return null;
    }

    private IEnumerator FlipCoroutine(GameObject card, Sprite newSprite, bool faceUpAfter = false)
    {
        if (card == null || newSprite == null) yield break;
        SoundManager.instance.PlaySFX("flip");

        Image cardImage = card.GetComponent<Image>();
        Vector3 originalScale = card.transform.localScale;
        Vector3 targetScale = new Vector3(0, originalScale.y, originalScale.z);
        float elapsed = 0f;

        // shrink
        while (elapsed < flipDuration / 2)
        {
            if (card == null) yield break;
            card.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / (flipDuration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (card == null) yield break;
        card.transform.localScale = targetScale;

        // swap sprite
        cardImage.sprite = newSprite;

        // expand
        elapsed = 0f;
        while (elapsed < flipDuration / 2)
        {
            if (card == null) yield break;
            card.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / (flipDuration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (card != null)
            card.transform.localScale = originalScale;

        // update state
        cardFaceUp[card] = faceUpAfter;
    }

    void Matchfinished()
    {
        SoundManager.instance.PlaySFX("finish");
    }
}
