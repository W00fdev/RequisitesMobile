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

        public void Connect()
        {
            _currentState.TryConnect();

            if (!(_currentState is ConnectingState))
            {
                if (_retryConnection != null)
                {
                    StopCoroutine(_retryConnection);
                    _retryConnection = null;
                }
            }
            else
            {
                if (_retryConnection == null)
                    _retryConnection = StartCoroutine(RetryConnection());
            }
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
            else if (_currentState is RunningState)
            {
                if (_retryConnection != null)
                {
                    StopCoroutine(_retryConnection);
                    _retryConnection = null;
                }
            }
        }

        private IEnumerator RetryConnection()
        {
            int attempts = 0;
            while (attempts < _maxAttempts && _currentState is ConnectingState)
            {
                Debug.Log($"Try connection attempt: {attempts}");
                yield return new WaitForSeconds(_attemptDelay);
                
                _currentState.TryConnect();
                attempts++;
            }

            if (_currentState is ConnectingState)
            {
                Debug.Log("Please, check your network connection.");
                _retryConnection = StartCoroutine(RetryConnection());
            }
        }
    }
}