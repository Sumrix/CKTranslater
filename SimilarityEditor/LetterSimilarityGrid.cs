using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Core.Storages;
using Core.Translation;

namespace SimilarityEditor
{
    public class LetterSimilarityGrid : DataGridView
    {
        private readonly List<Point> highlightedCells = new();
        private readonly Language language0 = Language.Load(Db.EngLetters);
        private readonly Language language1 = Language.Load(Db.RusLetters);
        private Point selectedCell;

        public LetterSimilarityGrid()
        {
            this.InitializeComponent();
            this.SetTableDoubleBuffering();
        }

        private int DBToTableSimilarity(float value)
        {
            return Convert.ToInt32(value * 10);
        }

        public void FillTable()
        {
            DataGridViewColumn column;
            foreach (char rusLetter in Db.RusLetters)
            {
                column = new DataGridViewTextBoxColumn
                {
                    HeaderText = rusLetter.ToString(),
                    Width = 28,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                this.Columns.Add(column);
            }

            // Для пустого символа
            column = new DataGridViewTextBoxColumn
            {
                HeaderText = "",
                Width = 28,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            this.Columns.Add(column);

            for (int rowIndex = 0; rowIndex < Db.EngToRusSimilarities.EngCount; rowIndex++)
            {
                DataGridViewRow row = new();
                if (rowIndex < Db.EngLetters.Count)
                {
                    row.HeaderCell.Value = Db.EngLetters[rowIndex].ToString();
                }

                this.Rows.Add(row);

                for (int colIndex = 0; colIndex < Db.EngToRusSimilarities.RusCount; colIndex++)
                {
                    this.Rows[rowIndex].Cells[colIndex].Value
                        = this.DBToTableSimilarity(Db.EngToRusSimilarities[rowIndex, colIndex]);
                }
            }

            for (int rowIndex = 0; rowIndex < this.Rows.Count; rowIndex++)
            {
                for (int colIndex = 0; colIndex < this.Columns.Count; colIndex++)
                {
                    this.UpdateCellColor(colIndex, rowIndex);
                }
            }
        }

        public void HighlightCells(IEnumerable<(char letter0, char letter1)> pairs)
        {
            this.highlightedCells.AddRange(
                from p in pairs
                let index0 = p.letter0 == '_' ? this.Rows.Count - 1 : this.language0.ToIdentifier(p.letter0)
                let index1 = p.letter1 == '_' ? this.Columns.Count - 1 : this.language1.ToIdentifier(p.letter1)
                select new Point(index1, index0)
            );

            this.Invalidate();
        }

        private void InitializeComponent()
        {
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToResizeColumns = false;
            this.AllowUserToResizeRows = false;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.EditMode = DataGridViewEditMode.EditOnKeystroke;
            this.EnableHeadersVisualStyles = false;
            this.RowHeadersWidth = 50;
            this.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            e.Paint(e.CellBounds, DataGridViewPaintParts.All);

            bool redHorizontalBorders = this.highlightedCells.Any(p => p.X >= e.ColumnIndex && p.Y == e.RowIndex);
            bool redVerticalBorders = this.highlightedCells.Any(p => p.X == e.ColumnIndex && p.Y >= e.RowIndex);

            Rectangle rect = e.CellBounds;
            rect.Width -= 2;
            rect.Height -= 2;

            DataGridViewCell cell = this.SelectedCells[0];

            if (cell != null && (
                    e.RowIndex == -1 && cell.ColumnIndex == e.ColumnIndex ||
                    e.ColumnIndex == -1 && cell.RowIndex == e.RowIndex
                )
            )
            {
                using (Brush backColorBrush = new SolidBrush(this.DefaultCellStyle.SelectionBackColor))
                {
                    e.Graphics.FillRectangle(backColorBrush, rect);
                }

                StringFormat sf = new()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                using (SolidBrush bred = new(this.DefaultCellStyle.SelectionForeColor))
                {
                    e.Graphics.DrawString(e.Value?.ToString(), e.CellStyle.Font, bred, e.CellBounds, sf);
                }
            }

            if (redHorizontalBorders || redVerticalBorders)
            {
                using Pen p = new(Color.Red, 1);
                if (redVerticalBorders)
                {
                    e.Graphics.DrawLine(p, rect.Left, rect.Top, rect.Left, rect.Bottom);
                    e.Graphics.DrawLine(p, rect.Right, rect.Top, rect.Right, rect.Bottom);
                }

                if (redHorizontalBorders)
                {
                    e.Graphics.DrawLine(p, rect.Left, rect.Top, rect.Right, rect.Top);
                    e.Graphics.DrawLine(p, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                }
            }

            e.Handled = true;
        }

        protected override void OnCellValidating(DataGridViewCellValidatingEventArgs e)
        {
            object value = e.FormattedValue;

            if (!(value is string text) || !int.TryParse(text, out int num) || num < 0 || num > 10)
            {
                e.Cancel = true;
            }
        }

        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            this.UpdateCellColor(e.ColumnIndex, e.RowIndex);

            int value = Convert.ToInt32(this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
            Db.EngToRusSimilarities[e.RowIndex, e.ColumnIndex] = LetterSimilarityGrid.TableToDbSimilarity(value);

            base.OnCellValueChanged(e);
        }

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);

            if (this.SelectedCells.Count <= 0)
            {
                return;
            }

            DataGridViewCell cell = this.SelectedCells[0];
            Point wasSelectedCell = this.selectedCell;
            this.selectedCell = new Point(cell.ColumnIndex, cell.RowIndex);
            this.InvalidateCell(this.Columns[wasSelectedCell.X].HeaderCell);
            this.InvalidateCell(this.Rows[wasSelectedCell.Y].HeaderCell);
            this.InvalidateCell(this.Columns[this.selectedCell.X].HeaderCell);
            this.InvalidateCell(this.Rows[this.selectedCell.Y].HeaderCell);
        }

        public void RemoveHighlights()
        {
            this.highlightedCells.Clear();
            this.Invalidate();
        }

        private void SetTableDoubleBuffering()
        {
            if (SystemInformation.TerminalServerSession)
            {
                return;
            }

            Type dgvType = this.GetType();
            PropertyInfo? pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi?.SetValue(this, true, null);
        }

        private static float TableToDbSimilarity(object value)
        {
            return Convert.ToInt32(value) / 10f;
        }

        private void UpdateCellColor(int colIndex, int rowIndex)
        {
            DataGridViewCell cell = this.Rows[rowIndex].Cells[colIndex];
            float k = Convert.ToInt32(cell.Value) / 10f;
            cell.Style.BackColor = Program.GetColorFotSimilarity(k);
        }
    }
}