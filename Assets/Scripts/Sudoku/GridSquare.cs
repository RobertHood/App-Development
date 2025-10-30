using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : Selectable
{
    public GameObject number_text;
    private int number_ = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame
    public void DisplayText()
    {
        if (number_ <= 0)
        {
            number_text.GetComponent<TextMeshProUGUI>().text = " ";
        }
        else
            number_text.GetComponent<TextMeshProUGUI>().text = number_.ToString();
    }
    
    public void SetNumber(int number)
    {
        number_ = number;
        DisplayText();
    }
}
