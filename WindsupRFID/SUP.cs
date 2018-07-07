using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindsupRFID
{
    public class SUP
    {
        private readonly Int64 ID;
        // TODO: TimeStamps
        private SUPData Type;
        public DateTime LeftTime;

        public SUP(Int64 _ID, SUPData _Type)
        {
            ID = _ID;
            Type = _Type;
        }
        public SUP(Int64 _ID, SUPData _Type, DateTimeOffset Left)
        {
            ID = _ID;
            Type = _Type;
            LeftTime = Left.UtcDateTime;
        }
        public Int64 getID()
        {
            return ID;
        }
        public string strID
        {
            get
            { return ID.ToString(); }
        }
        
        public string strType
        {
            get
            { return Type.Name; }
        }

        public string strTime
        {
            get
            {
                return getTime().ToString(@"hh\:mm");
            }
        }

        public bool IsSameID(Int64 _ID)
        {
            return (ID == _ID);
        }



        public bool IsSameType(SUPData typ)
        {
            return (typ.ID == Type.ID);
        }

        ///<summary>
        ///get prices according to Type of SUP
        ///</summary>
        public int GetPrice()
        {
            TimeSpan timeMin = getTime();

            int price = 0;
            if (timeMin.CompareTo(TimeSpan.FromMinutes(30)) < 1) { price = Type.Demi; } // half hour price 
            else if (timeMin.CompareTo(TimeSpan.FromMinutes(60)) < 1) { price = Type.Hour; } // full hour price
            else
            {
                price = Type.Hour;
                timeMin.Subtract(TimeSpan.FromHours(1));
                while (timeMin.CompareTo(TimeSpan.FromMinutes(10)) >= 0) // 10mn overtime max
                {
                    price += Type.Supp;
                    timeMin.Subtract(TimeSpan.FromHours(1));
                }
            }

            return price;
        }

        /// <summary>
        /// Set Timer starting point to "Now"
        /// </summary>
        public void StartTimer()
        {
            LeftTime = DateTime.Now;
        }


        public TimeSpan getTime()
        {
            //get tick since left
            TimeSpan timeLeft = DateTime.Now - LeftTime;

            return timeLeft;
        }
        public DateTimeOffset getDeparture()
        {
            DateTimeOffset tm;
            tm = LeftTime;
            
            return (DateTimeOffset) tm;
        }
    }
}