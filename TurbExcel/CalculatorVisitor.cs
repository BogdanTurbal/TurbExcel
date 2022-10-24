using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurbExcel
{
    internal class CalculatorVisitor: CalculatorBaseVisitor<double>
    {
        DataGrid dataGrid;
        public CalculatorVisitor(DataGrid dataGrid)
        {
            this.dataGrid = dataGrid;
        }
        public override double VisitCompileUnit(CalculatorParser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }
        public override double VisitNumberExpr(CalculatorParser.NumberExprContext context)
        {
            var result = double.Parse(context.GetText());
            return result;
        }
        //IdentifierExpr
        public override double VisitIdentifierExpr(CalculatorParser.IdentifierExprContext context)
        {
            //Debug.WriteLine("hehehehjhjh");
            var result = context.GetText().ToUpper();
            var currentCell = dataGrid.Cells[result.ToString()];
            if (currentCell.State != CellState.ERROR && currentCell.State != CellState.STRING)
            {
                return currentCell.Value;
            }
            else
            {
                throw new ArgumentException("cant find " + result);
            }
            //видобути значення змінної з таблиці
        }
        public override double VisitParenthesizedExpr(CalculatorParser.ParenthesizedExprContext context)
        {
            return Visit(context.expression());
        }
        public override double VisitExponentialExpr(CalculatorParser.ExponentialExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);
            if (left == 0 && right < 0) throw new DivideByZeroException("0 in denimunator");
            Debug.WriteLine("{0} ^ {1}", left, right);
            return System.Math.Pow(left, right);
        }
        public override double VisitAdditiveExpr(CalculatorParser.AdditiveExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);
            if (context.operatorToken.Type == CalculatorLexer.ADD)
            {
                Debug.WriteLine("{0} + {1}", left, right);
                return left + right;
            }
            else //LabCalculatorLexer.SUBTRACT
            {
                Debug.WriteLine("{0} - {1}", left, right);
                return left - right;
            }
        }
        public override double VisitMultiplicativeExpr(CalculatorParser.MultiplicativeExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);
            if (context.operatorToken.Type == CalculatorLexer.MULTIPLY)
            {
                //Debug.WriteLine("{0} * {1}", left, right);
                return left * right;
            }
            else //LabCalculatorLexer.DIVIDE
            {
                if (right == 0) throw new DivideByZeroException("divide by zero");
                //Debug.WriteLine("{0} / {1}", left, right);
                return left / right;
            }
        }
        public override double VisitSubOper([NotNull] CalculatorParser.SubOperContext context)
        {
            var left = WalkLeft(context);
            return -left;
        }
        public override double VisitAddOper([NotNull] CalculatorParser.AddOperContext context)
        {
            var left = WalkLeft(context);
            return left;
        }
        private double WalkLeft(CalculatorParser.ExpressionContext context)
        {
                 return Visit(context.GetRuleContext<CalculatorParser.ExpressionContext>(0));
        }
        private double WalkRight(CalculatorParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<CalculatorParser.ExpressionContext>(1));
        }

        public override double VisitModDivExpr([NotNull] CalculatorParser.ModDivExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);
            if (right == 0) throw new DivideByZeroException("in mod divide by 0");
            return context.operatorToken.Type == CalculatorLexer.MOD
                ? left % right
                : (int)left / (int)right;
        }

        public override double VisitMminExpr([NotNull] CalculatorParser.MminExprContext context)
        {
            double minValue = Double.PositiveInfinity;

            foreach (var child in context.paramlist.children.OfType<CalculatorParser.ExpressionContext>())
            {
                double childValue = this.Visit(child);
                if (childValue < minValue)
                {
                    minValue = childValue;
                }
            }
            return minValue;
        }

        public override double VisitMmaxExpr([NotNull] CalculatorParser.MmaxExprContext context)
        {
            double maxValue = Double.NegativeInfinity;

            foreach (var child in context.paramlist.children.OfType<CalculatorParser.ExpressionContext>())
            {
                double childValue = this.Visit(child);
                if (childValue > maxValue)
                {
                    maxValue = childValue;
                }
            }
            return maxValue;
        }

        public override double VisitIncExpr([NotNull] CalculatorParser.IncExprContext context)
        {
            var left = WalkLeft(context);
            return left + 1;
        }
    }
}
