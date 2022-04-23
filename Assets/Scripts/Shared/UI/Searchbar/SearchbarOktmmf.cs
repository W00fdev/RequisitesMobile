using Assets.Scripts.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static Assets.Scripts.Core.ParserResponse.ResponseType;

namespace Assets.Scripts.Shared
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class SearchbarOktmmf : SearchbarBase
    {
        [SerializeField]
        protected char[] digits;

        private void Awake()
        {
            // <InputField>
            CharacterLimit = 8;
            BaseAwake();
            // </InputField>

            // <Dropdown>
            
            // OptionsTier[0] = start
            // OptionsTier[1] = current;
            // Можно добавить с 4ой цифры
            OptionsCachedNumerics = new List<string>[2]
            {
                new List<string>(350) { EmptyOption },
                new List<string>(200) { EmptyOption },
            };

            digits = new char[CharacterLimit];
            ClearDigits();
            // </Dropdown>
        }

        public override void Initialize(UnityAction<string> inputCompleteAction, TouchScreenInputAndroid keyboardAndroid)
        {
            //ClearDigits();
            base.Initialize(inputCompleteAction, keyboardAndroid);
            SwitchStateDropdown(false);
        }

        // --------------------------- <DROPDOWN> ---------------------------

        protected override ParserResponse UpdateDropdown(string newInput)
        {
            if (CheckUpdatingDropdown(newInput))
                return DropdownResponse;

            Dropdown.Hide();

            // It uses next Dropdown.options
            //Dropdown.ClearOptions();

            if (newInput.Length == 0)
            {
                if (OptionsCachedNumerics[0].Count <= 1)
                {
                    // InitializeOptionsFirstTier();
                    throw new Exception("Tier 0 doesn't initialized.");
                }

                Dropdown.ClearOptions();
                AddOptionsOptimized(0);
                DropdownResponse = new ParserResponse(EMPTY);
                PreviousInputDropdown = newInput;
                SearchbarParser.SetPreviousInputOktmmf(PreviousInputDropdown);

                return DropdownResponse;
            }


            // Main logic
            //OptionsCachedNumerics[1].Clear();
            OptionsCachedNumerics[1] = SearchbarParser.UpdateDropdownOktmmf(OptionsCachedNumerics[0], newInput);


            if (OptionsCachedNumerics[1].Count > 1)
                DropdownResponse = new ParserResponse(OK, OptionsCachedNumerics[1][FIRST]);
            else
                DropdownResponse = new ParserResponse(NOTFOUND, newInput);

            // Remove hard dependency with OptionsNumerics
            // Make it list<string> param
            // Delete OptionsCached and OptionsCachedNumerics[1]
            AddOptionsOptimized(1);

            PreviousInputDropdown = newInput;
            return DropdownResponse;
        }

        protected override void InitializeOptionsFirst()
        {
            if (DataInputRequisites.DataOktmmf == null || 
                DataInputRequisites.DataOktmmf.Data == null || DataInputRequisites.DataOktmmf.Data.Count <= 0)
            {
                Debug.LogError("Oktmmf is empty");
                return;
            }

            ClearDigits();
            OptionsCachedNumerics[0] = SearchbarParser.InitializeFirstOptionsOktmmf(ref digits);
            UpdateDropdown("");
        }

        protected override bool CheckUpdatingDropdown(string newInput)
        {
            if (DataInputRequisites.DataOktmmf == null)
            {
                Debug.LogError("Data oktmmf is null");

                DropdownResponse = new ParserResponse(UNKNOWNERROR);
                return true;
            }

            if (base.CheckUpdatingDropdown(newInput) == true)
                return true;

            if (newInput.Length != 0 && newInput.Length > PreviousInputDropdown.Length)
            {
                char lastSymbol = digits[newInput.Length - 1];
                if (lastSymbol != '-' && lastSymbol != '+' && lastSymbol == newInput[newInput.Length - 1])
                    return true;
            }

            return false;
        }

        protected void ClearDigits()
        {
            for (int i = 0; i < CharacterLimit; i++)
                digits[i] = '-';
        }

        protected override void ClearOptions()
        {
            Dropdown.options.Clear();
            ClearDigits();
            for (int i = 0; i < 2; i++)
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

            if (newInput.Length == CharacterLimit && DropdownResponse.IsOk())
                OnInputComplete?.Invoke(newInput);
            else
                SelectInputField(); // After the options choosing focus is lost.

            PreviousInput = newInput;
        }

        protected override void SelectEventInputField(string newInput)
        {
            if (InputField.text.Length != CharacterLimit || PreviousInput.Length == CharacterLimit)
                ShowDropdown();

            // Don't call SelectEventInputField.
            // SelectEvent linked to OnSelect(), who call the ActivateInputField()
            SelectInputField();
            
            if (KeyboardAndroid != null)
                KeyboardAndroid.ShowKeyboard(this);
            
            //TouchScreenKeyboard.Open("");
        }

        protected override void UpdateHint()
        {
            //Debug.Log("Response: " + DropdownResponse.Response);
            HintText.UpdateHint(DropdownResponse);
        }

        protected override void UpdateInput(string newInput)
        {
            //DropdownResponse = new ParserResponse(OK, newInput);
            InputField.text = ParseValue(newInput);
        }

        // --------------------------- </INPUTFIELD> ---------------------------
    }
}