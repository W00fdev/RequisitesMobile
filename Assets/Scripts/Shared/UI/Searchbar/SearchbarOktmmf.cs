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
            Dropdown.ClearOptions();

            //int index = newInput.Length;
            if (newInput.Length == 0)
            {
                if (OptionsCachedNumerics[0].Count <= 1)
                {
                    // InitializeOptionsFirstTier();
                    throw new Exception("Tier 0 doesn't initialized.");
                }

                
                AddOptionsOptimized(0);
                // Dropdown.AddOptions(OptionsCached[0]);
                DropdownResponse = new ParserResponse(EMPTY);
                PreviousInputDropdown = newInput;

                return DropdownResponse;
            }

            //OptionsTier[0].Clear();
            //OptionsTier[0].Add(EmptyOption);
            OptionsCachedNumerics[1].Clear();
            OptionsCachedNumerics[1].Add(EmptyOption);

            var dropdownData = Dropdown.options;
            var tempListTmp = new List<TMP_Dropdown.OptionData>();
            if (newInput.Length < PreviousInputDropdown.Length)
            {
                tempListTmp.Add(new TMP_Dropdown.OptionData(""));
                //tempListTmp.AddRange()
                foreach(string optionString in OptionsCachedNumerics[0])
                {
                    tempListTmp.Add(new TMP_Dropdown.OptionData(optionString));
                }

                dropdownData = tempListTmp;

            }

            bool foundMatch = false;
            foreach (var option in dropdownData)
            {
                if (option.text == "")
                    continue;

                OptionString.Clear();
                OptionString.Append(option.text.Substring(0, CharacterLimit));

                // Elements are sorted.
                int checkSortedMatch = string.Compare(newInput.Substring(0, newInput.Length),
                    OptionString.ToString().Substring(0, newInput.Length));

                if (checkSortedMatch != 0)
                {
                    if (foundMatch == true)
                        break;
                    continue;
                }
                foundMatch = true;

                OptionString.Append(option.text.Substring(CharacterLimit).ToASCII());
                OptionsCachedNumerics[1].Add(OptionString.ToString());
                //if (newInput.Length == 0)
                //    OptionsTier[0].Add(new TMP_Dropdown.OptionData(OptionString.ToString()));
            }

            //Dropdown.AddOptions(OptionsCached[1]);
            AddOptionsOptimized(1);

            if (Dropdown.options.Count > 1)
            {
                DropdownResponse = new ParserResponse(OK, Dropdown.options[FIRST].text);
            }
            else
            {
                DropdownResponse = new ParserResponse(NOTFOUND, newInput);
            }

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