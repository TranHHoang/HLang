namespace HLang.Token
{
    public struct Token
    {
        public enum TokenType
        {
            EndOfStream,
            IntLiteral, DoubleLiteral, StringLiteral, CharLiteral, BoolLiteral,
            HexLiteral, OctLiteral, BinLiteral,
            Identifier,
            Plus, Minus, Star, Slash, Div, Percent, Mod, DoubleStar,
            Equal, NotEqual, Less, LessOrEqual, Greater, GreaterOrEqual,
            Assign,

            Quest, Colon,

            Indent, Dedent,
        }

        public Token(TokenType type, string value = "", int line = 0, int column = 0)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
        }

        public TokenType Type { get; set; }
        public string Value { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

#if DEBUG
        public override string ToString() => string.Format("{0} {1} {2} {3}", Type, Value, Line, Column);
#else
        public override string ToString() => string.Format("Token(Type={0}, Val={1}, Pos=({2}, {3}))", Type, Value, Line, Column);
#endif

    }
}
