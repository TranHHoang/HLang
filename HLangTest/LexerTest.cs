using HLang.Lexer;
using HLang.Error;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace HLangTest
{
    [TestClass]
    public class LexerTest
    {
        [TestMethod]
        public void SimpleAdd()
        {
            string src = "1+1";

            var lexer = new Lexer(new MemoryStream(Encoding.UTF8.GetBytes(src)));
            lexer.Start();

            var actual = string.Join("", lexer.TokStream.Stream);

            Assert.AreEqual(
                "INT_LITERAL 1 0 0" +
                "PLUS + 0 1" +
                "INT_LITERAL 1 0 2" +
                "END_OF_STREAM EOS 0 0", actual);
        }

        [TestMethod]
        [ExpectedException(typeof(SyntaxError))]
        public void SimpleIntExpr()
        {
            string src = "1_+2*3";

            var lexer = new Lexer(new MemoryStream(Encoding.UTF8.GetBytes(src)));
            lexer.Start();
        }

        [TestMethod]
        public void SimpleIntExpr2()
        {
            string src = "1**2>3";

            var lexer = new Lexer(new MemoryStream(Encoding.UTF8.GetBytes(src)));
            lexer.Start();

            var actual = string.Join("", lexer.TokStream.Stream);

            Assert.AreEqual(
                "INT_LITERAL 1 0 0" +
                "DSTAR ** 0 1" +
                "INT_LITERAL 2 0 3" +
                "GREATER > 0 4" +
                "INT_LITERAL 3 0 5" +
                "END_OF_STREAM EOS 0 0", actual);
        }

        [TestMethod]
        public void SimpleRealExpr()
        {
            string src = "1.0+2.0*3.0";

            var lexer = new Lexer(new MemoryStream(Encoding.UTF8.GetBytes(src)));
            lexer.Start();

            var actual = string.Join("", lexer.TokStream.Stream);

            Assert.AreEqual(
                "DOUBLE_LITERAL 1.0 0 0" +
                "PLUS + 0 3" +
                "DOUBLE_LITERAL 2.0 0 4" +
                "STAR * 0 7" +
                "DOUBLE_LITERAL 3.0 0 8" +
                "END_OF_STREAM EOS 0 0", actual);
        }

        [TestMethod]
        public void ComplexTest()
        {
            string src = @"1_1.10
1.0
1.1234
1.2e2
1.2e+222
1.2e-222
1.123e22
1.12e+22
1.12e-22
1e9
1e+222
1e-922
10e8
1_000_00
912837192837+_+_+_-213123214=-=-=-=-=-
0xfffff
0xABCDEF
0xaBcDeF0121212
0b1010100101010010
0716253613561253";
            
            var lexer = new Lexer(new MemoryStream(Encoding.UTF8.GetBytes(src)));
            lexer.Start();

            var actual = string.Join("", lexer.TokStream.Stream);

            Assert.AreEqual(
                "DOUBLE_LITERAL 1.0 0 0" +
                "PLUS + 0 3" +
                "DOUBLE_LITERAL 2.0 0 4" +
                "STAR * 0 7" +
                "DOUBLE_LITERAL 3.0 0 8" +
                "END_OF_STREAM EOS 0 0", actual);
        }

        [TestMethod]
        public void IndentTest()
        {
            string src = "1\n  1\n    1";

            var lexer = new Lexer(new MemoryStream(Encoding.UTF8.GetBytes(src)));
            lexer.Start();

            var actual = string.Join("", lexer.TokStream.Stream);

            Assert.AreEqual(
                "DOUBLE_LITERAL 1.0 0 0" +
                "PLUS + 0 3" +
                "DOUBLE_LITERAL 2.0 0 4" +
                "STAR * 0 7" +
                "DOUBLE_LITERAL 3.0 0 8" +
                "END_OF_STREAM EOS 0 0", actual);
        }
    }
}
