using HLang.Visitor;

namespace HLang.Parser.Ast
{
    public class TernaryNode : AstNode
    {
        public TernaryNode(Token.Token token, AstNode testExpr, AstNode trueExpr, AstNode falseExpr) : base(NodeType.TernaryOperator, token)
        {
            TestExpr = testExpr;
            TrueExpr = trueExpr;
            FalseExpr = falseExpr;
        }

        public override object Accept(AstVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public AstNode TestExpr { get; }
        public AstNode TrueExpr { get; }
        public AstNode FalseExpr { get; }
    }
}
