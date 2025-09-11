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
    [SerializeField] private ResponsiveGridLayout gridSettings; // Grid layout handler
    [SerializeField] private ComboManager comboManager;         // Combo feedback manager
    [SerializeField] private Transform cardHolder;              // Parent transform holding all cards
    [SerializeField] private GameObject cardPrefab;             // Prefab for a single card

    [Header("UI References")]
    [SerializeField] private TMP_Text gridSizeTxt;
    [SerializeField] private TMP_Text scoreTxt;
    [SerializeField] private TMP_Text turnsTxt;
    [SerializeField] private TMP_Text matchesTxt;

    private CardsData cardsData; // Reference to all card data (images + names)

    // Track card states
    private Dictionary<GameObject, bool> cardFaceUp = new Dictionary<GameObject, bool>(); // Whether card is face-up
    private Dictionary<GameObject, bool> cardMatched = new Dictionary<GameObject, bool>(); // Whether card is matched

    // Track currently flipped pair
    private List<GameObject> openPair = new List<GameObject>();

    // Stats
    private int score, turns, matches, comboCount;
    private int currentRows, currentColumns, currentPairCount;

    bool isGameOver;

    void Start()
    {
        cardsData = GetComponent<CardsData>(); // Cache card data component
    }

    public void ContinueGame()
    {
        if (SaveManager.DoesSaveFileExist())
        {
            LoadGame(); // Restore previous state
        }
        isGameOver = false;
    }

    #region Save & Load System

    // Saves the current game state
    void SaveGame()
    {
        SaveData savedata = new SaveData();

        // Save stats
        savedata.score = score;
        savedata.turns = turns;
        savedata.matches = matches;

        // Save grid setup
        savedata.rows = currentRows;
        savedata.columns = currentColumns;

        savedata.cardLayoutNames = new List<string>();
        savedata.cardIsMatched = new List<bool>();

        // Save each card’s name and matched state
        foreach (Transform cardTransform in cardHolder)
        {
            GameObject cardObj = cardTransform.gameObject;
            savedata.cardLayoutNames.Add(cardObj.name);
            if (cardMatched.Count > 0)
            {
                savedata.cardIsMatched.Add(cardMatched[cardObj]);
            }
            else
            {
                savedata.cardIsMatched.Add(false);
            }
        }

        SaveManager.SaveGame(savedata);
    }

    // Loads the game state from saved data
    void LoadGame()
    {
        SaveData loadedData = SaveManager.LoadGame();
        if (loadedData == null) return;

        ResetGame();

        // Restore stats
        score = loadedData.score;
        turns = loadedData.turns;
        matches = loadedData.matches;
        currentRows = loadedData.rows;
        currentColumns = loadedData.columns;
        currentPairCount = (currentRows * currentColumns) / 2;

        gridSizeTxt.text = "GRID : " + currentRows + "X" + currentColumns;
        gridSettings.columnNos = currentColumns;

        // Restore card layout
        for (int i = 0; i < loadedData.cardLayoutNames.Count; i++)
        {
            string cardName = loadedData.cardLayoutNames[i];
            bool isMatched = loadedData.cardIsMatched[i];

            GameObject newCard = Instantiate(cardPrefab, cardHolder);
            newCard.name = cardName;

            Image cardImage = newCard.GetComponent<Image>();
            Button cardButton = newCard.GetComponent<Button>();
            cardButton.onClick.AddListener(() => OnCardClicked(newCard));

            cardFaceUp[newCard] = false;
            cardMatched[newCard] = isMatched;

            // Show correct sprite based on match state
            if (isMatched)
            {
                cardImage.sprite = GetFrontSprite(cardName);
                cardButton.interactable = false;
            }
            else
            {
                cardImage.sprite = cardsData.cardBackImage;
            }
        }
        UpdateUI();
    }

    #endregion

    // Creates a new shuffled grid of cards
    public void DistributeCards(int rows, int columns)
    {
        isGameOver = false;
        currentRows = rows;
        currentColumns = columns;
        ResetGame();

        gridSizeTxt.text = "GRID : " + rows + "X" + columns;
        gridSettings.columnNos = columns;

        int totalCards = rows * columns;
        currentPairCount = totalCards / 2;

        if (cardsData.cards.Count < currentPairCount)
        {
            Debug.LogError("Not enough unique cards in CardsData to create the grid!");
            return;
        }

        // Select unique cards
        List<Cards> availableCards = new List<Cards>(cardsData.cards);
        List<Cards> selectedCards = new List<Cards>();
        for (int i = 0; i < currentPairCount; i++)
        {
            int randomIndex = Random.Range(0, availableCards.Count);
            selectedCards.Add(availableCards[randomIndex]);
            availableCards.RemoveAt(randomIndex);
        }

        // Duplicate to make pairs
        List<Cards> finalDeck = new List<Cards>();
        finalDeck.AddRange(selectedCards);
        finalDeck.AddRange(selectedCards);

        // Shuffle deck
        finalDeck = finalDeck.OrderBy(x => System.Guid.NewGuid()).ToList();

        // Instantiate cards in grid
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
        SaveGame(); // Save initial state
    }

    // Clears all cards and resets stats
    public void ResetGame()
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

    // Updates UI values
    void UpdateUI()
    {
        turnsTxt.text = turns.ToString();
        scoreTxt.text = score.ToString();
        matchesTxt.text = matches.ToString();
    }

    // Called when a card is clicked
    public void OnCardClicked(GameObject cardObj)
    {
        // Ignore if already matched, already face up, or already two cards open
        if (cardMatched[cardObj] || cardFaceUp[cardObj] || openPair.Count >= 2) return;

        // Flip card face up
        StartCoroutine(FlipCoroutine(cardObj, GetFrontSprite(cardObj.name), true));

        openPair.Add(cardObj);

        // If 2 cards are flipped, check for a match
        if (openPair.Count == 2)
        {
            turns++;
            StartCoroutine(CheckMatch(openPair[0], openPair[1]));
            openPair.Clear();
        }
    }

    // Checks if two cards match after flipping
    private IEnumerator CheckMatch(GameObject c1, GameObject c2)
    {
        yield return new WaitForSeconds(flipDuration); // Wait until flip animations finish

        if (c1 == null || c2 == null) yield break;

        if (c1.name == c2.name)
        {
            // Cards matched
            cardMatched[c1] = true;
            cardMatched[c2] = true;
            matches++;
            SoundManager.instance.PlaySFX("match");

            if (matches >= currentPairCount)
            {
                Matchfinished();
            }

            comboCount++;
            score += 10 * comboCount;
            comboManager.StartCounter(comboCount);
        }
        else
        {
            // Not a match, flip them back
            comboCount = 0;
            StartCoroutine(FlipCoroutine(c1, cardsData.cardBackImage, false));
            StartCoroutine(FlipCoroutine(c2, cardsData.cardBackImage, false));
        }

        UpdateUI();

        if (!isGameOver)
        {
            SaveGame(); // Save progress after each turn
        }
    }

    // Finds the front image for a card by name
    private Sprite GetFrontSprite(string cardName)
    {
        Cards cardInfo = cardsData.cards.Find(c => c.CardName == cardName);
        if (cardInfo != null)
            return cardInfo.frontImage;

        Debug.LogWarning("No front sprite for card: " + cardName);
        return null;
    }

    // Handles flip animation
    private IEnumerator FlipCoroutine(GameObject card, Sprite newSprite, bool faceUpAfter = false)
    {
        if (card == null || newSprite == null) yield break;
        SoundManager.instance.PlaySFX("flip");

        Image cardImage = card.GetComponent<Image>();
        Vector3 originalScale = card.transform.localScale;
        Vector3 targetScale = new Vector3(0, originalScale.y, originalScale.z);
        float elapsed = 0f;

        // Shrink animation
        while (elapsed < flipDuration / 2)
        {
            if (card == null) yield break;
            card.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / (flipDuration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (card == null) yield break;
        card.transform.localScale = targetScale;

        // Swap sprite
        cardImage.sprite = newSprite;

        // Expand animation
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

        // Update card state
        if (card != null)
        {
            cardFaceUp[card] = faceUpAfter;
        }
    }

    // Handles when all pairs are matched
    void Matchfinished()
    {
        isGameOver = true;
        SoundManager.instance.PlaySFX("finish");
        SaveManager.DeleteSaveData();
    }
}
