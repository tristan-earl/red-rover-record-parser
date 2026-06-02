namespace RedRover.RecordParser
{
    public class RecordParser
    {
        private readonly string _data;
        private int _currentIndex;

        public bool EndOfData => _currentIndex >= _data.Length;

        public RecordParser(string data, int startIndex)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new RecordParserException("Data must be non-empty.");
            }

            _data = data;
            _currentIndex = startIndex;
            SkipToNonWhitespace();
        }

        public string ReadValue()
        {
            string value = ReadTo(',');
            SkipToNonWhitespace();
            return value.Trim();
        }

        public IEnumerable<string> ReadListValues()
        {
            char[] terminatingChars = [',', ')'];
            while (_currentIndex < _data.Length && _data[_currentIndex] != ')')
            {
                string value = ReadTo(terminatingChars, skipEndChar: false);
                if (_currentIndex < _data.Length && _data[_currentIndex] == ',')
                {
                    // Landed on comma. Skip over comma and whitespace to the next value.
                    _currentIndex++;
                    SkipToNonWhitespace();
                }

                yield return value.Trim();
            }

            // Skip object end
            _currentIndex++;
        }

        public string ReadObjectMarker(string objectMarker)
        {
            string value = ReadTo('(');
            if (value.Trim() != objectMarker)
            {
                throw new RecordParserException($"Expected '{objectMarker}' marker at this position.");
            }

            SkipToNonWhitespace();
            return value;
        }

        public string ReadToObjectEnd()
        {
            return ReadTo(')').Trim();
        }

        public void SkipToNextValue()
        {
            ReadTo(',');
            SkipToNonWhitespace();
        }

        /// <summary>
        /// Reads chars from the current position and up to, but not including, the specified end char.
        /// Returns those chars in a string.
        /// </summary>
        /// <param name="endChar">Char at which to terminate reading.</param>
        /// <param name="skipEndChar">
        /// If true, current position is advanced one position past the end char.
        /// Otherwise, current position remains on end char.
        /// </param>
        private string ReadTo(char endChar, bool skipEndChar = true)
        {
            return ReadTo([endChar], skipEndChar);
        }

        /// <summary>
        /// Reads chars from the current position and up to, but not including, the specified end char.
        /// Returns those chars in a string.
        /// </summary>
        /// <param name="endChars">Chars at which to terminate reading.</param>
        /// <param name="skipEndChar">
        /// If true, current position is advanced one position past the end char.
        /// Otherwise, current position remains on end char.
        /// </param>
        private string ReadTo(char[] endChars, bool skipEndChar = true)
        {
            if (EndOfData)
            {
                throw new RecordParserException("Reached end of data.");
            }

            string value = string.Empty;
            char c;
            while (_currentIndex < _data.Length && !endChars.Contains(c = _data[_currentIndex]))
            {
                value += c;
                _currentIndex++;
            }

            if (skipEndChar)
            {
                _currentIndex++;
            }

            return value;
        }

        /// <summary>
        /// Advances the current position to the next non-whitespace char.
        /// </summary>
        private void SkipToNonWhitespace()
        {
            if (EndOfData)
            {
                throw new RecordParserException("Reached end of data.");
            }

            while (_currentIndex < _data.Length && char.IsWhiteSpace(_data[_currentIndex]))
            {
                _currentIndex++;
            }
        }
    }
}
