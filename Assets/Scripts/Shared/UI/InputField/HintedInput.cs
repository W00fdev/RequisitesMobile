using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Core;
using TMPro;

namespace Assets.Scripts.Shared
{
    public abstract class HintedInput : MonoBehaviour
    {
        public bool DebugOutput;

        [SerializeField] protected TMP_InputField InputField;
        [SerializeField] protected int CharacterLimit;

        public HintedDropdown Dropdown;
        public TMP_Text HintText;
        public bool Initialized = false;

        protected DataInputRequisites Data;
        protected string CurrentInput = "";

        public void Initialize(DataInputRequisites data)
        {
            Data = data;
            Initialized = true;
            SwitchState(true);

            if (DebugOutput)
                Data.DataIfns.Debug();

            Dropdown.Initialize(data);
            Dropdown.Show();
        }
        public virtual void SwitchState(bool enabled)
        {
            InputField.text = "";
            InputField.interactable = enabled;

            if (enabled)
                InputField.ActivateInputField();
            else
                InputField.DeactivateInputField();
        }

        public abstract void UpdateInput(string newInput);

        protected abstract void ChangeValueEvent(string newInput);
        protected abstract void UpdateHint(string hint);
    }

}