using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RobotOM;

namespace Fraser
{
    public partial class Form1 : Form
    {
        private Population LastPop;
        static private Population CurrentPop;
        private Population NextPop;
        private Genome BaseDNA;
        private int _individual = 0;
        
        public Form1()
        {
            InitializeComponent();
            Robot_call.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void subdiv_int_ValueChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


        private void geom_tab_Click(object sender, EventArgs e)
        {

        }

        private void Start_geom_Click(object sender, EventArgs e)
        {
           //robApp = new RobotApplication();
           // if (robApp.Visible == 0) { robApp.Interactive = 1;robApp.Visible = 1; }
            
            //create []hcabos
            int[] h_cabos = new int[4] {(int)h_cabo1_int.Value, (int)h_cabo2_int.Value, (int)h_cabo3_int.Value, (int)h_cabo4_int.Value };
            //create []dist_centro
            double[] dist_centro = new double[4] { (double)w_cabo1_int.Value, (double)w_cabo2_int.Value, (double)w_cabo3_int.Value,(double)w_cabo4_int.Value };

            if( BaseDNA != null || CurrentPop != null) { BaseDNA = null; CurrentPop = null; }

            BaseDNA = new Fraser.Genome((double)Largura_ap_int.Value,(int)Altura_int.Value,(double)h_div_int.Value,(double)subdiv_int.Value,(int)n_cabos_int.Value,h_cabos,dist_centro);
            
             //display the init structure (for user confirmation + add forces)
            Robot_call.Start_pts(BaseDNA);
            Robot_call.Start_bars(BaseDNA);
            Robot_call.Refresh();

            // create initial population
            NextPop = new Population((int)Population_cnt.Value, BaseDNA);
        }

        private void draw_Click(object sender, EventArgs e)
        {
            int c = 0;
            this.Chart.Series.Clear();
            this.Chart.Titles.Add("Fitness");
            System.Windows.Forms.DataVisualization.Charting.Series series = this.Chart.Series.Add("fitness");
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;

            double best = 0.0;
            int count = 0;
            CurrentPop = new Population(NextPop.ind);
            for (int i=0; i<928; i++)
            {
                Generation.Text = i.ToString();
                // CurrentPop = new Population(NextPop.ind);
                if (i == 0)
                {
                    for (int a = 0; a < Population.Pop_size; a++)
                    {
                        CurrentPop.ind[a].Evaluate();
                        c++;
                        series.Points.AddXY(c, CurrentPop.ind[a].fitness);
                        //Robot_call.Robot_interactive(true);

                    } best = CurrentPop.ind[4].fitness;
                }else { /*CurrentPop.ind[0].Evaluate();*/
                    c++;
                    series.Points.AddXY(c, CurrentPop.ind[0].fitness);
                }

                Array.Sort(CurrentPop.ind);

                if(best == CurrentPop.ind[Population.Pop_size-1].fitness) { count++; }else { best = CurrentPop.ind[Population.Pop_size - 1].fitness; count = 0; }
                if(count > 200) { break; }

                Individual temp = Population.Evolve_single(CurrentPop.ind, i);
                temp.Evaluate();

                if (temp.fitness < CurrentPop.ind[0].fitness) { CurrentPop.ind[0] = temp; } else { CurrentPop.ind[0] = temp; }
                //CurrentPop.ind[0] = temp; //Population.Evolve_single(CurrentPop.ind, i).Evaluate();
                //Robot_call.Robot_interactive(true);
                //Robot_call.Refresh();

                //LastPop = new Population(CurrentPop.ind);
                //NextPop = new Population(Population.Evolve(CurrentPop.ind,i));
                
            }
            // CurrentPop.ind[Population.Pop_size - 1].Evaluate();

            Robot_call.Update_bars(CurrentPop.ind[Population.Pop_size-1]._DNA);
            Robot_call.Update_pts(CurrentPop.ind[Population.Pop_size - 1]._DNA);
            Robot_call.Robot_interactive(true);
            Robot_call.Refresh();
        }

        private void btn_get_sec_Click(object sender, EventArgs e)
        { 
            new Sections(sec_name.Text, (double)sec_area.Value , (double)sec_iz.Value, (double)sec_iy.Value, (double)sec_iv.Value);
            Robot_call.Set_sections(sec_name.Text,(double)sec_area.Value,(double)sec_iz.Value,(double)sec_iy.Value);

            // add to the matrix the area of each section in the get_sections
            Sec_list.Items.Add(sec_name.Text);
            Sec_list.Update();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void sort_Click(object sender, EventArgs e)
        {
            //Array.Sort(CurrentPop.ind); // works
            //Console.Write(Population.Select(CurrentPop.ind));
            Robot_call.Update_bars(CurrentPop.ind[Population.Pop_size - 1 - _individual]._DNA);
            Robot_call.Update_pts(CurrentPop.ind[Population.Pop_size - 1 - _individual]._DNA);
            Robot_call.Robot_interactive(true);
            Robot_call.Refresh();
            _individual++;

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void Sec_list_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
