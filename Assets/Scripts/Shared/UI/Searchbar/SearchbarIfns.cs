using Assets.Scripts.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static Assets.Scripts.Core.ParserResponse.ResponseType;

namespace Assets.Scripts.Shared 
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public sealed class SearchbarIfns : SearchbarBase
    {

        private void Awake()
        {
            // <InputField>
            CharacterLimit = 4;
            BaseAwake();
            // </InputField>

            // <Dropdown>
            OptionsCached = new List<TMP_Dropdown.OptionData>[5]
                {
                    new List<TMP_Dropdown.OptionData>(100) { EmptyOption },
                    new List<TMP_Dropdown.OptionData>(10) { EmptyOption },
                    new List<TMP_Dropdown.OptionData>(55) { EmptyOption },
                    new List<TMP_Dropdown.OptionData>(15) { EmptyOption },
                    new List<TMP_Dropdown.OptionData>(2) { EmptyOption },
                };
            // </Dropdown>
        }

        public override void Initialize(UnityAction<string> inputCompleteAction)
        {
            base.Initialize(inputCompleteAction);
            //InitializeOptionsFirst();
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


            int index = newInput.Length;
            if (newInput.Length == 0
                ||
                    (OptionsCached[index].Count > 1 && OptionsCached[index][FIRST].text.Length == newInput.Length
                    && OptionsCached[index][FIRST].text == newInput)
                )
            {
                if (OptionsCached[index].Count <= 1)
                    throw new Exception($"{index} tier doesn't initialized.");

                Dropdown.AddOptions(OptionsCached[index]);
                DropdownResponse = (newInput.Length == 0) ? new ParserResponse(EMPTY) : new ParserResponse(OK, OptionsCached[index][FIRST].text);
                //PreviousInputDropdown = newInput;
                return DropdownResponse;
            }
            OptionsCached[index].Clear();
            OptionsCached[index].Add(EmptyOption);


            bool foundMatch = false;
            foreach (var option in DataInputRequisites.DataIfns.Data)
            {
                if (option.IfnsData.Count <= 0)
                    continue;

                OptionString.Clear();
                OptionString.Append(option.IfnsData.First().Key.Substring(0, 2));

                // Elements are sorted.
                int highIndex = Mathf.Clamp(newInput.Length, 0, 2);
                int checkSortedMatch = string.Compare(newInput.Substring(0, highIndex),
                    OptionString.ToString().Substring(0, highIndex));

                if (checkSortedMatch != 0)
                {
                    if (foundMatch == true)
                        break;
                    continue;
                }
                foundMatch = true;

                if (newInput.Length == 1)
                {
                    OptionString.Append(" - ").Append(option.RepublicName);

                    OptionsCached[1].Add(new TMP_Dropdown.OptionData(OptionString.ToString()));
                    continue;
                }

                bool foundInnerMatch = false;
                foreach (var ifnsData in option.IfnsData)
                {
                    // Elements are sorted.
                    checkSortedMatch = string.Compare(newInput.Substring(0, newInput.Length),
                        ifnsData.Key.Substring(0, newInput.Length));

                    if (checkSortedMatch != 0)
                    {
                        if (foundInnerMatch == true)
                            break;
                        continue;
                    }
                    foundInnerMatch = true;

                    OptionString.Clear();
                    OptionString.Append(ifnsData.Key).Append(" - ").Append(ifnsData.Value);

                    OptionsCached[newInput.Length].Add(new TMP_Dropdown.OptionData(OptionString.ToString()));
                }
            }

            Dropdown.AddOptions(OptionsCached[newInput.Length]);

            if (Dropdown.options.Count > 1)
            {
                DropdownResponse = new ParserResponse(OK, Dropdown.options[FIRST].text);
            }
            else
            {
                DropdownResponse = new ParserResponse(NOTFOUND, newInput);
            }

            //PreviousInputDropdown = newInput;

            return DropdownResponse;
        }

        protected override void InitializeOptionsFirst()
        {
            OptionsCached[0].Clear();
            OptionsCached[0].Add(EmptyOption);

            foreach (var option in DataInputRequisites.DataIfns.Data)
            {
                if (option.IfnsData.Count <= 0)
                    continue;

                OptionString.Clear();
                OptionString.Append(option.IfnsData.First().Key.Substring(0, 2));
                OptionString.Append(" - ").Append(option.RepublicName);

                OptionsCached[0].Add(new TMP_Dropdown.OptionData(OptionString.ToString()));
            }

            if (OptionsCached[0].Count <= 1)
                throw new Exception("Tier 1 option can't be size <= 1");

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

            // Don't call SelectEventInputField.
            // SelectEvent linked to OnSelect(), who call the ActivateInputField()
            SelectInputField();

            SwitchStateDropdown(true);

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

