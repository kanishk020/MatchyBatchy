using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("How long the flip animation should take in seconds.")]
    public float flipDuration = 0.4f;

    private CardsData cardsData;
    private Camera mainCamera;


    private List<GameObject> openedcards;


    [SerializeField] private ResponsiveGridLayout gridSettings;

    void Start()
    {
        mainCamera = Camera.main;


        cardsData = GetComponent<CardsData>();

    }

    public void DistributeCards(int rows, int columns)
    {
        gridSettings.columnNos = columns;

        Debug.Log("start game with rows :" + rows + "  columns:" + columns);
    }


    void Update()
    {
        // Check for mouse click or any screen touch
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector2 inputPosition = Input.GetMouseButtonDown(0) ? (Vector2)Input.mousePosition : Input.GetTouch(0).position;
            Debug.Log(inputPosition);
            HandleInteraction(inputPosition);
        }
    }

    private void HandleInteraction(Vector2 screenPosition)
    {

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);


        FlipCard(hit.collider.gameObject);

    }

    private void FlipCard(GameObject cardObject)
    {

        Image cardImage = cardObject.GetComponent<Image>();

        Sprite targetSprite = null;

        // Determine the target state by checking the current sprite.
        bool isShowingBack = (cardImage.sprite == cardsData.cardBackImage);

        if (isShowingBack)
        {
            // Find the front sprite from CardsData by matching the GameObject's name.
            string cardName = cardObject.name;
            Cards cardInfo = cardsData.cards.Find(c => c.CardName == cardName);

            if (cardInfo != null)
            {
                targetSprite = cardInfo.frontImage;
            }

        }
        else
        {
            // If it's showing the front, the target is the common back sprite.
            targetSprite = cardsData.cardBackImage;
        }

        // Start the animation if we have a valid sprite to flip to.
        if (targetSprite != null)
        {
            StartCoroutine(FlipCoroutine(cardObject, targetSprite));
        }
    }

    /// <summary>
    /// The animation coroutine that handles the visual flip effect.
    /// </summary>
    private IEnumerator FlipCoroutine(GameObject cardToFlip, Sprite newSprite)
    {


        Image cardImage = cardToFlip.GetComponent<Image>();
        Vector3 originalScale = cardToFlip.transform.localScale;
        Vector3 targetScale = new Vector3(0, originalScale.y, originalScale.z);
        float elapsedTime = 0f;

        // First half: Scale down
        while (elapsedTime < flipDuration / 2)
        {
            cardToFlip.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / (flipDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cardToFlip.transform.localScale = targetScale;

        // Mid-point: Swap sprite
        cardImage.sprite = newSprite;

        // Second half: Scale back up
        elapsedTime = 0f;
        while (elapsedTime < flipDuration / 2)
        {
            cardToFlip.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / (flipDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cardToFlip.transform.localScale = originalScale;

        if (cardToFlip.GetComponent<Image>().sprite != cardsData.cardBackImage)
        {
            openedcards.Add(cardToFlip);

        }



        CheckMatch(cardToFlip);


    }

    void CheckMatch(GameObject card)
    {
        if (openedcards.Count > 1)
        {
            if (openedcards[0].name == openedcards[1].name)
            {
                Matched();
            }
            else
            {

            }
        }

    }

    void Matched()
    {

    }
}
