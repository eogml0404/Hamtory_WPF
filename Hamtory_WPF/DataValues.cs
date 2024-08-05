using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamtory_WPF
{
    public class DataValues
    {
        public DateTime date { get; private set; }
        public int index { get; private set; }
        public int melt_temperature { get; private set; }
        public int motor_speed { get; private set; }
        public int melt_weight { get; private set; }
        public double moisture { get; private set; }
        public string ok_ng { get; private set; }
        public DataValues(DateTime date,int index , int melt_temperature, int motor_speed, int melt_weight, double moisture, string ok_ng)
        {
            this.date = date;
            this.index = index;
            this.melt_temperature = melt_temperature;
            this.motor_speed = motor_speed;
            this.melt_weight = melt_weight;
            this.moisture = moisture;
            this.ok_ng = ok_ng;
        }


    }
}
