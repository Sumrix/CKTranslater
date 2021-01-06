using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using NameTranslation.Matching;
using NameTranslation.Storages;

namespace SimilarityEditor
{
    public partial class Form1 : Form
    {
        private readonly WordsSimilarities wordsModel;
        private readonly bool initialized;

        public Form1()
        {
            this.InitializeComponent();

            this.SuspendLayout();

            this.similarities.FillTable();

            this.details.RowFormatter = this.details_RowFormatter;

            this.wordsModel = new WordsSimilarities();
            this.wordsModel.Rebuild();
            this.words.VirtualListDataSource = this.wordsModel;
            this.RefreshDetails();

            this.ResumeLayout();

            this.initialized = true;
        }

        private void details_RowFormatter(OLVListItem olvItem)
        {
            LettersMatch match = (LettersMatch) olvItem.RowObject;
            olvItem.BackColor = Program.GetColorFotSimilarity(match.Similarity);
        }

        private void details_SelectedIndexChanged(object sender, EventArgs e)
        {
            LettersMatch match = (LettersMatch) this.details.SelectedObject;
            this.similarities.RemoveHighlights();

            if (match != null)
            {
                var pairs = from letter0 in string.IsNullOrEmpty(match.Letters0) ? "_" : match.Letters0
                    from letter1 in string.IsNullOrEmpty(match.Letters1) ? "_" : match.Letters1
                    select (letter0, letter1);
                this.similarities.HighlightCells(pairs);
            }
        }

        private void filter_TextChanged(object sender, EventArgs e)
        {
            this.wordsModel.Filter = this.filter.Text;

            //if (!this.similarityCalculationsButton.Checked)
            //{
            this.words.BuildList(true);
            this.RefreshDetails();
            //}
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DB.Save();
        }

        private void RefreshDetails()
        {
            if (this.words.SelectedIndex < 0)
            {
                this.details.Items.Clear();
                return;
            }

            WordsSimilarity match = (WordsSimilarity) this.wordsModel.GetNthObject(this.words.SelectedIndex);

            this.details.SetObjects(match.MatchInfo.LetterMatches);
        }

        private void RefreshWords()
        {
            this.wordsModel.Rebuild();
            this.words.Refresh();
            this.RefreshDetails();
        }

        private void Similarities_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!this.initialized)
            {
                return;
            }

            this.SuspendLayout();

            this.RefreshWords();

            this.ResumeLayout();
        }

        private void similarityCalculationsButton_Click(object sender, EventArgs e)
        {
            this.similarityCalculationsButton.BackColor = Color.Red;
            this.similarityCalculationsButton.Refresh();

            GC.Collect();

            DB.Save();
            this.words.BuildList(true);
            this.RefreshDetails();

            this.similarityCalculationsButton.BackColor = Color.Green;

            this.RefreshWords();
        }

        private void words_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RefreshDetails();
        }
    }
}