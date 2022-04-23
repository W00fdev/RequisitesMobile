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

        public override void Initialize(UnityAction<string> inputCompleteAction, TouchScreenInputAndroid keyboardAndroid)
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

                AddOptionsOptimized(0);
                DropdownResponse = new ParserResponse(EMPTY);
                return DropdownResponse;
            }

            // Main logic parser function call.
            int index = newInput.Length;
            // OptionsCachedNumerics[index].Clear();
            OptionsCachedNumerics[index] = SearchbarParser.UpdateDropdownIfns(OptionsCachedNumerics[index], newInput);
            // --------------------------------

            if (OptionsCachedNumerics[index].Count > 1)
                DropdownResponse = new ParserResponse(OK, OptionsCachedNumerics[newInput.Length][FIRST]);
            else
                DropdownResponse = new ParserResponse(NOTFOUND, newInput);

            AddOptionsOptimized(newInput.Length);
            return DropdownResponse;
        }

        protected override void InitializeOptionsFirst()
        {
            OptionsCachedNumerics[0] = SearchbarParser.InitializeFirstOptionsIfns();
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
            UpdateHint();

            if (DropdownResponse.IsNotFound() || newInput.Length != CharacterLimit)
                ShowDropdown();
            else if (newInput.Length == CharacterLimit)
                HideDropdown();

            if (PreviousInput.Length == CharacterLimit && newInput.Length < CharacterLimit)
                OnInputComplete?.Invoke(newInput);
            else if (newInput.Length == CharacterLimit && DropdownResponse.IsOk())
                OnInputComplete?.Invoke(newInput);
            else
                SelectInputField(); // After the options choosing focus is lost.

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