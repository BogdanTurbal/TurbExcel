using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurbExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurbExcel.Tests
{
    [TestClass()]
    public class CalculatorTests
    {
        [TestMethod()]
        public void ComputeNumericExpressionTest()
        {
            DataGrid dataGrid = new DataGrid(50, 50);
            Assert.AreEqual(Calculator.Evaluate("10* 2 - 21", dataGrid), -1);
            Assert.AreEqual(Calculator.Evaluate("(17 / 2) * 99 - 33", dataGrid), 808.5);
            Assert.AreEqual(Calculator.Evaluate("-2", dataGrid), -2);
        }
        [TestMethod()]
        public void ComputeExpressionTest()
        {
            DataGrid dataGrid = new DataGrid(50, 50);
            dataGrid.SetCellExp(0, 0, "=12 - 2");
            dataGrid.SetCellExp(1, 0, "=A1+a1 - 2");
            Assert.AreEqual(Calculator.Evaluate("A1", dataGrid), 10);
            Assert.AreEqual(Calculator.Evaluate("B1", dataGrid), 18);
            Assert.AreEqual(Calculator.Evaluate("B1+ a1", dataGrid), 28);
            //Assert.AreEqual(Calculator.Evaluate("-2", dataGrid), -2);
        }

        [TestMethod()]
        public void TestDivideByZero()
        {
            DataGrid dataGrid = new DataGrid(50, 50);
            TestFail("1 / 0", dataGrid);
            TestFail("0 / 0", dataGrid);
            TestFail("12 / A1", dataGrid);
        }
        void TestFail(string exp, DataGrid dataGrid)
        {
            try
            {
                var i = Calculator.Evaluate(exp, dataGrid);
                Assert.Fail();
            }
            catch (DivideByZeroException) { }
            catch (Exception) { Assert.Fail(); }
        }
    }
}