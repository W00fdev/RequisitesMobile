using Assets.Scripts.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

using static Assets.Scripts.Core.ParserResponse.ResponseType;

namespace Assets.Scripts.Shared 
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class SearchbarIfns : SearchbarBase
    {

        private void Awake()
        {
            // <InputField>
            CharacterLimit = 4;
            BaseAwake();
            // </InputField>

            // <Dropdown>
            // Only for digits input
            OptionsCachedNumerics = new List<string>[5]
                {
                    new List<string>(100) { EmptyOption },
                    new List<string>(10) { EmptyOption },
                    new List<string>(55) { EmptyOption },
                    new List<string>(15) { EmptyOption },
                    new List<string>(2) { EmptyOption },
                };
            // </Dropdown>
        }

        public override void Initialize(UnityAction<string, bool> inputCompleteAction, TouchScreenInputAndroid keyboardAndroid)
        {
            base.Initialize(inputCompleteAction, keyboardAndroid);
            // Need to select input field.
            SwitchStateDropdown(false); 
        }

        // --------------------------- <DROPDOWN> ---------------------------

        protected override ParserResponse UpdateDropdown(string newInput)
        {
            if (CheckUpdatingDropdown(newInput))
                return DropdownResponse;

            Dropdown.ClearOptions();
            Dropdown.Hide();

            if (newInput.Length == 0)
            {
                if (OptionsCachedNumerics[0].Count <= 1)
                    throw new Exception("0 tier doesn't initialized.");

                AddOptionsOptimized(OptionsCachedNumerics[0]);
                DropdownResponse = new ParserResponse(EMPTY);
                return DropdownResponse;
            }

            // Main logic parser function call.
            int index = newInput.Length;
            List<string> newDropdownOptions;
            if (index < CharacterLimit)
                newDropdownOptions = SearchbarParser.UpdateDropdownIfns(OptionCached, ref OptionsCachedNumerics[index], newInput);
            else
                newDropdownOptions = SearchbarParser.UpdateDropdownIfns(OptionCached, ref OptionsCachedNumerics[0], newInput);

            // --------------------------------

            if (newDropdownOptions.Count > 1)
                DropdownResponse = new ParserResponse(OK, newDropdownOptions[FIRST]);
            else
                DropdownResponse = new ParserResponse(NOTFOUND, newInput);

            AddOptionsOptimized(newDropdownOptions);
            return DropdownResponse;
        }

        protected override void InitializeOptionsFirst()
        {
            // It return copy to internal struct _ifnsParser.options. And changes by ref OptionsCachedNumerics[0].
            OptionCached = SearchbarParser.InitializeFirstOptionsIfns(ref OptionsCachedNumerics[0]);
            UpdateDropdown("");
        }

        protected override bool CheckUpdatingDropdown(string newInput)
        {
            if (DataInputRequisites.DataIfns == null)
            {
                Debug.LogError("Data ifns is null");

                DropdownResponse = new ParserResponse(UNKNOWNERROR);
                return true;
            }

            return base.CheckUpdatingDropdown(newInput);
        }

        protected override void ClearOptions()
        {
            Dropdown.options.Clear();
            for (int i = 0; i < 5; i++)
            {
                OptionsCachedNumerics[i].Clear();
                OptionsCachedNumerics[i].Add(EmptyOption);
            }
        }

        // --------------------------- </DROPDOWN> ---------------------------




        // --------------------------- <INPUTFIELD> ---------------------------
        protected override void ChangeValueEventInputField(string newInput)
        {
            if (PreviousInput == "" && newInput == "")
                return;

            UpdateDropdown(newInput);

            bool isNumeric = SearchbarParser.IsStringNumeric(newInput);

            if (isNumeric)
            {
                UpdateHint();

                if (newInput.Length != CharacterLimit || DropdownResponse.IsNotFound())
                    ShowDropdown();
                else if (newInput.Length == CharacterLimit)
                    HideDropdown();

                if (PreviousInput.Length == CharacterLimit && newInput.Length < CharacterLimit
                    || newInput.Length == CharacterLimit && DropdownResponse.IsOk()
                    )
                {
                    InputField.SetTextWithoutNotify(DropdownResponse.Response);
                    OnInputComplete?.Invoke(newInput, true);
                }
                else
                {
                    SelectInputField(); // After the options choosing focus is lost.
                }
            }
            else
            {
                HintText.UpdateHint("");
            }

            SelectInputField();
            PreviousInput = newInput;
        }

        protected override void SelectEventInputField(string newInput)
        {

            if (InputField.text.Length != CharacterLimit || PreviousInput.Length == CharacterLimit)
                ShowDropdown();

            SwitchStateDropdown(true);

            // Don't call SelectEventInputField.
            // SelectEvent linked to OnSelect(), who call the ActivateInputField()
            SelectInputField();

            if (KeyboardAndroid != null)
                KeyboardAndroid.ShowKeyboard(this);
            //TouchScreenKeyboard.Open("", TouchScreenKeyboardType.DecimalPad);
        }

        protected override void UpdateHint()
        {
            //Debug.Log("Response: " + DropdownResponse.Response);
            HintText.UpdateHint(DropdownResponse);
        }

        // Updating from dropdown => input is correct.
        protected override void UpdateInput(string newInput)
        {
            //DropdownResponse = new ParserResponse(OK, newInput);
            InputField.text = ParseValue(newInput);
        }

        // --------------------------- </INPUTFIELD> ---------------------------
    }
}






// ====================== UPDATE DROPDOWN =====================

/*            if (OptionsCached[index].Count > 1 && OptionsCached[index][FIRST].Length == newInput.Length
                    && OptionsCached[index][FIRST] == newInput)
            {
                if (OptionsCached[index].Count <= 1)
                    throw new Exception($"{index} tier doesn't initialized.");

                AddOptionsOptimized(index);
                DropdownResponse = new ParserResponse(OK, OptionsCached[index][FIRST]);
                //PreviousInputDropdown = newInput;
                return DropdownResponse;
            }*/

// ================================================================