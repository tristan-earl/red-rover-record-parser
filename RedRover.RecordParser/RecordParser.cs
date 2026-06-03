namespace RedRover.RecordParser
{
    /// <summary>
    /// Parses records that use commas as field delimiters and parentheses as object delimiters.
    /// </summary>
    public class RecordParser
    {
        private readonly string _data;
        private int _currentIndex;

        public bool EndOfData => _currentIndex >= _data.Length;

        public RecordParser(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new RecordParserException("Data must be non-empty.");
            }

            _data = data;
            _currentIndex = 0;
        }

        /// <summary>
        /// Reads a field value at the current position.
        /// </summary>
        public string ReadValue()
        {
            string value = ReadTo(',');
            return value.Trim();
        }

        /// <summary>
        /// Enumerates field values from the current list object.
        /// </summary>
        public IEnumerable<string> ReadListValues()
        {
            char[] terminatingChars = [',', ')'];
            while (!EndOfData && _data[_currentIndex] != ')')
            {
                yield return ReadTo(terminatingChars, skipEndChar: false).Trim();

                if (!EndOfData && _data[_currentIndex] == ',')
                {
                    _currentIndex++;
                    
                    if (!EndOfData && _data[_currentIndex] == ')')
                    {
                        // Edge case where last list item is empty
                        yield return ReadTo(terminatingChars, skipEndChar: false).Trim();
                    }
                }
            }

            // Skip object end
            _currentIndex++;
        }

        /// <summary>
        /// Reads the object marker from the current position.
        /// </summary>
        public string ReadObjectMarker(string objectMarker)
        {
            string value = ReadTo('(');
            if (value.Trim() != objectMarker)
            {
                throw new RecordParserException($"Expected '{objectMarker}' marker at this position.");
            }

            return value;
        }

        /// <summary>
        /// Reads to the end of the current object.
        /// </summary>
        public string ReadToObjectEnd()
        {
            return ReadTo(')').Trim();
        }

        /// <summary>
        /// Advances position to the next field value.
        /// </summary>
        public void SkipToNextValue()
        {
            ReadTo(',');
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
        /// Reads chars from the current position and up to, but not including, the specified end chars.
        /// Returns those chars in a string.
        /// </summary>
        /// <param name="endChars">Chars at which to terminate reading. Reading stops when any of these chars are encountered.</param>
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
            while (!EndOfData && !endChars.Contains(c = _data[_currentIndex]))
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
    }
}
