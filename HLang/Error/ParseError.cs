using System;

namespace HLang.Error
{
    [Serializable]
    class ParseError : Exception
    {
        public ParseError() { }

        public ParseError(string message, int line, int col)
        {
            Message = $"[ParseError] At ({line}, {col}): {message}";
        }

        public override string Message { get; }
    }
}
