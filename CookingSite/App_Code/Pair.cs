using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;

namespace CookingSite.App_Code
{
    public class Pair : IComparable<Pair>
    {
        int m_PairID;
        Ingredient m_Base;
        Ingredient m_Additive;
        int m_Str;
        int m_Dex;
        int m_End;
        int m_Spd;
        int m_Con;
        int m_Foc;
        int m_Per;
        int m_Dur7;
        int m_Dur14;
        int m_Dur43;
        int m_Bulk;
        float m_Distance;
        DateTime m_LastUpdated;

        public int PairID { get; }
        public Ingredient Base { get => m_Base; }
        public Ingredient Additive { get => m_Additive; }
        public int Str { get => m_Str; set => m_Str = value; }
        public int Dex { get => m_Dex; set => m_Dex = value; }
        public int End { get => m_End; set => m_End = value; }
        public int Spd { get => m_Spd; set => m_Spd = value; }
        public int Con { get => m_Con; set => m_Con = value; }
        public int Foc { get => m_Foc; set => m_Foc = value; }
        public int Per { get => m_Per; set => m_Per = value; }
        public int Dur7 { get => m_Dur7; set => m_Dur7 = value; }
        public int Dur14 { get => m_Dur14; set => m_Dur14 = value; }
        public int Dur43 { get => m_Dur43; set => m_Dur43 = value; }
        public DateTime LastUpdated { get => m_LastUpdated; }

        public float X1 => Base.X;
        public float Y1 => Base.Y;
        public float X2 => Additive.X;
        public float Y2 => Additive.Y;

        public int Potency
        {
            get
            {
                if (Additive != null)
                    return Additive.Potency;
                return 0;
            }
        }

        public int Bulk
        {
            get
            {
                if (m_Bulk != 0)
                    return m_Bulk;

                if (m_Dur7 != 0 && m_Dur14 != 0)
                    m_Bulk = m_Dur7 - (Potency / 7);

                return m_Bulk;
            }
            set
            {
                m_Bulk = value;
            }
        }

        public float Distance
        {
            get
            {
                if (m_Distance != 0)
                    return m_Distance;
                if (m_Bulk != 0)
                    m_Distance = 1000 - m_Bulk;
                else if ((Base.X != 0 || Base.Y != 0) && (Additive.X != 0 || Additive.Y != 0))
                {
                    Segment s = new Segment(new PointF(Base.X, Base.Y), new PointF(Additive.X, Additive.Y));
                    m_Distance = (float)s.Distance;
                }
                return m_Distance;
            }
            set => m_Distance = value;
        }


        public int Attribute(int index)
        {
            switch (index)
            {
                case 0: return Str;
                case 1: return Dex;
                case 2: return End;
                case 3: return Spd;
                case 4: return Con;
                case 5: return Foc;
                case 6: return Per;
                case 7: return Dur7;
                case 8: return Dur14;
                case 9: return Dur43;
            }
            return 0;
        }

        public int[] AttributeArray
        {
            get
            {
                return new int[] { Str, Dex, End, Spd, Con, Foc, Per, Dur7, Dur14, Dur43 };
            }
        }

        public string Key { get => $"{Base.Name}|{Additive.Name}"; }

        public string[] NameArray { get => new string[] { Base.Name, Additive.Name }; }

        public bool HasStats { get => Str != 0 || Dex != 0 || End != 0 || Spd != 0 || Con != 0 || Foc != 0 || Per != 0; }

        public Pair() { }

        public Pair(string b, string additive)
        {
            m_Base = Ingredient.Fetch(b);
            m_Additive = Ingredient.Fetch(additive);
        }

        public static Pair Fetch(string b, string additive)
        {
            //if (cache.ContainsKey($"{b}|{additive}"))
            //    return cache[$"{b}|{additive}"];

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "Base", b },
                { "Additive" , additive }
            };
            DataRow dr = db.FetchDataRow("SELECT * FROM Pairs WHERE Base=@Base AND Additive=@Additive", parameters);
            if (dr == null)
                return null;

            Pair p = new Pair(b, additive);
            p.m_PairID = (int)dr["PairID"];
            p.m_Str = (int)dr["Strength"];
            p.m_Dex = (int)dr["Dexterity"];
            p.m_End = (int)dr["Endurance"];
            p.m_Spd = (int)dr["Speed"];
            p.m_Con = (int)dr["Constitution"];
            p.m_Foc = (int)dr["Focus"];
            p.m_Per = (int)dr["Perception"];
            p.m_Dur7 = (int)dr["Dur7"];
            p.m_Dur14 = (int)dr["Dur14"];
            p.m_Dur43 = (int)dr["Dur43"];
            p.m_Bulk = (int)dr["BulkVal"];
            p.m_Distance = (float)(double)dr["Distance"];
            p.m_LastUpdated = (DateTime)dr["LastUpdated"];

            //cache[$"{b}|{additive}"] = p;
            return p;
        }

        public static Pair FetchPair(string s)
        {
            string[] p = s.Split('|');
            if (p.Length != 2) return null;
            return Fetch(p[0], p[1]);
        }

        public void Save(bool setRecorded = true)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "Base", Base.Name },
                { "Additive", Additive.Name },
                { "Strength", Str },
                { "Dexterity", Dex },
                { "Endurance", End },
                { "Speed", Spd },
                { "Constitution", Con },
                { "Focus", Foc },
                { "Perception", Per },
                { "Dur7", Dur7 },
                { "Dur14", Dur14 },
                { "Dur43", Dur43 },
                { "BulkVal", Bulk },
                { "Distance", Distance },
                { "Recorded", setRecorded }
            };

            db.ExecuteNonQuery("Pairs_AddOrUpdate", parameters, true);
            parameters = new Dictionary<string, object>
            {
                { "base", Base.Name },
                { "additive", Additive.Name }
            };
            if (PairID == 0)
            {
                m_PairID = db.ExecuteIntScalar("SELECT PairID FROM Pairs WHERE Base=@base AND Additive=@additive", parameters);
            }
            m_LastUpdated = db.ExecuteDateTimeScalar("SELECT LastUpdated FROM Pairs WHERE Base=@base AND Additive=@additive", parameters);
        }

        public int CompareTo(Pair other)
        {
            if (other == null)
                return 1;
            else
            {
                return Key.CompareTo(other.Key);
            }
        }

        public static int Compare(Pair p1, Pair p2)
        {
            return p1.CompareTo(p2);
        }

        public override string ToString()
        {
            return $"{Key}:{Base.X},{Base.Y}-{Additive.X},{Additive.Y}";
        }

        public string Export()
        {
            return $"{Base.Name}\t{Additive.Name}\t{Str}\t{Dex}\t{End}\t{Spd}\t{Con}\t{Foc}\t{Per}\t{Bulk}\t{Distance}\t{LastUpdated:yyyy-MM-dd}";
        }

        public string BulkExport()
        {
            string d7 = new TimeSpan(0, 0, Dur7).ToString(@"hh\:mm\:ss");
            string d14 = new TimeSpan(0, 0, Dur14).ToString(@"hh\:mm\:ss");

            return $"{Base.Name}\t{Additive.Name}\t{Str}\t{Dex}\t{End}\t{Spd}\t{Con}\t{Foc}\t{Per}\t{d7}\t{d14}";
        }

        public void UpdateDistance()
        {
            m_Distance = Util.CalcDistance(Base.Coord, Additive.Coord);
            Save();
        }
    }
}