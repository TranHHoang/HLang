namespace HLang.Parser.Expr.Base
{
    public enum Precedence
    {
        Assignment = 1,
        Ternary,
        Or, And, Not,
        Comparison,
        Sum,
        Product,
        Prefix,
        Exponent,
        Postfix
    }
}
