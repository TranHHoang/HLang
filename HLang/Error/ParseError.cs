using System;

namespace HLang.Error
{
    class ParseError : Exception
    {
        public ParseError(string message, int line, int col)
        {
            Message = $"[ParseError] At ({line}, {col}): {message}";
        }

        public override string Message { get; }
    }
}
