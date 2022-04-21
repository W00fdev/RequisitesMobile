using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Core;

namespace Assets.Scripts.Shared
{
    public class RunningState : BaseState
    {
        public const string RunningTrigger = "Running";
        public const string StopTrigger = "Running";

        private readonly Parser _parser;

        public RunningState(InputHubRequisites inputHubRequisites, OutputHubRequisites outputHubRequisites,
            Animator animatorUI, IStationStateSwitcher stateSwitcher)
            : base(inputHubRequisites, outputHubRequisites, animatorUI, RunningTrigger, StopTrigger, stateSwitcher)
        {
            // Add differences or make parserText in Connecting.
            _parser = new ParserSite();

            Debug.Log("Running state created");
        }

        public override void Start()
        {
            base.Start();
            Debug.Log("Running state started");

            if (DataInputRequisites.DataIfns == null)
            {
                bool ifnsDataGetSuccess = HandleIfnsData();
                if (ifnsDataGetSuccess == false)
                {
                    DataInputRequisites.IfnsComplete = "";
                    StateSwitcher.SwitchState<ConnectingState>();
                    return;
                }
                else
                {
                    InputHubRequisites.Initialize(OnIfnsSet, OnOktmmfSet);
                }
            }

            // enable inputhub
        }

        public override void Stop()
        {
            base.Stop();

            Debug.Log("Running state stopped");
            // Disable inputhub
        }

        public override bool IsConnect()
        {
            return true;
        }

        public void OnIfnsSet(string ifns)
        {
            Debug.Log(ifns);

            // TODO: Add check internet connection.
            if (IsConnect() == false)
            {
                DataInputRequisites.OktmmfComplete = "";
                StateSwitcher.SwitchState<ConnectingState>();

                // Error connection
                return;
            }

            // TODO: Check these expressions correct.
            // TODO: Before this there are check on correctness
            string oktmmfRawResponse = NetworkRunner.GetOktmmf(ifns);
            Debug.Log(oktmmfRawResponse);

            var ParsedResponse = _parser.ParseOktmmf(oktmmfRawResponse);
            if (ParsedResponse == null)
            {
                DataInputRequisites.OktmmfComplete = "";

                // Hint pop-up: "error ifns entered"
                return;
            }

            DataInputRequisites.IfnsComplete = ifns;
            DataInputRequisites.DataOktmmf = new DataOktmmf(ParsedResponse);
        }

        public void OnOktmmfSet(string oktmmf)
        {
            Debug.Log(DataInputRequisites.OktmmfComplete);

            // TODO: Add entered input check.
            if (IsConnect() == false)
            {
                StateSwitcher.SwitchState<ConnectingState>();

                // Error connection
                return;
            }

            DataInputRequisites.OktmmfComplete = oktmmf;
            HandlePayeeDetails();
        }

        private bool HandleIfnsData()
        {
            var ResponseIfns = _parser.ParseIfns(NetworkRunner.GetIfns());
            if (ResponseIfns == null)
                return false;

            DataInputRequisites.DataIfns = new DataIfns(ResponseIfns);
            Debug.Log("Ifns successfully handled");
            return true;
        }

        private void HandlePayeeDetails()
        {
            if (IsConnect() == false)
            {
                StateSwitcher.SwitchState<ConnectingState>();

                // Error connection
                return;
            }

            var PayeeRequisites = NetworkRunner.GetPayeeRequisites(DataInputRequisites.IfnsComplete,
                DataInputRequisites.OktmmfComplete);

            OutputHubRequisites.UpdateHub(PayeeRequisites);
        }
    }
}