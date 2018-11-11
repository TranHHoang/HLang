using HLang.Error;
using HLang.Parser.Ast;
using HLang.Parser.Expr.Base;
using HLang.Parser.Expr.Derived;
using HLang.Token;
using System.Collections.Generic;

namespace HLang.Parser
{
    public sealed class ExprParser
    {
        private readonly TokenStream _tokenStream;
        private readonly Dictionary<Token.Token.TokenType, IInfix> _infixMap;
        private readonly Dictionary<Token.Token.TokenType, IPrefix> _prefixMap;

        public ExprParser(TokenStream tokenStream)
        {
            _tokenStream = tokenStream;
            _infixMap = new Dictionary<Token.Token.TokenType, IInfix>();
            _prefixMap = new Dictionary<Token.Token.TokenType, IPrefix>();
            RegisterParser();
        }

        private void RegisterParser()
        {
            Register(Token.Token.TokenType.IntLiteral, new LiteralParse());
            Register(Token.Token.TokenType.DoubleLiteral, new LiteralParse());
            // Identifier
            Register(Token.Token.TokenType.Identifier, new IdentifierParse());
            // Prefix
            RegisterPrefix(Token.Token.TokenType.Plus); // Ex: +2, +3
            RegisterPrefix(Token.Token.TokenType.Minus); // Ex: -2, -3

            // Binary operator
            RegisterBinaryOperator(Token.Token.TokenType.Plus, Precedence.Sum);
            RegisterBinaryOperator(Token.Token.TokenType.Minus, Precedence.Sum);

            RegisterBinaryOperator(Token.Token.TokenType.Star, Precedence.Product);
            RegisterBinaryOperator(Token.Token.TokenType.Slash, Precedence.Product);
            RegisterBinaryOperator(Token.Token.TokenType.Div, Precedence.Product);
            RegisterBinaryOperator(Token.Token.TokenType.Percent, Precedence.Product);
            RegisterBinaryOperator(Token.Token.TokenType.Mod, Precedence.Product);

            RegisterBinaryOperatorR(Token.Token.TokenType.DoubleStar, Precedence.Exponent);

            Register(Token.Token.TokenType.Quest, new TernaryParse());

            // Comparison
            RegisterComparison(Token.Token.TokenType.Equal);
            RegisterComparison(Token.Token.TokenType.NotEqual);
            RegisterComparison(Token.Token.TokenType.Less);
            RegisterComparison(Token.Token.TokenType.LessOrEqual);
            RegisterComparison(Token.Token.TokenType.Greater);
            RegisterComparison(Token.Token.TokenType.GreaterOrEqual);

            // Assignment
            Register(Token.Token.TokenType.Assign, new AssignmentParse());
        }

        /// <summary>
        /// Used Pratt Parser Algorithm to parse expression
        /// </summary>
        /// <param name="precedence"></param>
        /// <returns></returns>
        public AstNode Parse(int precedence)
        {
            var token = _tokenStream.Next();

            if (!_prefixMap.ContainsKey(token.Type))
            {
                throw new ParseError($"Expected expression, but '{token.Value}' found", token.Line, token.Column);
            }

            var expr = _prefixMap[token.Type].Parse(this, token);

            while (precedence < GetPrecedence())
            {
                token = _tokenStream.Next();
                expr = _infixMap[token.Type].Parse(this, expr, token);
            }

            return expr;
        }

        private int GetPrecedence()
        {
            return _infixMap.TryGetValue(_tokenStream.Peek().Type, out var res) ? res.GetPrecedence() : 0;
        }

        private void Register(Token.Token.TokenType type, IInfix infix)
        {
            _infixMap[type] = infix;
        }

        private void Register(Token.Token.TokenType type, IPrefix prefix)
        {
            _prefixMap[type] = prefix;
        }

        private void RegisterPrefix(Token.Token.TokenType type)
        {
            Register(type, new PrefixParse());
        }

        private void RegisterBinaryOperator(Token.Token.TokenType type, Precedence precedence)
        {
            Register(type, new BinaryOperatorParse(precedence, false));
        }

        private void RegisterComparison(Token.Token.TokenType type)
        {
            Register(type, new ComparisonParse());
        }

        private void RegisterBinaryOperatorR(Token.Token.TokenType type, Precedence precedence)
        {
            Register(type, new BinaryOperatorParse(precedence, true));
        }

        public void Consume(Token.Token.TokenType type)
        {
            var nextToken = _tokenStream.Next();
            if (nextToken.Type != type)
            {
                throw new ParseError($"Expected '{type}', but '{nextToken.Value}' found", nextToken.Line, nextToken.Column);
            }
        }
    }
}
