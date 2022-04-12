using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Core;
using TMPro;

namespace Assets.Scripts.Shared
{
    public class HintedDropdownIfns : HintedDropdown
    {
        private void Start()
        {
            Dropdown ??= GetComponent<TMP_Dropdown>();

            Label.enabled = false;
            SwitchState(true);
        }

        public override void UpdateDropdown(string newInput)
        {
            if (Initialized == false)
                return;

            if (Data.DataIfns == null)
            {
                Debug.LogError("Data ifns is null");
                return;
            }

            var data = Data.DataIfns.Data;
            var options = new List<string>();

            foreach (var option in data)
            {
                if (option.IfnsData.Keys.Count <= 0)
                    continue;
                
                string part1 = option.IfnsData.First().Key.Substring(0, 2) + " - ";
                options.Add(part1 + option.RepublicName);
            }

            Dropdown.AddOptions(options);
        }

        protected override void ChooseValueEvent(TMP_Dropdown dropdown)
        {
            throw new System.NotImplementedException();
        }
    }
}