using Siderion_Reloaded.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Siderion_Reloaded
{
    class Helpers
    {
        public static DialogResult ShowInputDialog(ref string input, string formName = "Name")
        {
            System.Drawing.Size size = new System.Drawing.Size(250, 150);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = formName;

            System.Windows.Forms.RichTextBox textBox = new RichTextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 100);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "Done";
            okButton.Location = new System.Drawing.Point(size.Width - (okButton.Size.Width + 5), 113);
            inputBox.Controls.Add(okButton);

            inputBox.AcceptButton = okButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        public static DialogResult ShowCreditsDialog(string formName = "Name")
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 80);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = formName;

            Label labelText = new Label();
            labelText.Name = "leLabel";

            labelText.Size = new System.Drawing.Size(size.Width - 10, 45);
            labelText.Location = new System.Drawing.Point(5, 5);
            labelText.Text = Resources.Credits;
            inputBox.Controls.Add(labelText);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "Bruh";
            okButton.Location = new System.Drawing.Point((size.Width/2)-33, 55);
            inputBox.Controls.Add(okButton);

            okButton.Click += new EventHandler(handleOkButton);

            inputBox.AcceptButton = okButton;

            DialogResult result = inputBox.ShowDialog();
            return result;
        }

        private static void handleOkButton(object sender, EventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Resources.bruh);
            player.Play();
        }
    }
}
