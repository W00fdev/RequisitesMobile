using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Core;
using UnityEngine.Events;
using System;

namespace Assets.Scripts.Shared {
    public class InputHubRequisites : MonoBehaviour
    {
        public SearchbarBase SearchBarIfns;
        public SearchbarBase SearchBarOktmmf;

        [HideInInspector]
        //public UnityAction<string> SearchOktmmf;
        public Action<string> OnIfnsSet;
        public Action<string> OnOktmmfSet;

        private void Start()
        {
        }

        public void Initialize(Action<string> onIfnsSet, Action<string> onOktmmfSet)
        {
            OnIfnsSet = onIfnsSet;
            OnOktmmfSet = onOktmmfSet;
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
                // Replace to HintPopup
            }


            // De-activate oktmmf dropdown and input

            // HintedInputOktmmf.SwitchState(false);
            // HintedDropdownOktmmf.SwitchState(false);
        }

        public void GotResponseIfns()
        {
            if (SearchBarIfns.Initialized == false)
                SearchBarIfns.Initialize(IfnsComplete);

            SearchBarIfns.SwitchState(true);
        }

        public void GotResponseOktmmf()
        {
            if (SearchBarOktmmf.Initialized == false)
                SearchBarOktmmf.Initialize(OktmmfComplete);

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

                // Replace to HintPopup
            }
        }
    }
}
