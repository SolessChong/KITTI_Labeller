using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;

namespace labeler
{
    
    public partial class Form1 : Form
    {
        string dir;
        string xmlFileName;

        int         frameCnt;
        List<TE>    TEList;

        // Output xml elements
        XmlDocument xmlOutput;
        XmlNode xmlTEList;

        // Current TE, corresponding to each buttons
        Dictionary<string, int> currentEventList = new Dictionary<string, int>();

        // Create event operations
        private int StartEvent(string type, int first_frame){
            TE te = new TE(type, first_frame);
            TEList.Add(te);
            toolStripStatusLabel1.Text = "Start Event";
            return te.ID;
        }
        private void EndEvent(int ID)
        {
            for (int i = 0; i < TEList.Count; i++)
            {
                if (TEList[i].ID == ID)
                {
                    TE te = TEList[i];
                    te.Frame_cnt = frameCnt - te.First_frame;
                    if (te.Frame_cnt <= 0)
                    {
                        toolStripStatusLabel1.Text = "Event too short";
                        return;
                    }
                    
                    XmlNode xmlTE = xmlOutput.CreateElement("event");
                    XmlNode xmlType = xmlOutput.CreateElement("type");
                    xmlType.InnerText = te.Type;
                    xmlTE.AppendChild(xmlType);
                    XmlNode xmlFirstFrame = xmlOutput.CreateElement("first_frame");
                    xmlFirstFrame.InnerText = te.First_frame.ToString();
                    xmlTE.AppendChild(xmlFirstFrame);
                    XmlNode xmlFrameCnt = xmlOutput.CreateElement("frame_cnt");
                    xmlFrameCnt.InnerText = te.Frame_cnt.ToString();
                    xmlTE.AppendChild(xmlFrameCnt);

                    xmlTEList.AppendChild(xmlTE);
            
                    TEList.RemoveAt(i);
                    toolStripStatusLabel1.Text = "End Event";

                    return;
                }
            }
        }

        // Workflow utilities
        private void FreshImage(int ind)
        {
            lbFrameInd.Text = frameCnt.ToString();

            try
            {
                string fn = Path.Combine(dir, frameCnt.ToString("D10") + ".png");
                imageBox.Image = new Image<Bgr, Byte>(fn);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                toolStripStatusLabel1.Text = ex.Message;
            }
        }
        private void NextFrame()
        {
            frameCnt++;
            FreshImage(frameCnt);
        }
        private void PrevFrame()
        {
            frameCnt--;
            FreshImage(frameCnt);
        }

        private void Init()
        {

        }
        private void EndLabeling()
        {
            while (TEList.Count > 0)
            {
                EndEvent(TEList[0].ID);
            }
        }
        private void SaveLabels()
        {
            if (!Directory.Exists(Path.Combine(dir, "eventlabel")))
                Directory.CreateDirectory(Path.Combine(dir, "eventlabel"));
            xmlOutput.Save(xmlFileName);
        }

        // UI utilities
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MouseWheel += Form1_MouseWheel;
        }

        void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
                btnNext_Click(sender, e);
            else if (e.Delta > 0)
                btnPrev_Click(sender, e);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {

            dir = Path.Combine(tbDir.Text, @"image_02\data");
            xmlFileName = Path.Combine(tbDir.Text, @"eventlabel\events.xml");

            TEList = new List<TE>();
            frameCnt = 0;

            xmlOutput = new XmlDocument();
            xmlOutput.CreateXmlDeclaration("1.0", "utf-8", "yes");

            XmlNode xmlRoot = xmlOutput.CreateElement("labelfile");
            xmlOutput.AppendChild(xmlRoot);

            xmlTEList = xmlOutput.CreateElement("events");
            xmlRoot.AppendChild(xmlTEList);

            XmlNode xmlDir = xmlOutput.CreateElement("directory");
            xmlDir.InnerText = dir;
            xmlRoot.AppendChild(xmlDir);

            FreshImage(frameCnt);
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            PrevFrame();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            NextFrame();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveLabels();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                btnNext_Click(sender, e);
            }
            if (e.KeyCode == Keys.Left)
            {
                btnPrev_Click(sender, e);
            }
        }

        private void EventButtonClick(object sender, EventArgs e)
        {
            /***********************************\
             * Process Event Button Click Event
            \***********************************/
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked)
            {
                int ID = StartEvent(cb.Text, frameCnt);
                currentEventList.Add(cb.Text, ID);
            }
            else
            {
                EndEvent(currentEventList[cb.Text]);
                currentEventList.Remove(cb.Text);
            }
        }

        private void btnOpenLabel_Click(object sender, EventArgs e)
        {
            /***********************************\
             * Load events label file and add them into list view
            \***********************************/
            dir = Path.Combine(tbDir.Text, @"image_02\data");
            xmlFileName = Path.Combine(tbDir.Text, @"eventlabel\events.xml");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName);

            var sn = xmlDoc.SelectSingleNode("labelfile").SelectSingleNode("events");

            XmlNodeList events = xmlDoc.SelectSingleNode("labelfile").SelectSingleNode("events").ChildNodes;
            foreach (XmlNode xn in events)
            {
                XmlElement ele = (XmlElement)xn;
                ListViewItem lvi = new ListViewItem(new[]{
                    ele.GetElementsByTagName("type")[0].InnerText,
                    ele.GetElementsByTagName("first_frame")[0].InnerText,
                    ele.GetElementsByTagName("frame_cnt")[0].InnerText
                });
                lvEvents.Items.Add(lvi);
            }
        }

        private void lvEvents_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var s0 = e.Item.SubItems[0];
            var s1 = e.Item.SubItems[1];
            var s2 = e.Item.SubItems[2];

            var firstFrameStr = e.Item.SubItems[1].Text;
            frameCnt = int.Parse(firstFrameStr);
            FreshImage(frameCnt);
        }

    }


    public class TE
    {
        static int global_ID;

        #region Attributes

        int first_frame;
        public int First_frame
        {
            get { return first_frame; }
            set { first_frame = value; }
        }

        int frame_cnt;
        public int Frame_cnt
        {
            get { return frame_cnt; }
            set { frame_cnt = value; }
        }

        string type;
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        int _ID;
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        #endregion

        public TE(string type)
        {
            this.ID = global_ID++;

            this.Type = type;
        }

        public TE(string type, int first_frame)
        {
            this.ID = global_ID++;

            this.Type = type;
            this.First_frame = first_frame;
        }

    }

}
