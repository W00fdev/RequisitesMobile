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

        public RunningState(InputHubRequisites inputHubRequisites, OutputHubRequisites outputHubRequisites,
            Animator animatorUI, IStationStateSwitcher stateSwitcher)
            : base(inputHubRequisites, outputHubRequisites, animatorUI, RunningTrigger, StopTrigger, stateSwitcher)
        {
            //Debug.Log("Running state created");
        }

        public override void Start()
        {
            base.Start();
            //Debug.Log("Running state started");

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
                    InputHubRequisites.Initialize(OnIfnsSet, OnOktmmfSet, ClearOutputHub);
                }
            }

            // enable inputhub
            InputHubRequisites.SetEnableInput(true);
        }

        public override void Stop()
        {
            base.Stop();

            //Debug.Log("Running state stopped");
            // Disable inputhub
        }

        public override void TryConnect()
        {
            base.TryConnect();
        }

        private bool HandleIfnsData()
        {
            NetworkRunner.GetIfns();
            StateSwitcher.SwitchState<ConnectingState>();

            return true;
        }

        public void OnIfnsSet(string ifns)
        {
            // DataInputRequisites.OktmmfComplete = "";

            IfnsSaved = ifns;
            StateSwitcher.SetHasToUpdateOktmmf(true);
            NetworkRunner.GetOktmmf(ifns);
            StateSwitcher.SwitchState<ConnectingState>();
        }

        public void OnOktmmfSet(string oktmmf)
        {
            Debug.Log(DataInputRequisites.OktmmfComplete);

            DataInputRequisites.OktmmfComplete = oktmmf;
            HandlePayeeDetails();
        }

        private void HandlePayeeDetails()
        {
            NetworkRunner.GetPayeeRequisites(DataInputRequisites.IfnsComplete,
                DataInputRequisites.OktmmfComplete);

            StateSwitcher.SwitchState<ConnectingState>();
        }
    }
}