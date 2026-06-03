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
        public string ReadValue(bool handleEscaping = true)
        {
            string value = ReadTo(',', handleEscaping: handleEscaping);
            return value.Trim();
        }

        /// <summary>
        /// Enumerates field values from the current list object.
        /// </summary>
        public IEnumerable<string> ReadListValues(bool handleEscaping = true)
        {
            char[] terminatingChars = [',', ')'];
            while (!EndOfData && _data[_currentIndex] != ')')
            {
                yield return ReadTo(terminatingChars, skipEndChar: false, handleEscaping: handleEscaping).Trim();

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
        public string ReadToObjectEnd(bool handleEscaping = true)
        {
            return ReadTo(')', handleEscaping: handleEscaping).Trim();
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
        /// <param name="handleEscaping">
        /// True to handle escaped sequences surrounded by double quotes.
        /// </param>
        private string ReadTo(char endChar, bool skipEndChar = true, bool handleEscaping = false)
        {
            return ReadTo([endChar], skipEndChar, handleEscaping);
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
        /// <param name="handleEscaping">
        /// True to handle escaped sequences surrounded by double quotes.
        /// </param>
        private string ReadTo(char[] endChars, bool skipEndChar = true, bool handleEscaping = false)
        {
            if (EndOfData)
            {
                throw new RecordParserException("Reached end of data.");
            }

            bool escapedMode = false;
            if (handleEscaping)
            {
                // Escaped segments are surrounded by double quotes.
                // Check if the first non-whitespace char is a double quote.
                SkipToNonWhitespaceChar();

                if (!EndOfData && _data[_currentIndex] == '"')
                {
                    escapedMode = true;
                    _currentIndex++;
                }
            }

            string value = string.Empty;
            char c;
            while (!EndOfData && (!endChars.Contains(c = _data[_currentIndex]) || escapedMode))
            {
                if (c == '"')
                {
                    // Double quote (") could mean we've reached the end of the escaped segment.
                    // Check if there are two consecutive double quotes (""), which is just an
                    // escaped double quote.
                    _currentIndex++;
                    if (EndOfData)
                    {
                        throw new RecordParserException("Reached end of data.");
                    }

                    c = _data[_currentIndex];
                    if (c != '"')
                    {
                        SkipToNonWhitespaceChar();
                        if (!EndOfData && endChars.Contains(_data[_currentIndex]))
                        {
                            // End char follows double quote. That means we're done reading.
                            break;
                        }

                        throw new RecordParserException("Invalid escape sequence.");
                    }
                }

                value += c;
                _currentIndex++;
            }

            if (skipEndChar)
            {
                _currentIndex++;
            }

            return value;
        }

        private void SkipToNonWhitespaceChar()
        {
            while (!EndOfData && char.IsWhiteSpace(_data[_currentIndex]))
            {
                _currentIndex++;
            }
        }
    }
}
