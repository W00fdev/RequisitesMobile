using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.Shared
{
    public abstract class BaseState
    {
        protected readonly InputHubRequisites InputHubRequisites;
        protected readonly OutputHubRequisites OutputHubRequisites;
        protected readonly Animator AnimatorUI;
        protected readonly IStationStateSwitcher StateSwitcher;

        protected readonly Parser ParserResponse;

        protected readonly string StartTriggerName;
        protected readonly string StopTriggerName;

        protected readonly string ClearHubPopupTriggerName = "ClearPayee";

        protected static string IfnsSaved = "";
        
        public int PacketsSent = 0;
        public bool CanRecieveCheck = false;

        public BaseState(InputHubRequisites inputHubRequisites, OutputHubRequisites outputHubRequisites, Animator animatorUI, 
            string startTriggerName, string stopTriggerName, IStationStateSwitcher stateSwitcher)
        {
            InputHubRequisites = inputHubRequisites;
            OutputHubRequisites = outputHubRequisites;
            AnimatorUI = animatorUI;
            StartTriggerName = startTriggerName;
            StopTriggerName = stopTriggerName;
            StateSwitcher = stateSwitcher;

            // Add differences or make parserText in Connecting.
            ParserResponse = new ParserSite();
        }

        public virtual void Start()
        {
            AnimatorUI.SetTrigger(StartTriggerName);
        }

        public virtual void Stop()
        {
            AnimatorUI.SetTrigger(StopTriggerName);
        }

        public virtual void TryConnect()
        {
            PacketsSent++;
            NetworkRunner.CheckInternetConnection();
            CanRecieveCheck = true;
        }

        protected virtual void ClearOutputHub()
        {
            OutputHubRequisites.ClearHub();
            AnimatorUI.SetTrigger(ClearHubPopupTriggerName);
        }

        protected void CheckInternetConnection(string responseConnection)
        {
            if (CanRecieveCheck == true)
            {
                if (responseConnection == null || responseConnection == "")
                {
                    Debug.Log("Google doesn't response");

                    if (this is RunningState)
                        StateSwitcher.SwitchState<ConnectingState>();

                    PacketsSent--;

                    return;
                }

                CanRecieveCheck = false;

                if (this is ConnectingState)
                    StateSwitcher.SwitchState<RunningState>();

                PacketsSent--;
            }
        }
    }
}
