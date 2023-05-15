using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace eTOM
{
    public class SampleData : List<double>
    {
        StreamReader sr = new StreamReader("C:\\Users\\abrik\\Downloads\\Exame-main\\Exame-main\\eTOM\\SampleData.txt");
        public SampleData()
        {
            Add(Convert.ToDouble(sr.ReadLine()));
            Add(Convert.ToDouble(sr.ReadLine()));
            Add(Convert.ToDouble(sr.ReadLine()));
            Add(Convert.ToDouble(sr.ReadLine()));
            Add(Convert.ToDouble(sr.ReadLine()));
            Add(Convert.ToDouble(sr.ReadLine()));
            Add(Convert.ToDouble(sr.ReadLine()));
            Add(Convert.ToDouble(sr.ReadLine()));
            Add(Convert.ToDouble(sr.ReadLine()));
            Add(Convert.ToDouble(sr.ReadLine()));
        }
    }
}
