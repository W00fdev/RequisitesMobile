using UnityEngine;
using Assets.Scripts.Core;

namespace Assets.Scripts.Shared
{
    public class ConnectingState : BaseState
    {
        public const string ConnectingTrigger = "Loading";
        public const string StopTrigger = "Running";

        public bool CanRecieveCheck = false;

        public ConnectingState(InputHubRequisites inputHubRequisites, OutputHubRequisites outputHubRequisites,
            Animator animatorUI, IStationStateSwitcher stateSwitcher)
            :base(inputHubRequisites, outputHubRequisites, animatorUI, ConnectingTrigger, StopTrigger, stateSwitcher)
        {
            NetworkRunner.Initialize(CheckInternetConnection, HandleIfnsDataResponse,
                OnOktmmfResponse, HandlePayeeDetailsResponse, false);

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

        public override void TryConnect()
        {
            NetworkRunner.CheckInternetConnection();
            CanRecieveCheck = true;
        }

        private void CheckInternetConnection(string responseConnection)
        {
            if (CanRecieveCheck == true)
            {
                if (responseConnection == null || responseConnection == "")
                {
                    Debug.Log("Google doesn't response");
                    return;
                }

                CanRecieveCheck = false;
                StateSwitcher.SwitchState<RunningState>();
            }
        }

        private void HandleIfnsDataResponse(string responseIfns)
        {
            var ResponseIfns = ParserResponse.ParseIfns(responseIfns);
            if (ResponseIfns == null)
                return;

            DataInputRequisites.DataIfns = new DataIfns(ResponseIfns);
            Debug.Log("Ifns successfully handled");

            InputHubRequisites.GotResponseIfns();
            StateSwitcher.SwitchState<RunningState>();
        }

        public void OnOktmmfResponse(string responseOktmmf)
        {
            // TODO: Check these expressions correct.
            // TODO: Before this there are check on correctness

            var ParsedResponse = ParserResponse.ParseOktmmf(responseOktmmf);
            if (ParsedResponse == null)
            {
                DataInputRequisites.OktmmfComplete = "";

                // Hint pop-up: "error ifns entered"
                return;
            }

            DataInputRequisites.IfnsComplete = IfnsSaved;
            DataInputRequisites.DataOktmmf = new DataOktmmf(ParsedResponse);

            InputHubRequisites.GotResponseOktmmf();
            StateSwitcher.SwitchState<RunningState>();
        }

        private void HandlePayeeDetailsResponse(string responsePayeeDetails)
        {
            PayeeDetails dataRequisites = null;
            if (responsePayeeDetails != null)
            {
                dataRequisites = new PayeeDetails();
                dataRequisites = JsonUtility.FromJson<PayeeDetails>(responsePayeeDetails);
            }

            if (dataRequisites == null)
            {
                Debug.LogError("Get null payee details");
                return;
            }

            OutputHubRequisites.UpdateHub(dataRequisites);
            StateSwitcher.SwitchState<RunningState>();
        }
    }
}
