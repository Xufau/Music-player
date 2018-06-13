using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxWMPLib;
using WMPLib;

namespace Music_player
{
    public partial class Form1 : Form
    {
        int currentsongplay = 0;
        string currentdir;
        string currentsongpath;
        string currentsongname;
        string musicdir;
        string delmusicdir;
        bool shuffle = false;
        bool replay = false;
        bool autoplay = true;
        bool songfinished = true;
        bool paused = true;
        bool playprev = false;
        bool playnext = false;
        bool updating = false;
        bool playwhenready = false;
        bool selectsong = false;
        List<string> allsongs = new List<string>();
        List<string> allsongs1 = new List<string>();
        List<string> songs = new List<string>();

        public Form1()
        {
            InitializeComponent();
            currentdir = Directory.GetCurrentDirectory();
            musicdir = currentdir + @"\music";
            delmusicdir = currentdir + @"\music" + @"\del";
            Directory.CreateDirectory(musicdir);
            Directory.CreateDirectory(delmusicdir);
            button4.ForeColor = Color.Green;
            updatesongs();
            playnewsong();
            timer1.Start();
        }

        private void updatesongs()
        {
            updating = true;
            currentsongplay = 0;
            currentdir = Directory.GetCurrentDirectory();
            currentsongpath = "";
            currentsongname = "";
            musicdir = currentdir + @"\music";
            delmusicdir =currentdir + @"\music" + @"\del";
            Directory.CreateDirectory(musicdir);
            Directory.CreateDirectory(delmusicdir);
            songfinished = true;
            paused = true;
            playprev = false;
            currentsongplay = -1;
            songs.Clear();
            listBox1.Items.Clear();

            String[] wavs = Directory.GetFiles(musicdir, "*.*wav");
            String[] mp3s = Directory.GetFiles(musicdir, "*.*mp3");
            String[] aacs = Directory.GetFiles(musicdir, "*.*aac");
            String[] m4as = Directory.GetFiles(musicdir, "*.*m4a");

            foreach (String wav in wavs)
            {
                try
                {
                    songs.Add(wav);
                    listBox1.Items.Add(new FileInfo(wav).Name);
                }
                catch{}
            }
            foreach (String mp3 in mp3s)
            {
                try
                {
                    songs.Add(mp3);
                    listBox1.Items.Add(new FileInfo(mp3).Name);
                }
                catch{}
            }
            foreach (String aac in aacs)
            {
                try
                {
                    songs.Add(aac);
                    listBox1.Items.Add(new FileInfo(aac).Name);
                }
                catch{}
            }
            foreach (String m4a in m4as)
            {
                try
                {
                    songs.Add(m4a);
                    listBox1.Items.Add(new FileInfo(m4a).Name);
                }
                catch { }
            }
            updating = false;
            playnewsong();
        }

        private void playnewsong()
        {
            int replaycurrent = currentsongplay;
            int rndnum = currentsongplay;
            try
            {
               currentsongpath = "";

                if (playprev == true)
                {
                    try
                    {
                        currentsongplay--;
                        currentsongpath = songs[currentsongplay];
                        Task.Run(() =>
                        {
                            axWindowsMediaPlayer1.URL = (currentsongpath);
                            axWindowsMediaPlayer1.Ctlcontrols.play();
                        });
                    }
                    catch
                    {
                        currentsongplay = 0;
                    }
                    currentsongpath = songs[currentsongplay];
                    playprev = false;
                }
                else if (selectsong)
                {
                    currentsongpath = songs[currentsongplay];
                    selectsong = false;
                }
                else if (playnext == true)
                {
                    currentsongplay++;
                    currentsongpath = songs[currentsongplay];
                    playnext = false;
                }
                else if (autoplay == true)
                {
                    currentsongplay++;
                    currentsongpath = songs[currentsongplay];
                }
                else if (shuffle == true)
                {
                    Random rnd = new Random();

                    while (rndnum == currentsongplay)
                    {
                        rndnum = rnd.Next(-1, songs.Count);
                    }
                    currentsongplay = rndnum;
                    currentsongpath = songs[currentsongplay];
                    axWindowsMediaPlayer1.URL = (currentsongpath);
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                }
                else if (replay == true)
                {
                    currentsongpath = songs[replaycurrent];
                }

                else if (replay == true){}
                string[] songnamme = (Path.GetFileName(currentsongpath).Split('.'));
                label2.Text = (songnamme[songnamme.Length - 2]);
                Console.WriteLine(songnamme[songnamme.Length - 1]);
                listBox1.SetSelected(listBox1.FindString(songnamme[songnamme.Length -2] + "." + songnamme[songnamme.Length - 1]), true);
                
                Task.Run(() =>
                {
                    if(shuffle == false)
                    {
                        try
                        {
                            axWindowsMediaPlayer1.URL = (currentsongpath);
                            axWindowsMediaPlayer1.Ctlcontrols.play();
                        }
                        catch { };
                    }
                });
            }
            catch
            {
                
                if (currentsongplay == songs.Count)
                {
                    currentsongplay = -1;
                }
                else if (currentsongplay >= 0)
                {
                    currentsongplay--;
                    updatesongs();
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)//Add songs
        {
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.Title = "Open Text File";
            filedialog.Filter = "Audio files (*.mp3, *.wav, *.aac, *.m4a) | *.mp3; *.wav; *.aac; *.m4a;";
            filedialog.InitialDirectory = @"C:\";

            if (filedialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Task.Run(() =>
                    {
                        string[] songnamme = (Path.GetFileName(filedialog.FileName).Split('.'));
                        string filename1 = (songnamme[songnamme.Length - 2] + "." + songnamme[songnamme.Length - 1]);
                        this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate () { button6.Text = "Downloading"; });
                        try
                        {
                            File.Copy(filedialog.FileName, musicdir + @"\" + filename1);
                        }
                        catch
                        {
                            File.Delete(musicdir + @"\" + filename1);
                            File.Copy(filedialog.FileName, musicdir + @"\" + filename1);
                        }
                        this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate () { button6.Text = "Add song"; });
                    });
                }
                catch { }
            }
            updatesongs();
            playwhenready = true;
        }

        private void button7_Click(object sender, EventArgs e)//Remove current song
        {
            try
            {
                groupBox1.Visible = false;
                File.Delete(currentsongpath);
                updatesongs();
            }
            catch { }
            updatesongs();
        }

        private void button9_Click(object sender, EventArgs e)//folder add music
        {
            allsongs.Clear();
            string filedirectory = "";

            try
            {
                allsongs.AddRange(Directory.GetFiles(@filedirectory, "*.mp3", SearchOption.AllDirectories));
                foreach(string sonng in allsongs)
                {
                    File.Copy(sonng, musicdir);
                }
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            shuffle = false;
            replay = false;
            autoplay = true;
            button8.ForeColor = Color.Black;
            button5.ForeColor = Color.Black;
            button4.ForeColor = Color.Green;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            shuffle = false;
            replay = true;
            autoplay = false;
            button8.ForeColor = Color.Black;
            button5.ForeColor = Color.Green;
            button4.ForeColor = Color.Black;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            shuffle = true;
            replay = false;
            autoplay = false;
            button8.ForeColor = Color.Green;
            button5.ForeColor = Color.Black;
            button4.ForeColor = Color.Black;
        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e){}

        private void button1_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            if (paused == true)
            {
                paused = false;
                axWindowsMediaPlayer1.Ctlcontrols.play();
                if (axWindowsMediaPlayer1.playState != WMPLib.WMPPlayState.wmppsPlaying)
                {
                    updatesongs();
                }
            }
            else if(paused == false)
            {
                paused = true;
                Task.Run(() =>
                {
                    axWindowsMediaPlayer1.Ctlcontrols.pause();
                });
            }
            if(listBox1.Items.Count == 0)
            {
                updatesongs();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)//check end of song
        {
            if (listBox1.Items.Count == 0)
            {
                updatesongs();
            }
            if(playwhenready == true)
            {
                axWindowsMediaPlayer1.Ctlcontrols.play();
                playwhenready = false;
            }
            if (updating == false)
            {
                if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped)
                {
                    playnewsong();
                }
                if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    paused = false;
                    button1.Text = "Pause";
                }
                if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused)
                {
                    paused = true;
                    button1.Text = "Play";
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)//go previous song
        {
            playprev = true;
            groupBox1.Visible = false;
            playnewsong();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (groupBox1.Visible == true)
            {
                groupBox1.Visible = false;
            }
            else if (groupBox1.Visible == false)
            {
                groupBox1.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            playnext = true;
            playnewsong();
        }

        private void button9_Click_1(object sender, EventArgs e)//Add folder
        {
            string folderdir = "";
            FolderBrowserDialog slctfolder = new FolderBrowserDialog();
            slctfolder.RootFolder = Environment.SpecialFolder.MyComputer;
            if (slctfolder.ShowDialog() == System.Windows.Forms.DialogResult.OK && System.IO.Directory.Exists(slctfolder.SelectedPath))
            {
                folderdir = @slctfolder.SelectedPath;
            }
            else{return;}

            String[] wavs = Directory.GetFiles(folderdir, "*.*wav");
            String[] mp3s = Directory.GetFiles(folderdir, "*.*mp3");
            String[] aacs = Directory.GetFiles(folderdir, "*.*aac");
            String[] m4as = Directory.GetFiles(folderdir, "*.*m4a");

            foreach (String wav in wavs)
            {
                try
                {
                    Console.WriteLine(wav);
                    File.Copy(wav, musicdir + @"\" + (new FileInfo(wav).Name));
                    updatesongs();
                }
                catch{}
            }
            foreach (String mp3 in mp3s)
            {
                try
                {
                    File.Copy(mp3, musicdir + @"\" + (new FileInfo(mp3).Name));
                    updatesongs();
                }
                catch { }
            }
            foreach (String aac in aacs)
            {
                try
                {
                    File.Copy(aac, musicdir + @"\" + (new FileInfo(aac).Name));
                    updatesongs();
                }
                catch { }
            }
            foreach (String m4a in m4as)
            {
                try
                {
                    File.Copy(m4a, musicdir + @"\" + (new FileInfo(m4a).Name));
                    updatesongs();
                }
                catch { }
            }      
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                File.Copy(file, musicdir);
            }
            updatesongs();
        }

        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            playpause(e);
        }
        private void playpause(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                groupBox1.Visible = false;
                if (paused == true)
                {
                    paused = false;
                    Task.Run(() =>
                    {
                        axWindowsMediaPlayer1.Ctlcontrols.play();
                    });
                }
                else if (paused == false)
                {
                    paused = true;
                    Task.Run(() =>
                    {
                        axWindowsMediaPlayer1.Ctlcontrols.pause();
                    });
                }
            }
        }

        private void axWindowsMediaPlayer1_KeyDownEvent(object sender, _WMPOCXEvents_KeyDownEvent e){}
        private void listBox1_KeyDown(object sender, KeyEventArgs e) { playpause(e); }
        private void button10_KeyDown(object sender, KeyEventArgs e) { playpause(e); }
        private void button7_KeyDown(object sender, KeyEventArgs e) { playpause(e); }
        private void button1_KeyDown(object sender, KeyEventArgs e) {  }
        private void button3_KeyDown(object sender, KeyEventArgs e) { playpause(e); }
        private void button2_KeyDown(object sender, KeyEventArgs e) { playpause(e); }
        private void button8_KeyDown(object sender, KeyEventArgs e) { playpause(e); }
        private void button5_KeyDown(object sender, KeyEventArgs e) { playpause(e); }
        private void button4_KeyDown(object sender, KeyEventArgs e) { playpause(e); }
        private void Form1_MouseClick(object sender, MouseEventArgs e) { groupBox1.Visible = false; }
        private void axWindowsMediaPlayer1_MouseDownEvent(object sender, _WMPOCXEvents_MouseDownEvent e) { groupBox1.Visible = false; }
        private void pictureBox2_MouseClick(object sender, MouseEventArgs e) { groupBox1.Visible = false; }
        private void pictureBox3_MouseClick(object sender, MouseEventArgs e) { groupBox1.Visible = false; }
        private void listView1_Click(object sender, EventArgs e) { groupBox1.Visible = false; }
        private void comboBox1_MouseClick(object sender, MouseEventArgs e) { groupBox1.Visible = false; }
        private void label1_MouseDown(object sender, MouseEventArgs e) { groupBox1.Visible = false; }
        private void label2_MouseClick(object sender, MouseEventArgs e) { groupBox1.Visible = false; }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selected = (listBox1.SelectedItem.ToString());
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    if (listBox1.Items[i] == selected)
                    {
                        Console.WriteLine(songs[i]);
                        currentsongplay = i;
                        selectsong = true;
                        playnewsong();
                    }
                }
            }
        }
    }
}