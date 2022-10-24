using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;

namespace TurbExcel
{
    public partial class Form1 : Form
    {
        int NUM_COL = 50;
        int NUM_ROW = 50;
        private bool isSaved = true;
        private DataGrid dataGrid;
        private string tmp = "";
        private string lastPath = "";
        public Form1()
        {
            InitializeComponent();
            InitializeDataGrid(NUM_COL, NUM_ROW);
        }

        private void InitializeDataGrid(int numCol, int numRow)
        {
            dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.RowHeadersWidth = 60;
            dataGrid = new DataGrid(numCol, numRow);
            dataGridView.ColumnCount = dataGrid.NumberOfColumns;
            for (int i = 0; i < dataGrid.NumberOfColumns; i++)
            {
                dataGridView.Columns[i].Name = ConverterAlphabetNumbers.ToAlphabetNumber(i);
            }
            for (int i = 0; i < dataGrid.NumberOfRows; i++)
            {
                dataGridView.Rows.Add();
                dataGridView.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
            isSaved = true;

        }
        private void UpdateDataGrid()
        {
            for (int i = 0; i < dataGrid.NumberOfColumns; i++)
            {
                for (int j = 0; j < dataGrid.NumberOfRows; j++)
                {
                    dataGrid.Cells[DataGrid.ComputeName(i, j)].Exp = dataGrid.Cells[DataGrid.ComputeName(i, j)].Exp;
                    dataGridView.Rows[j].Cells[i].Value = dataGrid.GetShownCellValue(i, j);
                }
            }
            isSaved = true;
        }

        private void dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            string text = dataGrid.Cells[DataGrid.ComputeName(e.ColumnIndex, e.RowIndex)].Exp;
            expressionLine.Focus();
            tmp = text;
            expressionLine.Text = text; 
        }

        private void expressionLine_Leave(object sender, EventArgs e)
        {
            bool res = UpdateValues();
            if (tmp != expressionLine.Text) isSaved = false;
            expressionLine.Text = "";
        }
        private bool UpdateValues()
        {
            List<Cell> cells = dataGrid.SetCellExp(dataGridView.CurrentCell.ColumnIndex, dataGridView.CurrentCell.RowIndex, expressionLine.Text.ToString());
            foreach (Cell cell in cells)
            {
                cell.Exp = cell.Exp;
                dataGridView.Rows[cell.Row].Cells[cell.Col].Value = dataGrid.GetShownCellValue(cell.Col, cell.Row);
            }
            return true;
        }
        private void expressionLine_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                //dataGridView.ClearSelection();
                temp.Focus();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void ShowCycleWarning()
        {
            MessageBox.Show("Sorry can't do that :(. It's creating a cycle", "Warning!");
        }

        private void SaveAsTable()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JSON|*.json";
            //temp.Focus();


            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string path = Path.GetFullPath(sfd.FileName);
                //lastPath = path;
                SaveTable(path);
                //SaveLoadData.Serialize(dataGrid.Cells, dataGrid.NumberOfColumns, dataGrid.NumberOfRows, path);
                isSaved = true;
            }
        }
        private void SaveTable(string path)
        {
            lastPath = path;
            SaveLoadData.Serialize(dataGrid.Cells, dataGrid.NumberOfColumns, dataGrid.NumberOfRows, path);
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //dataGridView.ClearSelection();
            //temp.Focus();
            if (lastPath != "")
            {
                SaveTable(lastPath);
            }
            else
            {
                SaveAsTable();
            }
            
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsTable();
        }
        private void ClearDataGridView(int numCol, int numRow)
        {
            dataGridView.ClearSelection();
            dataGridView.Rows.Clear();
            dataGridView.Refresh();
            InitializeDataGrid(numCol, numRow);
            dataGridView.ClearSelection();
        }
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearDataGridView(NUM_COL, NUM_ROW);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            temp.Focus();
            //dataGridView.ClearSelection();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Debug.WriteLine("sorry-1)");
                    string path = Path.GetFullPath(sfd.FileName);
                    //Debug.WriteLine("sorry0)");
                    var dict = SaveLoadData.Deserialize(path);
                    //Debug.WriteLine("sorry1)");
                    int numCol = Int32.Parse(dict[("columns", "")]);
                    int numRow = Int32.Parse(dict[("rows", "")]);

                    //Debug.WriteLine("sorry2)");
                    temp.Focus();
                    ClearDataGridView(numCol, numRow);
                    temp.Focus();
                    //Debug.WriteLine("sorry3)");
                    lastPath = path;
                    dataGrid = new DataGrid(numCol, numRow, dict);
                } catch {
                    MessageBox.Show("Sorry can't do that :(. You chose wrong file of wron format(", "Warning!");
                    return;
                }
                
                UpdateDataGrid();
              
            }
        }


        private void dataGridView_MouseClick_1(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        editMenu.Show(this, new Point(e.X, e.Y));//places the menu at the pointer position
                    }
                    break;
            }
        }


        private void addColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView.ColumnCount += 1;

            dataGridView.Columns[dataGridView.ColumnCount - 1].Name =
                ConverterAlphabetNumbers.ToAlphabetNumber(dataGridView.ColumnCount - 1);
            dataGrid.AddColumn();
        }

        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView.RowCount += 1;
            dataGridView.Rows[dataGridView.RowCount - 1].HeaderCell.Value = (dataGridView.RowCount - 1 + 1).ToString();
            dataGrid.AddRow();
        }

        private void deleteColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView.ColumnCount -= 1;
            dataGrid.DeleteColumn();
        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView.RowCount -= 1;
            dataGrid.DeleteRow();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isSaved)
            {
                DialogResult dialogResult = MessageBox.Show("You haven't saved the file, close anyway?", "Warning", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
            
            
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@" Привіт! Це TurbExcel який допоможе тобі виконувати оперції з таблицями.
Підказки:
1.можна ввести просто чило або = число
21212 або = 3434
2.Можна ввести вираз з посиланнями на інши клітинки
= b1 * 2 + 434 + C1
3.Цикли як і ділення на 0 неможливі
4.Можна ввести просто рядок
5.Вкінці можна зберігти і загрузити", "Help");
           
        }
    }
}