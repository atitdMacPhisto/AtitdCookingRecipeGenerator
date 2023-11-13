using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;

namespace CookingSite.App_Code
{
    public class Ingredient : IEquatable<Ingredient>
    {
        int m_IngredientID;
        String m_Name;
        String m_Type;
        float m_X;
        float m_Y;
        int m_Potency;
        int m_Str;
        int m_Dex;
        int m_End;
        int m_Spd;
        int m_Con;
        int m_Foc;
        int m_Per;
        int m_Cost;
        bool m_Disabled;
        DateTime m_LastUpdated;
        float m_X2;
        float m_Y2;

        public int IngredientID { get => m_IngredientID; }
        public string Name { get => m_Name; set => m_Name = value; }
        public string Type { get => m_Type; set => m_Type = value; }
        public float X { get => m_X; set => m_X = value; }
        public float Y { get => m_Y; set => m_Y = value; }
        public int Potency { get => m_Potency; set => m_Potency = value; }
        public int Str { get => m_Str; set => m_Str = value; }
        public int Dex { get => m_Dex; set => m_Dex = value; }
        public int End { get => m_End; set => m_End = value; }
        public int Spd { get => m_Spd; set => m_Spd = value; }
        public int Con { get => m_Con; set => m_Con = value; }
        public int Foc { get => m_Foc; set => m_Foc = value; }
        public int Per { get => m_Per; set => m_Per = value; }
        public int Cost { get => m_Cost; set => m_Cost = value; }
        public bool Disabled { get => m_Disabled; set => m_Disabled = value; }
        public DateTime LastUpdated { get => m_LastUpdated; }
        public float X2 { get => m_X2; set => m_X2 = value; }
        public float Y2 { get => m_Y2; set => m_Y2 = value; }
        public PointF Coord { get => new PointF(X, Y); }
        public Ingredient()
        {
        }

        public static Ingredient Fetch(string name)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "IngredientName", name }
                };
            DataRow dr = db.FetchDataRow("SELECT * FROM Ingredients WHERE IngredientName=@IngredientName OR AltName=@IngredientName", parameters);
            if (dr == null)
                return null;
            Ingredient i = new Ingredient();
            i.m_IngredientID = (int)dr["IngredientID"];
            i.m_Name = dr["IngredientName"].ToString();
            i.m_Type = dr["IngredientType"].ToString();
            i.m_Potency = (int)dr["Potency"];
            i.m_X = (float)(double)dr["X"];
            i.m_Y = (float)(double)dr["Y"];
            i.m_Str = (int)dr["Strength"];
            i.m_Dex = (int)dr["Dexterity"];
            i.m_End = (int)dr["Endurance"];
            i.m_Spd = (int)dr["Speed"];
            i.m_Con = (int)dr["Constitution"];
            i.m_Foc = (int)dr["Focus"];
            i.m_Per = (int)dr["Perception"];
            i.m_Cost = (int)dr["Cost"];
            i.m_Disabled = (bool)dr["GenDisable"];
            i.m_LastUpdated = (DateTime)dr["LastUpdated"];
            i.m_X2 = (float)(double)dr["X2"];
            i.m_Y2 = (float)(double)dr["Y2"];

            return i;

        }

        public bool Equals(Ingredient other)
        {
            return m_Name.Equals(other.m_Name);
        }

        public void Save()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "IngredientName", Name },
                { "IngredientType", Type },
                { "Potency", Potency },
                { "X", X },
                { "Y" ,Y },
                { "Strength", Str },
                { "Dexterity", Dex },
                { "Endurance", End },
                { "Speed", Spd },
                { "Constitution", Con },
                { "Focus", Foc },
                { "Perception", Per },
                { "Cost", Cost },
                { "GenDisable", Disabled },
                { "X2", X2 },
                { "Y2" ,Y2 }
            };
            db.ExecuteNonQuery("Ingredients_AddOrUpdate", parameters, true);
            parameters = new Dictionary<string, object>
                {
                    { "name", Name }
                };
            if (IngredientID == 0)
            {
                m_IngredientID = db.ExecuteIntScalar("SELECT IngredientID FROM Ingredients WHERE IngredientName=@name", parameters);
                //cache[Name] = this;
            }
            m_LastUpdated = db.ExecuteDateTimeScalar("SELECT LastUpdated FROM Ingredients WHERE IngredientName=@name", parameters);
        }

        public override string ToString()
        {
            return $"{Name}:{m_X},{m_Y}";
        }

        public string Export()
        {
            return $"{Name}\t{Type}\t{X}\t{Y}\t{Potency}\t{LastUpdated:yyyy-MM-dd}";
        }
    }
}