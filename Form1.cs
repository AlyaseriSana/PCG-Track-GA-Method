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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static int noPoints = 18;
        static int PubSize = 500;
        int crossPoint = noPoints / 2;
        int Maxloop = 100;
        int maxIteration=1;
        static System.Random r = new System.Random();

        public static string getPath1 = @"C:\Users\Admin\Desktop\general\C#programming\dataGA\";
        public static string getPath2 = @"C:\Users\Admin\Desktop\general\C#programming\SampleGA\";
        public static string getPath3 = @"C:\Users\Admin\Desktop\general\C#programming\Testfile\";
        ExcelFile Xtest = new ExcelFile(@"C:\Users\Admin\Desktop\general\C#programming\Testfile\test.xlsx", 1);
        public static int CurrentCurve = Directory.GetFiles(getPath1).Length;
        GenerateTrack trackCurve = new GenerateTrack();


        List<chromosome> ChromosomesPopulation = new List<chromosome>(PubSize);   // save all the chromosomes
        List<chromosome> Offspring = new List<chromosome>(PubSize);   // generation
        float[] fitnessProbability = new float[PubSize];
        chromosome Parent1 = new chromosome();
        chromosome Parent2 = new chromosome();
        chromosome Child1 = new chromosome();
        chromosome Child2 = new chromosome();
        chromosome BestGeneration = new chromosome();
        
        chromosome ChroTrack = new chromosome();
        float [] probaplityFit = new float[PubSize];
        int r1; int r2;
        int row = 1;
        int col = 19;

        private void Form1_Load(object sender, EventArgs e)
        {

           // Xtest.excelclear();
            row = 1;
            for (int i = 0; i < PubSize; i++)
                trackCurve.GenerateNewTrack(i, getPath1);
            for (int i = 0; i < maxIteration; i++)
            {
                DoGA();
                row = 1;
                col = col + 3;
            }
               


            Xtest.ExcelSave();
            Xtest.excelClose();
            MessageBox.Show(" the work is done on " + BestGeneration.Track.ToString());
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            GenerateTrack track1 = new GenerateTrack();
            Pen pen1 = new Pen(Brushes.Red, 4);
            track1.drawTrack(BestGeneration, pen1, e);


        }


        void DoGA()
        {
            int k = 0;
            
            chromosome father = new chromosome();
            chromosome mother = new chromosome();
            
            for (int i = 0; i < PubSize; i++)
            {
                GetTracks(i, ChroTrack);
                ChromosomesPopulation.Insert(i, new chromosome());
                ChromosomesPopulation[i].DuplicateChromosome(i,ChroTrack);
            }
            
           
          
            
            do
            {
                Offspring.Clear();
                for (int i = 0; i < PubSize; i++)
                {
                    father.DuplicateChromosome(GetParent(), ChromosomesPopulation[GetParent()]);
                    mother.DuplicateChromosome(GetParent(),ChromosomesPopulation[GetParent()]);
                   // System.Diagnostics.Debug.WriteLine(" K is " + k + " ...i is " + i + "....." + father.Track + " is the father track " + mother.Track + " is the mother tarck ");
                   while (father.Track == mother.Track)
                    {

                     mother.DuplicateChromosome(GetParent(),ChromosomesPopulation[GetParent()]);
                       // System.Diagnostics.Debug.WriteLine(father.Track + " is the father track " + mother.Track + " is the mother track");
                    }
                   //System.Diagnostics.Debug.WriteLine(" K is "+k+" ...i is "+ i+"....."+ father.Track + " is the father track "+ mother.Track + " is the mother tarck ");
                    merage(crossPoint, father, mother, Child1, Child2); // generate the children 

                    if (Child1.getFitness() < Child2.getFitness())
                    {
                        //System.Diagnostics.Debug.WriteLine(" child befor Mutuation  " + Child1.getFitness());
                        MutationClasic(Child1);
                        Offspring.Insert(i, new chromosome());
                        Offspring[i].DuplicateChromosome(i,Child1);
                        //System.Diagnostics.Debug.WriteLine(" child AFTER Mutuation  " + Child1.getFitness());
                    }
                    else
                    {
                        MutationClasic(Child2);
                        Offspring.Insert(i, new chromosome());
                        Offspring[i].DuplicateChromosome(i, Child2);
                    }
                   


                }
                ChromosomesPopulation.Clear();
                for (int i = 0; i < PubSize; i++)
                {
                    ChromosomesPopulation.Insert(i, new chromosome());
                    ChromosomesPopulation[i].DuplicateChromosome(i,Offspring[i]);
                }
                int fitGeneration= getFitted(Offspring);// calculate the prbility of fitteing
                BestGeneration.DuplicateChromosome(fitGeneration, Offspring[fitGeneration]);
                System.Diagnostics.Debug.WriteLine(k+"-"+Offspring[fitGeneration].getFitness()+" best is "+ BestGeneration.Track+" fitness is ..."+ BestGeneration.getFitness());
                row++;
                Xtest.writeXcelsheet(row, col, k.ToString());
                Xtest.writeXcelsheet(row, col + 1, Offspring[fitGeneration].getFitness().ToString());
                k++;

            } while (k < Maxloop) ;
                    // BestGeneration.saveTrack(getPath1);
            //testFile.Close();
        }
        void GetTracks(int index, chromosome X)
        {

            string[] lines = new string[noPoints];
            
            StreamReader readTrack = new(getPath1 + "TrackBz" + index + ".txt");

            int i = 0;
            while (!readTrack.EndOfStream)
            {
                lines[i] = readTrack.ReadLine();
                System.Console.WriteLine(lines[i]);
                i++;
            }
            readTrack.Close();
            X.Track = index;
            for (int j = 0; j < i; j++)
            {

                X.GenSource[j] = ConPoint(lines[j]); // funcation call to return the the point of the segment
                X.GenSourceC1[j] = ConC1(lines[j]);  // return the second point in the line which is the first control in the segment 
                X.GenSourceC2[j] = ConC2(lines[j]); // return the third point in the line of read file which represent the second control in the segment 
                X.GenSourceW[j] = ConW(lines[j]);// return the list value in line which is the weight 

            }
            X.GenSource[noPoints-1] = X.GenSource[0];

            readTrack.Close();
        }

        PointF ConPoint(string rowfile)
        {
            string[] sArray = rowfile.Split(',');
            PointF result = new PointF(float.Parse(sArray[0]), float.Parse(sArray[1]));
            return result;
        }
        PointF ConC1(string rowfile)
        {
            string[] sArray = rowfile.Split(',');
            PointF result = new PointF(float.Parse(sArray[2]), float.Parse(sArray[3]));
            return result;
        }
        PointF ConC2(string rowfile)
        {
            string[] sArray = rowfile.Split(',');
            PointF result = new PointF(float.Parse(sArray[4]), float.Parse(sArray[5]));
            return result;
        }
        float ConW(string rowfile)
        {
            string[] sArray = rowfile.Split(',');
            float result = float.Parse(sArray[8]);
            return result;
        }
        void merage(int crossPiont, chromosome p1, chromosome p2, chromosome ch1, chromosome ch2)
        {
            
            for (int i = 0; i < noPoints; i++)
            {
                if (i < crossPiont)
                {
                    ch1.GenSource[i] = p2.GenSource[i];
                    ch1.GenSourceC1[i] = p2.GenSourceC1[i];
                    ch1.GenSourceC2[i] = p2.GenSourceC2[i];
                    ch1.GenSourceW[i] = p2.GenSourceW[i];

                    ch2.GenSource[i] = p1.GenSource[i];
                    ch2.GenSourceC1[i] = p1.GenSourceC1[i];
                    ch2.GenSourceC2[i] = p1.GenSourceC2[i];
                    ch2.GenSourceW[i] = p1.GenSourceW[i];
                  

                }
                else
                {
                    ch1.GenSource[i] = p1.GenSource[i];
                    ch1.GenSourceC1[i] = p1.GenSourceC1[i];
                    ch1.GenSourceC2[i] = p1.GenSourceC2[i];
                    ch1.GenSourceW[i] = p1.GenSourceW[i];

                    ch2.GenSource[i] = p2.GenSource[i];
                    ch2.GenSourceC1[i] = p2.GenSourceC1[i];
                    ch2.GenSourceC2[i] = p2.GenSourceC2[i];
                    ch2.GenSourceW[i] = p2.GenSourceW[i];

                }
            }

            ch1.GenSource[noPoints-1] = ch1.GenSource[0];
            ch2.GenSource[noPoints-1] = ch2.GenSource[0];

        }

        void Mutation(chromosome MuChro)
        {
            System.Random GenNo = new System.Random();

            int Gen1 = GenNo.Next(0, noPoints);
            int Gen2 = GenNo.Next(0, noPoints);
            int trackNo = r.Next(0, PubSize);
            chromosome Xcro = new chromosome();
            GetTracks(trackNo, Xcro);
            //MessageBox.Show("(" + trackNo + ")" + Gen1+ " , " + "(" +Gen2 + ")" );
            // swap the wieght
            float Xw = MuChro.GenSourceW[Gen1];
            MuChro.GenSourceW[Gen1] = Xcro.GenSourceW[Gen2];
            Xcro.GenSourceW[Gen2] = Xw;
            // diffrenc in first gen
            float d0X = MuChro.GenSource[Gen1].X - MuChro.GenSourceC1[Gen1].X;
            float d0Y = MuChro.GenSource[Gen1].Y - MuChro.GenSourceC1[Gen1].Y;

            float J0X = MuChro.GenSource[Gen1].X - MuChro.GenSourceC2[Gen1].X;
            float J0Y = MuChro.GenSource[Gen1].Y - MuChro.GenSourceC2[Gen1].Y;
            // diffrenc in second gen
            float d1X = Xcro.GenSource[Gen2].X - Xcro.GenSourceC1[Gen2].X;
            float d1Y = Xcro.GenSource[Gen2].Y - Xcro.GenSourceC1[Gen2].Y;

            float J1X = Xcro.GenSource[Gen2].X - Xcro.GenSourceC2[Gen2].X;
            float J1Y = Xcro.GenSource[Gen2].Y - Xcro.GenSourceC2[Gen2].Y;
            // add the diffrents of the second gen to the first gen 
            MuChro.GenSourceC1[Gen1] = new(MuChro.GenSource[Gen1].X - d1X, MuChro.GenSource[Gen1].Y - d1Y);
            MuChro.GenSourceC2[Gen1] = new(MuChro.GenSource[Gen1].X - J1X, MuChro.GenSource[Gen1].Y - J1Y);
            // add the diffrents of the first gen to the second gen 
            Xcro.GenSourceC1[Gen2] = new(Xcro.GenSource[Gen2].X - d0X, Xcro.GenSource[Gen2].Y - d0Y);
            Xcro.GenSourceC2[Gen2] = new(Xcro.GenSource[Gen2].X - J0X, Xcro.GenSourceC1[Gen2].Y - J0Y);
            
        }
        void MutationClasic(chromosome MuChro)
        {
            System.Random GenNo = new System.Random();

            int Gen1 = GenNo.Next(0, noPoints-1);
           
            MuChro.getNewWeight(Gen1);

        }

        int getFitted(List<chromosome> X) // return the best track in generation 

        {

            float fittedValue = float.MaxValue;
            
            int fitted = 0;
            for (int i=0; i<PubSize; i++)
            {
                if (X[i].getFitness() < fittedValue)
                {
                    fittedValue = X[i].getFitness();
                    fitted = i;
                }
                   
            }
            return fitted;
        }

        float GetTotalfitness(List<chromosome> X)
        {
            float Total = 0;
            foreach (chromosome Xc in X)
            {
                Total = Total + Xc.getFitness();
            }
            return Total;
        }

        int GetParent()
        {
            int rP = r.Next(0, 100);
            // System.Diagnostics.Debug.WriteLine(rP + ".....is to select which way " );
            if (rP > 50)

                return Tournament();
            else
                return Biased();



        }

        int Tournament()
        {
            float tPare1 = 0; float tPare2 = 0;
            r1 = r.Next(0, PubSize - 1);
            r2 = r.Next(0, PubSize - 1);
            while (r1 == r2)
            {
                r2 = r.Next(0, PubSize - 1);
            }
            Parent1.DuplicateChromosome(r1,ChromosomesPopulation[r1]);
            Parent2.DuplicateChromosome(r2, ChromosomesPopulation[r2]);
            tPare1 = Parent1.getFitness();
            tPare2 = Parent2.getFitness();

            if (tPare1 < tPare2)
                return r1;
            else
                return r2;
        }

        int Biased()
        {
            float totalWheel = GetTotalfitness(ChromosomesPopulation);
            float sum = ChromosomesPopulation.Sum(n => n.getFitness());
            var proportions = ChromosomesPopulation.Select(n => sum/n.getFitness());
            var ProportionSum = proportions.Sum();
            var normlization = proportions.Select(p => p / ProportionSum);
            var Cumulative = new List<double>(PubSize);
            var cumulativeTotal = 0.0;
            foreach (var propor in normlization)
            {
                cumulativeTotal += propor;
                Cumulative.Add(cumulativeTotal);
            }
             var selectiveValue = r.Next(0,100);
            for (int i = 0; i < Cumulative.Count(); i++)
            {
               // System.Diagnostics.Debug.WriteLine(selectiveValue +"....."+Cumulative[i]);
                var value = Cumulative[i]*100;
                if ( selectiveValue < value )
                    return i;



                // System.Diagnostics.Debug.WriteLine("total Wheel is " + totalWheel);
            }

            throw new Exception(" error not find the value");

        }




    }
}
