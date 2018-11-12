using HLang.Error;
using HLang.Parser.Ast;
using HLang.Parser.Expr.Base;
using HLang.Parser.Expr.Derived;
using HLang.Token;
using System.Collections.Generic;

using TokenType = HLang.Token.Token.TokenType;

namespace HLang.Parser
{
    public sealed class ExprParser
    {
        private readonly TokenStream _tokenStream;
        private readonly Dictionary<TokenType, IInfix> _infixMap;
        private readonly Dictionary<TokenType, IPrefix> _prefixMap;

        public ExprParser(TokenStream tokenStream)
        {
            _tokenStream = tokenStream;
            _infixMap = new Dictionary<TokenType, IInfix>();
            _prefixMap = new Dictionary<TokenType, IPrefix>();
            RegisterParser();
        }

        private void RegisterParser()
        {
            Register(TokenType.IntLiteral, new LiteralParse());
            Register(TokenType.DoubleLiteral, new LiteralParse());
            // Identifier
            Register(TokenType.Identifier, new IdentifierParse());
            // Prefix
            RegisterPrefix(TokenType.Plus, Precedence.Unary); // Ex: +2, +3
            RegisterPrefix(TokenType.Minus, Precedence.Unary); // Ex: -2, -3

            // Binary operator
            RegisterBinaryOperator(TokenType.Plus, Precedence.Additive);
            RegisterBinaryOperator(TokenType.Minus, Precedence.Additive);

            RegisterBinaryOperator(TokenType.Star, Precedence.Multiplicative);
            RegisterBinaryOperator(TokenType.Slash, Precedence.Multiplicative);
            RegisterBinaryOperator(TokenType.Div, Precedence.Multiplicative);
            RegisterBinaryOperator(TokenType.Percent, Precedence.Multiplicative);
            RegisterBinaryOperator(TokenType.Mod, Precedence.Multiplicative);

            RegisterBinaryOperatorR(TokenType.DoubleStar, Precedence.Exponent);

            Register(TokenType.Quest, new TernaryParse());
            
            // Bitwise
            RegisterBitwiseOperator(TokenType.BitwiseAnd, Precedence.BitwiseAnd);
            RegisterBitwiseOperator(TokenType.BitwiseOr, Precedence.BitwiseOr);
            RegisterBitwiseOperator(TokenType.BitwiseXor, Precedence.BitwiseXor);
            RegisterPrefix(TokenType.BitwiseNot, Precedence.BitwiseNot);
            RegisterBitwiseOperator(TokenType.LeftShift, Precedence.BitwiseShift);
            RegisterBitwiseOperator(TokenType.RightShift, Precedence.BitwiseShift);

            // Comparison
            RegisterComparison(TokenType.Equal, Precedence.Equality);
            RegisterComparison(TokenType.NotEqual, Precedence.Equality);

            RegisterComparison(TokenType.Less, Precedence.Relational);
            RegisterComparison(TokenType.LessOrEqual, Precedence.Relational);
            RegisterComparison(TokenType.Greater, Precedence.Relational);
            RegisterComparison(TokenType.GreaterOrEqual, Precedence.Relational);

            // Logical
            Register(TokenType.And, new LogicalOperatorParse(Precedence.LogicalAnd));
            Register(TokenType.Or, new LogicalOperatorParse(Precedence.LogicalOr));
            RegisterPrefix(TokenType.Not, Precedence.LogicalNot); // Ex: !(a == b)

            // Group
            Register(TokenType.LeftParen, new ParenParse()); // Ex: -2, -3

            // Assignment
            Register(TokenType.Assign, new AssignmentParse());
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

        private void Register(TokenType type, IInfix infix)
        {
            _infixMap[type] = infix;
        }

        private void Register(TokenType type, IPrefix prefix)
        {
            _prefixMap[type] = prefix;
        }

        private void RegisterPrefix(TokenType type, Precedence prec)
        {
            Register(type, new PrefixParse(prec));
        }

        private void RegisterBinaryOperator(TokenType type, Precedence precedence)
        {
            Register(type, new BinaryOperatorParse(precedence, false));
        }

        private void RegisterBitwiseOperator(TokenType type, Precedence precedence)
        {
            Register(type, new BitwiseOperatorParse(precedence));
        }

        private void RegisterComparison(TokenType type, Precedence precedence)
        {
            Register(type, new ComparisonParse(precedence));
        }

        private void RegisterBinaryOperatorR(TokenType type, Precedence precedence)
        {
            Register(type, new BinaryOperatorParse(precedence, true));
        }

        public void Consume(TokenType type)
        {
            var nextToken = _tokenStream.Next();
            if (nextToken.Type != type)
            {
                throw new ParseError($"Expected '{type}', but '{nextToken.Value}' found", nextToken.Line, nextToken.Column);
            }
        }
    }
}
