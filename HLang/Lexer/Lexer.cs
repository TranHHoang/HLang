using HLang.Helper;
using HLang.Token;
using HLang.Error;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace HLang.Lexer
{
    public sealed class Lexer : IDisposable
    {
        private enum IndentMarker
        {
            Space, Tab, None
        }
        private IndentMarker _indentStyle;
        private readonly StreamReader _fileReader;
        private Queue<char> _ungetStream; // Put back character to stream
        private static readonly Dictionary<string, Token.Token.TokenType>
            KeywordMap = new Dictionary<string, Token.Token.TokenType>()
            {
                { "mod", Token.Token.TokenType.Mod },
                { "div", Token.Token.TokenType.Div },
            };
        private static readonly Dictionary<string, Token.Token.TokenType>
            OperatorMap = new Dictionary<string, Token.Token.TokenType>()
            {
                { "+", Token.Token.TokenType.Plus },
                { "-", Token.Token.TokenType.Minus },
                { "*", Token.Token.TokenType.Star },
                { "**", Token.Token.TokenType.DoubleStar },
                { "/", Token.Token.TokenType.Slash },
                { "%", Token.Token.TokenType.Percent },
                { "=", Token.Token.TokenType.Assign },
                { "<", Token.Token.TokenType.Less },
                { "<=", Token.Token.TokenType.LessOrEqual },
                { ">", Token.Token.TokenType.Greater },
                { ">=", Token.Token.TokenType.GreaterOrEqual },
                { "==", Token.Token.TokenType.Equal },
                { "!=", Token.Token.TokenType.NotEqual },
                { "?", Token.Token.TokenType.Quest },
                { ":", Token.Token.TokenType.Colon },
            };

        private const int BasePrefixLength = 2; // 0x 0b 0o

        /// <summary>
        /// Initialize lexer's components
        /// </summary>
        private void Init()
        {
            TokStream = new TokenStream();
            _ungetStream = new Queue<char>();
            Line = 0;
            Column = 0;
        }

        /// <summary>
        /// Constructs new Lexer using file path
        /// </summary>
        /// <param name="fileName"></param>
        public Lexer(string fileName)
        {
            _fileReader = new StreamReader(fileName);
            Init();
        }

        /// <summary>
        /// Constructs Lexer from existing stream
        /// </summary>
        /// <param name="stream"></param>
        public Lexer(Stream stream)
        {
            _fileReader = new StreamReader(stream);
            Init();
        }

        public void Dispose()
        {
            _fileReader.Close();
        }

        public TokenStream TokStream { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        private char Next()
        {
            var c = _ungetStream.Any() ? _ungetStream.Dequeue() : (char)_fileReader.Read();
            ++Column;
            return c;
        }

        private bool IsEOF() => _fileReader.EndOfStream && !_ungetStream.Any();

        private void Unget(char c)
        {
            _ungetStream.Enqueue(c);
            --Column;
        }

        private char Peek() => _ungetStream.Any() ? _ungetStream.Peek() : (char)_fileReader.Peek();

        private Token.Token ParseIdentifier()
        {
            string value = "";
            while (!IsEOF())
            {
                char currentChar = Peek();
                if (currentChar.IsIdentifier())
                {
                    value += Next();
                }
                else
                {
                    break;
                }
            }

            return KeywordMap.ContainsKey(value) ?
                new Token.Token(KeywordMap[value], value, Line, Column) :
                new Token.Token(Token.Token.TokenType.Identifier, value, Line, Column);
        }

        private Token.Token ParseNumber()
        {
            var value = "";
            char currentChar = Next();

            switch (currentChar)
            {
                case '0' when Peek().IsBasePrefix():
                    currentChar = Next();
                    switch (char.ToLower(currentChar))
                    {
                        case 'x':
                            return ParseInt(16);
                        case 'b':
                            return ParseInt(2);
                        default:
                            return ParseInt(8);
                    }
                case '.':
                    return ParseReal(currentChar.ToString());
                default:
                {
                    value += currentChar;
                    while (!IsEOF())
                    {
                        currentChar = Peek();
                        if (currentChar.IsNumberLiteral())
                        {
                            value += Next();
                        }
                        else if (currentChar.IsUniqueRealLiteral())
                        {
                            return ParseReal(value);
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Validate string
                    if (!value.EndsWith("_") && BigInteger.TryParse(value.Replace("_", ""), out _))
                    {
                        return new Token.Token(Token.Token.TokenType.IntLiteral, value, Line, Column - value.Length);
                    }
                    throw new SyntaxError($"Invalid integer number '{value}'", Line, Column);
                }
            }
        }

        private Token.Token ParseReal(string parsedValue)
        {
            var tempVal = Next().ToString();
            var hasPlusOrMinus = false;

            while (!IsEOF())
            {
                if (Peek().IsUniqueRealLiteral()
                    || Peek().IsNumberLiteral()
                    || (Peek() == '+' || Peek() == '-') && !hasPlusOrMinus)
                {
                    hasPlusOrMinus = Peek() == '+' || Peek() == '-';
                    tempVal += Next();
                }
                else
                {
                    break;
                }
            }

            // Check if valid
            if (!(parsedValue + tempVal).EndsWith("_") && double.TryParse((parsedValue + tempVal).Replace("_", ""), out _))
            {
                return new Token.Token(Token.Token.TokenType.DoubleLiteral, parsedValue + tempVal, Line, Column - parsedValue.Length - tempVal.Length);
            }
            throw new SyntaxError($"Invalid double literal '{parsedValue + tempVal}'", Line, Column);
        }

        private Token.Token ParseInt(int numberBase)
        {
            var value = "";

            while (!IsEOF())
            {
                switch (numberBase)
                {
                    case 2 when Peek().IsBin():
                    case 8 when Peek().IsOctal():
                    case 16 when Peek().IsHex():
                        value += Next();
                        continue;
                }

                break;
            }

            return new Token.Token(numberBase == 2 ? Token.Token.TokenType.BinLiteral :
                                    numberBase == 8 ? Token.Token.TokenType.OctLiteral :
                                    Token.Token.TokenType.HexLiteral, value, Line, Column - value.Length - BasePrefixLength);
        }

        private Token.Token ParseString()
        {
            var value = "";

            while (!IsEOF())
            {
                if (Peek() == '"' || Peek() == '\n')
                {
                    break;
                }

                value += Next();
            }
            Next(); // Skip close quote '"'

            return new Token.Token(Token.Token.TokenType.StringLiteral, value, Line, Column - value.Length);
        }

        /// <summary>
        /// Auto insert INDENT and DEDENT token based on current depth, output the total indentation token
        /// </summary>
        /// <param name="prevDepth"></param>
        /// <param name="currentDepth"></param>
        /// <param name="totalIndent"></param>
        void HandleIndent(int prevDepth, int currentDepth, ref int totalIndent)
        {
            totalIndent += currentDepth > prevDepth ? 1 : -1;
            TokStream.Stream.Add(new Token.Token(currentDepth > prevDepth ? Token.Token.TokenType.Indent : Token.Token.TokenType.Dedent));
        }

        int GetIndentDepth()
        {
            var depth = 0;
            while (!IsEOF() && (Peek() == '\t' || Peek() == ' '))
            {
                if (Peek() == '\t' && _indentStyle == IndentMarker.Space
                    || Peek() == ' ' && _indentStyle == IndentMarker.Tab)
                {
                    throw new SyntaxError($"Inconsistent indentation marker: Expected '{_indentStyle}' but {1 - _indentStyle} found.",
                        Line, Column);
                }

                Next();
                ++depth;
            }

            return depth;
        }

        public void Start()
        {
            try
            {
                var prevDepth = 0;
                var totalIndent = 0;

                while (!IsEOF())
                {
                    var currentChar = Peek();
                    if (char.IsLetter(currentChar) || currentChar == '_')
                    {
                        TokStream.Stream.Add(ParseIdentifier());
                    }
                    else if (char.IsDigit(currentChar) || currentChar == '.' && char.IsDigit(Peek())) // .1234
                    {
                        TokStream.Stream.Add(ParseNumber());
                    }
                    else
                    {
                        switch (currentChar)
                        {
                            case '\n':
                            {
                                Next();

                                var currentDepth = GetIndentDepth();
                                HandleIndent(prevDepth, currentDepth, ref totalIndent);
                                prevDepth = currentDepth;

                                ++Line;
                                Column = 0;
                                break;
                            } 
                            case '"':
                                Next();
                                TokStream.Stream.Add(ParseString());
                                break;
                            default:
                            {
                                if (char.IsWhiteSpace(currentChar))
                                {
                                    Next();
                                    continue;
                                }
                                var actualOperator = Next().ToString();

                                if (OperatorMap.ContainsKey(currentChar.ToString()))
                                {
                                    // We do not need to check if EOF because if it happens, 
                                    // Peek() return -1
                                    // Lookahead two chars
                                    if (OperatorMap.ContainsKey(currentChar.ToString() + Peek()))
                                    {
                                        actualOperator += Next();
                                    }

                                    TokStream.Stream.Add(new Token.Token(OperatorMap[actualOperator], actualOperator, Line, Column - actualOperator.Length));
                                }
                                else
                                {
                                    Unget(currentChar);
                                }
                            }
                            break;
                        }
                    }
                }
                // Insert dedent
                while (totalIndent-- > 0) TokStream.Stream.Add(new Token.Token(Token.Token.TokenType.Dedent));
                // We already go through the source file
                TokStream.Stream.Add(new Token.Token(Token.Token.TokenType.EndOfStream, "", Line, Column));
            }
            catch (SyntaxError e)
            {
                Console.Error.WriteLine(e.Message);
#if DEBUG
                throw;
#endif
            }
        }
    }
}
