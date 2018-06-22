using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WindsupRFID
{
    public class SUPData
    {

        public SUPData(string name, int id, int demi, int hour, int supp)
        {
            Name = name;
            ID = id;
            Demi = demi;
            Hour = hour;
            Supp = supp;

        }

        public string Name { get; set; }
        public int ID { get; set; }
        public int Demi { get; set; }
        public int Hour { get; set; }
        public int Supp { get; set; }
    }
}