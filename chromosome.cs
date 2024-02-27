using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TrackGA
{
    class chromosome
    {
        static int NoGens = 18;
        public int NoG = NoGens;
        private int TrackNo;
        int rang = 25;
        public float maxValue = 0;
        public PointF[] GenSource = new PointF[NoGens];
        public PointF[] GenSourceC1 = new PointF[NoGens];
        public PointF[] GenSourceC2 = new PointF[NoGens];
        public float[] GenSourceW = new float[NoGens];

        public chromosome()
        {

        }
        public int Track
        {
            get { return TrackNo; }
            set { TrackNo = value; }
        }

        public PointF getGen(int index)
        {
            
            PointF result = new PointF();
            result.X = GenSource[index].X;
            result.Y = GenSource[index].Y;
            return result;
        }
        public PointF getC1(int index)
        {
            
            PointF result = new PointF();
            result.X = this.GenSourceC1[index].X;
            result.Y = this.GenSourceC1[index].Y;
            return result;
        }
        public PointF getC2(int index)
        {
            
            PointF result = new PointF();
            result.X = this.GenSourceC2[index].X;
            result.Y = this.GenSourceC2[index].Y;
            return result;
        }

        public float getW(int index)
        {
            
            float result = 0f;
           
            result = this.GenSourceW[index];
            return result;
        }

        public void getNewWeight(int SegNo)
        {
            System.Random r = new System.Random();
            System.Random r1 = new System.Random();
            int SegNo1 = SegNo + 1;
            if (SegNo1 > NoGens - 1)
                SegNo1 = 0;

            float X1 = this.GenSource[SegNo].X;
            float X2 = this.GenSource[SegNo1].X;
            int max = (int)X1 + rang;
            int min = (int)X2 - rang;

            if (X2 > X1)
            {
                max = (int)X2 + rang;
                min = (int)X1 - rang;
            }

            float cX1 = r.Next(min, max) + (float)r1.NextDouble();
            float cX2 = r.Next(min, max) + (float)r1.NextDouble();
            float Y1 = this.GenSource[SegNo].Y;
            float Y2 = this.GenSource[SegNo1].Y;
            max = (int)Y1 + rang;
            min = (int)Y2 - rang;
            if (Y2 > Y1)
            {
                max = (int)Y2 + rang;
                min = (int)Y1 - rang;
            }
            float cY1 = r.Next(min, max) + (float)r1.NextDouble();
            float cY2 = r.Next(min, max) + (float)r1.NextDouble();
            PointF C1 = new PointF(cX1, cY1);
            PointF C2 = new PointF(cX2, cY2);
            float w = getWieght(GenSource[SegNo], C1, C2, GenSource[SegNo1]);
            this.GenSourceC1[SegNo] = C1;
            this.GenSourceC2[SegNo] = C2;
            this.GenSourceW[SegNo] = w;

        }
        float getWieght(PointF P1, PointF P2, PointF P3, PointF P4)
        {
            float a1 = P1.X - P2.X;
            float b1 = P1.Y - P2.Y;
            float side1 = (float)Math.Sqrt(a1 * a1 + b1 * b1);

            float a2 = P4.X - P2.X;
            float b2 = P4.Y - P2.Y;
            float side2 = (float)Math.Sqrt(a2 * a2 + b2 * b2);
            float angl1 = Math.Abs(side1 - side2);

            float a3 = P1.X - P3.X;
            float b3 = P1.Y - P3.Y;
            float side3 = (float)Math.Sqrt(a3 * a3 + b3 * b3);

            float a4 = P4.X - P3.X;
            float b4 = P4.Y - P3.Y;
            float side4 = (float)Math.Sqrt(a4 * a4 + b4 * b4);
            float angl2 = Math.Abs(side3 - side4);
            return (angl1 + angl2);
        }

        public float getFitness()
        {
            float t = 0;
            for (int i = 0; i < GenSourceW.Length; i++)
            {
                t = t + GenSourceW[i];

            }
            return t;
        }
        public float getMaxSegmentWeight()
        {
            this.maxValue = GenSourceW.Max();
            return GenSourceW.Max();
        }

        public int indexMax(float maxValue)
        {
            return Array.IndexOf(GenSourceW, maxValue);
        }

        public float getMinSegmentWeight()
        {
            this.maxValue = GenSourceW.Min();
            return GenSourceW.Min();
        }

        public void DuplicateChromosome(int index, chromosome DupChro)
        {

            this.Track = index;
            for (int i = 0; i < NoG; i++)
            {
                this.GenSource[i] = DupChro.GenSource[i];
                this.GenSourceC1[i] = DupChro.GenSourceC1[i];
                this.GenSourceC2[i] = DupChro.GenSourceC2[i];
                this.GenSourceW[i] = DupChro.GenSourceW[i];
            }
        }
        public void saveTrack(string path)
        {
            StreamWriter BizFile = new StreamWriter(Path.Combine(path, "TrackBz" + this.Track + ".txt"));
            for (int i = 0; i < this.NoG - 1; i++)
                BizFile.WriteLine(GenSource[i].X + "," + GenSource[i].Y + "," + GenSourceC1[i].X + "," + GenSourceC1[i].Y + ","
                    + GenSourceC2[i].X + "," + GenSourceC2[i].Y + "," + GenSource[i + 1].X + "," + GenSource[i + 1].Y + "," + GenSourceW[i]);
            BizFile.Close();
        }
    }
}