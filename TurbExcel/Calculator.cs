using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurbExcel
{
    public class Calculator
    {
        public static double Evaluate(string expression, DataGrid dataGrid)
        {
            var lexer = new CalculatorLexer(new AntlrInputStream(expression));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new ThrowExceptionErrorListener());
            var tokens = new CommonTokenStream(lexer);
            var parser = new CalculatorParser(tokens);
            var tree = parser.compileUnit();
            var visitor = new CalculatorVisitor(dataGrid);
            return visitor.Visit(tree);
        }
    }
}
