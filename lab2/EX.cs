using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Drawing;


namespace lab2
{
    class EX
    {

        Excel.Application ex = new Microsoft.Office.Interop.Excel.Application();
        public EX()
        { 
            double a = ex.WorksheetFunction.ChiInv(0.5, 10);    
        }

        public double xi(double ver, double k)
        {
            double a = ex.WorksheetFunction.ChiInv(ver, k);
            return a;
        }

        public double st(double ver, double k)
        {
            double a = ex.WorksheetFunction.T_Inv_2T(ver, k);
            return a;
        }

        public double normdist(double k1)
        {
            double a = ex.WorksheetFunction.NormDist(k1,0,1,false);
            return a;
        }
    }
}
