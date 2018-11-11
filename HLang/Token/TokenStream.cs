using System;
using System.Collections.Generic;

namespace HLang.Token
{
    public sealed class TokenStream
    {
        private int currentId = -1;

        public List<Token> Stream { get; set; }

        public TokenStream()
        {
            Stream = new List<Token>();
        }

        public Token Current() => Stream[Math.Min(currentId, Stream.Count)];
        public Token Peek(int distance = 1) => Stream[Math.Min(currentId + distance, Stream.Count)];
        public Token Next() => Stream[Math.Min(++currentId, Stream.Count)];
        public void Unget() => --currentId;

        public void Append(Token token) => Stream.Add(token);
    }
}
