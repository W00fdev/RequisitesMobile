using UnityEngine;

namespace Assets.Scripts.Shared
{
    public abstract class BaseState
    {
        protected readonly InputHubRequisites InputHubRequisites;
        protected readonly OutputHubRequisites OutputHubRequisites;
        protected readonly Animator AnimatorUI;
        protected readonly IStationStateSwitcher StateSwitcher;

        protected readonly Core.Parser ParserResponse;

        protected readonly string StartTriggerName;
        protected readonly string StopTriggerName;

        protected static string IfnsSaved = "";

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
            ParserResponse = new Core.ParserSite();
        }

        public virtual void Start()
        {
            AnimatorUI.SetTrigger(StartTriggerName);
        }

        public virtual void Stop()
        {
            AnimatorUI.SetTrigger(StopTriggerName);
        }

        public abstract void TryConnect();
    }
}
