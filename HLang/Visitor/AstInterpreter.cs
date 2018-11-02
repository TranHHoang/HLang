using HLang.Parser.Ast;
using System;
using System.Collections.Generic;
using Math = System.Math;

namespace HLang.Visitor
{
    public class AstInterpreter : AstVisitor
    {
        private readonly Dictionary<string, object> _symbolTable = new Dictionary<string, object>();

        public override object Visit(LiteralNode node)
        {
            switch (node.Token.Type)
            {
                case Token.Token.TokenType.IntLiteral:
                    return Convert.ToInt64(node.Token.Value);
                case Token.Token.TokenType.BinLiteral:
                    return Convert.ToInt64(node.Token.Value, 2);
                case Token.Token.TokenType.OctLiteral:
                    return Convert.ToInt64(node.Token.Value, 8);
                case Token.Token.TokenType.HexLiteral:
                    return Convert.ToInt64(node.Token.Value, 16);
                case Token.Token.TokenType.DoubleLiteral:
                    return Convert.ToDouble(node.Token.Value);
                case Token.Token.TokenType.BoolLiteral:
                    return Convert.ToBoolean(node.Token.Value);
                default:
                    return null;
            }
        }

        public override object Visit(IdentifierNode node)
        {
            if (_symbolTable.ContainsKey(node.Token.Value))
            {
                return _symbolTable[node.Token.Value];
            }

            return node.Token.Value;
        }

        public override object Visit(BinaryOperatorNode node)
        {
            // Eval left
            var lhs = node.Left.Accept(this);
            // Eval right
            var rhs = node.Right.Accept(this);

            switch (node.Token.Type)
            {
                case Token.Token.TokenType.Plus:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) + Convert.ToDouble(rhs);
                    }
                    return (long)lhs + (long)rhs;

                case Token.Token.TokenType.Minus:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) - Convert.ToDouble(rhs);
                    }
                    return (long)lhs - (long)rhs;

                case Token.Token.TokenType.Star:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) * Convert.ToDouble(rhs);
                    }
                    return (long)lhs * (long)rhs;

                case Token.Token.TokenType.Slash:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) / Convert.ToDouble(rhs);
                    }
                    return Convert.ToDouble(lhs) / (long)rhs;

                case Token.Token.TokenType.Div:
                    if (lhs is double || rhs is double)
                    {
                        return (long)(Convert.ToDouble(lhs) / Convert.ToDouble(rhs));
                    }
                    return (long)lhs / (long)rhs;

                case Token.Token.TokenType.Percent:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) % Convert.ToDouble(rhs);
                    }
                    return (long)lhs % (long)rhs;

                case Token.Token.TokenType.Mod:
                    if (lhs is double || rhs is double)
                    {
                        return (Convert.ToDouble(lhs) % Convert.ToDouble(rhs) + Convert.ToDouble(rhs) % Convert.ToDouble(rhs));
                    }
                    return ((long)lhs % (long)rhs + (long)rhs) % (long)rhs;

                case Token.Token.TokenType.DoubleStar:
                    if (lhs is double || rhs is double)
                    {
                        return Math.Pow(Convert.ToDouble(lhs), Convert.ToDouble(rhs));
                    }
                    return Math.Pow((long)lhs, (long)rhs);
            }

            return null;
        }

        public override object Visit(PrefixOperatorNode node)
        {
            var expr = node.Expr.Accept(this);

            if (node.Token.Type != Token.Token.TokenType.Minus)
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

        public override object Visit(ComparisonNode node)
        {
            // Eval left
            var lhs = node.Left.Accept(this);
            // Eval right
            var rhs = node.Right.Accept(this);

            switch (node.Token.Type)
            {
                case Token.Token.TokenType.Equal:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) == Convert.ToDouble(rhs);
                    }
                    return (long)lhs == (long)rhs;

                case Token.Token.TokenType.NotEqual:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) != Convert.ToDouble(rhs);
                    }
                    return (long)lhs != (long)rhs;

                case Token.Token.TokenType.Less:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) < Convert.ToDouble(rhs);
                    }
                    return (long)lhs < (long)rhs;

                case Token.Token.TokenType.LessOrEqual:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) <= Convert.ToDouble(rhs);
                    }
                    return Convert.ToDouble(lhs) <= (long)rhs;

                case Token.Token.TokenType.Greater:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) > Convert.ToDouble(rhs);
                    }
                    return (long)lhs > (long)rhs;

                case Token.Token.TokenType.GreaterOrEqual:
                    if (lhs is double || rhs is double)
                    {
                        return Convert.ToDouble(lhs) >= Convert.ToDouble(rhs);
                    }
                    return (long)lhs >= (long)rhs;
            }

            return null;
        }

        public override object Visit(TernaryNode node)
        {
            return Convert.ToBoolean(node.TestExpr.Accept(this)) ? node.TrueExpr.Accept(this) : node.FalseExpr.Accept(this);
        }

        public override object Visit(AssignmentNode node)
        {
            var exprLeft = node.Left.Accept(this);
            var value = node.Right.Accept(this);

            _symbolTable[exprLeft is null ? node.Left.Token.Value : exprLeft.ToString()] = value;
            return value;
        }

        public object Eval(AstNode root)
        {
            switch (root)
            {
                case LiteralNode l:
                    return Visit(l);
                case BinaryOperatorNode b:
                    return Visit(b);
                case PrefixOperatorNode p:
                    return Visit(p);
                case ComparisonNode c:
                    return Visit(c);
                case TernaryNode t:
                    return Visit(t);
                case AssignmentNode a:
                    return Visit(a);
                default:
                    return null;
            }
        }
    }
}
