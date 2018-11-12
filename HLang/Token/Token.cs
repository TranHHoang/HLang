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

            And, Or, Not,

            LeftParen, RightParen,

            Assign,

            Quest, Colon,

            LeftShift, RightShift, BitwiseAnd, BitwiseOr, BitwiseXor, BitwiseNot,

            Indent, Dedent,
        }

        public Token(TokenType type, string value, int line, int column)
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
        public override string ToString() => $"{Type} {Value} {Line} {Column}";
#else
        public override string ToString() => $"Token(Type={Type}, Val={Value}, Pos=({Line}, {Column}))";
#endif

    }
}
