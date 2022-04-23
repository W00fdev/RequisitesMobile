using Assets.Scripts.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public static List<string> InitializeFirstOptionsIfns()
        {
            _filterIfns.AccordingOptions.Clear();
            _filterIfns.AccordingOptions.Add("");

            foreach (var option in DataInputRequisites.DataIfns.Data)
            {
                if (option.IfnsData.Count <= 0)
                    continue;

                _string.Clear();
                _string.Append(option.IfnsData.First().Key.Substring(0, 2));
                _string.Append(" - ").Append(option.RepublicName);

                _filterIfns.AccordingOptions.Add(_string.ToString());
            }

            if (_filterIfns.AccordingOptions.Count <= 1)
                throw new System.Exception("Tier 1 option can't be size <= 1");

            return _filterIfns.AccordingOptions;
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

            return _filterOktmmf.AccordingOptions;
        }

        public static List<string> UpdateDropdownIfns(List<string> OptionsCached, string newInput)
        {
            // newInput.Length != 0.
            int length = newInput.Length;

            // Check logics
            if (IsStringNumeric(newInput) == true)
            {
                return UpdateDropdownNumericsIfns(OptionsCached, newInput);
            }

            throw new System.Exception("Not realized logics");

            return _filterIfns.AccordingOptions;
        }

        // Original algorithm
        private static List<string> UpdateDropdownNumericsIfns(List<string> OptionsCached, string newInput)
        {
            if (OptionsCached.Count > 1 && OptionsCached[FIRST].Length == newInput.Length
                && OptionsCached[FIRST] == newInput)
            {
                if (OptionsCached.Count <= 1)
                    throw new System.Exception($"{newInput.Length} tier doesn't initialized.");

                // TODO: do it))
                return OptionsCached;
            }

            // Update list logics

            OptionsCached.Clear();
            OptionsCached.Add("");

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

                    OptionsCached.Add(_string.ToString());
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
                    OptionsCached.Add(_string.ToString());
                }
            }

            return OptionsCached;
        }

        private static bool IsStringNumeric(string newInput)
        {
            for (int i = 0; i < newInput.Length; i++)
            {
                if (char.IsDigit(newInput[i]) == false)
                    return false;
            }

            return true;
        }

    }

}