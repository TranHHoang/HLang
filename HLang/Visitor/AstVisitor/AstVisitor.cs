using HLang.Parser.Ast;

namespace HLang.Visitor.AstVisitor
{
    public abstract class AstVisitor
    {
        public abstract void Visit(LiteralNode node);
        public abstract void Visit(IdentifierNode node);
        public abstract void Visit(BinaryOperatorNode node);
    }
}
