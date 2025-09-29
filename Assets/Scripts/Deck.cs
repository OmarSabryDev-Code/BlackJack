using UnityEngine;
using System.Collections.Generic;

public class Deck
{
    private List<CardData> cards;
    private System.Random rng = new System.Random();

    public Deck()
    {
        cards = new List<CardData>();
        GenerateDeck();
        Shuffle();
    }

    private void GenerateDeck()
    {
        string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        string[] suits = { "♠", "♥", "♦", "♣" };

        foreach (string suit in suits)
        {
            Color suitColor = (suit == "♥" || suit == "♦") ? Color.red : Color.black;

            foreach (string rank in ranks)
            {
                cards.Add(new CardData(rank, suit, suitColor));
            }
        }
    }

    public void Shuffle()
    {
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            CardData value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }

    public CardData DrawCard()
    {
        if (cards.Count == 0) return null;

        CardData card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public int Count => cards.Count;
}
