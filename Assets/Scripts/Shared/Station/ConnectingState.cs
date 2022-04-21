using UnityEngine;
using Assets.Scripts.Core;

namespace Assets.Scripts.Shared
{
    public class ConnectingState : BaseState
    {
        public const string ConnectingTrigger = "Loading";
        public const string StopTrigger = "Running";

        public ConnectingState(InputHubRequisites inputHubRequisites, OutputHubRequisites outputHubRequisites,
            Animator animatorUI, IStationStateSwitcher stateSwitcher)
            :base(inputHubRequisites, outputHubRequisites, animatorUI, ConnectingTrigger, StopTrigger, stateSwitcher)
        {
            Debug.Log("Connecting state created");
        }

        public override void Start()
        {
            base.Start();
            Debug.Log("Connecting state started");
        }

        public override void Stop()
        {
            base.Stop();
            Debug.Log("Connecting state stopped");
        }

        public override bool IsConnect()
        {
            bool getAccess = NetworkRunner.GetIfns() != "" && NetworkRunner.GetIfns() != null;

            if (getAccess)
            {
                StateSwitcher.SwitchState<RunningState>();
                return true;
            }

            return false;
        }
    }
}
