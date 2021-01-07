using System.Drawing;
using System.Windows.Forms;

namespace SimilarityEditor
{
    public class NoPaddingButton : Button
    {
        private string ownerDrawText;

        public string OwnerDrawText
        {
            get => this.ownerDrawText;
            set
            {
                this.ownerDrawText = value;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (string.IsNullOrEmpty(this.Text) && !string.IsNullOrEmpty(this.ownerDrawText))
            {
                StringFormat stringFormat = new();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                e.Graphics.DrawString(this.ownerDrawText, this.Font, new SolidBrush(this.ForeColor),
                    this.ClientRectangle, stringFormat);
            }
        }
    }
}