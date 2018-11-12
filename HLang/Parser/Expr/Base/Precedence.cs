namespace HLang.Parser.Expr.Base
{
    public enum Precedence
    {
        Assignment = 1,
        Conditional,
        LogicalOr, LogicalAnd,
        // Lots of languages out there make bitwise precedence lower than comparison, which is not true
        // In HLang, we reverse the precedence to be more appropriate
        BitwiseOr, BitwiseXor, BitwiseAnd,
        Equality,
        Relational,
        BitwiseShift,
        Additive,
        Multiplicative,
        Unary,
        BitwiseNot = Unary,
        LogicalNot = Unary,
        Exponent,
    }
}
