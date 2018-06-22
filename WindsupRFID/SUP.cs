using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindsupRFID
{
    public class SUP
    {
        private int ID;
        // TODO: TimeStamps
        private SUPData Type;

        public SUP(int _ID, SUPData _Type)
        {
            ID = _ID;
            Type = _Type;
        }

        public bool IsSameID(int _ID)
        {
            return (ID == _ID);
        }

        public bool IsSameType(SUPData typ)
        {
            return (typ.ID == Type.ID);
        }

        public int GetPrice( int timeMin)
        {
            int price = 0;
            if      (timeMin <= 30) { price = Type.Demi; } // half hour price 
            else if (timeMin <= 60) { price = Type.Hour; } // full hour price
            else
            {
                price = Type.Hour;
                timeMin -= 60;
                while(timeMin > 0)
                {
                    price += Type.Supp;
                    timeMin -= 60;
                }
            }

            return price;
        }
    }
}