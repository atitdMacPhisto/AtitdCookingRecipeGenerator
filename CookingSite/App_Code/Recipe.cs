using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace CookingSite.App_Code
{
    public class Recipe : IEquatable<Recipe>
    {
        public static Dictionary<string, Pair> pairCache = new Dictionary<string, Pair>();

        public SortedList<string, Pair> pairs = new SortedList<string, Pair>();
        List<string> ingredients = new List<string>();
        public float sortedAttr = 0;

        public float SorterAttr => Util.SSqrt(sortedAttr);

        public Recipe()
        {
        }

        public Recipe(string p, int sortAttr)
        {
            Pair pair = null;
            if (pairCache.ContainsKey(p))
                pair = pairCache[p];
            else
                pair = Pair.FetchPair(p);
            pairs.Add(pair.Key, pair);
            ingredients.Add(pair.Base.Name);
            ingredients.Add(pair.Additive.Name);
            sortedAttr = Util.SSqr(pair.Attribute(sortAttr));
        }

        public Recipe(Recipe r, string p, int sortAttr)
        {
            Pair pair = null;
            if (pairCache.ContainsKey(p))
                pair = pairCache[p];
            else
                pair = Pair.FetchPair(p);
            foreach (KeyValuePair<string, Pair> kvp in r.pairs)
                pairs.Add(kvp.Key, kvp.Value);
            ingredients.AddRange(r.ingredients);
            sortedAttr = r.sortedAttr;
            pairs.Add(pair.Key, pair);
            ingredients.Add(pair.Base.Name);
            ingredients.Add(pair.Additive.Name);
            sortedAttr += CalcSortVal(pair, sortAttr);
        }

        public bool Add(string p, int sortAttr)
        {
            if (Contains(p))
                return false;
            Pair pair = null;
            if (pairCache.ContainsKey(p))
                pair = pairCache[p];
            else
                pair = Pair.FetchPair(p);
            if (ingredients.Contains(pair.Base.Name) || ingredients.Contains(pair.Additive.Name))
                return false;
            pairs.Add(pair.Key, pair);
            ingredients.Add(pair.Base.Name);
            ingredients.Add(pair.Additive.Name);
            sortedAttr = CalcSortVal(pair, sortAttr);
            return true;
        }

        private float CalcSortVal(Pair pair, int sortAttr)
        {
            switch (sortAttr)
            {
                case 7:
                    return Util.SSqr((pair.Attribute(0) + pair.Attribute(1))/2);
                default:
                    return Util.SSqr(pair.Attribute(sortAttr));
            }
        }

        public float Attribute(int index)
        {
            return (float)Math.Sqrt(pairs.Values.Sum(s => (float)Math.Pow(s.Attribute(index), 2)));
        }

        public bool Contains(string p)
        {
            return pairs.Where(t => t.Key == p).Count() > 0;
        }

        public bool Equals(Recipe other)
        {
            return ToString().Equals(other.ToString());
        }

        public override string ToString()
        {
            return string.Join(",", pairs.Keys);
        }

        public string ToWikiString(string ratio)
        {
            string[] ratioParts = ratio.Split(':');
            int baseQty = int.Parse(ratioParts[0]);
            int addQty = int.Parse(ratioParts[1]);
            List<string> pairlist  = new List<string>();
            foreach (Pair p in pairs.Values)
            {
                pairlist.Add($"{baseQty} {p.Base.Name}. {addQty} {p.Additive.Name}.");
            }
            return string.Join(" ", pairlist);
        }

        /// <summary>
        /// Checks recipe where ingredients in a pair are closer to each other than to other ingredients of recipe
        /// </summary>
        /// <returns></returns>
        public bool isValid()
        {
            if (ingredients.Distinct().Count() != pairs.Count * 2)
                return false;

            for (int x = 0; x < pairs.Count; x++)
            {
                float min = 3000;
                for (int y = 0; y < pairs.Count; y++)
                    min = Math.Min(min, Util.Sqrt(Util.Sqr(pairs.Values[y].X1, pairs.Values[x].X2) + Util.Sqr(pairs.Values[y].Y1, pairs.Values[x].Y2)));
                if (min != Util.Sqrt(Util.Sqr(pairs.Values[x].X1, pairs.Values[x].X2) + Util.Sqr(pairs.Values[x].Y1, pairs.Values[x].Y2)))
                    return false;
            }

            for (int y = 0; y < pairs.Count; y++)
            {
                float min = 3000;
                for (int x = 0; x < pairs.Count; x++)
                    min = Math.Min(min, Util.Sqrt(Util.Sqr(pairs.Values[y].X1, pairs.Values[x].X2) + Util.Sqr(pairs.Values[y].Y1, pairs.Values[x].Y2)));
                if (min != Util.Sqrt(Util.Sqr(pairs.Values[y].X1, pairs.Values[y].X2) + Util.Sqr(pairs.Values[y].Y1, pairs.Values[y].Y2)))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Verify desired stats are positive, or greater than 0 if also require boosted attributes
        /// </summary>
        /// <param name="stats">Desired Stats 0 or greater</param>
        /// <param name="boosted">Desired Stats should be greater than 0</param>
        /// <returns></returns>
        public bool isPositive(bool[] stats, bool boosted)
        {
            bool ret = true;

            for (int i = 0; i < stats.Length; i++)
            {
                if (stats[i])
                {
                    float s = 0;
                    foreach (Pair p in pairs.Values)
                    {
                        s += Util.SSqr(p.Attribute(i));
                    }

                    s = Util.SSqrt(s);

                    if (boosted)
                    {
                        if (s <= 0)
                            return false;
                    }
                    else
                    {
                        if (s < 0)
                            return false;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Recipe must have all unique ingredients, no replication
        /// </summary>
        /// <returns></returns>
        public bool isUnique()
        {
            bool ret = true;

            List<string> ing = new List<string>();
            foreach (Pair pair in pairs.Values)
            {
                if (ing.Contains(pair.Base.Name))
                    return false;
                ing.Add(pair.Base.Name);
                if (ing.Contains(pair.Additive.Name))
                    return false;
                ing.Add(pair.Additive.Name);
            }

            return ret;
        }

        /// <summary>
        /// Calculate overall stats and duration for entire recipe.
        /// Square root of sum of squares, retaining signs.
        /// </summary>
        /// <param name="ratio">The base:additive rato to use in calculations. 6:1, 13:1, 4:3</param>
        /// <returns></returns>
        public float[] Attributes(int ratio)
        {
            float[] att = new float[8];
            foreach (Pair p in pairs.Values)
            {
                int[] a = p.AttributeArray;
                for (int i = 0; i < 7; i++)
                    att[i] += Util.SSqr(a[i]);
                att[7] += Util.SSqr(a[7 + ratio]);
            }
            for (int i = 0; i < 8; i++)
                att[i] = Util.SSqrt(att[i]);
            att[7] *= 1.3f;

            return att;
        }

        /// <summary>
        /// Calculates the cost of the recipe
        /// </summary>
        /// <param name="ratio">The base:additive rato to use in calculations. 6:1, 13:1, 4:3</param>
        /// <returns></returns>
        public int Cost(int ratio)
        {
            int cost = 0;
            foreach (Pair p in pairs.Values)
            {
                switch (ratio)
                {
                    case 0:
                        cost += (p.Base.Cost * 6) + p.Additive.Cost;
                        break;
                    case 1:
                        cost += (p.Base.Cost * 13) + p.Additive.Cost;
                        break;
                    case 2:
                        cost += (p.Base.Cost * 4) + (p.Additive.Cost * 3);
                        break;
                }
            }
            return cost;
        }

        public DataRow MakeDataRow(DataTable dt, int ratio)
        {
            float[] results = Attributes(ratio);
            DataRow dr = dt.NewRow();
            dr["Recipe"] = ToString();
            for (int i = 0; i < 8; i++)
                dr[i + 1] = (int)results[i];
            dr["Cost"] = Cost(ratio);
            dr["Time"] = new TimeSpan(0, 0, (int)results[7]);
            return dr;
        }
    }
}