using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Core;
using TMPro;
using UnityEngine.Events;

using static Assets.Scripts.Core.ParserResponse.ResponseType;

namespace Assets.Scripts.Shared
{
    public class HintedDropdownIfns : HintedDropdown
    {

        private void Awake()
        {
            Dropdown ??= GetComponent<TMP_Dropdown>();
            //Dropdown.onValueChanged.RemoveAllListeners();
            Dropdown.onValueChanged.AddListener(ChooseValueEvent);

            Label.enabled = false;
            SwitchState(false);
        }

        public override ParserResponse UpdateDropdown(string newInput)
        {
            if (Initialized == false)
                return new ParserResponse(EMPTY);

            if (Data.DataIfns == null)
            {
                Debug.LogError("Data ifns is null");
                return new ParserResponse(UNKNOWNERROR);
            }

            Dropdown.ClearOptions();
            //Dropdown.Hide();
            Dropdown.AddOptions(new List<string>(){ "" });

            var data = Data.DataIfns.Data;
            var options = new List<string>();

            if (newInput.Length == 0)
            {
                foreach (var option in data)
                {
                    if (option.IfnsData.Count <= 0)
                        continue;

                    string part1 = option.IfnsData.First().Key.Substring(0, 2) + " - ";
                    options.Add(part1 + option.RepublicName);
                }

                Dropdown.AddOptions(options);
            }
            else
            {
                foreach (var option in data)
                {
                    if (option.IfnsData.Count <= 0)
                        continue;

                    string firstKey = option.IfnsData.First().Key;

                    if (firstKey[0] != newInput[0])
                        continue;

                    if (newInput.Length > 1 && firstKey[1] != newInput[1])
                        continue;

                    string part1 = "";
                    if (newInput.Length < 2)
                    {
                        part1 = firstKey.Substring(0, 2);
                        part1 += " - ";
                        options.Add(part1 + option.RepublicName);
                        continue;
                    }

                    foreach (var ifnsData in option.IfnsData)
                    {
                        if (newInput.Length > 2 && ifnsData.Key[2] != newInput[2])
                            continue;

                        if (newInput.Length > 3 && ifnsData.Key[3] != newInput[3])
                            continue;

                        options.Add(ifnsData.Key + " - " + ifnsData.Value);
                    }
                }
                Dropdown.AddOptions(options);
            }

            Dropdown.RefreshShownValue();

            if (newInput.Length == 0)
                return new ParserResponse(EMPTY);

            if (Dropdown.options.Count > 1)
                return new ParserResponse(OK, Dropdown.options[1].text);

            return new ParserResponse(NOTFOUND);
        }

        public override void ChooseValueEvent(int optionIndex)
        {
            string optionString = Dropdown.options[optionIndex].text;
            Input.UpdateInput(optionString);
            Input.Select();
        }
    }
}