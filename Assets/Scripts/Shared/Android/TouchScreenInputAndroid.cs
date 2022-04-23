using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Shared
{
    public class TouchScreenInputAndroid : MonoBehaviour
    {
        private TouchScreenKeyboard _keyboard = null;
        private SearchbarBase _currentSearchBar = null;

        private string _previousInput = "";

        public void ShowKeyboard(SearchbarBase searchbar)
        {
            _currentSearchBar = searchbar;
            _keyboard = TouchScreenKeyboard.Open(_currentSearchBar.Text, TouchScreenKeyboardType.NumberPad, false,
                false, false, false, "", _currentSearchBar.CharacterLimitAccessor);
            _previousInput = _keyboard.text;
        }

        [System.Obsolete]
        private void Update()
        {
            if (_keyboard != null && _keyboard.done == true)
            {
                _currentSearchBar.UpdateByKeyboard(_keyboard.text);
                _keyboard = null;
            }
            else if (_keyboard != null && _keyboard.text != _previousInput)
            {
                _currentSearchBar.UpdateByKeyboard(_keyboard.text);
                _previousInput = _keyboard.text;
            }
        }
    }

}
