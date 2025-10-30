using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public delegate void UpdateSquareNumber(int number);

    public static event UpdateSquareNumber OnUpdateSquareNumber;

    public static void UpdateSquareNumberMethod(int number)
    {
        if (OnUpdateSquareNumber != null)
        {
            OnUpdateSquareNumber(number);
        }
    }

    public delegate void SquareSelected(int square_index);
    public static event SquareSelected OnSquareSelected;
    public static void SquareSelectedMethod(int square_index)
    {
        if (OnSquareSelected != null)
        {
            OnSquareSelected(square_index);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
