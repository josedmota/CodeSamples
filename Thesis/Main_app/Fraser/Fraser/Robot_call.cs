using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotOM;

namespace Fraser
{
    static class Robot_call
    {
        static IRobotApplication robApp;

        public static Sections sec_prop;

        static IRobotBarForceServer results;

        static int instances = 0;

        static Robot_call()
        {
            if (robApp == null)
            {
                robApp = new RobotApplication();
       
                robApp.Project.New(IRobotProjectType.I_PT_FRAME_3D);
                if (robApp.Visible == 0) { robApp.Interactive = 1; robApp.Visible = 1; }
                instances = 1;
            }
            

        }
        public static void Robot_interactive(bool a)
        {
            if (a) {
                robApp.Interactive = 1;
            } else {
                robApp.Interactive = 0;
            }
        }
        public static void Start()
        {
            // define section Db
            robApp.Project.Preferences.SetCurrentDatabase(IRobotDatabaseType.I_DT_SECTIONS, "DIN");
            //define materials Db
            robApp.Project.Preferences.Materials.Load("Eurocode");
            //set default material S235
            robApp.Project.Preferences.Materials.SetDefault(IRobotMaterialType.I_MT_STEEL, "S 275");

        }
        public static void Start_pts(Genome geometry)
        {
            if (robApp.Project.Structure.Nodes.GetAll().Count != 0) // delete any existing pts
            {
                for (int i = robApp.Project.Structure.Nodes.GetAll().Count; i > 0; i--)
                {
                    robApp.Project.Structure.Nodes.Delete(i);
                }
            }

            for (int i = 0; i < Genome.pt_cnt; i++)
            {
                robApp.Project.Structure.Nodes.Create((int)geometry.pt_cloud[0, i] + 1, geometry.pt_cloud[1, i], geometry.pt_cloud[2, i], geometry.pt_cloud[3, i]);
            } // +1 porque robot nao aceita pt 0

        }

        public static void Start_bars(Genome geometry)
        {
            for (int i = 0; i < robApp.Project.Structure.Bars.FreeNumber; i++)
            {
                robApp.Project.Structure.Bars.Delete(i);
            }
            for (int i = 0; i < Genome.bar_cnt; i++)
            {
                robApp.Project.Structure.Bars.Create((int)geometry.bars[0, i] + 1, (int)geometry.bars[1, i] + 1, (int)geometry.bars[2, i] + 1);
                robApp.Project.Structure.Bars.Get((int)geometry.bars[0, i] + 1).SetLabel(IRobotLabelType.I_LT_BAR_SECTION, Sections.section_names[0]);
                robApp.Project.Structure.Bars.Get((int)geometry.bars[0, i] + 1).SetLabel(IRobotLabelType.I_LT_MATERIAL, "AÇO");
            }
        }

        public static void Update_pts(Genome geometry)
        {
            //Console.Write(robApp.Project.Structure.Labels.GetAvailableNames(IRobotLabelType.I_LT_SUPPORT).Get(2).ToString());
            //Console.Write(robApp.Project.Structure.Labels.GetAvailableNames(IRobotLabelType.I_LT_BAR_SECTION).Get(2).ToString());

            for (int i = 0; i < Genome.pt_cnt; i++)
            {
                robApp.Project.Structure.Nodes.Create((int)geometry.pt_cloud[0, i] + 1, geometry.pt_cloud[1, i], geometry.pt_cloud[2, i], geometry.pt_cloud[3, i]);
            }//+1 porque robot nao aceita barra 0 e pt 0

        }
        public static void Update_bars(Genome geometry)
        {
            for (int i = 0; i < Genome.towerBar_cnt; i++) //apenas para as barras da torre
            {
                if(robApp.Project.Structure.Bars.IsInactive(i+1) && geometry.bars[4,i]!= -1)
                {
                    robApp.Project.Structure.Bars.SetInactive((i + 1).ToString(), false); // activar se estiver desactivado da outra iteração
                }
                if (geometry.bars[4, i] == -1 && geometry.bars[3,i] == 1) // se é para desactivar e pode ser desactivado
                {
                   robApp.Project.Structure.Bars.SetInactive((i + 1).ToString()); // desactivar
                }
                else{
                    robApp.Project.Structure.Bars.Get((int)geometry.bars[0, i] + 1).SetLabel(IRobotLabelType.I_LT_BAR_SECTION, Sections.section_names[(int)geometry.bars[4, i]]);
                    robApp.Project.Structure.Bars.Get((int)geometry.bars[0, i] + 1).SetLabel(IRobotLabelType.I_LT_MATERIAL, "AÇO");
                }

            }
        }

        public static void Refresh()
        {
            robApp.Project.ViewMngr.Refresh();
        }
        public static void Addsupports()
        {

            for (int i = 1; i <= 4; i++)
            {
                robApp.Project.Structure.Nodes.Get(i).SetLabel(IRobotLabelType.I_LT_SUPPORT, "Fixed");
            }

        }
        public static void Set_sections(string name,double area, double iz, double iy)
        {
            List<String> _section_names = new List<String>();
            List<double> _Area = new List<double>();
            List<double> _Ix = new List<double>();
            List<double> _Iy = new List<double>();


            IRobotLabel a = robApp.Project.Structure.Labels.Create(IRobotLabelType.I_LT_BAR_SECTION, name);

            IRobotBarSectionData data = a.Data;
            data.Type = IRobotBarSectionType.I_BST_STANDARD;
            data.ShapeType = IRobotBarSectionShapeType.I_BSST_CAE;

            data.SetValue(IRobotBarSectionDataValue.I_BSDV_AX, area);
            data.SetValue(IRobotBarSectionDataValue.I_BSDV_IZ, iz);
            data.SetValue(IRobotBarSectionDataValue.I_BSDV_IY, iy);
            
            robApp.Project.Structure.Labels.Store(a);

            /*

            for (int i = 1; i <= robApp.Project.Structure.Labels.GetAvailableNames(IRobotLabelType.I_LT_BAR_SECTION).Count; i++)
            {
                _section_names.Add(robApp.Project.Structure.Labels.GetAvailableNames(IRobotLabelType.I_LT_BAR_SECTION).Get(i).ToString());
                // API needs to copy labels from robot at runtime for the properties to be accessible without being assigned to any bar
                RobotLabel label = robApp.Project.Structure.Labels.Get(RobotOM.IRobotLabelType.I_LT_BAR_SECTION, _section_names[i-1]) as RobotOM.RobotLabel;
                bool available = robApp.Project.Structure.Labels.IsAvailable(RobotOM.IRobotLabelType.I_LT_BAR_SECTION, _section_names[i-1]);

                if (label == null && available)
                {
                    label = robApp.Project.Structure.Labels.CreateLike(RobotOM.IRobotLabelType.I_LT_BAR_SECTION, _section_names[i - 1], _section_names[i - 1]) as RobotOM.RobotLabel;
                }
                
                IRobotBarSectionData dt = (IRobotBarSectionData)robApp.Project.Structure.Labels.Get(IRobotLabelType.I_LT_BAR_SECTION,_section_names[i-1]).Data;
                Console.Write(dt.GetValue(IRobotBarSectionDataValue.I_BSDV_AX));

                _Area.Add(dt.GetValue(IRobotBarSectionDataValue.I_BSDV_AX));
                _Ix.Add(dt.GetValue(IRobotBarSectionDataValue.I_BSDV_IX));
                _Iy.Add(dt.GetValue(IRobotBarSectionDataValue.I_BSDV_IY));
            }*/
        }

        public static double[,] Run_analysis()
        {
            robApp.Project.CalcEngine.Calculate();
            
            results = robApp.Project.Structure.Results.Bars.Forces;
            double[,] a = new double[6, Genome.towerBar_cnt];
           // for (int k = 0; k < 4; k++) //para 5 casos de carga
           // {
                for (int i = 0; i < Genome.towerBar_cnt; i++)
                {
               
                    IRobotBar current_bar = (IRobotBar)robApp.Project.Structure.Bars.Get(i + 1);
                    a[0, i] = i + 1;
                    a[1, i] = current_bar.Length;

                    a[2, i] = Max(results.Value(i + 1, 1, 0).FX * -1.0, results.Value(i + 1, 1, 0.5).FX * -1.0, results.Value(i + 1, 1, 1).FX * -1.0) / 1000; //converter N para Kn *-1 porque comp = + no robot

                // a[3, i] = Max(results.Value(i + 1, 1, 0).MY, results.Value(i + 1, 1, 0.5).MY, results.Value(i + 1, 1, 1).MY) / 1000; //N/m -> kN/m
                // a[4, i] = Max(results.Value(i + 1, 1, 0).MZ, results.Value(i + 1, 1, 0.5).MZ, results.Value(i + 1, 1, 1).MZ) / 1000;
                //a[5, i] = 1; //remover(era para o V)
                // para comparar multiplos load cases
                for (int k = 2; k < 5; k++) {
                        double temp = Max(results.Value(i + 1, k, 0).FX * -1.0, results.Value(i + 1, k, 0.5).FX * -1.0, results.Value(i + 1, k, 1).FX * -1.0) / 1000;

                        if (a[2, i] < 0 && temp <0) // se a verificação e de buckling
                        {
                            a[2, i] = Math.Min(a[2, i], temp);
                        }else if(a[2,i]<0 && temp>0)
                        {
                            if (temp * 0.4 > Math.Abs(a[2, i])) { a[2, i] = temp; } // so se força de traçao for muito maior é que substitui força de comp
                        }else if(a[2,i]>0 && temp < 0)
                        {
                            if (Math.Abs(temp) > 0.4 * a[2, i]) { a[2, i] = temp; } // so se a F tração for muito grande é que nao é subst por comp
                        }else if(a[2,i]>0 && temp > 0)
                        {
                            a[2, i] = Math.Max(temp, a[2, i]);
                        }
                    }
            }

            return a; //[rbt_bar_num,Lenght,Fx,My,Mz]
        }
        private static double Max(double start, double middle, double end){

            if (Math.Abs(start) >= Math.Abs(middle) && Math.Abs(start) >= Math.Abs(end)) { return start; }
            if (Math.Abs(middle) >= Math.Abs(start) && Math.Abs(middle) >= Math.Abs(end)) { return middle; }
            if (Math.Abs(end) >= Math.Abs(start) && Math.Abs(end) >= Math.Abs(middle)) { return end; }
            return -1;
       }
    }
}
