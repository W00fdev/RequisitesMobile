using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Core;
using TMPro;

namespace Assets.Scripts.Shared
{
    public sealed class HintedInputIfns : HintedInput
    {

        void Awake()
        {
            CharacterLimit = 4;
            PreviousInput = "";

            InputField ??= GetComponent<TMP_InputField>();
            InputField.characterValidation = TMP_InputField.CharacterValidation.Digit;
            InputField.characterLimit = CharacterLimit;
            InputField.onSelect.AddListener(SelectEvent);
            InputField.onValueChanged.AddListener(ChangeValueEvent);

            DropdownResponse = new ParserResponse(ParserResponse.ResponseType.EMPTY);

            SwitchState(false);
            UpdateHint();
        }

        // Updating from dropdown => input is correct.
        public override void UpdateInput(string newInput)
        {
            DropdownResponse = new ParserResponse(ParserResponse.ResponseType.OK, newInput);
            InputField.text = ParseIfns(newInput);
        }

        protected override void ChangeValueEvent(string newInput)
        {
            if (PreviousInput == "" && newInput == "")
                return;

            PreviousInput = newInput;

            if (newInput.Length < 4)
            {
                DropdownResponse = Dropdown.UpdateDropdown(newInput);
            }
            else
            {
                var responseTemp = Dropdown.UpdateDropdown(newInput);
                if (responseTemp.IsOk() == false)
                    DropdownResponse = responseTemp;
            }

            UpdateHint();
            Dropdown.Show();
            Select();
        }


        protected override void UpdateHint()
        {
            HintText.text = DropdownResponse.MakeHint(InputField.text);
        }

        private string ParseIfns(string source)
        {
            if (source.IndexOf(' ') == -1)
                return "";

            string parsedIfns = source.Substring(0, source.IndexOf(' '));

            if (parsedIfns.Length > CharacterLimit)
                Debug.LogError("Ifns option has more than 4 digits...");

            return parsedIfns;
        }

        protected override void SelectEvent(string newInput)
        {
            if (DropdownResponse.IsNotFound() || InputField.text.Length != 4)
                Dropdown.Show();

            Select();
        }
    }
}