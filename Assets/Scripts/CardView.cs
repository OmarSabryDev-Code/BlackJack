using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public Text rankTopLeft;
    public Text suitCenter;
    public Image backgroundImage;   // card’s white panel
    public Sprite frontSprite;      // normal card front
    public Sprite backSprite;       // card back

    private bool isFaceDown = false;

    public void ShowCard(string rank, string suit, Color color, bool faceDown = false)
    {
        isFaceDown = faceDown;

        if (faceDown)
        {
            // Hide details, show back
            rankTopLeft.text = "";
            suitCenter.text = "";
            backgroundImage.sprite = backSprite;
        }
        else
        {
            // Show real card
            rankTopLeft.text = rank;
            suitCenter.text = suit;
            rankTopLeft.color = color;
            suitCenter.color = color;
            backgroundImage.sprite = frontSprite;
        }
    }

    public void Reveal(string rank, string suit, Color color)
    {
        ShowCard(rank, suit, color, false);
    }
}
