using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Core;
using TMPro;

namespace Assets.Scripts.Shared
{
    public sealed class HintedInputIfns : HintedInput
    {
        void Start()
        {
            CharacterLimit = 4;

            InputField ??= GetComponent<TMP_InputField>();
            InputField.characterValidation = TMP_InputField.CharacterValidation.Digit;
            InputField.characterLimit = CharacterLimit;
            InputField.onSelect.AddListener(SelectEvent);
            InputField.onValueChanged.AddListener(ChangeValueEvent);


            SwitchState(false);
            UpdateHint(string.Empty);
        }

        public override void UpdateInput(string newInput)
        {            
            InputField.text = ParseIfns(newInput);
        }

        protected override void ChangeValueEvent(string newInput)
        {
            string hint = Dropdown.UpdateDropdown(newInput);
            Dropdown.Show();
            Dropdown.Refresh();

            //UpdateHint(ParseIfns(hint));
        }

        protected override void UpdateHint(string hint) => HintText.text = hint;

        private string ParseIfns(string source)
        {
            string parsedIfns = source.Substring(0, source.IndexOf(' '));

            if (parsedIfns.Length > CharacterLimit)
                Debug.LogError("Ifns option has more than 4 digits...");

            return parsedIfns;
        }

        protected override void SelectEvent(string newInput)
        {
            Dropdown.Show();
        }
    }
}