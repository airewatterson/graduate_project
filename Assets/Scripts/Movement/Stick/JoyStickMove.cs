using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;

namespace Movement.Stick
{
    public class JoyStickMove : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public Transform stickBack;
        public Transform stickButton;
        public void OnPointerDown(PointerEventData eventData)
        {
            stickBack.gameObject.SetActive(true);
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);

            stickBack.transform.localPosition = m_PointerDownPos;
            //stickButton.transform.localPosition = m_PointerDownPos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
            var delta = position - m_PointerDownPos;

            delta = Vector2.ClampMagnitude(delta, movementRange);
            ((RectTransform)stickButton).anchoredPosition = m_StartPos + (Vector3)delta;

            var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
            SendValueToControl(newPos);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ((RectTransform)stickButton).anchoredPosition = m_StartPos;
            SendValueToControl(Vector2.zero);
            stickBack.gameObject.SetActive(false);
        }

        private void Start()
        {
            m_StartPos = ((RectTransform)stickButton).anchoredPosition;
        }

        public float movementRange
        {
            get => m_MovementRange;
            set => m_MovementRange = value;
        }

        [FormerlySerializedAs("movementRange")]
        [SerializeField]
        private float m_MovementRange = 50;

        [InputControl(layout = "Vector2")]
        [SerializeField]
        private string m_ControlPath;

        private Vector3 m_StartPos;
        private Vector2 m_PointerDownPos;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
    }
}
