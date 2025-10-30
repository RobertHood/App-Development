using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class GridSquare : Selectable, IPointerClickHandler, ISubmitHandler, IPointerUpHandler, IPointerExitHandler
{
    public GameObject number_text;
    private int number_ = 0;
    private bool selected_ = false;

    private int square_index = -1;
    private bool IsSelected() { return selected_; }
    public void SetSquareIndex(int index)
    {
        square_index = index;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selected_ = false;
    }
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

    public void OnPointerClick(PointerEventData eventData)
    {
        selected_ = true;
        GameEvents.SquareSelectedMethod(square_index);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        
    }

    private void OnEnable()
    {
        GameEvents.OnUpdateSquareNumber += OnSetNumber;
        GameEvents.OnSquareSelected += OnSquareSelected;
    }

    private void OnDisable()
    {
        GameEvents.OnUpdateSquareNumber += OnSetNumber;
        GameEvents.OnSquareSelected += OnSquareSelected;
    }
    public void SetNumber(int number)
    {
        number_ = number;
        DisplayText();
    }

    public void OnSetNumber(int number)
    {
        if (selected_)
        {
            SetNumber(number);
        }
    }
    
    public void OnSquareSelected(int square_index_)
    {
        if (square_index != square_index_)
        {
            selected_ = false;
        }
    }
}
