using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class NumberButtons : Selectable, IPointerClickHandler, ISubmitHandler, IPointerUpHandler, IPointerExitHandler
{
    public int value = 0;
    public void OnPointerClick(PointerEventData eventData)
    {
        GameEvents.UpdateSquareNumberMethod(value);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
