using HLang.Parser.Ast;
using System;
using System.Collections.Generic;

using Math = System.Math;
using TokenType = HLang.Token.Token.TokenType;

namespace HLang.Visitor
{
    public sealed class AstInterpreter
    {
        private readonly Dictionary<string, object> _symbolTable = new Dictionary<string, object>();

        public object Visit(LiteralNode node)
        {
            switch (node.Token.Type)
            {
                case TokenType.IntLiteral:
                    return Convert.ToInt64(node.Token.Value);
                case TokenType.BinLiteral:
                    return Convert.ToInt64(node.Token.Value, 2);
                case TokenType.OctLiteral:
                    return Convert.ToInt64(node.Token.Value, 8);
                case TokenType.HexLiteral:
                    return Convert.ToInt64(node.Token.Value, 16);
                case TokenType.DoubleLiteral:
                    return Convert.ToDouble(node.Token.Value);
                case TokenType.BoolLiteral:
                    return Convert.ToBoolean(node.Token.Value);
                default:
                    return null;
            }
        }

        public object Visit(IdentifierNode node)
        {
            if (_symbolTable.ContainsKey(node.Token.Value))
            {
                return _symbolTable[node.Token.Value];
            }

            return node.Token.Value;
        }

        public object Visit(BinaryOperatorNode node)
        {
            // Eval left
            var lhs = Visit((dynamic)node.Left);
            // Eval right
            var rhs = Visit((dynamic)node.Right);

            switch (node.Token.Type)
            {
                case TokenType.Plus:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) + Convert.ToDouble(rhs);
                    }
                    return (long)lhs + (long)rhs;

                case TokenType.Minus:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) - Convert.ToDouble(rhs);
                    }
                    return (long)lhs - (long)rhs;

                case TokenType.Star:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) * Convert.ToDouble(rhs);
                    }
                    return (long)lhs * (long)rhs;

                case TokenType.Slash:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) / Convert.ToDouble(rhs);
                    }
                    return Convert.ToDouble(lhs) / (long)rhs;

                case TokenType.Div:
                    if (lhs is double || rhs is double)
                    {
                        return (long)(Convert.ToDouble(lhs) / Convert.ToDouble(rhs));
                    }
                    return (long)lhs / (long)rhs;

                case TokenType.Percent:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) % Convert.ToDouble(rhs);
                    }
                    return (long)lhs % (long)rhs;

                case TokenType.Mod:
                    if (lhs is double || rhs is double)
                    {
                        return (Convert.ToDouble(lhs) % Convert.ToDouble(rhs) 
                            + Convert.ToDouble(rhs) % Convert.ToDouble(rhs));
                    }
                    return ((long)lhs % (long)rhs + (long)rhs) % (long)rhs;

                case TokenType.DoubleStar:
                    if (lhs is double || rhs is double)
                    {
                        return Math.Pow(Convert.ToDouble(lhs), Convert.ToDouble(rhs));
                    }
                    return Math.Pow((long)lhs, (long)rhs);
            }

            return null;
        }

        public object Visit(PrefixOperatorNode node)
        {
            var expr = Visit((dynamic)node.Expr);

            if (node.Token.Type != TokenType.Minus)
            {
                return expr;
            }

            switch (expr)
            {
                case long i:
                    return -i;
                case double d:
                    return -d;
            }

            return null;
        }

        public object Visit(ComparisonNode node)
        {
            // Eval left
            var lhs = Visit((dynamic)node.Left);
            // Eval right
            var rhs = Visit((dynamic)node.Right);

            switch (node.Token.Type)
            {
                case TokenType.Equal:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) == Convert.ToDouble(rhs);
                    }
                    return (long)lhs == (long)rhs;

                case TokenType.NotEqual:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) != Convert.ToDouble(rhs);
                    }
                    return (long)lhs != (long)rhs;

                case TokenType.Less:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) < Convert.ToDouble(rhs);
                    }
                    return (long)lhs < (long)rhs;

                case TokenType.LessOrEqual:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) <= Convert.ToDouble(rhs);
                    }
                    return Convert.ToDouble(lhs) <= (long)rhs;

                case TokenType.Greater:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) > Convert.ToDouble(rhs);
                    }
                    return (long)lhs > (long)rhs;

                case TokenType.GreaterOrEqual:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) >= Convert.ToDouble(rhs);
                    }
                    return (long)lhs >= (long)rhs;
            }

            return null;
        }

        public object Visit(TernaryNode node)
        {
            return Convert.ToBoolean(Visit((dynamic)node.TestExpr)) 
                ? Visit((dynamic)node.TrueExpr) 
                : Visit((dynamic)node.FalseExpr);
        }

        public object Visit(AssignmentNode node)
        {
            var exprLeft = Visit((dynamic)node.Left);
            var value = Visit((dynamic)node.Right);

            _symbolTable[exprLeft?.ToString() ?? node.Left.Token.Value] = value;
            return value;
        }

        public object Eval(AstNode root)
        {
            return Visit((dynamic)root);
        }
    }
}
