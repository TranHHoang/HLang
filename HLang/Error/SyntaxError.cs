using System;

namespace HLang.Error
{
    [Serializable]
    public class SyntaxError : Exception
    {
        public SyntaxError() { }

        public SyntaxError(string message, int line, int col)
        {
            Message = $"[SyntaxError] At ({line}, {col}): {message}";
        }

        public override string Message { get; }
    }
}
