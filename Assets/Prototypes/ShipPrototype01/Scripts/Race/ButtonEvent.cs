using System;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IButtonEventHandler : IPointerDownHandler, IPointerUpHandler,
    IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler { }

public class ButtonEvent : MonoBehaviour, IButtonEventHandler
{
    [field:SerializeField] public bool isPressed { get; set; }
    [field:SerializeField] public bool isHover { get; set; }
    
    public Action OnButtonDown { get; set; }
    public Action OnButtonUp { get; set; }
    public Action OnButtonClick { get; set; }
    public Action OnButtonEnter { get; set; }
    public Action OnButtonExit { get; set; }
    public Action<Vector2> OnButtonMove { get; set; }
    public Action OnButtonUpdate { get; set; }
    
    
    // MONOBEHAVIOUR METHODS
    private void OnEnable()
    {
        OnButtonDown += InvokeOnButtonUpdate;
        OnButtonUp += InvokeOnButtonUpdate;
        OnButtonEnter += InvokeOnButtonUpdate;
        OnButtonExit += InvokeOnButtonUpdate;
    }
    
    private void OnDisable()
    {
        OnButtonDown += InvokeOnButtonUpdate;
        OnButtonUp += InvokeOnButtonUpdate;
        OnButtonEnter += InvokeOnButtonUpdate;
        OnButtonExit += InvokeOnButtonUpdate;
    }
    
    
    // BUTTON EVENT HANDLER METHODS
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        OnButtonDown?.Invoke();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        OnButtonUp?.Invoke();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnButtonClick?.Invoke();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;
        OnButtonEnter?.Invoke();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
        OnButtonExit?.Invoke();
    }
    
    public void OnPointerMove(PointerEventData eventData)
    {
        OnButtonMove?.Invoke(eventData.position);
    }
    
    public void InvokeOnButtonUpdate()
    {
        OnButtonUpdate?.Invoke();
    }
}
