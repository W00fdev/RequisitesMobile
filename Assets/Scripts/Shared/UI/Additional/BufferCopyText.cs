using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

namespace Assets.Scripts.Shared
{
    [RequireComponent(typeof(Image))]
    public class BufferCopyText : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Animator _animatorPopup;
        [SerializeField] private string _popupTriggerName = "Buffer";
        [SerializeField] private float _clickedColorDelay = 0.2f;

        private Image _image;
        private TMP_Text _text;

        private readonly Color32 _mouseExitColor = new Color32(255, 255, 255, 255);
        private readonly Color32 _mouseEnterColor = new Color32(225, 225, 225, 255);
        private readonly Color32 _mouseClickColor = new Color32(205, 205, 205, 255);

        private Coroutine _clickedCoroutine = null;
        private bool _isClicked = false;

        private void Start()
        {
            _image = GetComponent<Image>();
            _text = transform.GetChild(0).GetComponent<TMP_Text>();

            _image.color = _mouseExitColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GUIUtility.systemCopyBuffer = _text.text;
            _animatorPopup.SetTrigger(_popupTriggerName);


            if (_clickedCoroutine != null)
            {
                StopCoroutine(_clickedCoroutine);
            }

            _isClicked = true;
            _clickedCoroutine = StartCoroutine(ClickedCoroutine());

            _image.color = _mouseClickColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isClicked == false)
            {
                _image.color = _mouseEnterColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isClicked == false)
            {
                _image.color = _mouseExitColor;
            }
        }

        private IEnumerator ClickedCoroutine()
        {
            yield return new WaitForSeconds(_clickedColorDelay);
            _isClicked = false;
            _clickedCoroutine = null;
        }
    }
}