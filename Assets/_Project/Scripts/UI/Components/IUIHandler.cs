using UnityEngine.EventSystems;

namespace ProjectRacingSky.UI
{
    public interface IInteractableHandler :
        IClickableHandler, IHoverableHandler { }
    
    public interface IClickableHandler :
        IPointerDownHandler, IPointerUpHandler { }
    
    public interface IHoverableHandler :
        IPointerEnterHandler, IPointerExitHandler { }
    
    public interface ISelectableHandler :
        ISelectHandler, IDeselectHandler { }
    
    public interface IConfirmableHandler :
        IPointerClickHandler, ISubmitHandler { }
    
    public interface IMovableHandler :
        IMoveHandler { }
    
}
