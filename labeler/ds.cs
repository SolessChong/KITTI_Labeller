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

        int frame_cnt;
        int first_frame;
        string type;
        int _ID;
        int indexA;
        int indexB;
        Rectangle bbA;
        Rectangle bbB;
        // Number of terms
        int numTerms;

        #region Interface

        public int First_frame
        {
            get { return first_frame; }
            set { first_frame = value; }
        }


        public int Frame_cnt
        {
            get { return frame_cnt; }
            set { frame_cnt = value; }
        }


        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public int NumTerms
        {
            get { return numTerms; }
            set { numTerms = value; }
        }


        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        

        public int IndexA
        {
            get { return indexA; }
            set { indexA = value; }
        }


        public int IndexB
        {
            get { return indexB; }
            set { indexB = value; }
        }

        
        public Rectangle BbA
        {
            get { return bbA; }
            set { bbA = value; }
        }

        
        public Rectangle BbB
        {
            get { return bbB; }
            set { bbB = value; }
        }

        #endregion

        #endregion

        public TE(string type, int numTerms)
        {
            this.ID = global_ID++;

            this.Type = type;
            this.NumTerms = numTerms;
        }

        public TE(string type, int numTerms, int first_frame)
        {
            this.ID = global_ID++;

            this.Type = type;
            this.NumTerms = numTerms;
            this.First_frame = first_frame;
        }

    }
}
