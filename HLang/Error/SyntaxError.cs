using System;

namespace HLang.Error
{
    public class SyntaxError : Exception
    {
        public SyntaxError(string message, int line, int col)
        {
            Message = $"[SyntaxError] At ({line}, {col}): {message}";
        }

        public override string Message { get; }
    }
}
