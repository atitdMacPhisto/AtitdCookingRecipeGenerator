using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CookingSite.App_Code
{
    public class PairData
    {
        public Pair pair;
        public int PairID => pair.PairID;
        public Ingredient Base => pair.Base;
        public Ingredient Additive => pair.Additive;
        public string BaseName => pair.Base.Name;
        public string AdditiveName => pair.Additive.Name;
        public int Str => pair.Str;
        public int Dex => pair.Dex;
        public int End => pair.End;
        public int Spd => pair.Spd;
        public int Con => pair.Con;
        public int Foc => pair.Foc;
        public int Per => pair.Per;
        public int Dur
        {
            get
            {
                switch (m_durIndex)
                {
                    case 0:
                        return pair.Dur7;
                    case 1:
                        return pair.Dur14;
                    case 2:
                        return pair.Dur43;
                }
                return 0;
            }
        }
        public int ID1 => pair.Base.IngredientID;
        public int ID2 => pair.Additive.IngredientID;
        public float X1 => pair.Base.X;
        public float Y1 => pair.Base.Y;
        public float X2 => pair.Additive.X;
        public float Y2 => pair.Additive.Y;
        int m_durIndex = 0;

        public int[] Attributes
        {
            get
            {
                return new int[] { Str, Dex, End, Spd, Con, Foc, Per, Dur };
            }
        }

        public PairData(Pair pair, int durIndex)
        {
            this.pair = pair;
            m_durIndex = durIndex;
        }
    }
}