using UnityEngine.EventSystems;

namespace ProjectRacingSky.UI
{
    // POINTER EVENT SEQUENCE: > ENTER, v DOWN, ^ UP, < EXIT
    public interface ITouchControlHandler
        : IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public void OnButtonPress();
        public void OnButtonRelease();
        public void OnButtonOverrideHold();
        public void OnButtonReturnHold();
    }
}
