using HLang.Parser.Ast;

namespace HLang.Visitor
{
    public abstract class AstVisitor
    {
        public abstract object Visit(LiteralNode node);
        public abstract object Visit(IdentifierNode node);
        public abstract object Visit(BinaryOperatorNode node);
        public abstract object Visit(PrefixOperatorNode node);
        public abstract object Visit(ComparisonNode node);
        public abstract object Visit(TernaryNode node);
        public abstract object Visit(AssignmentNode node);
    }
}
