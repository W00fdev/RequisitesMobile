using Assets.Scripts.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Shared
{
    public static class SearchbarParser
    {
        private struct OptimizedParserFilter
        {
            public bool HasDigits;
            public bool DigitsComplete;

            public string PreviousInput;
            public string FoundSubstring;

            public List<string> AccordingOptions;
        }

        private static OptimizedParserFilter _filterIfns;
        private static OptimizedParserFilter _filterOktmmf;

        private static System.Text.StringBuilder _string;

        private static readonly int CharacterLimitIfns;
        private static readonly int CharacterLimitOktmmf;

        private static readonly int FIRST = 1;

        static SearchbarParser()
        {
            _filterIfns = new OptimizedParserFilter
            {
                HasDigits = false,
                DigitsComplete = false,
                PreviousInput = "NOT INITIALIZED",
                FoundSubstring = "",
                AccordingOptions = new List<string>()
            };

            _filterOktmmf = new OptimizedParserFilter
            {
                HasDigits = false,
                DigitsComplete = false,
                PreviousInput = "NOT INITIALIZED",
                FoundSubstring = "",
                AccordingOptions = new List<string>()
            };

            _string = new System.Text.StringBuilder(75);

            CharacterLimitIfns = 4;
            CharacterLimitOktmmf = 8;
        }

        // ============================ INITIALIZE FIRST OPTIONS ===============================

        // Returns the copy of origin list and changes the ref to numerics

        public static List<string> InitializeFirstOptionsIfns(ref List<string> OptionsNumerics)
        {
            OptionsNumerics.Clear();
            OptionsNumerics.Add("");

            _filterIfns.AccordingOptions.Clear();
            _filterIfns.AccordingOptions.Add("");

            // Numeric and non-numeric initializing
            foreach (var data in DataInputRequisites.DataIfns.Data)
            {
                if (data.IfnsData.Count <= 0)
                    continue;

                _string.Clear();
                _string.Append(data.IfnsData.First().Key.Substring(0, 2));
                _string.Append(" - ").Append(data.RepublicName);

                OptionsNumerics.Add(_string.ToString());

                // Non-numeric initializing
                foreach(var option in data.IfnsData)
                {
                    _string.Clear();
                    _string.Append(option.Key).Append(" - ").Append(option.Value);
                    _filterIfns.AccordingOptions.Add(_string.ToString());
                }
            }

            if (OptionsNumerics.Count <= 1)
                throw new System.Exception("Tier 1 option can't be size <= 1");

            return _filterIfns.AccordingOptions.ToList();
        }

        public static List<string> InitializeFirstOptionsOktmmf(ref char[] digits)
        {
            _filterOktmmf.AccordingOptions.Clear();
            _filterOktmmf.AccordingOptions.Add("");

            foreach (var option in DataInputRequisites.DataOktmmf.Data)
            {
                _string.Clear();
                _string.Append(option.Key).Append(" - ").Append(option.Value.ToASCII());

                for (int i = 0; i < CharacterLimitOktmmf; i++)
                {
                    if (digits[i] == '-')
                        digits[i] = _string[i];
                    else if (digits[i] == '+')
                        break;
                    else if (digits[i] != _string[i])
                    {
                        digits[i] = '+';
                        for (int j = i; j < CharacterLimitOktmmf; j++)
                            digits[j] = '+';
                        break;
                    }
                }

                _filterOktmmf.AccordingOptions.Add(_string.ToString());
            }

            if (_filterOktmmf.AccordingOptions.Count <= 1)
                throw new System.Exception("Tier 1 option oktmmf can't be size <= 1");

            return _filterOktmmf.AccordingOptions.ToList();
        }

        // ======================================================================================




        // ============================ UPDATE DROPDOWN IFNS ===============================

        // Returns the copy of list

        // Original + words parser
        public static List<string> UpdateDropdownIfns(List<string> OptionsOrigin, ref List<string> OptionsCached, string newInput)
        {
            // newInput.Length != 0.
            int length = newInput.Length;

            // Check logics
            if (length < CharacterLimitIfns && IsStringNumeric(newInput) == true)
            {
                return UpdateDropdownNumericsIfns(ref OptionsCached, newInput);
            }

            /*  foreach (var ifnsDictionary in DataInputRequisites.DataIfns.Data)
            {
                if (ifnsDictionary.IfnsData.Count <= 0)
                    continue;

                foreach (var option in ifnsDictionary.IfnsData)
                {
                    _string.Clear();
                    _string.Append(option.Key).Append(" - ").Append(option.Value);

                    if ()
                }
            }*/

            List<string> currentOptions;
            if (length > _filterIfns.PreviousInput.Length)
                currentOptions = _filterIfns.AccordingOptions.ToList();
            else
                currentOptions = OptionsOrigin;

            _filterIfns.AccordingOptions.Clear();
            _filterIfns.AccordingOptions.Add("");

            foreach (var option in currentOptions)
            {
                int index = option.IndexOf(newInput);
                if (index == -1)
                    continue;

                StringBuilder optionBuilder = new StringBuilder(option);
                optionBuilder.Insert(index, "<b>");
                optionBuilder.Insert(index + 3 + newInput.Length, "</b>");

                _filterIfns.AccordingOptions.Add(optionBuilder.ToString());
            }

            // return {null list for error}
            return _filterIfns.AccordingOptions.ToList();
        }

        // Changes the original OptionsCached

        // Original algorithm
        private static List<string> UpdateDropdownNumericsIfns(ref List<string> OptionsCachedNumerics, string newInput)
        {
            if (OptionsCachedNumerics.Count > 1 && OptionsCachedNumerics[FIRST].Length == newInput.Length
                && OptionsCachedNumerics[FIRST] == newInput)
            {
                if (OptionsCachedNumerics.Count <= 1)
                    throw new System.Exception($"{newInput.Length} tier doesn't initialized.");

                // TODO: do it))
                return OptionsCachedNumerics;
            }

            // Update list logics

            OptionsCachedNumerics.Clear();
            OptionsCachedNumerics.Add("");

            bool foundMatch = false;
            foreach (var option in DataInputRequisites.DataIfns.Data)
            {
                if (option.IfnsData.Count <= 0)
                    continue;

                _string.Clear();
                _string.Append(option.IfnsData.First().Key.Substring(0, 2));

                // Elements are sorted.
                int highIndex = Mathf.Clamp(newInput.Length, 0, 2);
                int checkSortedMatch = string.Compare(newInput.Substring(0, highIndex),
                    _string.ToString().Substring(0, highIndex));

                if (checkSortedMatch != 0)
                {
                    if (foundMatch == true)
                        break;
                    continue;
                }
                foundMatch = true;

                if (newInput.Length == 1)
                {
                    _string.Append(" - ").Append(option.RepublicName);

                    OptionsCachedNumerics.Add(_string.ToString());
                    continue;
                }

                bool foundInnerMatch = false;
                foreach (var ifnsData in option.IfnsData)
                {
                    // Elements are sorted.
                    checkSortedMatch = string.Compare(newInput.Substring(0, newInput.Length),
                        ifnsData.Key.Substring(0, newInput.Length));

                    if (checkSortedMatch != 0)
                    {
                        if (foundInnerMatch == true)
                            break;
                        continue;
                    }
                    foundInnerMatch = true;

                    _string.Clear();
                    _string.Append(ifnsData.Key).Append(" - ").Append(ifnsData.Value);

                    // Length == [2..4]
                    OptionsCachedNumerics.Add(_string.ToString());
                }
            }

            return OptionsCachedNumerics;
        }


        // ==================================================================================




        // ============================ UPDATE DROPDOWN OKTMMF ===============================

        // TODO: change by the same way: params and return types

        public static List<string> UpdateDropdownOktmmf(List<string> optionsOrigin, string newInput)
        {
            if (IsStringNumeric(newInput) == true)
                return UpdateDropdownNumericsOktmmf(optionsOrigin, newInput);

            throw new System.Exception("Not implemented logic");

            return _filterOktmmf.AccordingOptions;
        }

        private static List<string> UpdateDropdownNumericsOktmmf(List<string> optionsOrigin, string newInput)
        {
            List<string> previousCopy;
            //
            // Use previous or use otigin
            if (newInput.Length > _filterOktmmf.PreviousInput.Length)
                previousCopy = new List<string>(_filterOktmmf.AccordingOptions);
            else
                previousCopy = new List<string>(optionsOrigin);

            _filterOktmmf.AccordingOptions.Clear();
            _filterOktmmf.AccordingOptions.Add("");

            // Main cycle-logic
            bool foundMatch = false;
            foreach (var option in previousCopy)
            {
                if (option == "")
                    continue;

                _string.Clear();
                _string.Append(option.Substring(0, CharacterLimitOktmmf));

                // Elements are sorted.
                int checkSortedMatch = string.Compare(newInput.Substring(0, newInput.Length),
                    _string.ToString().Substring(0, newInput.Length));

                if (checkSortedMatch != 0)
                {
                    if (foundMatch == true)
                        break;
                    continue;
                }
                foundMatch = true;

                _string.Append(option.Substring(CharacterLimitOktmmf).ToASCII());
                _filterOktmmf.AccordingOptions.Add(_string.ToString());
            }

            _filterOktmmf.PreviousInput = newInput;
            return _filterOktmmf.AccordingOptions;
        }

        // ==================================================================================





        // ============================ ADDITIONAL ===============================

        public static void SetPreviousInputOktmmf(string previousInput)
        {
            // Need to implement this in UpdateDropdown { newInput.Length == 0 }
            _filterOktmmf.PreviousInput = previousInput;
        }

        public static bool IsStringNumeric(string newInput)
        {
            // Numeric length can't be more than oktmmf.lenght
            for (int i = 0; i < newInput.Length; i++)
            {
                if (char.IsDigit(newInput[i]) == false)
                    return false;

                if (i >= CharacterLimitOktmmf)
                    return false;
            }

            return true;
        }

        // =======================================================================
    }

}