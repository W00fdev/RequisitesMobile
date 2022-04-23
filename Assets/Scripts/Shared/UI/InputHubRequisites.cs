using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Core;
using UnityEngine.Events;
using System;

namespace Assets.Scripts.Shared {
    public class InputHubRequisites : MonoBehaviour
    {
        public TouchScreenInputAndroid KeyboardAndroid = null;

        public SearchbarBase SearchBarIfns;
        public SearchbarBase SearchBarOktmmf;

        [HideInInspector]
        //public UnityAction<string> SearchOktmmf;
        public Action<string> OnIfnsSet;
        public Action<string> OnOktmmfSet;

        public Action ClearHubAction;

        private void Start()
        {
            if (KeyboardAndroid == null)
                KeyboardAndroid = GetComponent<TouchScreenInputAndroid>();
        }

        public void Initialize(Action<string> onIfnsSet, Action<string> onOktmmfSet, Action clearHubAction)
        {
            OnIfnsSet = onIfnsSet;
            OnOktmmfSet = onOktmmfSet;
            ClearHubAction = clearHubAction;
        }

        public void SetEnableInput(bool enabled)
        {
            SearchBarIfns.EnableSelect(enabled);
            SearchBarOktmmf.EnableSelect(enabled);
        }

        private void IfnsComplete(string ifns)
        {
            if (DataInputRequisites.DataIfns.CheckIfnsExistence(ifns))
            {
                DataInputRequisites.IfnsComplete = ifns;
                OnIfnsSet?.Invoke(ifns);
            }
            else
            {
                Debug.LogError($"Ifns {ifns} doesn't exist");

                DataInputRequisites.IfnsComplete = "";
                DataInputRequisites.OktmmfComplete = "";

                SearchBarOktmmf.SwitchState(false);
                SearchBarOktmmf.CloseSearchbar();

                // HintPopup and ClearHub
                ClearHubAction?.Invoke();
            }


            // De-activate oktmmf dropdown and input

            // HintedInputOktmmf.SwitchState(false);
            // HintedDropdownOktmmf.SwitchState(false);
        }

        public void GotResponseIfns()
        {
            if (SearchBarIfns.Initialized == false)
                SearchBarIfns.Initialize(IfnsComplete, KeyboardAndroid);

            SearchBarIfns.SwitchState(true);
        }

        public void GotResponseOktmmf()
        {
            if (SearchBarOktmmf.Initialized == false)
                SearchBarOktmmf.Initialize(OktmmfComplete, KeyboardAndroid);

            SearchBarOktmmf.SwitchState(true);
        }


        private void OktmmfComplete(string oktmmf)
        {
            if (DataInputRequisites.DataOktmmf.CheckOktmmfExistence(oktmmf))
            {
                DataInputRequisites.OktmmfComplete = oktmmf;
                OnOktmmfSet?.Invoke(oktmmf);
            }
            else
            {
                Debug.LogError($"Oktmmf {oktmmf} doesn't exist");

                DataInputRequisites.OktmmfComplete = "";

                // HintPopup and ClearHub
                ClearHubAction?.Invoke();
            }
        }
    }
}
