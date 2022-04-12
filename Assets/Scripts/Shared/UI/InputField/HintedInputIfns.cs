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
            InputField.onValueChanged.AddListener(ChangeValueEvent);

            SwitchState(false);
            UpdateHint(string.Empty);
        }

        public override void UpdateInput(string newInput)
        {

        }

        protected override void ChangeValueEvent(string newInput)
        {
            CurrentInput = newInput;
            // DropDown.UpdateInput
        }

        protected override void UpdateHint(string hint) => HintText.text = hint;
    }
}