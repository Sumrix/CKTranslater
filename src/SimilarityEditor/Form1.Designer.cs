namespace SimilarityEditor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.rightPanel = new System.Windows.Forms.TableLayoutPanel();
            this.words = new BrightIdeasSoftware.FastObjectListView();
            this.lang1Word = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.lang2Word = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.details = new BrightIdeasSoftware.ObjectListView();
            this.letters0 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.letters1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.similarity = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.flowLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.filter = new System.Windows.Forms.TextBox();
            this.similarityCalculationsButton = new System.Windows.Forms.Button();
            this.similarities = new SimilarityEditor.LetterSimilarityGrid();
            this.mainPanel.SuspendLayout();
            this.rightPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.words)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.details)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.similarities)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.ColumnCount = 2;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1050F));
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Controls.Add(this.rightPanel, 1, 0);
            this.mainPanel.Controls.Add(this.similarities, 0, 0);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 1;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Size = new System.Drawing.Size(1355, 631);
            this.mainPanel.TabIndex = 1;
            // 
            // rightPanel
            // 
            this.rightPanel.ColumnCount = 1;
            this.rightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightPanel.Controls.Add(this.words, 0, 1);
            this.rightPanel.Controls.Add(this.details, 0, 2);
            this.rightPanel.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(1050, 0);
            this.rightPanel.Margin = new System.Windows.Forms.Padding(0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.RowCount = 3;
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.rightPanel.Size = new System.Drawing.Size(305, 631);
            this.rightPanel.TabIndex = 0;
            // 
            // words
            // 
            this.words.AllColumns.Add(this.lang1Word);
            this.words.AllColumns.Add(this.lang2Word);
            this.words.AllColumns.Add(this.olvColumn1);
            this.words.CellEditUseWholeCell = false;
            this.words.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lang1Word,
            this.lang2Word,
            this.olvColumn1});
            this.words.Cursor = System.Windows.Forms.Cursors.Default;
            this.words.Dock = System.Windows.Forms.DockStyle.Fill;
            this.words.FullRowSelect = true;
            this.words.GridLines = true;
            this.words.HideSelection = false;
            this.words.Location = new System.Drawing.Point(0, 22);
            this.words.Margin = new System.Windows.Forms.Padding(0);
            this.words.Name = "words";
            this.words.ShowGroups = false;
            this.words.Size = new System.Drawing.Size(305, 426);
            this.words.TabIndex = 0;
            this.words.UseAlternatingBackColors = true;
            this.words.UseCompatibleStateImageBehavior = false;
            this.words.View = System.Windows.Forms.View.Details;
            this.words.VirtualMode = true;
            this.words.SelectedIndexChanged += new System.EventHandler(this.words_SelectedIndexChanged);
            // 
            // lang1Word
            // 
            this.lang1Word.AspectName = "Lang1Word";
            this.lang1Word.FillsFreeSpace = true;
            this.lang1Word.Sortable = false;
            this.lang1Word.Text = "Язык 1";
            this.lang1Word.Width = 107;
            // 
            // lang2Word
            // 
            this.lang2Word.AspectName = "Lang2Word";
            this.lang2Word.FillsFreeSpace = true;
            this.lang2Word.Sortable = false;
            this.lang2Word.Text = "Язык 2";
            this.lang2Word.Width = 99;
            // 
            // olvColumn1
            // 
            this.olvColumn1.AspectName = "MatchInfo.Similarity";
            this.olvColumn1.FillsFreeSpace = true;
            this.olvColumn1.Sortable = false;
            this.olvColumn1.Text = "Похожесть";
            this.olvColumn1.Width = 89;
            // 
            // details
            // 
            this.details.AllColumns.Add(this.letters0);
            this.details.AllColumns.Add(this.letters1);
            this.details.AllColumns.Add(this.similarity);
            this.details.CellEditUseWholeCell = false;
            this.details.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.letters0,
            this.letters1,
            this.similarity});
            this.details.Cursor = System.Windows.Forms.Cursors.Default;
            this.details.Dock = System.Windows.Forms.DockStyle.Fill;
            this.details.FullRowSelect = true;
            this.details.GridLines = true;
            this.details.HideSelection = false;
            this.details.Location = new System.Drawing.Point(0, 448);
            this.details.Margin = new System.Windows.Forms.Padding(0);
            this.details.Name = "details";
            this.details.Size = new System.Drawing.Size(305, 183);
            this.details.TabIndex = 2;
            this.details.UseCompatibleStateImageBehavior = false;
            this.details.View = System.Windows.Forms.View.Details;
            this.details.SelectedIndexChanged += new System.EventHandler(this.details_SelectedIndexChanged);
            // 
            // letters0
            // 
            this.letters0.AspectName = "Letters0";
            this.letters0.FillsFreeSpace = true;
            this.letters0.Sortable = false;
            this.letters0.Text = "Язык 1";
            this.letters0.Width = 103;
            // 
            // letters1
            // 
            this.letters1.AspectName = "Letters1";
            this.letters1.FillsFreeSpace = true;
            this.letters1.Sortable = false;
            this.letters1.Text = "Язык 2";
            this.letters1.Width = 102;
            // 
            // similarity
            // 
            this.similarity.AspectName = "Similarity";
            this.similarity.FillsFreeSpace = true;
            this.similarity.Sortable = false;
            this.similarity.Text = "Похожесть";
            this.similarity.Width = 89;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.ColumnCount = 2;
            this.flowLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.flowLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.flowLayoutPanel1.Controls.Add(this.filter, 1, 0);
            this.flowLayoutPanel1.Controls.Add(this.similarityCalculationsButton, 0, 0);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.flowLayoutPanel1.Size = new System.Drawing.Size(305, 22);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // filter
            // 
            this.filter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filter.Location = new System.Drawing.Point(27, 0);
            this.filter.Margin = new System.Windows.Forms.Padding(0);
            this.filter.Name = "filter";
            this.filter.Size = new System.Drawing.Size(278, 22);
            this.filter.TabIndex = 1;
            this.filter.TextChanged += new System.EventHandler(this.filter_TextChanged);
            // 
            // similarityCalculationsButton
            // 
            this.similarityCalculationsButton.AutoSize = true;
            this.similarityCalculationsButton.BackColor = System.Drawing.Color.Green;
            this.similarityCalculationsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.similarityCalculationsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.similarityCalculationsButton.Location = new System.Drawing.Point(0, 0);
            this.similarityCalculationsButton.Margin = new System.Windows.Forms.Padding(0);
            this.similarityCalculationsButton.Name = "similarityCalculationsButton";
            this.similarityCalculationsButton.Size = new System.Drawing.Size(27, 22);
            this.similarityCalculationsButton.TabIndex = 2;
            this.similarityCalculationsButton.UseVisualStyleBackColor = false;
            this.similarityCalculationsButton.Click += new System.EventHandler(this.similarityCalculationsButton_Click);
            // 
            // similarities
            // 
            this.similarities.AllowUserToAddRows = false;
            this.similarities.AllowUserToDeleteRows = false;
            this.similarities.AllowUserToResizeColumns = false;
            this.similarities.AllowUserToResizeRows = false;
            this.similarities.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.similarities.DefaultCellStyle = dataGridViewCellStyle1;
            this.similarities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.similarities.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.similarities.EnableHeadersVisualStyles = false;
            this.similarities.Location = new System.Drawing.Point(0, 0);
            this.similarities.Margin = new System.Windows.Forms.Padding(0);
            this.similarities.Name = "similarities";
            this.similarities.RowHeadersWidth = 50;
            this.similarities.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.similarities.Size = new System.Drawing.Size(1050, 631);
            this.similarities.TabIndex = 1;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1355, 631);
            this.Controls.Add(this.mainPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.mainPanel.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.words)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.details)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.similarities)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.FastObjectListView words;
        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.TableLayoutPanel rightPanel;
        private LetterSimilarityGrid similarities;
        private System.Windows.Forms.TextBox filter;
        private BrightIdeasSoftware.OLVColumn lang1Word;
        private BrightIdeasSoftware.OLVColumn lang2Word;
        private BrightIdeasSoftware.OLVColumn olvColumn1;
        private BrightIdeasSoftware.ObjectListView details;
        private BrightIdeasSoftware.OLVColumn letters0;
        private BrightIdeasSoftware.OLVColumn letters1;
        private BrightIdeasSoftware.OLVColumn similarity;
        private System.Windows.Forms.TableLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button similarityCalculationsButton;
    }
}