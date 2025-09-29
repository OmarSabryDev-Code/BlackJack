public class CardData
{
    public string rank;      // "A", "2", ..., "K"
    public string suit;      // "♠", "♥", "♦", "♣"
    public UnityEngine.Color color; // red or black

    public CardData(string r, string s, UnityEngine.Color c)
    {
        rank = r;
        suit = s;
        color = c;
    }
}
