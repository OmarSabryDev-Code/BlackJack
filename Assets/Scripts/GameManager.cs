using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs & References")]
    public GameObject cardPrefab;
    public Transform playerHand;
    public Transform dealerHand;
    public Transform deckPosition;
    

    [Header("Controls")]
    public Button hitButton;
    public Button stayButton;
    public Button restartButton;

    [Header("UI References")]
    public Text playerScoreText;
    public Text dealerScoreText;

    [Header("Chips & Betting")]
    public Text balanceText;
    public Text betText;

    public Button chip10Button;
    public Button chip50Button;
    public Button chip100Button;

    public int playerBalance = 500;
    public int currentBet = 0;
    public RectTransform chipSpawnPoint; // <-- UI spawn point (off-screen or above table)

    public Transform chip10;
    public Transform chip50;
    public Transform chip100;
    public Transform betArea; // middle of table where chips land
    [SerializeField] private ResultUI resultUI; // drag UIManager here in Inspector

    private Deck deck;
    private List<CardData> playerCards = new List<CardData>();
    private List<CardData> dealerCards = new List<CardData>();
    private CardView hiddenDealerCard;

    private bool playerTurn = true;
    private int chipStackCount = 0;
    //Dealer's Animation
    private int dealerChipStackCount = 0;
    [Header("Dealer Betting")]
    public RectTransform dealerChipSpawnPoint; // where dealer chips appear (offscreen or above dealer)
    public RectTransform dealerBetArea;        // where dealer chips land on the table


    void Start()
    {
        chip10Button.onClick.AddListener(() => PlaceBet(10));
        chip50Button.onClick.AddListener(() => PlaceBet(50));
        chip100Button.onClick.AddListener(() => PlaceBet(100));

        UpdateBetUI();

        deck = new Deck();

        // Initial deal
        for (int i = 0; i < 2; i++)
            DealCard(playerHand, playerCards);

        DealCard(dealerHand, dealerCards, true);
        DealCard(dealerHand, dealerCards, false);

        UpdateScores();

        hitButton.onClick.AddListener(OnHit);
        stayButton.onClick.AddListener(OnStay);
        //restartButton.onClick.AddListener(OnRestart);
    }

    #region Cards
    void OnHit()
    {
        if (!playerTurn) return;

        DealCard(playerHand, playerCards);
        UpdateScores();

        if (CalculateScore(playerCards) > 21)
        {
            playerTurn = false;
            EndRound("Player Busts! Dealer Wins!");
        }
    }

    void OnStay()
    {
        if (!playerTurn) return;

        playerTurn = false;

        if (hiddenDealerCard != null)
        {
            CardData realCard = dealerCards[0];
            hiddenDealerCard.transform.DORotate(new Vector3(0, 90, 0), 0.2f).OnComplete(() =>
            {
                

                hiddenDealerCard.Reveal(realCard.rank, realCard.suit, realCard.color);
                hiddenDealerCard.transform.DORotate(Vector3.zero, 0.2f);
            });

        }

        StartCoroutine(DealerTurn());
        
    }

    IEnumerator DealerTurn()
    {
        yield return new WaitForSeconds(1f);

        while (CalculateScore(dealerCards) < 17)
        {
            DealCard(dealerHand, dealerCards);
            UpdateScores();
            yield return new WaitForSeconds(1f);
        }

        EndRound(DetermineWinner());
    }

    void DealCard(Transform parent, List<CardData> hand, bool faceDown = false)
    {
        CardData data = deck.DrawCard();
        if (data == null) return;

        hand.Add(data);

        GameObject cardObj = Instantiate(cardPrefab, deckPosition.position, Quaternion.identity, parent);
        CardView view = cardObj.GetComponent<CardView>();

        view.ShowCard(data.rank, data.suit, data.color, true);

        cardObj.transform.DOMove(parent.position, 0.6f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            if (faceDown && parent == dealerHand)
            {
                hiddenDealerCard = view;
            }
            else
            {
                cardObj.transform.DORotate(new Vector3(0, 90, 0), 0.2f).OnComplete(() =>
                {
                    
                    view.ShowCard(data.rank, data.suit, data.color, false);
                    cardObj.transform.DORotate(Vector3.zero, 0.2f);
                });
            }
        });
    }
    #endregion
void AnimateDealerChip(int amount)
{
    Button sourceChip = null;
    if (amount == 10) sourceChip = chip10Button;
    else if (amount == 50) sourceChip = chip50Button;
    else if (amount == 100) sourceChip = chip100Button;

    if (sourceChip != null)
    {
        // Spawn dealer chip as clone under Canvas
        GameObject chipClone = Instantiate(sourceChip.gameObject, dealerChipSpawnPoint);
        RectTransform rect = chipClone.GetComponent<RectTransform>();

        // Re-parent to Canvas (or betArea parent) to ensure it’s visible
        chipClone.transform.SetParent(dealerBetArea.parent, false);

        // Target position: horizontal stacking in dealer bet area
        Vector2 targetPos = dealerBetArea.anchoredPosition + new Vector2(dealerChipStackCount * 50f, 0);
        dealerChipStackCount++;

        // Animate movement and rotation
        rect.DOAnchorPos(targetPos, 0.5f).SetEase(Ease.OutQuad);
        rect.DORotate(new Vector3(0, 0, Random.Range(0, 360)), 0.5f, RotateMode.FastBeyond360);
    }
}

void DealerPlaceBet()
{
    int dealerBet = 50; // for example
    AnimateDealerChip(dealerBet);
}
void ClearDealerBetArea()
{
    foreach (Transform child in dealerBetArea.parent)
    {
        if (child != dealerBetArea && child != dealerChipSpawnPoint)
            Destroy(child.gameObject);
    }
    dealerChipStackCount = 0;
}



    #region Betting & Chips
    public void PlaceBet(int amount)
    {
        if (playerBalance >= amount)
        {
            playerBalance -= amount;
            currentBet += amount;
            UpdateBetUI();
            AnimateChip(amount);
        }
    }

    void AnimateChip(int amount)
{
    Button sourceChip = null;
    if (amount == 10) sourceChip = chip10Button;
    else if (amount == 50) sourceChip = chip50Button;
    else if (amount == 100) sourceChip = chip100Button;

    if (sourceChip != null)
    {
        // Spawn new chip as button clone
        GameObject chipClone = Instantiate(sourceChip.gameObject, chipSpawnPoint);
        RectTransform rect = chipClone.GetComponent<RectTransform>();

        // Parent to the main Canvas (so it's visible)
        chipClone.transform.SetParent(betArea.parent, false);

        // Target position: horizontal stack in BetArea
        Vector2 targetPos = betArea.GetComponent<RectTransform>().anchoredPosition + new Vector2(chipStackCount * 50f, 0);
        chipStackCount++;

        // Animate UI button from spawn to bet area
        rect.DOAnchorPos(targetPos, 0.5f).SetEase(Ease.OutQuad);
        rect.DORotate(new Vector3(0, 0, Random.Range(0, 360)), 0.5f, RotateMode.FastBeyond360);
    }
}


    void ClearBetArea()
{
    foreach (Transform child in betArea.parent)
    {
        if (child != betArea && child != chipSpawnPoint)
            Destroy(child.gameObject);
    }
    chipStackCount = 0;
}


    void UpdateBetUI()
    {
        balanceText.text = "Balance: $" + playerBalance;
        betText.text = "Bet: $" + currentBet;
    }
    #endregion

    #region Scores & Round
    int CalculateScore(List<CardData> hand)
    {
        int score = 0;
        int aceCount = 0;

        foreach (CardData card in hand)
        {
            if (card.rank == "J" || card.rank == "Q" || card.rank == "K")
                score += 10;
            else if (card.rank == "A")
            {
                score += 11;
                aceCount++;
            }
            else
                score += int.Parse(card.rank);
        }

        while (score > 21 && aceCount > 0)
        {
            score -= 10;
            aceCount--;
        }

        return score;
    }

    string DetermineWinner()
    {
        int playerScore = CalculateScore(playerCards);
        int dealerScore = CalculateScore(dealerCards);

        if (dealerScore > 21) return "Dealer Busts! Player Wins!";
        if (playerScore > dealerScore) return "Player Wins!";
        if (dealerScore > playerScore) return "Dealer Wins!";
        return "Push (Draw)";
    }

    void EndRound(string result)
{
    Debug.Log(result);

    // 💰 Balance handling
    if (result.Contains("Player Wins"))
        playerBalance += currentBet * 2;
    else if (result.Contains("Push"))
        playerBalance += currentBet;

    currentBet = 0;
    UpdateBetUI();

    // 🎮 Disable buttons
    hitButton.interactable = false;
    stayButton.interactable = false;
    chipStackCount = 0;

    // 🖼️ Show UI message
    if (resultUI != null)
    {
        if (result.Contains("Player Wins"))
            resultUI.ShowResult("You Win!");
        else if (result.Contains("Dealer Wins"))
            resultUI.ShowResult("You Lost!");
        else if (result.Contains("Bust"))
            resultUI.ShowResult("Busted!");
        else if (result.Contains("Push"))
            resultUI.ShowResult("Push");
    }
}


    void UpdateScores()
{
    // Always show player’s score
    playerScoreText.text = "Player: " + CalculateScore(playerCards);

    // If it’s still the player’s turn, hide dealer score
    if (playerTurn)
    {
        dealerScoreText.text = "Dealer: ?";
    }
    else
    {
        dealerScoreText.text = "Dealer: " + CalculateScore(dealerCards);
    }
}

    #endregion

    void OnRestart()
    {
        ClearBetArea();
        SceneManager.LoadScene(0);
    }
}
