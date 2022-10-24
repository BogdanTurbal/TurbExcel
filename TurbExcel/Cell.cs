using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TurbExcel
{
    public enum CellState
    {
        NUMBER,
        STRING,
        NUMERIC_EXPRESSION,
        EMPTY,
        ERROR
    }
    public class Cell
    {
        private int _row;
        private int _col;
        private string _exp;
        private double _value = 0.0;
        private CellState _state = CellState.EMPTY;
        private readonly DataGrid dataGrid;
        public List<Cell> dependOn = new List<Cell>();
        public List<Cell> dependOnThis = new List<Cell>();
        public int Row { get { return _row; } }
        public int Col { get { return _col; } }
        public CellState State { get { return _state; } }
        public string Name { get { return DataGrid.ComputeName(Col, Row); } }

        public Cell(DataGrid dataGrid, int col, int row, string exp)
        {
            this.dataGrid = dataGrid;
            _row = row;
            _col = col;
            _exp = "";
            Exp = exp;
        }

        public double Value
        {
            get { return _value; }
        }
        public string Exp { 
            get { return _exp; } 
            set
            {
                _exp = value;
                if (_exp.Length > 0 && _exp[0] == '=')
                UpdateDependencies();
                try
                {
                    UpdateVal(_exp);
                }
                catch
                {
                    _state = CellState.ERROR;
                }
            } }
        
        private void UpdateDependencies()
        {
            this.dependOn = dataGrid.ParseStringCellDependOnThis(_exp);
            foreach (Cell cell in dependOn)
            {
                if(!cell.dependOnThis.Contains(this)) cell.dependOnThis.Add(this);
            }
        }

        private void UpdateVal(string exp)
        {
            if (exp.Length == 0)
            {
                _state = CellState.EMPTY;
                return;
            }
            if (exp[0] == '=')
            {
                double val;
                try
                {
                    exp = exp.Substring(1);
                    val = Calculator.Evaluate(exp, dataGrid);
                    _state = CellState.NUMERIC_EXPRESSION;
                }
                catch {
                    _state = CellState.ERROR;
                    throw new FormatException("can't parse " + exp);
                }
                _value = val; 
            }
            else
            {
                double res;
                if (double.TryParse(exp, out res))
                {
                    _value = res;
                    _state = CellState.NUMBER;
                }
                else
                {
                    _state = CellState.STRING;
                }
            }
        }
        
    }
}
