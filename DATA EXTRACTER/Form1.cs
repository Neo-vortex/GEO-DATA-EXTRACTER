using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATA_EXTRACTER
{
    public partial class Form1 : Form
    {
        private static List<Data> PG = new List<Data>();
        private static List<Data> SG = new List<Data>();
        public BindingSource source1 = new BindingSource();
        public BindingSource source2 = new BindingSource();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("این برنامه با زحمت زیاد نوشته شده است لطفا خوب از ان استفاده کنید!", "پیام",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            dataGridView1.AutoGenerateColumns = true;
            dataGridView2.AutoGenerateColumns = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ofd.Multiselect = false;
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                path.Text = ofd.FileName;
                button2.Enabled = true;
            }
            else
            {
                path.Text =string.Empty;
                button2.Enabled = false;
            }
        }
        public class Data
        {
            public string station { get; set; }
            public TimeSpan time { get; set; }
            public double dis { get; set; }
            public string phase { get; set; }
            public  DateTime RefrenceDateTime { get; set; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var counter = 0;
            var text = System.IO.File.ReadAllText(path.Text).Split(Environment.NewLine);
            bar.Maximum = text.Length ;
            try
            {
                TimeSpan headertimer;
                DateTime daytime;
                string[] buffer;
                Data databuffer;
                var shifter = 0;
                while (counter < text.Length -1)
                {
                    headertimer = TimeFinder(text[counter]);
                    daytime = DaytimrFinder(text[counter]);
                    while (!text[counter].Split(" ", StringSplitOptions.None).Contains("STAT"))
                    {
                        counter++;
                        bar.Value = bar.Value + 1;
                        Application.DoEvents();
                    }
                    counter++;
                    bar.Value = bar.Value + 1;
                    Application.DoEvents();
                    while (text[counter].Trim() != string.Empty)
                    {
                        buffer = text[counter].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        databuffer = new Data();
                        shifter = 0;
                        databuffer.station = buffer[0];
                        databuffer.phase = buffer[2];
                        var buffer2 = buffer[buffer[3] != "D" ? 3 : 4];
                        switch (buffer2.Length)
                        {
                            case var @case when @case == 1:
                                var hhhh =  int.Parse((buffer[buffer[3] != "D" ? 3 : 4]));
                                var mmmm =  int.Parse((buffer[buffer[3] != "D" ? 4 : 5]));
                                var ssss = Convert.ToInt32(Math.Floor(Convert.ToDouble(buffer[buffer[3] != "D" ? 5 : 6])));
                                var msmsmsms = Convert.ToInt32((Convert.ToDouble(buffer[buffer[3] != "D" ? 5 : 6]) - ssss) * 1000);
                                databuffer.time = (headertimer - new TimeSpan(0, hhhh, mmmm, ssss, msmsmsms)).Duration();
                                shifter = 1;
                                databuffer.RefrenceDateTime = daytime + headertimer + databuffer.time;
                                break;

                            case var @case when @case == 4:
                                var h =  int.Parse(string.Concat(buffer[buffer[3] != "D" ? 3 : 4][0], buffer[buffer[3] != "D" ? 3 : 4][1]));
                                var m =  int.Parse(string.Concat(buffer[buffer[3] != "D" ? 3 : 4][2], buffer[buffer[3] != "D" ? 3 : 4][3]));
                                var s = Convert.ToInt32(Math.Floor(Convert.ToDouble(buffer[buffer[3] != "D" ? 4 : 5])));
                                var ms = Convert.ToInt32((Convert.ToDouble(buffer[buffer[3] != "D" ? 4 : 5]) - s) * 1000);
                                databuffer.time = (headertimer - new TimeSpan(0, h, m, s, ms)).Duration();
                                databuffer.RefrenceDateTime = daytime + headertimer + databuffer.time;
                                break;
                            case var @case when @case == 2:
                                var hh =  int.Parse(string.Concat(buffer[buffer[3] != "D" ? 3 : 4][0], buffer[buffer[3] != "D" ? 3 : 4][1]));
                                var mm =  int.Parse(string.Concat("0", buffer[buffer[3] != "D" ? 4 : 5][0]));
                                var ss = Convert.ToInt32(Math.Floor(Convert.ToDouble(buffer[buffer[3] != "D" ? 5 : 6])));
                                var msms = Convert.ToInt32((Convert.ToDouble(buffer[buffer[3] != "D" ? 5 : 6]) - ss) * 1000);
                                databuffer.time = (headertimer - new TimeSpan(0, hh, mm, ss, msms)).Duration();
                                shifter = 1;
                                databuffer.RefrenceDateTime = daytime + headertimer + databuffer.time;
                                break;
                            case var @case when @case == 3:
                                var hhh =  int.Parse(string.Concat("0", buffer[buffer[3] != "D" ? 3 : 4][0]));
                                var mmm =  int.Parse(string.Concat(buffer[buffer[3] != "D" ? 3 : 4][1], buffer[buffer[3] != "D" ? 3 : 4][2]));
                                var sss = Convert.ToInt32(Math.Floor(Convert.ToDouble(buffer[buffer[3] != "D" ? 5 : 6])));
                                var msmsms =  Convert.ToInt32((Convert.ToDouble(buffer[buffer[3] != "D" ? 4 : 5]) - sss) * 1000);
                                databuffer.time = (headertimer - new TimeSpan(0, hhh, mmm, sss, msmsms)).Duration();
                                databuffer. RefrenceDateTime = daytime + headertimer + databuffer.time ;
                                break;
                        }

      
                            databuffer.dis = Convert.ToDouble(buffer[buffer.Length -2]);
                            switch (databuffer.phase)
                            {
                                case "EPg":
                                    PG.Add(databuffer);
                                    break;
                                case "ESg":
                                    SG.Add(databuffer);
                                    break;
                            }
                            counter++;
                            bar.Value = bar.Value + 1;
                            Application.DoEvents();
                    }
                    counter++;
                    bar.Value = bar.Value + 1;
                    Application.DoEvents();
                }
            }
            catch (Exception ee)
            {
       
            }
            PG.Sort(delegate (Data c1, Data c2) { return c1.dis.CompareTo(c2.dis); });
            source1.DataSource = PG;
            dataGridView1.DataSource = source1;
            StringBuilder stringthing = new StringBuilder();
            stringthing.Append("-----PG PHASE-----");
            stringthing.Append(Environment.NewLine);
            foreach (var VARIABLE in PG)
            {
                stringthing.Append(VARIABLE.station + "\t" + VARIABLE.phase + "\t" + VARIABLE.time.TotalSeconds + "\t" + VARIABLE.dis + "\t" + VARIABLE.RefrenceDateTime);
                stringthing.Append(Environment.NewLine);
            }
            stringthing.Append("-------------");
            System.IO.File.WriteAllText(System.IO.Path.Combine(Environment.CurrentDirectory, "PG.txt"), stringthing.ToString());
            stringthing.Clear();
            SG.Sort(delegate (Data c1, Data c2) { return c1.dis.CompareTo(c2.dis); });
            source2.DataSource = SG;
            dataGridView2.DataSource = source2;
            dataGridView2.Refresh();
            dataGridView1.Refresh();
            stringthing.Append("-----SG PHASE-----");
            stringthing.Append(Environment.NewLine);
            foreach (var VARIABLE in SG)
            {
                stringthing.Append(VARIABLE.station + "\t" + VARIABLE.phase + "\t" + VARIABLE.time.TotalSeconds + "\t" + VARIABLE.dis + "\t" + VARIABLE.RefrenceDateTime);
                stringthing.Append(Environment.NewLine);
            }
            stringthing.Append("-------------");
            System.IO.File.WriteAllText(System.IO.Path.Combine(Environment.CurrentDirectory, "SG.txt"), stringthing.ToString());
            MessageBox.Show("DONE!");

        }

        private static DateTime DaytimrFinder(string x)
        {
            var buff = x.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var year =  int.Parse( buff[0]);
            var month = 0;
            var day = 0;
         
            switch (buff[1].Length)
            {
                case 1:
                    month = int.Parse(buff[1]);
                    day = int.Parse(buff[2]);
                    break;
                case 2:
                    month = int.Parse(buff[1]);
                    day = int.Parse(buff[2]);
                    break;
                case 3:
                    month = int.Parse(buff[1][0].ToString());
                    day = int.Parse(string.Concat(buff[1][1], buff[1][2]) );
                    break;
                case 4:
                    month = int.Parse(string.Concat(buff[1][0], buff[1][1]));
                    day = int.Parse(string.Concat(buff[1][2], buff[1][3]));
                    break;

            }
            return  new DateTime(year,month,day);
        }
        private static TimeSpan TimeFinder(string x)
        {

            var buff = x.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (buff[2].Length == 4)
            {
                var h =  int.Parse(string.Concat(buff[2][0], buff[2][1]));
                var m =  int.Parse(string.Concat(buff[2][2], buff[2][3]));
                var s = Convert.ToInt32(Math.Floor(Convert.ToDouble(buff[3])));
                var ms = Convert.ToInt32((Convert.ToDouble(buff[3]) - s) * 1000);
                return new TimeSpan(0, h, m, s, ms);
            }
            else
            {

                var h =  int.Parse(string.Concat(buff[3][0], buff[3][1]));
                var m =  int.Parse(string.Concat(buff[3][2], buff[3][3]));
                var s = Convert.ToInt32(Math.Floor(Convert.ToDouble(buff[4])));
                var ms = Convert.ToInt32((Convert.ToDouble(buff[4]) - s) * 1000);
                return new TimeSpan(0, h, m, s, ms);
            }

        }
    }
}
