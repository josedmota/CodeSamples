using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraser
{
     public class Genome
    {
        //vars
        public double[,] pt_cloud;
        //[numero,x,y,z,mutation_constants]
        public double[,] bars;
        //[numero,pt1,pt2,secçaopode ser desativado 0/1,secção,id]

        static public int pt_cnt;
        static public int bar_cnt;
        static public int towerBar_cnt;
        static private List<Int32> connection_rings;
        //needed for calc_operations class:
        public static double subd; 
        public static double horizd;

        //Constants
        //for pt mutation
        const double zero_mutation = 0;
        const double min_mutation = 0.0;
        const double med_mutation = 0.0;
        const double max_mutation = 0.0;
        const double arm_mutation = 0.0;

        //methods
        //constructor
        public Genome(double Largura,int Altura, double horiz_div,double subdiv, int N_cabos, int[] h_cabos,double[] dist_centro)
        {
            subd = subdiv;
            horizd = horiz_div;
            // init matrix dim
            pt_cloud = new double[5, (int)(17*N_cabos + 4 + (4 * subdiv) * (horiz_div - 1))]; // 17*cabos para os pts dos braços
            bars = new double[6, (int)((4 * horiz_div - 8) * (subdiv * subdiv) + (12 * horiz_div - 12) * subdiv - 8 * horiz_div + 36 * N_cabos + 20 + 4*((int)(horiz_div/3)+1)+4*((int)(horiz_div/3)))];


            pt_add_tower(ref pt_cloud, Largura, Altura,horiz_div,subdiv,ref pt_cnt);
            pt_add_arms(ref pt_cloud, Largura, Altura, horiz_div, subdiv, N_cabos, h_cabos, dist_centro, ref pt_cnt, ref connection_rings);
            bar_cnt = connect_bars(ref bars,(int)subdiv,(int)horiz_div);

            add_arm_bars(ref bars, ref bar_cnt, connection_rings, (int)subdiv, N_cabos,(int)horiz_div);
            
            towerBar_cnt = bar_cnt - 36 * N_cabos;
        }
        //2nd constructor, to use when creating a new individual
        public Genome()
        {
            pt_cloud = new double[5, pt_cnt];
            bars = new double[6, bar_cnt];

        }

        private void pt_add_tower(ref double[,] pt, double Largura, int Altura, double horiz_div, double subdiv,ref int _pt_cnt)
        {

            //tower pts
            int x = 0;
            int y = 1;
            int h = 1;
            bool reverse = false;
            int pt_num = 4; //numero do 1º pt criado em loop for

            //#######################//
            //Tilt calc + N of rings //
            //#######################//
            double ring_z_step = (Altura) / horiz_div; // ok

            double tilt;
            tilt = (Largura * 0.5) / (Altura) * ring_z_step; //ok

            

            // ADD support pts
            addPt(ref pt, 0, 0, 0, 0,0);
            addPt(ref pt, 1, Largura, 0, 0,0);
            addPt(ref pt, 2, Largura, Largura, 0,0);
            addPt(ref pt, 3, 0, Largura, 0,0);


            //##################//
            //main pt cloud loop//
            //##################//

            for (h = 1; h <= horiz_div - 1; h++)
            { // ring loop
                double new_h = (-0.04 * h * h + 1.35 * h + 0.7);
                double scale_factor = (1 - (new_h / horiz_div));
                double step = new_h * tilt;

                if (!reverse)
                { // x++ y++

                    for (x = 0; x <= subdiv; x++)
                    {

                        addPt(ref pt, pt_num, step + x * (Largura / subdiv) * scale_factor, step, new_h * ring_z_step,(double)Altura);
                        pt_num++;

                        if (x == subdiv)
                        {

                            for (y = 1; y <= subdiv; y++)
                            {
                                addPt(ref pt, pt_num, Largura - step, step + y * (Largura / subdiv) * scale_factor, new_h * ring_z_step, (double)Altura);
                                pt_num++;

                                if (x == subdiv && y == subdiv) { reverse = true; } // start backwards loop
                            }
                        }
                    }

                }

                if (reverse)
                { // x-- y--
                    for (x = (int)subdiv - 1; x >= 0; x--)
                    {
                        addPt(ref pt, pt_num, step + x * (Largura / subdiv) * scale_factor, Largura - step, new_h * ring_z_step, (double)Altura);
                        pt_num++;

                        if (x == 0)
                        {
                            for (y = (int)subdiv - 1; y >= 1; y--)
                            {
                                addPt(ref pt, pt_num, step, step + y * (Largura / subdiv) * scale_factor, new_h * ring_z_step, (double)Altura);
                                pt_num++;


                            }
                        }
                    }
                }

                reverse = false;
            }
            _pt_cnt = pt_num; // para começar a numerar correctamente os pts dos braços

        }
        
        private void pt_add_arms(ref double[,] pt, double Largura, int Altura, double horiz_div, double subdiv, int N_cabos, int[] h_cabos, double[] dist_centro, ref int _pt_cnt, ref List<Int32> connection_rings) {

            double ring_z_step = (Altura) / horiz_div; // ok

            double tilt;
            tilt = (Largura * 0.5) / (Altura) * ring_z_step; //ok

            ///////////////////////////
            //#######################//
            //######## Arms #########//
            //#######################//
            ///////////////////////////

            List<Int32> con_ring_set = new List<Int32>();

            for (int n = 0; n < (N_cabos / 2); n++)
            {   //for each set of cables

                int init_h = find_nearest(h_cabos[n], horiz_div, ring_z_step); //encontrar os horiz ring + perto
                con_ring_set.Add(init_h);

                double new_init_h = (-0.04 * init_h * init_h + 1.35 * init_h + 0.7);
                double new_init_hplusOne = (-0.04 * (init_h+1) * (init_h+1) + 1.35 * (init_h+1) + 0.7);

                double arm_lenght = Math.Abs(dist_centro[n] - (Largura * 0.5 - new_init_h * tilt));

                //lower arm angle
                double XY_m = (Largura * 0.5 - new_init_h * tilt) / arm_lenght; //inclinação da reta Y=mx em XY
                double XZ_m = (h_cabos[n] - new_init_h * ring_z_step) / arm_lenght; //inclinação da reta Z=mx em XZ

                //upper arm angle
                double XY_m_upp = (Largura * 0.5 - new_init_hplusOne * tilt) / arm_lenght;
                double XZ_m_upp = (h_cabos[n] - new_init_hplusOne * ring_z_step) / arm_lenght;


                //############//
                //#Right arm#//
                //###########//

                //1st lower arm
                for (int _subdiv = 1; _subdiv <= 5; _subdiv++)
                { // criar variavel se necessario controlo sobre o refinamento do braço

                    double _x = (Largura - new_init_h * tilt) + (arm_lenght / 5) * _subdiv;

                    if (_subdiv == 5)
                    {
                        addArmPt(ref pt, pt_cnt, _x, Largura * 0.5, h_cabos[n],zero_mutation); // cable pt
                        pt_cnt++;
                        break;
                    }
                    addArmPt(ref pt, pt_cnt, _x, new_init_h * tilt + XY_m * (_x - (Largura - new_init_h * tilt)), new_init_h * ring_z_step + XZ_m * (_x - (Largura - new_init_h * tilt)),arm_mutation); // 1st lower arm
                    pt_cnt++;
                }

                //2nd lower arm
                for (int _subdiv = 1; _subdiv < 5; _subdiv++)
                { // nao "<=" para nao criar 2 pts de convergencia
                    double _x = (Largura - new_init_h * tilt) + (arm_lenght / 5) * _subdiv;
                    addArmPt(ref pt, pt_cnt, _x, Largura - new_init_h * tilt - XY_m * (_x - (Largura - new_init_h * tilt)), new_init_h * ring_z_step + XZ_m * (_x - (Largura - new_init_h * tilt)), arm_mutation);
                    pt_cnt++;
                }

                //1st upper arm
                for (int _subdiv = 1; _subdiv < 5; _subdiv++)
                { // criar variavel se necessario controlo sobre o refinamento do braço
                    double _x = (Largura - new_init_h * tilt) + (arm_lenght / 5) * _subdiv;

                    addArmPt(ref pt, pt_cnt, _x, new_init_hplusOne * tilt + XY_m_upp * (_x - (Largura - new_init_h * tilt)), new_init_hplusOne * ring_z_step + XZ_m_upp * (_x - (Largura - new_init_h * tilt)), arm_mutation);
                    pt_cnt++;
                }
                //2nd upper arm
                for (int _subdiv = 1; _subdiv < 5; _subdiv++)
                { // nao "<=" para nao criar 2 pts de convergencia
                    double _x = (Largura - new_init_h * tilt) + (arm_lenght / 5) * _subdiv;

                    addArmPt(ref pt, pt_cnt, _x, Largura - new_init_hplusOne * tilt - XY_m_upp * (_x - (Largura - new_init_h * tilt)), new_init_hplusOne * ring_z_step + XZ_m_upp * (_x - (Largura - new_init_h * tilt)), arm_mutation);
                    pt_cnt++;
                }

                //############//
                //##Left arm##//
                //############//

                //left
                //1st lower arm
                for (int _subdiv = 1; _subdiv <= 5; _subdiv++)
                { // criar variavel se necessario controlo sobre o refinamento do braço

                    double _x = (new_init_h * tilt) - (arm_lenght / 5) * _subdiv;

                    if (_subdiv == 5)
                    {
                        addArmPt(ref pt, pt_cnt, _x, Largura * 0.5, h_cabos[n],zero_mutation); // cable pt
                        pt_cnt++;
                        break;
                    }

                    addArmPt(ref pt, pt_cnt, _x, new_init_h * tilt - XY_m * (_x - new_init_h * tilt), new_init_h * ring_z_step - XZ_m * (_x - new_init_h * tilt), arm_mutation);//1st lower arm
                    pt_cnt++;
                }

                //2nd lower arm
                for (int _subdiv = 1; _subdiv < 5; _subdiv++)
                { // nao "<=" para nao criar 2 pts de convergencia
                    double _x = (new_init_h * tilt) - (arm_lenght / 5) * _subdiv;

                    addArmPt(ref pt, pt_cnt, _x, Largura - new_init_h * tilt + XY_m * (_x - new_init_h * tilt), new_init_h * ring_z_step - XZ_m * (_x - new_init_h * tilt), arm_mutation);
                    pt_cnt++;
                }

                //1st upper arm
                for (int _subdiv = 1; _subdiv < 5; _subdiv++)
                { // criar variavel se necessario controlo sobre o refinamento do braço
                    double _x = (new_init_h * tilt) - (arm_lenght / 5) * _subdiv;

                    addArmPt(ref pt, pt_cnt, _x, new_init_hplusOne * tilt - XY_m_upp * (_x - new_init_h * tilt), new_init_hplusOne * ring_z_step - XZ_m_upp * (_x - new_init_h * tilt), arm_mutation);
                    pt_cnt++;
                }
                //2nd upper arm
                for (int _subdiv = 1; _subdiv < 5; _subdiv++)
                { // nao "<=" para nao criar 2 pts de convergencia
                    double _x = (new_init_h * tilt) - (arm_lenght / 5) * _subdiv;

                    addArmPt(ref pt, pt_cnt, _x, Largura - new_init_hplusOne * tilt + XY_m_upp * (_x - new_init_h * tilt), new_init_hplusOne * ring_z_step - XZ_m_upp * (_x - new_init_h * tilt), arm_mutation);
                    pt_cnt++;
                }
            }
            connection_rings = con_ring_set;
        }

        private int connect_bars(ref double[,] bars,int subdiv, int horiz_div)
        {
            int bar_num = 0;
            //Support pts
            for (int i = 0; i <= 4; i++)
            {

                if (i == 0)
                {
                    for (int j = 4; j <= 4 + subdiv; j++)
                    {
                        if (j == 4)
                        {
                            addBar(ref bars, bar_num, 0, j, 0, 0,0);//cantos nao sao desactivados / id = 0 leg
                            bar_num++;
                        }
                        else { 

                            addBar(ref bars, bar_num, 0, j,1,0,1); // id =1 bracing
                            bar_num++;
                        }
                    }
                    for (int j = 8 + 4 * (subdiv - 1) - 1; j >= 8 + 4 * (subdiv - 1) - subdiv; j--)
                    {
                        addBar(ref bars, bar_num, 0, j, 1, 0,1); // id =1 bracing
                        bar_num++;
                    }

                }
                if (i == 1)
                {
                    for (int j = 4; j <= 4 + 2 * subdiv; j++)
                    {
                        if (j == 4+subdiv)
                        {
                            addBar(ref bars, bar_num, 1, j, 0, 0,0);//cantos nao sao desactivados /id =0 leg
                            bar_num++;
                        }
                        else
                        {
                            addBar(ref bars, bar_num, 1, j, 1, 0,1); // id =1 bracing
                            bar_num++;
                        }
                    }
                }

                if (i == 2)
                {
                    for (int j = 4 + subdiv; j <= 4 + 3 * subdiv; j++)
                    {
                        if (j == 4 + 2 * subdiv)
                        {
                            addBar(ref bars, bar_num, 2, j, 0, 0,0);//cantos nao sao desactivados / id=0 leg
                            bar_num++;
                        }else
                        {
                            addBar(ref bars, bar_num, 2, j, 1, 0,1); // id =1 bracing
                            bar_num++;
                        }
                    }
                }
                    
                if (i == 3)
                {
                    for (int j = 4 + subdiv * 2; j <= 4 + 4 * subdiv - 1; j++)
                    {
                        if (j == 4 + 3 * subdiv)
                        {
                            addBar(ref bars, bar_num, 3, j, 0, 0,0);//cantos nao sao desactivados / id=0 leg
                            bar_num++;
                        }else
                        {
                            addBar(ref bars, bar_num, 3, j, 1, 0,1); // id = 1 bracing
                            bar_num++;
                        }
                    }
                    addBar(ref bars, bar_num, 3, 4, 1, 0,1); // id =1 bracing
                    bar_num++;
                }
            }

            //###############//
            //Main tower bars//
            //###############//

            //add Lcr nesta fase !!!
            int ring_pt = 4 + (subdiv - 1) * 4;
            //init and end pts
            for (int h = 0; h <= horiz_div - 3; h++)
            {
                // Lado +XX
                for (int i = 4 + ring_pt * h; i < 4 + ring_pt * (h + 1); i++)
                {//init coord.
                 //lados 
                    if (i == 4 + ring_pt * h)
                    {//cantos
                        for (int j = 4 + ring_pt * (h + 1); j <= subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                        { // -1 para nao conectar a diagonal oposta
                            if (j  == 4 + ring_pt * (h + 1))
                            {
                                addBar(ref bars, bar_num, i, j, 0, 0,0); // barras dos cantos nao podem ser desactivadas / id=0 leg
                                bar_num++;
                            }else
                            {
                                addBar(ref bars, bar_num, i, j, 1, 0,1);  // id = 1 bracing
                                bar_num++;
                            }
                        }
                    }
                    else if (i == 4 + ring_pt * h + subdiv)
                    {
                        for (int j = 4 + ring_pt * (h + 1) + 1; j <= subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                        { //+1 para nao conectar a diagonal oposta
                            addBar(ref bars, bar_num, i, j, 1, 0,1); //quando i = j-ring_pt  id bar as active(always)  // id =1 bracing
                            bar_num++; 
                        }
                    }
                    else
                    { //pts centrais
                        if (i > 4 + ring_pt * h && i < 4 + ring_pt * h + subdiv)
                        {
                            for (int j = 4 + ring_pt * (h + 1); j <= subdiv + 4 + ring_pt * (h + 1); j++)
                            {
                                addBar(ref bars, bar_num, i, j,1,0,1); // id =1 bracing
                                bar_num++;
                            }
                        }
                    }
                }
                 // Lado +YY
                for (int i = 4 + subdiv + ring_pt * h; i < 4 + subdiv + ring_pt * (h + 1); i++)
                {//init coord.

                    //lados
                    if (i == 4 + subdiv + ring_pt * h)
                    {//cantos
                        for (int j = 4 + subdiv + ring_pt * (h + 1); j <= 2 * subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                        {
                            if (j == subdiv + 4 + ring_pt * (h + 1))
                            {
                                addBar(ref bars, bar_num, i, j, 0, 0,0); // barras de canto nao podem ser desactivadas / id=0 leg
                                bar_num++;
                            }else
                            {
                                addBar(ref bars, bar_num, i, j, 1, 0,1); //id = 1 bracing
                                bar_num++;
                            }
                        }
                    }
                    else if (i == 4 + ring_pt * h + 2 * subdiv)
                    {
                        for (int j = 4 + subdiv + ring_pt * (h + 1) + 1; j <= 2 * subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                        {
                            addBar(ref bars, bar_num, i, j,1,0,1); //id = 1 bracing
                            bar_num++;
                        }

                    }
                    else
                    { //pts centrais
                        if (i > 4 + subdiv + ring_pt * h && i < 4 + ring_pt * h + 2 * subdiv)
                        {
                            for (int j = 4 + subdiv + ring_pt * (h + 1); j <= 2 * subdiv + 4 + ring_pt * (h + 1); j++)
                            {
                                addBar(ref bars, bar_num, i, j, 1, 0, 1); //id = 1 bracing
                                bar_num++;
                            }
                        }
                    }
                }

                // Lado -XX
                for (int i = 4 + 2 * subdiv + ring_pt * h; i < 4 + 2 * subdiv + ring_pt * (h + 1); i++)
                {//init coord.
                 //lados
                    if (i == 4 + 2 * subdiv + ring_pt * h)
                    {//cantos
                        for (int j = 4 + 2 * subdiv + ring_pt * (h + 1); j <= 3 * subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                        {
                            if (j == 2*subdiv + 4 + ring_pt * (h + 1))
                            {
                                addBar(ref bars, bar_num, i, j, 0, 0,0);// barras de canto nao podem ser desactivadas /id=0 leg
                                bar_num++;
                            }
                            else
                            {
                                addBar(ref bars, bar_num, i, j, 1, 0,1);  //id = 1 bracing
                                bar_num++;
                            }
                        }
                    }
                    else if (i == 4 + ring_pt * h + 3 * subdiv)
                    {
                        for (int j = 4 + 2 * subdiv + ring_pt * (h + 1) + 1; j <= 3 * subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                        {
                            addBar(ref bars, bar_num, i, j,1,0,1); //id = 1 bracing
                            bar_num++;
                        }
                    }
                    else
                    { //pts centrais
                        if (i > 4 + 2 * subdiv + ring_pt * h && i < 4 + ring_pt * h + 3 * subdiv)
                        {
                            for (int j = 4 + 2 * subdiv + ring_pt * (h + 1); j <= 3 * subdiv + 4 + ring_pt * (h + 1); j++)
                            {
                                addBar(ref bars, bar_num, i, j,1,0,1); //id = 1 bracing
                                bar_num++;
                            }
                        }
                    }
                }

                // Lado -YY
                for (int i = 4 + 3 * subdiv + ring_pt * h; i < 4 + 3 * subdiv + ring_pt * (h + 1); i++)
                {//init coord.
                 //lados
                    if (i == 4 + 3 * subdiv + ring_pt * h)
                    {//cantos
                        for (int j = 4 + 3 * subdiv + ring_pt * (h + 1); j <= 4 * subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                        {
                            if (j == 3*subdiv + 4 + ring_pt * (h + 1))
                            {
                                addBar(ref bars, bar_num, i, j, 0, 0,0);// barras de canto nao podem ser desactivadas /id=0 leg
                                bar_num++;
                            }
                            else
                            {
                                addBar(ref bars, bar_num, i, j, 1, 0,1); //id = 1 bracing
                                bar_num++;
                            }

                            if (j != 4 + 3 * subdiv + ring_pt * (h + 1))
                            { //exceto lado oposto
                                addBar(ref bars, bar_num, 4+ring_pt*h, j,1,0,1); //id = 1 bracing
                                bar_num++;
                            } //connect first corner w/ side -YY

                        }
                    }
                    else
                    { //pts centrais
                        if (i > 4 + 3 * subdiv + ring_pt * h && i < 4 + ring_pt * h + 4 * subdiv)
                        {
                            for (int j = 4 + 3 * subdiv + ring_pt * (h + 1); j <= 4 * subdiv + 4 + ring_pt * (h + 1) - 1; j++)
                            {
                                addBar(ref bars, bar_num, i, j,1,0,1); //id = 1 bracing
                                bar_num++;
                            }
                            addBar(ref bars, bar_num, i, 4+ring_pt*(h+1),1,0,1); //conect to init pt  //id = 1 bracing
                            bar_num++;
                        }
                    }
                }

            }

            // Horizontal connections

            /*for (int h = 0; h <= horiz_div - 2; h++)
            {
                for (int i = 4 + h * ring_pt; i < 4 + (h + 1) * ring_pt - 1; i++)
                {
                    addBar(ref bars, bar_num, i, i+1,1,0,2); //id = 2 horiz bracing
                    bar_num++;
                    if (i == 4 + (h + 1) * ring_pt - 2)
                    {
                        addBar(ref bars, bar_num, i+1, 4+h*ring_pt,1,0,2);  //id = 2 horiz bracing
                        bar_num++;
                    }
                }
            }*/

            for (int h = 0; h <= horiz_div - 2; h++)
            {
                for (int i = 4 + h * ring_pt; i < 4 + (h + 1) * ring_pt - 1; i++)
                {
                    //barras que nao podem desaparecer mas podem ter sec min
                    if (i == (4 + h * ring_pt) || i == (4 + h * ring_pt + subdiv) || i == 4 + h * ring_pt + 2 * subdiv || i == 4 + h * ring_pt + 3 * subdiv)
                    {
                        addBar(ref bars, bar_num, i, i + 1, 0, 0, 2);
                        bar_num++;
                    }
                    else if (i + 1 == (4 + h * ring_pt) || i + 1 == (4 + h * ring_pt + subdiv) || i + 1 == 4 + h * ring_pt + 2 * subdiv || i + 1 == 4 + h * ring_pt + 3 * subdiv)
                    {
                        addBar(ref bars, bar_num, i, i + 1, 0, 0, 2);
                        bar_num++;
                    }
                    else
                    {
                        //todas as barras que podem ser otimizadas e desaparecer
                        addBar(ref bars, bar_num, i, i + 1, 1, 0, 2);
                        bar_num++;
                    }

                    if (i == 4 + (h + 1) * ring_pt - 2/*     4 + (h + 1) * ring_pt - 2*/)
                    {
                        addBar(ref bars, bar_num, i+1, 4+h*ring_pt, 0, 0, 2);
                        bar_num++;
                        Console.WriteLine(bar_num);
                    }
                }
            }

            ///horiz tapered edges
            ///
            for (int h = 0; h <= (int)horiz_div/3; h++)
            { //percorrer altura
                for (int i = 4 + h * ring_pt + 1; i < 4 + (h + 1) * ring_pt - 1; i++)
                {
                    // barras que nao podem desaparecer
                    if (i == 4 + h * ring_pt + 1)
                    {
                        addBar(ref bars, bar_num, i, 5 + (h + 1) * ring_pt - 2, 0, -1, 5);
                        bar_num++;
                    }
                    if (i == 4 + h * ring_pt + subdiv - 1)
                    {
                        addBar(ref bars, bar_num, i, i + 2, 0, -1, 5);
                        bar_num++;
                    }
                    if (i == 4 + h * ring_pt + 2 * subdiv - 1)
                    {
                        addBar(ref bars, bar_num, i, i + 2, 0, -1, 5);
                        bar_num++;
                    }
                    if (i == 4 + h * ring_pt + 3 * subdiv - 1)
                    {
                        addBar(ref bars, bar_num, i, i + 2, 0, -1, 5);
                        bar_num++;
                    }
                }
            }

            ///plane to plane triangulation
            ///
            for (int h = 0; h < (int)horiz_div/3 /*- 3*/; h++)
            {
                addBar(ref bars, bar_num, 5 + ring_pt * h, 3 + (h + 2) * ring_pt, 0, -1, 5);
                bar_num++;
                addBar(ref bars, bar_num, 3 + subdiv + ring_pt * h, 3 + (subdiv + 2) + ring_pt * (h + 1), 0, -1, 5);
                bar_num++;
                addBar(ref bars, bar_num, 5 + 2 * subdiv + ring_pt * h, 3 + 2 * subdiv + ring_pt * (h + 1), 0, -1, 5);
                bar_num++;
                addBar(ref bars, bar_num, 3 + 3 * subdiv + ring_pt * h, 5 + 3 * subdiv + (h + 1) * ring_pt, 0, -1, 5);
                bar_num++;
            }

            return bar_num;
        }
        private void add_arm_bars(ref double[,] bars, ref int bar_num, List<Int32> connection_rings, int subdiv, int n_cabos, int horiz_div)
        {
            int last_tower_pt = 4+(4*subdiv)*(horiz_div-1);
            //a*34 é o incremento para mudar de conjunto de braços
            for (int a = 0; a < n_cabos / 2; a++)
            {
                for (int i = 0; i <= 2; i++)
                {
                    if (i == 0)
                    {
                        // connect w/ tower

                        //Lower right
                        addBar(ref bars, bar_num, 4 + (4 + (subdiv - 1) * 4) * (connection_rings[0 + a] - 1) + subdiv, last_tower_pt + i + a * 34, 0, -1,4);
                        bar_num++;
                        addBar(ref bars, bar_num, 4 + (4 + (subdiv - 1) * 4) * (connection_rings[0 + a] - 1)+2 * subdiv, last_tower_pt + 5 + a * 34, 0, -1, 4);
                        bar_num++;
                        //Upper right
                        addBar(ref bars, bar_num, 4 + (4 + (subdiv - 1) * 4) * (connection_rings[0 + a]) + subdiv, last_tower_pt + 9 + a * 34, 0, -1, 4);
                        bar_num++;
                        addBar(ref bars, bar_num, 4 + (4 + (subdiv - 1) * 4) * (connection_rings[0 + a]) + 2 * subdiv, last_tower_pt + i + 13 + a * 34, 0, -1, 4);
                        bar_num++;

                        //Lower left
                        addBar(ref bars, bar_num, 4 + (4 + (subdiv - 1) * 4) * (connection_rings[0 + a] - 1), last_tower_pt + i + 17 + a * 34, 0, -1, 4);
                        bar_num++;
                        addBar(ref bars, bar_num, 4 + (4 + (subdiv - 1) * 4) * (connection_rings[0 + a] - 1) + 3 * subdiv, last_tower_pt + i + 22 + a * 34, 0, -1, 4);
                        bar_num++;
                        //Upper left
                        addBar(ref bars, bar_num, 4 + (4 + (subdiv - 1) * 4) * (connection_rings[0 + a]), last_tower_pt + i + 26 + a * 34, 0, -1, 4);
                        bar_num++;
                        addBar(ref bars, bar_num, 4 + (4 + (subdiv - 1) * 4) * (connection_rings[0 + a]) + 3 * subdiv, last_tower_pt + i + 30 + a * 34, 0, -1, 4);
                        bar_num++;


                        //Diagonals
                        //1st plane right
                        addBar(ref bars, bar_num, 4 + (4 + (subdiv - 1) * 4) * (connection_rings[0 + a] - 1) + subdiv, last_tower_pt + 9 + a * 34, 0, -1, 4);
                        bar_num++;
                        //1st plane left
                        addBar(ref bars, bar_num, 4 + (4 + (subdiv - 1) * 4) * (connection_rings[0 + a] - 1), last_tower_pt + 26 + a * 34, 0, -1, 4);
                        bar_num++;

                        //2nd plane right
                        addBar(ref bars, bar_num, 4 + (4 + (subdiv - 1) * 4) * (connection_rings[0 + a] - 1) + 2 * subdiv, last_tower_pt + 13 + a * 34, 0, -1, 4);
                        bar_num++;
                        //2nd plane left
                        addBar(ref bars, bar_num, 4 + (4 + (subdiv - 1) * 4) * (connection_rings[0 + a] - 1) + 3 * subdiv, last_tower_pt + 30 + a * 34, 0, -1, 4);
                        bar_num++;

                    }

                    //diagonals 1st plane right
                    addBar(ref bars, bar_num, last_tower_pt+i+a*34, last_tower_pt + 10+i + a * 34, 0, -1, 4);
                    bar_num++;
                    addBar(ref bars, bar_num, last_tower_pt + i + a * 34, last_tower_pt + 9 + i + a * 34, 0, -1, 4);
                    bar_num++;
                    //diagonals 1st plane left
                    addBar(ref bars, bar_num, last_tower_pt + 17 + i + a * 34, last_tower_pt + 27 + i + a * 34, 0, -1, 4);
                    bar_num++;
                    addBar(ref bars, bar_num, last_tower_pt + 17 + i + a * 34, last_tower_pt + 26 + i + a * 34, 0, -1, 4);
                    bar_num++;

                    //diagonals 2nd plane right
                    addBar(ref bars, bar_num, last_tower_pt + 5 + i + a * 34, last_tower_pt + 14 + i + a * 34, 0, -1, 4);
                    bar_num++;
                    addBar(ref bars, bar_num, last_tower_pt + 5 + i + a * 34, last_tower_pt + 13 + i + a * 34, 0, -1, 4);
                    bar_num++;

                    //diagonals 2nd plane left
                    addBar(ref bars, bar_num, last_tower_pt + 22 + i + a * 34, last_tower_pt + 31 + i + a * 34, 0, -1, 4);
                    bar_num++;
                    addBar(ref bars, bar_num, last_tower_pt + 22 + i + a * 34, last_tower_pt + 30 + i + a * 34, 0, -1, 4);
                    bar_num++;

                    //1st lower chord
                    //right
                    addBar(ref bars, bar_num, last_tower_pt + i + a * 34, last_tower_pt + 1 + i + a * 34, 0, -1, 4);
                    bar_num++;
                    //left
                    addBar(ref bars, bar_num, last_tower_pt + 17 + i + a * 34, last_tower_pt + 18 + i + a * 34, 0, -1, 4);
                    bar_num++;

                    //2nd lower chord
                    //right
                    addBar(ref bars, bar_num, last_tower_pt + 5 + i + a * 34, last_tower_pt + 6 + i + a * 34, 0, -1, 4);
                    bar_num++;
                    //left
                    addBar(ref bars, bar_num, last_tower_pt + 22 + i + a * 34, last_tower_pt + 23 + i + a * 34, 0, -1, 4);
                    bar_num++;

                    //1st upper chord
                    //right
                    addBar(ref bars, bar_num, last_tower_pt + 9 + i + a * 34, last_tower_pt + 10 + i + a * 34, 0, -1, 4);
                    bar_num++;
                    //left
                    addBar(ref bars, bar_num, last_tower_pt + 26 + i + a * 34, last_tower_pt + 27 + i + a * 34, 0, -1, 4);
                    bar_num++;

                    //2nd upper chord

                    addBar(ref bars, bar_num, last_tower_pt + 13 + i + a * 34, last_tower_pt + 14 + i + a * 34, 0, -1, 4);
                    bar_num++;
                    addBar(ref bars, bar_num, last_tower_pt + 30 + i + a * 34, last_tower_pt + 31 + i + a * 34, 0, -1, 4);
                    bar_num++;

                    //end lines
                    if (i == 2)
                    {
                        //1nd lower chord
                        //right
                        addBar(ref bars, bar_num, last_tower_pt + 1 + i + a * 34, last_tower_pt + 2 + i + a * 34, 0, -1, 4);
                        bar_num++;
                        //left
                        addBar(ref bars, bar_num, last_tower_pt + 18 + i + a * 34, last_tower_pt + 19 + i + a * 34, 0, -1, 4);
                        bar_num++;

                        //2nd lower chord
                        //right
                        addBar(ref bars, bar_num, last_tower_pt + 6 + i + a * 34, last_tower_pt + 4 + a * 34, 0, -1, 4);
                        bar_num++;
                        //left
                        addBar(ref bars, bar_num, last_tower_pt + 23 + i + a * 34, last_tower_pt + 21 + a * 34, 0, -1, 4);
                        bar_num++;

                        //1st upper chord
                        //right
                        addBar(ref bars, bar_num, last_tower_pt + 10 + i + a * 34, last_tower_pt + 4 + a * 34, 0, -1, 4);
                        bar_num++;
                        //left
                        addBar(ref bars, bar_num, last_tower_pt + 27 + i + a * 34, last_tower_pt + 21 + a * 34, 0, -1, 4);
                        bar_num++;

                        //2nd upper chord
                        //right
                        addBar(ref bars, bar_num, last_tower_pt + 14 + i + a * 34, last_tower_pt + 4 + a * 34, 0, -1, 4);
                        bar_num++;
                        //left
                        addBar(ref bars, bar_num, last_tower_pt + 31 + i + a * 34, last_tower_pt + 21 + a * 34, 0, -1, 4);
                        bar_num++;

                        //Diagonals
                        //1st plane right
                        addBar(ref bars, bar_num, last_tower_pt + 1 + i + a * 34, last_tower_pt + 10 + i + a * 34, 0, -1, 4);
                        bar_num++;
                        //1st plane left
                        addBar(ref bars, bar_num, last_tower_pt + 18 + i + a * 34, last_tower_pt + 27 + i + a * 34, 0, -1, 4);
                        bar_num++;
                        //2nd plane right
                        addBar(ref bars, bar_num, last_tower_pt + 6 + i + a * 34, last_tower_pt + 14 + i + a * 34, 0, -1, 4);
                        bar_num++;
                        //2nd plane left
                        addBar(ref bars, bar_num, last_tower_pt + 23 + i + a * 34, last_tower_pt + 31 + i + a * 34, 0, -1, 4);
                        bar_num++;
                    }
                }
            }
        }
        private int find_nearest(int altura, double horiz_div, double ring_z_step)
        {
            int nearest = -1;
            double dist = 10000;

            for (int h = 1; h <= horiz_div - 2; h++)
            {
                if (dist > Math.Abs(altura - (-0.04 * h * h + 1.35 * h + 0.7) * ring_z_step))
                {
                    dist = Math.Abs(altura - (-0.04 * h * h + 1.35 * h + 0.7) * ring_z_step);
                    nearest = h;
                }
            }

            return nearest;
        }

        //auxiliary functios for faster pt and bar add

        private void addPt(ref double[,] matrix ,int pt_number, double x, double y, double z,double h_max){
            double relative_h = z / h_max;

                matrix[0, pt_number] = pt_number;
                matrix[1, pt_number] = x;
                matrix[2, pt_number] = y;
                matrix[3, pt_number] = z;

            // mutation needs to be scaled as the same amount of mutation will distort the structure more at the top 
            if (relative_h < 0.33) { matrix[4, pt_number] = max_mutation; }
            if (relative_h > 0.33 && relative_h<0.66 ) { matrix[4, pt_number] = med_mutation; }
            if (relative_h > 0.66) { matrix[4, pt_number] = min_mutation; }
            //call method with h_max =0 to stop mutation
            if(h_max == 0) { matrix[4, pt_number] = zero_mutation; }

        }
        private void addArmPt(ref double[,] matrix, int pt_number, double x, double y, double z, double mutation_scale)
        {
            matrix[0, pt_number] = pt_number;
            matrix[1, pt_number] = x;
            matrix[2, pt_number] = y;
            matrix[3, pt_number] = z;
            matrix[4, pt_number] = mutation_scale;
            

        }
        private void addBar(ref double[,] bar, int numb,int start,int end, int can_deact, int section, int id)
        {
            bar[0, numb] = numb;
            bar[1, numb] = start;
            bar[2, numb] = end;
            //add here more options as needed (active,section,Lcr etc etc)
            bar[3, numb] = can_deact;
            bar[4, numb] = section;
            bar[5, numb] = id; // 0 = leg / 1= bracing /  2 = horizontal bar / 3 = plane bracing (triagulated or not) / 4 = arms (not needed for stability checks but need id)
        }

    }

    
}
