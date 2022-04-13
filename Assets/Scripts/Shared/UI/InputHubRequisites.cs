using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Core;
using UnityEngine.Events;

namespace Assets.Scripts.Shared {
    public class InputHubRequisites : MonoBehaviour
    {
        public HintedInput HintedInputIfns;
        public HintedInput HintedInputOktmmf;

        public HintedDropdown HintedDropdownIfns;
        public HintedDropdown HintedDropdownOktmmf;

        [HideInInspector]
        public UnityEvent<string> OnIfnsComplete;

        private DataInputRequisites Data;

        public void Initialize(DataInputRequisites data, UnityEvent<string> ifnsComplete)
        {
            Data = data;
            OnIfnsComplete = ifnsComplete;

            HintedInputIfns.Initialize();
            //HintedDropdownOktmmf.Initialize();

            HintedDropdownIfns.Initialize(Data);
            //HintedDropdownOktmmf.Initialize(Data);

            IfnsComplete(false, "");
        }

        private void IfnsComplete(bool completed, string ifns)
        {
            if (completed)
            {
                if (Data.DataIfns.CheckIfnsExistence(ifns))
                    OnIfnsComplete.Invoke(ifns);
                else
                    Debug.LogError($"Ifns {ifns} doesn't exist");
                // Replace to HintPopup
            }
            else
            {
                // De-activate oktmmf dropdown and input

                // HintedInputOktmmf.SwitchState(false);
                // HintedDropdownOktmmf.SwitchState(false);
            }

        }
    }
}
