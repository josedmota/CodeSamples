using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraser
{
    class Calc_operations
    {
        int start_bar;
        List<Int32> next_bars;
        int section_id;
        int bar_id;
        const double Young = 355000.0;

        public Calc_operations(int start,List<Int32> barlist,int s_id,int b_id)
        {
            this.start_bar = start;
            this.next_bars = barlist;
            this.section_id = s_id;
            this.bar_id = b_id;
        }

        ///EC3 CALCS///
        ///
        ///
        
        public static void EC3_Checks(int id, ref List<double[]> repair_instructions, List<Calc_operations> calc_ops,double[,] results)
        {

            ///     Leg Calcs         ///
            /// BS EN 1993-1-1:2005   /// 
            /// BS EN 1993-3-1.2006   ///
            ///######################///
            if (id == 0) { 
                for (int i = 0; i < calc_ops.Count; i++)
                {
                    double Nsd = 0;
                    double L = 0;

                    // get max N of every bar //
                    for (int b = 0; b < calc_ops[i].next_bars.Count; b++)
                    {
                        if (Math.Abs(results[2, calc_ops[i].next_bars[b] - 1]) > Math.Abs(Nsd)) { Nsd = results[2, calc_ops[i].next_bars[b] - 1]; }
                    }
                    // get total L for buckling //
                    for (int b = 0; b < calc_ops[i].next_bars.Count; b++)
                    {
                        L += results[1, calc_ops[i].next_bars[b] - 1];
                    }

                    if (Nsd > 0)//tension
                    {
                        double u_f = 0;
                        double Nu_rd = Sections.Area[calc_ops[i].section_id] * 275000; //Aeff * fy
                        u_f = Nsd / Nu_rd;

                        for (int y = 0; y < calc_ops[i].next_bars.Count; y++)
                        {
                            repair_instructions.Add(new double[] { calc_ops[i].next_bars[y], Math.Abs(u_f) }); // add to repair list (each individual bar)
                        }
                    }
                    else { //compression

                        //resistence check
                        double u_f = 0;
                        double Nc_rd = Sections.Area[calc_ops[i].section_id] * 275000;
                        u_f = Nsd / Nc_rd;

                        //buckling check
                        double lambda = L / Sections.ivv[calc_ops[i].section_id]; // L/ivv
                        double _lambda = lambda / (93.9 * Math.Sqrt(235.0 / 275.0));
                        double k = 0.8 + (_lambda / 10.0);

                        if (k > 1) { k = 1; }
                        if (k < 0.9) { k = 0.9; }

                        double _lambda_eff = k * _lambda;
                        double fi = 0.5 * (1 + 0.34 * (_lambda - 0.2) + _lambda * _lambda);
                        double xi = 1.0 / (fi + Math.Sqrt(fi * fi - _lambda * _lambda));
                        double Nb_rd = xi * Sections.Area[calc_ops[i].section_id] * 275000;
                        double b_uf = Nsd / Nb_rd;

                        
                        u_f = Math.Max(Math.Abs(u_f), Math.Abs(b_uf));
                        for (int y = 0; y < calc_ops[i].next_bars.Count; y++)
                        {
                            repair_instructions.Add(new double[] { calc_ops[i].next_bars[y], Math.Abs(u_f) }); // add to repair list (each individual bar) no valor do robot (+1)
                        }
                    }
                }
           }

            ///    Bracing Calcs
            /// BS EN 1993-1-1:2005   /// 
            /// BS EN 1993-3-1.2006   ///
            ///######################///
            if (id == 1)
            {
                for (int i = 0; i < calc_ops.Count; i++)// run through all bracing
                {
                    double Nsd = results[2, calc_ops[i].next_bars[0] - 1];
                    double L = results[1, calc_ops[i].next_bars[0] - 1];

                    if (Nsd > 0) // tension
                    {
                        double u_f = 0;
                        double Nu_rd = Sections.Area[calc_ops[i].section_id] * 275000; //Aeff * fy
                        u_f = Nsd / Nu_rd;
                        for (int y = 0; y < calc_ops[i].next_bars.Count; y++)
                        {
                            repair_instructions.Add(new double[] { calc_ops[i].next_bars[y], Math.Abs(u_f) }); // add to repair list (each individual bar)
                        }
                    }
                    else // compression
                    {
                        //resistence check
                        double u_f = 0;
                        double Nc_rd = Sections.Area[calc_ops[i].section_id] * 275000;
                        u_f = Nsd / Nc_rd;

                        //buckling check
                        double lambda = L / Sections.ivv[calc_ops[i].section_id]; // L/ivv
                        double _lambda = lambda / (93.9 * Math.Sqrt(235.0 / 275.0));
                        double k = 0.7 + (0.35 / _lambda);
                        
                        /*if (k > 1) { k = 1; }
                        if (k < 0.9) { k = 0.9; }*/    // nao ha limite para o K ???

                        double _lambda_eff = k * _lambda;
                        double fi = 0.5 * (1 + 0.34 * (_lambda - 0.2) + _lambda * _lambda);
                        double xi = 1 / (fi + Math.Sqrt(fi * fi - _lambda * _lambda));
                        double Nb_rd = xi * Sections.Area[calc_ops[i].section_id] * 275000;
                        double b_uf = Nsd / Nb_rd;

                        u_f = Math.Max(Math.Abs(u_f), Math.Abs(b_uf));

                        for (int y = 0; y < calc_ops[i].next_bars.Count; y++)
                        {
                            repair_instructions.Add(new double[] { calc_ops[i].next_bars[y], Math.Abs(u_f) }); // add to repair list (each individual bar)
                        }

                    }
                }
            }

            ///    Horiz bars in Plane buckling
            /// BS EN 1993-1-1:2005   /// 
            /// BS EN 1993-3-1.2006   ///
            ///######################///
            if (id == 2) // horiz member in plane buckling
            {
                for (int i = 0; i < calc_ops.Count; i++)
                {
                    double Nsd = 0;
                    double L = 0;

                    // get max N of every bar //
                    for (int b = 0; b < calc_ops[i].next_bars.Count; b++)
                    {
                        if (Math.Abs(results[2, calc_ops[i].next_bars[b] - 1]) > Nsd) { Nsd = results[2, calc_ops[i].next_bars[b] - 1]; }
                    }
                    // get total L for buckling //
                    for (int b = 0; b < calc_ops[i].next_bars.Count; b++)
                    {
                        L += results[1, calc_ops[i].next_bars[b] - 1];
                    }

                    if (Nsd > 0) //tension
                    {
                        double u_f = 0;
                        double Nu_rd = Sections.Area[calc_ops[i].section_id] * 275000; //Aeff * fy
                        u_f = Nsd / Nu_rd;
                        for (int y = 0; y < calc_ops[i].next_bars.Count; y++)
                        {
                            repair_instructions.Add(new double[] { calc_ops[i].next_bars[y], Math.Abs(u_f) }); // add to repair list (each individual bar)
                        }
                    }
                    else
                    {
                        //resistence check
                        double u_f = 0;
                        double Nc_rd = Sections.Area[calc_ops[i].section_id] * 275000;
                        u_f = Nsd / Nc_rd;

                        //buckling check
                        double lambda = L / Sections.ivv[calc_ops[i].section_id]; // L/ivv
                        double _lambda = lambda / (93.9 * Math.Sqrt(235.0 / 275.0));
                        //previous
                        // double k = 0.8 + (_lambda / 10.0);           
                        //if (k > 1) { k = 1; }
                        //if (k < 0.9) { k = 0.9; }
                        //end
                        double k = 0.7 + (0.35 / _lambda); //table g.2 ivv
                        double _lambda_eff = k * _lambda;
                        double fi = 0.5 * (1 + 0.34 * (_lambda - 0.2) + _lambda * _lambda);
                        double xi = 1.0 / (fi + Math.Sqrt(fi * fi - _lambda * _lambda));
                        double Nb_rd = xi * Sections.Area[calc_ops[i].section_id] * 275000;
                        double b_uf = Nsd / Nb_rd;

                        
                        u_f = Math.Max(Math.Abs(u_f), Math.Abs(b_uf));
                        for (int y = 0; y < calc_ops[i].next_bars.Count; y++)
                        {
                            repair_instructions.Add(new double[] { calc_ops[i].next_bars[y], Math.Abs(u_f) }); // add to repair list (each individual bar)
                        }

                    }
                }
            }


        }

    }

}
