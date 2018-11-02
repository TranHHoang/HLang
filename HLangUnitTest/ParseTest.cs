using HLang.Lexer;
using HLang.Parser;
using HLang.Visitor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace HLangUnitTest
{
    [TestClass]
    public class ParseTest
    {
        [TestMethod]
        public void SimpleExpr()
        {
            var src = "a=b=c=1";
            var lexer = new Lexer(new MemoryStream(Encoding.UTF8.GetBytes(src)));
            lexer.Start();
            var parser = new Parser(lexer.TokStream);
            var ast = parser.Parse(0);
            var interpreter = new AstInterpreter();
            long a = Convert.ToInt64(interpreter.Eval(ast));
        }
    }
}