using Assets.Scripts.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static Assets.Scripts.Core.ParserResponse.ResponseType;

namespace Assets.Scripts.Shared
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public sealed class SearchbarOktmmf : SearchbarBase
    {
        [SerializeField]
        private char[] digits;

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
            OptionsCached = new List<TMP_Dropdown.OptionData>[2]
            {
                new List<TMP_Dropdown.OptionData>(150) { EmptyOption },
                new List<TMP_Dropdown.OptionData>(150) { EmptyOption },
            };

            digits = new char[CharacterLimit];
            ClearDigits();
            // </Dropdown>
        }

        public override void Initialize(UnityAction<string> inputCompleteAction)
        {
            ClearDigits();
            base.Initialize(inputCompleteAction);
            //InitializeOptionsFirst();
            SwitchStateDropdown(false);
        }

        // --------------------------- <DROPDOWN> ---------------------------

        protected override ParserResponse UpdateDropdown(string newInput)
        {
            if (CheckUpdatingDropdown(newInput))
                return DropdownResponse;

            Dropdown.Hide();

            //int index = newInput.Length;
            if (newInput.Length == 0)
            {
                if (OptionsCached[0].Count <= 1)
                {
                    // InitializeOptionsFirstTier();
                    throw new Exception("Tier 0 doesn't initialized.");
                }

                Dropdown.ClearOptions();
                Dropdown.AddOptions(OptionsCached[0]);
                DropdownResponse = new ParserResponse(EMPTY);
                PreviousInputDropdown = newInput;

                return DropdownResponse;
            }

            //OptionsTier[0].Clear();
            //OptionsTier[0].Add(EmptyOption);
            OptionsCached[1].Clear();
            OptionsCached[1].Add(EmptyOption);

            var dropdownData = Dropdown.options;
            if (newInput.Length < PreviousInputDropdown.Length)
                dropdownData = OptionsCached[0];

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
                OptionsCached[1].Add(new TMP_Dropdown.OptionData(OptionString.ToString()));
                //if (newInput.Length == 0)
                //    OptionsTier[0].Add(new TMP_Dropdown.OptionData(OptionString.ToString()));
            }

            Dropdown.ClearOptions();
            Dropdown.AddOptions(OptionsCached[1]);

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

            OptionsCached[0].Clear();
            OptionsCached[0].Add(EmptyOption);
            ClearDigits();

            foreach (var option in DataInputRequisites.DataOktmmf.Data)
            {
                OptionString.Clear();
                OptionString.Append(option.Key).Append(" - ").Append(option.Value.ToASCII());

                for (int i = 0; i < CharacterLimit; i++)
                {
                    if (digits[i] == '-')
                        digits[i] = OptionString[i];
                    else if (digits[i] == '+')
                        break;
                    else if (digits[i] != OptionString[i])
                    {
                        digits[i] = '+';
                        for(int j = i; j < CharacterLimit; j++)
                            digits[j] = '+';
                        break;
                    }
                }
                

                OptionsCached[0].Add(new TMP_Dropdown.OptionData(OptionString.ToString()));
            }

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

        private void ClearDigits()
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
                OptionsCached[i].Clear();
                OptionsCached[i].Add(EmptyOption);
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