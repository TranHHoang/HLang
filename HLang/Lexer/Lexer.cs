using HLang.Helper;
using HLang.Token;
using HLang.Error;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

using TokenType = HLang.Token.Token.TokenType;

namespace HLang.Lexer
{
    public sealed class Lexer : IDisposable
    {
        private enum IndentMarker
        {
            Space, Tab, None
        }

        private IndentMarker _indentStyle = IndentMarker.None;
        private readonly Queue<char> _ungetStream = new Queue<char>(); // Put back character to stream

        private static readonly Dictionary<string, TokenType>
            KeywordMap = new Dictionary<string, TokenType>()
            {
                { "div", TokenType.Div },
                { "mod", TokenType.Mod },
            };

        private static readonly Dictionary<string, TokenType>
            OperatorMap = new Dictionary<string, TokenType>()
            {
                { "+", TokenType.Plus },
                { "-", TokenType.Minus },
                { "*", TokenType.Star },
                { "**", TokenType.DoubleStar },
                { "/", TokenType.Slash },
                { "%", TokenType.Percent },
                { "=", TokenType.Assign },
                { "<", TokenType.Less },
                { "<=", TokenType.LessOrEqual },
                { ">", TokenType.Greater },
                { ">=", TokenType.GreaterOrEqual },
                { "==", TokenType.Equal },
                { "!=", TokenType.NotEqual },
                { "?", TokenType.Quest },
                { ":", TokenType.Colon },
                { "&&", TokenType.And },
                { "||", TokenType.Or },
                { "!", TokenType.Not },

                { "(", TokenType.LeftParen },
                { ")", TokenType.RightParen },

                { "&", TokenType.BitwiseAnd },
                { "|", TokenType.BitwiseOr },
                { "^", TokenType.BitwiseXor },
                { "~", TokenType.BitwiseNot },

                { "<<", TokenType.LeftShift },
                { ">>", TokenType.RightShift },
            };

        private const int BasePrefixLength = 2; // 0x 0b 0o

        private readonly StreamReader _fileReader;

        /// <summary>
        /// Initialize lexer's components
        /// </summary>
        private void Init()
        {
            TokStream = new TokenStream();
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

        public TokenStream TokStream { get; private set; }
        public int Line { get; set; }
        public int Column { get; set; }

        private char Next()
        {
            ++Column;
            return _ungetStream.Any() ? _ungetStream.Dequeue() : (char)_fileReader.Read();
        }

        private bool IsEof() => _fileReader.EndOfStream && !_ungetStream.Any();

        private void Unget(char c)
        {
            _ungetStream.Enqueue(c);
            --Column;
        }

        private char Peek() => _ungetStream.Any() ? _ungetStream.Peek() : (char)_fileReader.Peek();

        private Token.Token ParseIdentifier()
        {
            string value = "";
            while (!IsEof())
            {
                if (!Peek().IsIdentifier()) break;
                value += Next();
            }

            return KeywordMap.ContainsKey(value) ?
                new Token.Token(KeywordMap[value], value, Line, Column) :
                new Token.Token(TokenType.Identifier, value, Line, Column);
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
                    while (!IsEof())
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
                        return new Token.Token(TokenType.IntLiteral, value, Line, Column - value.Length);
                    }
                    throw new SyntaxError($"Invalid integer number '{value}'", Line, Column);
                }
            }
        }

        private Token.Token ParseReal(string parsedValue)
        {
            var tempVal = Next().ToString();
            var hasPlusOrMinus = false;

            while (!IsEof())
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
                return new Token.Token(TokenType.DoubleLiteral, parsedValue + tempVal, Line, Column - parsedValue.Length - tempVal.Length);
            }
            throw new SyntaxError($"Invalid double literal '{parsedValue + tempVal}'", Line, Column);
        }

        private Token.Token ParseInt(int numberBase)
        {
            var value = "";

            while (!IsEof())
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

            return new Token.Token(numberBase == 2 ? TokenType.BinLiteral :
                                    numberBase == 8 ? TokenType.OctLiteral :
                                    TokenType.HexLiteral, value, Line, Column - value.Length - BasePrefixLength);
        }

        private Token.Token ParseString()
        {
            var value = "";

            while (!IsEof())
            {
                if (Peek() == '"' || Peek() == '\n') break;
                value += Next();
            }
            Next(); // Skip close quote '"'

            return new Token.Token(TokenType.StringLiteral, value, Line, Column - value.Length);
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
            TokStream.Stream.Add(new Token.Token(currentDepth > prevDepth 
                ? TokenType.Indent 
                : TokenType.Dedent, "", Line, Column));
        }

        int GetIndentDepth()
        {
            var depth = 0;
            while (!IsEof() && (Peek() == '\t' || Peek() == ' '))
            {
                if (Peek() == '\t' && _indentStyle == IndentMarker.Space
                    || Peek() == ' ' && _indentStyle == IndentMarker.Tab)
                {
                    throw new SyntaxError($"Inconsistent indentation marker: Expected '{_indentStyle}' but {1 - _indentStyle} found.",
                        Line, Column);
                }
                else if (_indentStyle == IndentMarker.None)
                {
                    _indentStyle = Peek() == '\t' ? IndentMarker.Tab : IndentMarker.Space;
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

                while (!IsEof())
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

                                if (OperatorMap.ContainsKey(currentChar.ToString()) || OperatorMap.ContainsKey(currentChar.ToString() + Peek()))
                                {
                                    // We do not need to check if EOF because if it happens, 
                                    // Peek() return -1
                                    // Lookahead two chars
                                    if (OperatorMap.ContainsKey(currentChar.ToString() + Peek()))
                                    {
                                        actualOperator += Next();
                                    }

                                    TokStream.Stream.Add(new Token.Token(
                                        OperatorMap[actualOperator], actualOperator, 
                                        Line, Column - actualOperator.Length
                                    ));
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
                while (totalIndent-- > 0)
                    TokStream.Stream.Add(new Token.Token(TokenType.Dedent, "", Line, Column));
                // We already go through the source file
                TokStream.Stream.Add(new Token.Token(TokenType.EndOfStream, "", Line, Column));
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
