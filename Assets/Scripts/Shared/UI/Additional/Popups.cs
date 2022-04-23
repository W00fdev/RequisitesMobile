using UnityEngine.EventSystems;
using UnityEngine;

namespace Assets.Scripts.Shared
{
    public class Popups : MonoBehaviour, IPointerClickHandler
    {
        public Animator PopupAnimator;
        public string ExitTriggerName = "Running";

        public void OnPointerClick(PointerEventData eventData)
        {
            PopupAnimator.SetTrigger(ExitTriggerName);
        }

        private void Start()
        {
            if (PopupAnimator == null || ExitTriggerName == null || ExitTriggerName == "")
            {
                Debug.LogError("Popup don't initialized.");
            }
        } 
    }
}

