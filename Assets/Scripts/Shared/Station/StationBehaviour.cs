using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Assets.Scripts.Shared
{
    public class StationBehaviour : MonoBehaviour, IStationStateSwitcher
    {
        [SerializeField] private InputHubRequisites _inputHubRequisites;
        [SerializeField] private OutputHubRequisites _outputHubRequisites;
        [SerializeField] private Animator _animatorUI;

        [SerializeField] private int _maxAttempts = 10;
        [SerializeField] private float _attemptDelay = 2f;

        private BaseState _currentState;
        private List<BaseState> _allStates;

        private Coroutine _retryConnection;

        private void Start()
        {
            _allStates = new List<BaseState>()
            {
                new ConnectingState(_inputHubRequisites, _outputHubRequisites, _animatorUI, this),
                new RunningState(_inputHubRequisites, _outputHubRequisites, _animatorUI, this)
            };
            _currentState = _allStates[0];
            _retryConnection = null;

            _currentState.Start();
            Connect();
        }

        public bool Connect()
        {
            bool connectionEstablished = _currentState.IsConnect();

            if (connectionEstablished == false)
            {
                if (_retryConnection == null)
                    _retryConnection = StartCoroutine(RetryConnection());
            }
            else
            {
                StopCoroutine(_retryConnection);
                _retryConnection = null;
            }

            return connectionEstablished;
        }

/*        
        public void OnConnectionLost()
        {
            SwitchState<ConnectingState>();
        }
*/

        public void SwitchState<T>() where T : BaseState
        {
            var state = _allStates.FirstOrDefault(s => s is T);
            
            _currentState.Stop();
            _currentState = state;
            _currentState.Start();

            if (_currentState is ConnectingState)
            {
                Connect();
            }
        }

        private IEnumerator RetryConnection()
        {
            int attempts = 0;
            bool isConnectionEstablished = false;
            while (attempts < _maxAttempts)
            {
                Debug.Log($"Try connection attempt: {attempts}");
                yield return new WaitForSeconds(_attemptDelay);
                isConnectionEstablished = _currentState.IsConnect();

                if (isConnectionEstablished)
                    break;
            }

            if (!isConnectionEstablished)
            {
                Debug.Log("Please, check your network connection.");
                _retryConnection = StartCoroutine(RetryConnection());
            }
        }
    }
}