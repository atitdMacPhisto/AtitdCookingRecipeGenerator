using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CookingSite.App_Code
{
    public class Recipe
    {
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

        public bool isValid(PairData[] pdc)
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

        public bool isUnique(PairData[] pdc)
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

        public void Prep(PairData[] pdc, int ratio)
        {
            int b = 0;
            int a = 0;
            cost = 0;

            switch (ratio)
            {
                case 0:
                    b = 6;
                    a = 1;
                    break;
                case 1:
                    b = 13;
                    a = 1;
                    break;
                case 2:
                    b = 4;
                    a = 3;
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

        public override string ToString()
        {
            return recipe;
        }
    }
}