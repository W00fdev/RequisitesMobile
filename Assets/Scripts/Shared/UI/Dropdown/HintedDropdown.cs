using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Core;
using TMPro;

namespace Assets.Scripts.Shared
{
    public abstract class HintedDropdown : MonoBehaviour
    {
        [SerializeField] protected TMP_Dropdown Dropdown;
        [SerializeField] protected TMP_Text Label;

        public HintedInput Input;
        public bool Initialized = false;

        protected DataInputRequisites Data;

        public void Initialize(DataInputRequisites data)
        {
            Data = data;
            Initialized = true;

            SwitchState(true);
            UpdateDropdown("");
        }

        public virtual void SwitchState(bool enabled)
        {
            Dropdown.ClearOptions();
            Dropdown.interactable = enabled;

            if (enabled == false)
                Dropdown.Hide();
        }

        public virtual void Show() => Dropdown.Show();


        public abstract void UpdateDropdown(string newInput);
        protected abstract void ChooseValueEvent(TMP_Dropdown dropdown);
    }
}