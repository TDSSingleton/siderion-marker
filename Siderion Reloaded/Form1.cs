using Siderion_Reloaded.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Siderion_Reloaded
{
    public partial class Form1 : Form
    {
        string currentDir;

        /// <summary>Player on top is left on false, right on true.</summary>
        bool playerTop = false;
        ///<summary>0 is left, 1 is right</summary>
        List<string> teamNames = new List<string>(2) { "Team A", "Team B" };
        List<int> teamScores = new List<int>(2) {0,0};
        List<string> team1Players = new List<string>(5) {"Player", "Player" , "Player" , "Player" , "Player" };
        List<string> team2Players = new List<string>(5) { "Player", "Player", "Player", "Player", "Player" };
        ///<summary>Count A and A+1 as the scores for a single player</summary>
        List<int> scoresTeam1 = new List<int>(10) {0,0,0,0,0,0,0,0,0,0};
        List<bool> scoresValid1 = new List<bool>(10) {true, true, true, true, true, true, true, true, true, true};
        List<int> scoresTeam2 = new List<int>(10) { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        List<bool> scoresValid2 = new List<bool>(10) { true, true, true, true, true, true, true, true, true, true };
        /// <summary>Deck 0 and 1 is for Team A, Deck 2 and 3 is for Team 2</summary>
        List<bool> deckActive = new List<bool>(4) {true, true, true, true};
        List<bool> repeatList = new List<bool>(4) { true, true, true, true };

        public Form1()
        {
            currentDir = Directory.GetCurrentDirectory();
            if (Settings.Default.Firsttime)
            {
                DialogResult res = MessageBox.Show(Resources.FirstUse,"Welcome",MessageBoxButtons.OKCancel,MessageBoxIcon.Information,MessageBoxDefaultButton.Button2);
                if (res == DialogResult.Cancel)
                    Environment.Exit(0);
                Settings.Default.Firsttime = false;
                Settings.Default.Save();
            }
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Size = new Size(428, 365);
            InitializeComponent();
            SetPlayerScores(true);
            SetPlayerScores(false);

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
            {
                playerTop = false;
                ReverseDecks();
                SetTopPlayer();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string content = "";
            Helpers.ShowInputDialog(ref content, "Copy Mode");
            List<string> lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            lines.RemoveAll(x => string.IsNullOrWhiteSpace(x));

            if(lines.Count < 6)
            {
                MessageBox.Show("Not enough lines to fill either teams, ignoring input.","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }else if(lines.Count < 12)
            {
                DialogResult message = MessageBox.Show("Not enough lines to fill both teams. Should I try to fill Team A at least?","Warning",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation);
                if(message == DialogResult.No)
                {
                    DialogResult messageII = MessageBox.Show("What about Team B?", "Warning Cont.", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if(message == DialogResult.Yes)
                    {
                        SetTeamFull(true, lines.GetRange(0, 6));
                    }
                }else
                {
                    SetTeamFull(false, lines.GetRange(0, 6));
                }
            }else if(lines.Count == 12)
            {
                SetTeamFull(true, lines.GetRange(6, 6));
                SetTeamFull(false, lines.GetRange(0, 6));
            }else
            {
                DialogResult message = MessageBox.Show("Too many lines, the input will be truncated.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                SetTeamFull(true, lines.GetRange(6, 6));
                SetTeamFull(false, lines.GetRange(0, 6));
            }

        }

        void SetTeamFull(bool teamSide, List<string> whatToSet)
        { //3 5 16 22 28 34
            if(teamSide)
            {
                textBox4.Text = whatToSet[0];
                textBox8.Text = whatToSet[1];
                textBox13.Text = whatToSet[2];
                textBox19.Text = whatToSet[3];
                textBox25.Text = whatToSet[4];
                textBox31.Text = whatToSet[5];
            }else
            {
                textBox3.Text = whatToSet[0];
                textBox5.Text = whatToSet[1];
                textBox16.Text = whatToSet[2];
                textBox22.Text = whatToSet[3];
                textBox28.Text = whatToSet[4];
                textBox34.Text = whatToSet[5];
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        { 
            string imagePath;
            OpenFileDialog file = new OpenFileDialog();
            file.InitialDirectory = Directory.Exists(@"C:\Team Wars Season 11\Logos")? @"C:\Team Wars Season 11\Logos" : "";
            if (file.ShowDialog() == DialogResult.OK)
            {
                imagePath = file.FileName;
                pictureBox1.ImageLocation = imagePath;
                string destFile = currentDir + @"\Team1.png";
                System.IO.File.Copy(imagePath, destFile, true);
                File.SetLastWriteTime(destFile,DateTime.Now);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            string imagePath;
            OpenFileDialog file = new OpenFileDialog();
            file.InitialDirectory = Directory.Exists(@"C:\Team Wars Season 10\Logos") ? @"C:\Team Wars Season 10\Logos" : "";
            if (file.ShowDialog() == DialogResult.OK)
            {
                imagePath = file.FileName;
                pictureBox2.ImageLocation = imagePath;
                string destFile = currentDir + @"\Team2.png";
                System.IO.File.Copy(imagePath, destFile, true);
                File.SetLastWriteTime(destFile, DateTime.Now);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text.All(char.IsDigit) && !String.IsNullOrEmpty(textBox1.Text)) //If text is all a numerical, transcribe into the file.
            {
                File.WriteAllText(currentDir + @"\Team1Score.txt",textBox1.Text);
                teamScores[0] = int.Parse(textBox1.Text);
                progressBar1.Value = teamScores[0] * 10;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.All(char.IsDigit) && !String.IsNullOrEmpty(textBox2.Text)) //If text is all a numerical, transcribe into the file.
            {
                File.WriteAllText(currentDir + @"\Team2Score.txt", textBox2.Text);
                teamScores[1] = int.Parse(textBox2.Text);
                progressBar2.Value = teamScores[1] * 10;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(currentDir + @"\Team1Name.txt", textBox3.Text);
            teamNames[0] = textBox3.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(currentDir + @"\Team2Name.txt", textBox4.Text);
            teamNames[1] = textBox3.Text;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton2.Checked)
            {
                playerTop = true;
                ReverseDecks();
                SetTopPlayer();
            } 
        }

        void SetTopPlayer()
        {
            File.WriteAllText(currentDir + @"\TopPlayerName.txt", GetSelectedPlayer(playerTop));
            File.WriteAllText(currentDir + @"\BottomPlayerName.txt", GetSelectedPlayer(!playerTop));
        }

        /// <summary>
        /// Called when a Radio Button is changed.
        /// </summary>
        void ReverseDecks()
        {
            bool temp1 = checkBox1.Checked, temp2 = checkBox2.Checked;
            checkBox1.Checked = checkBox4.Checked;
            checkBox2.Checked = checkBox3.Checked;
            checkBox4.Checked = temp1;
            checkBox3.Checked = temp2;
        }

        string GetSelectedPlayer(bool playerMode) //True means Team B.
        {
            if(playerMode)
            {
                if (radioButton4.Checked)
                    return textBox8.Text;
                else if (radioButton5.Checked)
                    return textBox13.Text;
                else if (radioButton7.Checked)
                    return textBox19.Text;
                else if (radioButton9.Checked)
                    return textBox25.Text;
                else
                    return textBox31.Text;
            }else
            {
                if (radioButton3.Checked)
                    return textBox5.Text;
                else if (radioButton6.Checked)
                    return textBox16.Text;
                else if (radioButton8.Checked)
                    return textBox22.Text;
                else if (radioButton10.Checked)
                    return textBox28.Text;
                else
                    return textBox34.Text;
            }
        }

        string ReturnPlayerList(bool side)
        {
            StringBuilder newStringBuilder = new StringBuilder();
            if (side)
            {
                newStringBuilder.AppendLine(textBox8.Text);
                newStringBuilder.AppendLine(textBox13.Text);
                newStringBuilder.AppendLine(textBox19.Text);
                newStringBuilder.AppendLine(textBox25.Text);
                newStringBuilder.AppendLine(textBox31.Text);

            }
            else
            {
                newStringBuilder.AppendLine(textBox5.Text);
                newStringBuilder.AppendLine(textBox16.Text);
                newStringBuilder.AppendLine(textBox22.Text);
                newStringBuilder.AppendLine(textBox28.Text);
                newStringBuilder.AppendLine(textBox34.Text);
            }
            return newStringBuilder.ToString();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(currentDir + @"\Team1Names.txt", ReturnPlayerList(false));
            SetTopPlayer();
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(currentDir + @"\Team1Names.txt", ReturnPlayerList(false));
            SetTopPlayer();
        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(currentDir + @"\Team1Names.txt", ReturnPlayerList(false));
            SetTopPlayer();
        }

        private void textBox28_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(currentDir + @"\Team1Names.txt", ReturnPlayerList(false));
            SetTopPlayer();
        }

        private void textBox34_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(currentDir + @"\Team1Names.txt", ReturnPlayerList(false));
            SetTopPlayer();
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(currentDir + @"\Team2Names.txt", ReturnPlayerList(true));
            SetTopPlayer();
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(currentDir + @"\Team2Names.txt", ReturnPlayerList(true));
            SetTopPlayer();
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(currentDir + @"\Team2Names.txt", ReturnPlayerList(true));
            SetTopPlayer();
        }

        private void textBox25_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(currentDir + @"\Team2Names.txt", ReturnPlayerList(true));
            SetTopPlayer();
        }

        private void textBox31_TextChanged(object sender, EventArgs e)
        {
            File.WriteAllText(currentDir + @"\Team2Names.txt", ReturnPlayerList(true));
            SetTopPlayer();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            SetTopPlayer();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap deckBMap = Resources.deck;
            if (checkBox1.Checked)
            {
                deckBMap.Save(currentDir + @"\Team1Deck1.png",ImageFormat.Png);
            }else
            {
                Bitmap emptyBMap = new Bitmap(Resources.deck.Width, Resources.deck.Height);
                emptyBMap.Save(currentDir + @"\Team1Deck1.png", ImageFormat.Png);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap deckBMap = Resources.deck;
            if (checkBox2.Checked)
            {
                deckBMap.Save(currentDir + @"\Team1Deck2.png", ImageFormat.Png);
            }
            else
            {
                Bitmap emptyBMap = new Bitmap(Resources.deck.Width, Resources.deck.Height);
                emptyBMap.Save(currentDir + @"\Team1Deck2.png", ImageFormat.Png);
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap deckBMap = Resources.repeat;
            if (checkBox6.Checked)
            {
                deckBMap.Save(currentDir + @"\Team1Repeat1.png", ImageFormat.Png);
            }
            else
            {
                Bitmap emptyBMap = new Bitmap(Resources.deck.Width, Resources.deck.Height);
                emptyBMap.Save(currentDir + @"\Team1Repeat1.png", ImageFormat.Png);
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap deckBMap = Resources.repeat;
            if (checkBox5.Checked)
            {
                deckBMap.Save(currentDir + @"\Team1Repeat2.png", ImageFormat.Png);
            }
            else
            {
                Bitmap emptyBMap = new Bitmap(Resources.deck.Width, Resources.deck.Height);
                emptyBMap.Save(currentDir + @"\Team1Repeat2.png", ImageFormat.Png);
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap deckBMap = Resources.deck;
            if (checkBox4.Checked)
            {
                deckBMap.Save(currentDir + @"\Team2Deck1.png", ImageFormat.Png);
            }
            else
            {
                Bitmap emptyBMap = new Bitmap(Resources.deck.Width, Resources.deck.Height);
                emptyBMap.Save(currentDir + @"\Team2Deck1.png", ImageFormat.Png);
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap deckBMap = Resources.deck;
            if (checkBox3.Checked)
            {
                deckBMap.Save(currentDir + @"\Team2Deck2.png", ImageFormat.Png);
            }
            else
            {
                Bitmap emptyBMap = new Bitmap(Resources.deck.Width, Resources.deck.Height);
                emptyBMap.Save(currentDir + @"\Team2Deck2.png", ImageFormat.Png);
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap deckBMap = Resources.repeat;
            if (checkBox8.Checked)
            {
                deckBMap.Save(currentDir + @"\Team2Repeat1.png", ImageFormat.Png);
            }
            else
            {
                Bitmap emptyBMap = new Bitmap(Resources.deck.Width, Resources.deck.Height);
                emptyBMap.Save(currentDir + @"\Team2Repeat1.png", ImageFormat.Png);
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            Bitmap deckBMap = Resources.repeat;
            if (checkBox7.Checked)
            {
                deckBMap.Save(currentDir + @"\Team2Repeat2.png", ImageFormat.Png);
            }
            else
            {
                Bitmap emptyBMap = new Bitmap(Resources.deck.Width, Resources.deck.Height);
                emptyBMap.Save(currentDir + @"\Team2Repeat2.png", ImageFormat.Png);
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            TextBox currTBox = sender as TextBox;
            string[] rippedInfo = currTBox.AccessibleDescription.Split('-');
            if (rippedInfo.Length < 3) { MessageBox.Show("Something very wrong happened. Call Sin and tell him of this!"); return;}
            int loc1 = int.Parse(rippedInfo[1]), loc2 = int.Parse(rippedInfo[2]);
            if (currTBox.Text.All(char.IsDigit) && !String.IsNullOrEmpty(currTBox.Text))
            {
                SetPlayerScores(false);
            }else
            {
                scoresValid1[loc1 - 1 + (loc2 == 2 ? 5 : 0)] = false;
                return;
            }
            scoresValid1[loc1 - 1 + (loc2 == 2 ? 5 : 0)] = true;
        }

        void SetPlayerScores(bool teamSide)
        {
            StringBuilder newStringB = new StringBuilder();
            if(teamSide)
            {
                if (scoresValid2.Any(x => x == false)) 
                    return;
                newStringB.Append(textBox10.Text).Append("-").AppendLine(textBox9.Text);
                newStringB.Append(textBox11.Text).Append("-").AppendLine(textBox12.Text);
                newStringB.Append(textBox17.Text).Append("-").AppendLine(textBox18.Text);
                newStringB.Append(textBox23.Text).Append("-").AppendLine(textBox24.Text);
                newStringB.Append(textBox29.Text).Append("-").AppendLine(textBox30.Text); 
                File.WriteAllText(currentDir + @"\Team2Scores.txt", newStringB.ToString());
            }
            else
            {
                if (scoresValid1.Any(x => x == false))
                    return;
                newStringB.Append(textBox6.Text).Append("-").AppendLine(textBox7.Text);
                newStringB.Append(textBox15.Text).Append("-").AppendLine(textBox14.Text);
                newStringB.Append(textBox21.Text).Append("-").AppendLine(textBox20.Text);
                newStringB.Append(textBox27.Text).Append("-").AppendLine(textBox26.Text);
                newStringB.Append(textBox33.Text).Append("-").AppendLine(textBox32.Text);
                File.WriteAllText(currentDir + @"\Team1Scores.txt", newStringB.ToString());
            }
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            TextBox currTBox = sender as TextBox;
            string[] rippedInfo = currTBox.AccessibleDescription.Split('-');
            if (rippedInfo.Length < 3) { MessageBox.Show("Something very wrong happened. Call Sin and tell him of this!"); return; }
            int loc1 = int.Parse(rippedInfo[1]), loc2 = int.Parse(rippedInfo[2]);
            //File.WriteAllText(currentDir + @"\Team2Names.txt", ReturnPlayerList(true));
            if (currTBox.Text.All(char.IsDigit) && !String.IsNullOrEmpty(currTBox.Text))
                SetPlayerScores(true);
            else
            {
                scoresValid2[loc1 - 1 + (loc2 == 2 ? 5 : 0)] = false;
                return;
            }
            scoresValid2[loc1 - 1 + (loc2 == 2 ? 5 : 0)] = true;
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Helpers.ShowCreditsDialog("Credits");
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
