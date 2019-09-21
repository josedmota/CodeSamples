using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraser
{
    class Sections
    {
        public static List<String> section_names=new List<String>();
        public static List<double> Area = new List<double>();
        public static List<double> Iz= new List<double>();
        public static List<double> Iy= new List<double>();
        public static List<double> Iv = new List<double>();
        public static List<double> ivv = new List<double>();

        public static int count=0;

        public Sections(string name, double A, double z, double y, double v)
        {
            section_names.Add(name);
            Area.Add(A); ;
            Iz.Add(z);
            Iy.Add(y);
            Iv.Add(v);

            //////////////
            // Calc ivv //
            //////////////
            ivv.Add(Math.Sqrt(v / A));


            count++;

        }
    }
}
