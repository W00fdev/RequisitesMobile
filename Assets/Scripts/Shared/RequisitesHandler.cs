using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Linq;

using Assets.Scripts.Core;
using UnityEngine.Events;

namespace Assets.Scripts.Shared
{
    public class RequisitesHandler : MonoBehaviour
    {
        public bool DebugMode = true;

        public InputHubRequisites InputHub;

        // Заменить структурой.
        [Header("Ссылки на тексты")]
        public TextMeshProUGUI TextIFNS;
        public TextMeshProUGUI TextOktmnf;

        public TextMeshProUGUI TextBIC;
        public TextMeshProUGUI TextCorrespAcc;
        public TextMeshProUGUI TextPayeeAcc;
        public TextMeshProUGUI TextPayeeInn;
        public TextMeshProUGUI TextPayeeKpp;
        public TextMeshProUGUI TextPayeeBankName;
        public TextMeshProUGUI TextPayeePayeeName;

        private DataInputRequisites _dataInputRequisites;
        private Parser _parser;

        private UnityEvent<string> _handleOktmmfEvent;

        private void Start()
        {
            _handleOktmmfEvent = new UnityEvent<string>();

            // Site-method
            _dataInputRequisites = new DataInputRequisites();
            _handleOktmmfEvent.AddListener(HandleOktmmfData);
            _parser = new ParserSite();

            HandleIfnsData();

            InputHub.Initialize(_dataInputRequisites, _handleOktmmfEvent);
            //DropdownIfns.Initialize();
        }

        public void HandleIfnsData() => _dataInputRequisites.DataIfns =
            new DataIfns(_parser.ParseIfns(NetworkRunner.GetIfns()));

        public void HandleOktmmfData(string ifns) => _dataInputRequisites.DataOktmmf =
            new DataOktmmf(_parser.ParseOktmmf(NetworkRunner.GetOktmmf(ifns)));

        public void HandlePayeeDetails()
        {
            string ifnsString = string.Join("", TextIFNS.text.Where(c => char.IsDigit(c)));
            string oktmnfString = string.Join("", TextOktmnf.text.Where(c => char.IsDigit(c)));

            var responce = NetworkRunner.GetPayeeRequisites(ifnsString, oktmnfString);

            if (responce != null)
            {
                TextBIC.text = responce.payeeDetails.bankBic;
                TextCorrespAcc.text = responce.payeeDetails.correspAcc;
                TextPayeeAcc.text = responce.payeeDetails.payeeAcc;
                TextPayeeInn.text = responce.payeeDetails.payeeInn;
                TextPayeeKpp.text = responce.payeeDetails.payeeKpp;
                TextPayeeBankName.text = responce.payeeDetails.bankName;
                TextPayeePayeeName.text = responce.payeeDetails.payeeName;
            }
            else
            {
                TextBIC.text = "Ошибка.";
                TextCorrespAcc.text = "Ошибка.";
                TextPayeeAcc.text = "Ошибка.";
                TextPayeeInn.text = "Ошибка.";
                TextPayeeKpp.text = "Ошибка.";
                TextPayeeBankName.text = "Ошибка.";
                TextPayeePayeeName.text = "Ошибка.";
            }
        }
    }
}
