using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace TurbExcel
{
    public enum RequestRespons
    {
        OK,
        ERROR, 
        CYCLE
    }
    public class DataGrid
    {
        private const string NamePattern = "[a-zA-Z]+[0-9]+";

        private int _numberOfColumns;
        private int _numberOfRows;
        public int NumberOfColumns
        {
            get { return _numberOfColumns; }
        }
        public int NumberOfRows
        {
            get { return _numberOfRows; }
        }
        private readonly Dictionary<string, Cell> cells;
        public Dictionary<string, Cell> Cells
        {
            get { return cells; }
        }
        public DataGrid(int numberOfColumns, int numberOfRows)
        {

            cells = new Dictionary<string, Cell>();
            _numberOfColumns = numberOfColumns;
            _numberOfRows = numberOfRows;
            for (int i = 0; i < _numberOfColumns; i++)
            {
                for (int j = 0; j < _numberOfRows; j++)
                {
                    string name = ComputeName(i, j);
                    Cell cell = new(this, i, j, "");
                    cells[name] = cell;
                }
            }
        }
        public void AddRow()
        {
            _numberOfRows++;
            for (int i = 0; i < _numberOfColumns; i++)
            {
                Cell cell = new(this, i, _numberOfRows - 1, "");
                cells[cell.Name] = cell;
            }
        }
        public void DeleteRow()
        {
            //_numberOfRows++;
            _numberOfRows--;
            for (int i = 0; i < _numberOfColumns; i++)
            {
                cells.Remove(ComputeName(i, NumberOfRows));
            }
            
        }
        public void AddColumn()
        {
            _numberOfColumns++;
            for (int i = 0; i < _numberOfRows; i++)
            {
                Cell cell = new(this, _numberOfColumns - 1, i, "");
                cells[cell.Name] = cell;
            }
        }
        public void DeleteColumn()
        {
            for (int i = 0; i < _numberOfRows; i++)
            {
                cells.Remove(ComputeName(_numberOfColumns - 1, i));
            }
            _numberOfColumns--;
        }
        public DataGrid(int numberOfColumns, int numberOfRows, Dictionary<(string, string), string> data_)
        {
            cells = new Dictionary<string, Cell>();
            _numberOfColumns = numberOfColumns;
            _numberOfRows = numberOfRows;
            //Debug.WriteLine("NUM_COL: {0} NUM_ROW: {1}", _numberOfColumns, _numberOfRows);
            //Debug.WriteLine("heh1");
            for (int i = 0; i < _numberOfColumns; i++)
            {
                for (int j = 0; j < _numberOfRows; j++)
                {
                    //string name = ComputeName(i, j);
                    Cell cell = new(this, i, j, "");
                    cells[DataGrid.ComputeName(i, j)] = cell;
                    //cell.Exp = ;
                }
            }
            //Debug.WriteLine("heh2");
            for (int i = 0; i < _numberOfColumns; i++)
            {
                for (int j = 0; j < _numberOfRows; j++)
                {
                    //string name = ComputeName(i, j);
                    cells[DataGrid.ComputeName(i, j)].Exp = data_[(i.ToString(), j.ToString())];
                    //cell.Exp = ;
                }
            }
            
        }
        // Get value that will be shown in the window(DataGrid)
        public string GetShownCellValue(int col, int row)
        {
            var currentCell = cells[ComputeName(col, row)];
            if (currentCell.State == CellState.ERROR)
            {
                return "ERROR";
            } else if(currentCell.State == CellState.NUMERIC_EXPRESSION ||
                      currentCell.State == CellState.NUMBER )
            {
                return cells[ComputeName(col, row)].Value.ToString();
            } else if(currentCell.State == CellState.NUMBER)
            {
                return "";
            }
            else
            {
                return cells[ComputeName(col, row)].Exp;
            }
        }
        public List<Cell> ParseStringCellDependOnThis(string exp)
        {
            List<string> dependencies = new List<string>();
            foreach (Match match in Regex.Matches(exp, NamePattern))
                dependencies.Add(match.Value);
            dependencies = dependencies.Distinct().ToList();
            List<Cell> dependenciesThis = new List<Cell>();
            foreach (string dep in dependencies)
            {
                dependenciesThis.Add(cells[dep.ToUpper()]);
            }
            return dependenciesThis.Distinct().ToList();
        }
        //Check if with new expression thehe is cycle, if is throw exception
        private void CheckCycle(string name, string exp)
        {
            //name = name.ToUpper();
            List<Cell> renewCells = new List<Cell>();
            renewCells.Add(cells[name]);
            List<Cell> cellList = new List<Cell>();
            cellList.AddRange(ParseStringCellDependOnThis(exp));
            while (cellList.Count > 0)
            {
                Cell first = cellList[0];
                cellList.RemoveAt(0);
                if (renewCells.Contains(first))
                {
                    throw new ArgumentException("invalid expression: contain cycle");
                    //throw new ("contains cyclic" + first.Name);
                }
                renewCells.Add(first);

                foreach (Cell cl in first.dependOn)
                {
                    cellList.Add(cl);
                }
            }
        }

        //Return all cells that will change if current cell is midified
        public List<Cell> GetAllDependOnThisCells(string name)
        {
            List<Cell> renewCells = new List<Cell>();
            renewCells.Add(cells[name]);
            List<Cell> cellList = new List<Cell>();
            cellList.AddRange(cells[name].dependOnThis);
            while (cellList.Count > 0)
            {
                Cell first = cellList[0];
                cellList.RemoveAt(0);
                if (renewCells.Contains(first))
                {
                    //throw new Exception("contains cyclic" + first.Name);
                    throw new ArgumentException("invalid expression: contain cycle");
                }
                renewCells.Add(first);

                foreach (Cell cl in first.dependOnThis)
                {
                    cellList.Add(cl);
                }
            }
            return renewCells;
        }

        //Sets cell expression from Form1
        public List<Cell> SetCellExp(int col, int row, string exp)
        {
            List<Cell> newCells;
            try
            {
                if (exp.Length > 0 && exp[0] == '=')
                {
                    CheckCycle(ComputeName(col, row), exp);
                }

                newCells = GetAllDependOnThisCells(DataGrid.ComputeName(col, row));
                cells[DataGrid.ComputeName(col, row)].Exp = exp;

                return newCells;
            }
            catch
            {
                MessageBox.Show("Sorry can't do that :(. It's creating a cycle", "Warning!");
                return new List<Cell>();
            }
        }

        //computes name
        public static string ComputeName(int col, int row)
        {
            return ConverterAlphabetNumbers.ToAlphabetNumber(col) + (row + 1).ToString();
        }

        
    }
}
