using UnityEngine;
using TMPro;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultText;

    // Call this when round ends
    public void ShowResult(string result)
    {
        if (resultPanel != null && resultText != null)
        {
            resultPanel.SetActive(true);

            switch (result.ToLower())
            {
                case "You Won!":
                    resultText.text = "<color=#00FF00><b>YOU WIN!</b></color>";
                    break;

                case "You Lost!":
                    resultText.text = "<color=#FF0000><b>YOU LOSE!</b></color>";
                    break;

                case "Busted!":
                    resultText.text = "<color=#FFA500><b>BUST!</b></color>";
                    break;

                default:
                    resultText.text = result;
                    break;
            }
        }
    }

    public void HideResult()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
    }
}
