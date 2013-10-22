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

        int indexA;

        public int IndexA
        {
            get { return indexA; }
            set { indexA = value; }
        }

        int indexB;
        public int IndexB
        {
            get { return indexB; }
            set { indexB = value; }
        }

        Rectangle bbA;
        public Rectangle BbA
        {
            get { return bbA; }
            set { bbA = value; }
        }

        Rectangle bbB;
        public Rectangle BbB
        {
            get { return bbB; }
            set { bbB = value; }
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
