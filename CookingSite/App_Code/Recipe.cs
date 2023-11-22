using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CookingSite.App_Code
{
    public class Recipe
    {
        public static PairData[] pdc;

        public List<int> PairID = new List<int>();
        public float[] Attributes = new float[8];
        public float SortVal = 0.0f;
        public int cost;
        string recipe;

        public bool isPositive(bool[] desired, bool boosted)
        {
            for (int i = 0; i < 7; i++)
            {
                if (desired[i])
                {
                    float s = Attributes[i];
                    if ((boosted && s <= 0) || s < 0)
                        return false;
                }
            }
            return true;
        }

        public bool isValid()
        {
            if (PairID.Count == 1)
                return true;

            for (int x = 0; x < PairID.Count; x++)
            {
                float min = 3000;
                for (int y = 0; y < PairID.Count; y++)
                    min = Math.Min(min, Util.Sqrt(Util.Sqr(pdc[PairID[y]].X1, pdc[PairID[x]].X2) + Util.Sqr(pdc[PairID[y]].Y1, pdc[PairID[x]].Y2)));
                if (min != Util.Sqrt(Util.Sqr(pdc[PairID[x]].X1, pdc[PairID[x]].X2) + Util.Sqr(pdc[PairID[x]].Y1, pdc[PairID[x]].Y2)))
                    return false;
            }

            for (int y = 0; y < PairID.Count; y++)
            {
                float min = 3000;
                for (int x = 0; x < PairID.Count; x++)
                    min = Math.Min(min, Util.Sqrt(Util.Sqr(pdc[PairID[y]].X1, pdc[PairID[x]].X2) + Util.Sqr(pdc[PairID[y]].Y1, pdc[PairID[x]].Y2)));
                if (min != Util.Sqrt(Util.Sqr(pdc[PairID[y]].X1, pdc[PairID[y]].X2) + Util.Sqr(pdc[PairID[y]].Y1, pdc[PairID[y]].Y2)))
                    return false;
            }

            return true;
        }

        public bool isUnique()
        {
            if (PairID.Count == 1)
                return true;

            List<int> ids = new List<int>();
            foreach (int id in PairID)
            {
                ids.Add(pdc[id].ID1);
                ids.Add(pdc[id].ID2);
            }

            return ids.Count == ids.Distinct().Count();
        }

        public void Prep(int ratio, int mult)
        {
            int b = 0;
            int a = 0;
            cost = 0;

            switch (ratio)
            {
                case 0:
                    b = 6 * mult;
                    a = mult;
                    break;
                case 1:
                    b = 13 * mult;
                    a = mult;
                    break;
                case 2:
                    b = 4 * mult;
                    a = 3 * mult;
                    break;
            }

            List<string> ingredients = new List<string>();
            foreach (int id in PairID)
            {
                Pair p = pdc[id].pair;
                ingredients.Add($"{b} {p.Base.Name}");
                ingredients.Add($"{a} {p.Additive.Name}");
                cost += b * p.Base.Cost;
                cost += a * p.Additive.Cost;
            }

            recipe = string.Join(" ", ingredients);
        }

        public void CalcStats(int sortby)
        {
            for (int i = 0; i < 8; i++)
            {
                float f = 0.0f;
                foreach (int p in PairID)
                {

                    f += Util.SSqr(pdc[p].Attributes[i]);
                }
                Attributes[i] = Util.SSqrt(f);
            }
            SortVal = 0.0f;
            foreach (int p in PairID)
            {
                switch (sortby)
                {
                    case 7:
                        SortVal = Util.SSqr(pdc[p].Attributes[0] + pdc[p].Attributes[1]);
                        break;
                    default:
                        SortVal += Util.SSqr(pdc[p].Attributes[sortby]);
                        break;
                }
            }
            SortVal = Util.SSqrt(SortVal);
        }

        public bool Validate(int sortby, bool[] filters, bool boosted)
        {
            CalcStats(sortby);

            if (!isPositive(filters, boosted))
                return false;
            if (!isUnique())
                return false;
            return isValid();
        }

        public override string ToString()
        {
            return recipe;
        }
    }
}