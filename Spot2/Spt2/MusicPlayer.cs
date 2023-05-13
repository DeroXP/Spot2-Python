using System;
using System.Windows.Forms;
using NAudio.Wave;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MusicPlayerApp
{
    public partial class MainForm : Form
    {
        private WaveOutEvent wavePlayer;
        private AudioFileReader audioReader;
        private Button pauseButton;
        private Button unpauseButton;
        private string MUSIC_DIRECTORY = "";
        private int currentSongIndex = 0;

        private void ScaleUI()
        {
            float scalingFactor = (float)this.ClientSize.Width / 284f; // Adjust the scaling factor as needed

            if (scalingFactor < 0)
            {
            this.statusLabel.Font = new System.Drawing.Font(this.Font.FontFamily, 13f * scalingFactor, System.Drawing.FontStyle.Regular); // Adjust font size
            this.chooseDirectoryButton.Font = new System.Drawing.Font(this.Font.FontFamily, 9f * scalingFactor, System.Drawing.FontStyle.Regular); // Adjust font size
            this.musicListBox.Font = new System.Drawing.Font(this.Font.FontFamily, 9f * scalingFactor, System.Drawing.FontStyle.Regular); // Adjust font size
            this.playButton.Font = new System.Drawing.Font(this.Font.FontFamily, 9f * scalingFactor, System.Drawing.FontStyle.Regular); // Adjust font size
            this.stopButton.Font = new System.Drawing.Font(this.Font.FontFamily, 9f * scalingFactor, System.Drawing.FontStyle.Regular); // Adjust font size
            }
        }

        public MainForm()
        {
            InitializeComponent();

            wavePlayer = new WaveOutEvent();
            audioReader = null; // Assign null initially

            pauseButton = new Button();
            unpauseButton = new Button();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Specify the scaling factor (e.g., 1.5 for 150% scaling)
            float scalingFactor = 1.5f;

            // Set the scaling factor for the form and its controls
            Scale(new SizeF(scalingFactor, scalingFactor));
            ScaleUI();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            ScaleUI();
        }

        private void LoadMusicFiles(string directory)
        {
            string[] fileExtensions = { ".wav", ".mp3", ".ogg" };
            List<string> musicFiles = Directory.GetFiles(directory)
                .Where(file => fileExtensions.Contains(Path.GetExtension(file).ToLower()))
                .ToList();

            // Update the musicListBox with the musicFiles
            musicListBox.Items.Clear();
            foreach (string musicFile in musicFiles)
            {
                musicListBox.Items.Add(musicFile);
            }
        }

        private string ConvertToWav(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            string wavFilePath = Path.ChangeExtension(filePath, ".wav");
            using (var reader = new Mp3FileReader(filePath))
            {
                WaveFileWriter.CreateWaveFile(wavFilePath, reader);
            }
            return wavFilePath;
        }

        private void PlaySelectedSong()
        {
            int selectedIndex = musicListBox.SelectedIndex;
            if (selectedIndex >= 0)
            {
                string selectedFile = musicListBox.SelectedItem.ToString();
                string wavFile = ConvertToWav(selectedFile);
                audioReader = new AudioFileReader(wavFile);
                wavePlayer.Init(audioReader);
                wavePlayer.Play();

                // Update the UI and handle song completion
                statusLabel.Text = "Playing: " + Path.GetFileName(selectedFile);
                currentSongIndex = selectedIndex;

                wavePlayer.PlaybackStopped += WavePlayer_PlaybackStopped;
            }
        }

        private void WavePlayer_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            // Song playback completed, move to the next song if available
            currentSongIndex++;
            if (currentSongIndex < musicListBox.Items.Count)
            {
                musicListBox.SelectedIndex = currentSongIndex;
                PlaySelectedSong();
            }
            else
            {
                Stop();
            }
        }

        private void Pause()
        {
            if (wavePlayer != null && wavePlayer.PlaybackState == PlaybackState.Playing)
            {
                wavePlayer.Pause();
            }
        }

        private void Unpause()
        {
            if (wavePlayer != null && wavePlayer.PlaybackState == PlaybackState.Paused)
            {
                wavePlayer.Play();
            }
        }

        private void Stop()
        {
            if (wavePlayer != null && wavePlayer.PlaybackState != PlaybackState.Stopped)
            {
                wavePlayer.Stop();
                wavePlayer.Dispose();
                audioReader.Dispose();
                statusLabel.Text = "Music Stopped";
            }
        }

        private void ChooseDirectory()
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.mp3");
                musicListBox.Items.Clear();
                musicListBox.Items.AddRange(files);
            }
        }

        private void chooseDirectoryButton_Click(object sender, EventArgs e)
        {
            ChooseDirectory();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            Stop();

            if (wavePlayer.PlaybackState == PlaybackState.Playing)
            {
                wavePlayer.Stop();
            }

            string filePath = musicListBox.SelectedItem.ToString();

            if (filePath != null)
            {
                statusLabel.Text = "Converting";
                ConvertToWav(filePath);
            }
            else
            {
                MessageBox.Show("File path is null.");
            }

            if (musicListBox.SelectedItem != null)
            {
                string wavFilePath = ConvertToWav(filePath);

                audioReader = new AudioFileReader(wavFilePath);
                wavePlayer.Init(audioReader);
                wavePlayer.Play();
                statusLabel.Text = "Playing: " + wavFilePath;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            Stop();

            if (wavePlayer.PlaybackState == PlaybackState.Playing)
            {
                wavePlayer.Stop();
            }
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            Pause();
        }

        private void unpauseButton_Click(object sender, EventArgs e)
        {
            Unpause();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }

        private void AutoplayFirstSong()
        {
            if (musicListBox.Items.Count > 0)
            {
                musicListBox.SelectedIndex = 0;
                PlaySelectedSong();
            }
        }

        // Other event handlers and UI setup code

        #region Windows Form Designer generated code

        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button chooseDirectoryButton;
        private System.Windows.Forms.ListBox musicListBox;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button stopButton;
        private void InitializeComponent()
        {
            this.statusLabel = new System.Windows.Forms.Label();
            this.chooseDirectoryButton = new System.Windows.Forms.Button();
            this.musicListBox = new System.Windows.Forms.ListBox();
            this.playButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.ForeColor = System.Drawing.Color.White; // Set label text color to white
            this.statusLabel.Location = new System.Drawing.Point(12, 15);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 13);
            this.statusLabel.TabIndex = 0;
            // 
            // chooseDirectoryButton
            // 
            this.chooseDirectoryButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31))))); // Set button background color
            this.chooseDirectoryButton.ForeColor = System.Drawing.Color.White; // Set button text color to white
            this.chooseDirectoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat; // Remove button border
            this.chooseDirectoryButton.Location = new System.Drawing.Point(12, 42);

            this.chooseDirectoryButton.Name = "chooseDirectoryButton";
            this.chooseDirectoryButton.Size = new System.Drawing.Size(120, 23);
            this.chooseDirectoryButton.TabIndex = 1;
            this.chooseDirectoryButton.Text = "Choose Directory";
            this.chooseDirectoryButton.UseVisualStyleBackColor = false;
            this.chooseDirectoryButton.Click += new System.EventHandler(this.chooseDirectoryButton_Click);
            // 
            // musicListBox
            // 
            this.musicListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48))))); // Set listbox background color
            this.musicListBox.ForeColor = System.Drawing.Color.White; // Set listbox text color to white
            this.musicListBox.FormattingEnabled = true;
            this.musicListBox.Location = new System.Drawing.Point(12, 71);
            this.musicListBox.Name = "musicListBox";
            this.musicListBox.Size = new System.Drawing.Size(250, 186);
            this.musicListBox.TabIndex = 2;
            // 
            // playButton
            // 
            this.playButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31))))); // Set button background color
            this.playButton.ForeColor = System.Drawing.Color.White; // Set button text color to white
            this.playButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat; // Remove button border
            this.playButton.Location = new System.Drawing.Point(12, 273);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(75, 23);
            this.playButton.TabIndex = 3;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = false;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31))))); // Set button background color
            this.stopButton.ForeColor = System.Drawing.Color.White; // Set button text color to white
            this.stopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat; // Remove button border
            this.stopButton.Location = new System.Drawing.Point(93, 273);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 4;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = false;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            //
            // pauseButton
            // Create pauseButton
            this.pauseButton = new System.Windows.Forms.Button();
            this.pauseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31))))); // Set button background color
            this.pauseButton.ForeColor = System.Drawing.Color.White; // Set button text color to white
            this.pauseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat; // Remove button border
            this.pauseButton.Location = new System.Drawing.Point(174, 273);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(75, 23);
            this.pauseButton.TabIndex = 5;
            this.pauseButton.Text = "Pause";
            this.pauseButton.UseVisualStyleBackColor = false;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);

            // Create unpauseButton
            this.unpauseButton = new System.Windows.Forms.Button();
            this.unpauseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31))))); // Set button background color
            this.unpauseButton.ForeColor = System.Drawing.Color.White; // Set button text color to white
            this.unpauseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat; // Remove button border
            this.unpauseButton.Location = new System.Drawing.Point(255, 273);
            this.unpauseButton.Name = "unpauseButton";
            this.unpauseButton.Size = new System.Drawing.Size(75, 23);
            this.unpauseButton.TabIndex = 6;
            this.unpauseButton.Text = "Unpause";
            this.unpauseButton.UseVisualStyleBackColor = false;
            this.unpauseButton.Click += new System.EventHandler(this.unpauseButton_Click);

            // Add pauseButton and unpauseButton to the form's controls
            this.Controls.Add(this.pauseButton);
            this.Controls.Add(this.unpauseButton);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31))))); // Set form background color
            this.ClientSize = new System.Drawing.Size(284, 311);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.musicListBox);
            this.Controls.Add(this.chooseDirectoryButton);
            this.Controls.Add(this.statusLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Music Player";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize); // Attach Resize event handler
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        #region Main Method

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        #endregion

    }
}