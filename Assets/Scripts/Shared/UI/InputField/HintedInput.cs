using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Core;
using TMPro;

namespace Assets.Scripts.Shared
{
    public abstract class HintedInput : MonoBehaviour
    {
        [SerializeField] protected TMP_InputField InputField;
        [SerializeField] protected int CharacterLimit;

        public HintedDropdown Dropdown;
        public TMP_Text HintText;
        public bool Initialized = false;

        protected ParserResponse DropdownResponse;
        protected string PreviousInput;

        public void Initialize()
        {
            Initialized = true;
            SwitchState(true);
        }
        public virtual void SwitchState(bool enabled)
        {
            //InputField.text = "";
            InputField.interactable = enabled;

            if (enabled)
                InputField.ActivateInputField();
            else
                InputField.DeactivateInputField();
        }

        public void Select()
        {
            InputField.Select();

            InputField.selectionAnchorPosition = 0;
            InputField.selectionFocusPosition = InputField.text.Length;
            InputField.caretPosition = InputField.text.Length;
        }

        public abstract void UpdateInput(string newInput);

        protected abstract void ChangeValueEvent(string newInput);
        protected abstract void SelectEvent(string newInput);
        protected abstract void UpdateHint();
    }

}