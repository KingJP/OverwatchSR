using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OverwatchSR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            toolStripStatus.Text = "Bereit";
            toolStripProgress.Maximum = 4;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string battleTag = textBoxBattleTag.Text;
            battleTag = battleTag.Replace('#', '-');

            if (!String.IsNullOrEmpty(battleTag))
            {
                toolStripStatus.Text = "Starte API Anfrage.";
                toolStripProgress.Value = 1;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://owapi.net/api/v3/u/" + battleTag + "/stats");
                request.Method = "GET";
                request.ContentType = "application/json";
                request.UserAgent = ".NET Overwatch SR Lookup";
                try
                {
                    var response = (HttpWebResponse)request.GetResponse();

                    toolStripStatus.Text = "API Antwort erhalten.";
                    toolStripProgress.Value = 2;

                    string json;
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        json = sr.ReadToEnd();
                    }
                    var data = (JObject)JsonConvert.DeserializeObject(json);

                    toolStripStatus.Text = "JSON konviertiert.";
                    toolStripProgress.Value = 3;

                    if (data != null)
                    {
                        if (data["eu"] != null)
                        {
                            if (data["eu"]["stats"] != null)
                            {
                                if (data["eu"]["stats"]["competitive"] != null)
                                {
                                    toolStripStatus.Text = "Lade Bilder.";
                                    toolStripProgress.Value = 4;

                                    Image avatar = GetImageFromURL(data["eu"]["stats"]["competitive"]["overall_stats"]["avatar"].Value<string>());
                                    Image tier = GetImageFromURL(data["eu"]["stats"]["competitive"]["overall_stats"]["tier_image"].Value<string>());

                                    string comprank = data["eu"]["stats"]["competitive"]["overall_stats"]["comprank"].Value<string>();
                                    dataGrid.Rows.Add(avatar, battleTag, tier, comprank);

                                    toolStripStatus.Text = "Fertig";
                                    toolStripProgress.Value = 0;
                                }
                            }
                        }
                    }
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private static Image GetImageFromURL(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebReponse.GetResponseStream();
            return Image.FromStream(stream);
        }

        private void schließenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
