using System;

namespace HLang.Error
{
    public class LexerError : Exception
    {
        public LexerError(string message, int line, int col)
        {
            Message = $"[ParseError] At ({line}, {col}): {message}";
        }

        public override string Message { get; }
    }
}
