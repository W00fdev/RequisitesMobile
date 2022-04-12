using System;
using TMPro;

namespace Assets.Scripts.Shared
{
    class OnlyNumericsValidator : TMP_InputValidator
    {
        //private readonly string _allowedSymbols = "0123456789";
        private int _maxLength;

        public OnlyNumericsValidator(int maxLength) => _maxLength = maxLength;
        public override char Validate(ref string text, ref int pos, char ch)
        {
            /*            if (_allowedSymbols.Contains(ch.ToString()) && pos <= text.Length
                            && pos < _maxLength && text.Length < _maxLength)
                        {
                            text = text.Insert(pos, ch.ToString());
                            pos++;
                            return ch;
                        }*/
            if (ch >= '0' && ch <= '9')
                return ch;
            return (char)0;
        }
    }
}
