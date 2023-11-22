using CookingSite.App_Code;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CookingSite
{
    public partial class _Default : Page
    {
        bool[] statFilters = { false, false, false, false, false, false, false };
        int sortby = 0;
        int ratio = 0;
        int level = 2;
        int servings = 1;
        bool posOnly;
        bool invLimited;
        bool boosted;
        string[] statAbbrevs = new string[] { "STR", "DEX", "END", "SPD", "CON", "FOC", "PER" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                switch (Request.Form["__EVENTTARGET"])
                {
                    case "ctl00$MainContent$cboSort":
                        sortby = cboSort.SelectedIndex;
                        break;
                    default:
                        statFilters = new bool[] { chkSTR.Checked, chkDEX.Checked, chkEND.Checked, chkSPD.Checked, chkCON.Checked, chkFOC.Checked, chkPER.Checked };
                        sortby = cboSort.SelectedIndex;
                        ratio = cboRatio.SelectedIndex;
                        level = cboCooklevel.SelectedIndex;
                        if (!int.TryParse(txtServings.Text, out servings))
                        {
                            servings = 1;
                            txtServings.Text = "1";
                        }
                        posOnly = chkPosOnly.Checked;
                        invLimited = chkInvLimit.Checked;
                        boosted = chkBoosted.Checked;
                        break;
                }
                lblNoRecipes.Visible = false;
            }
            else
            {
                txtServings.Text = "1";
                cboSort.SelectedIndex = 0;
                cboRatio.SelectedIndex = 0;
                cboCooklevel.SelectedIndex = 0;
                gvRecipes.Visible = false;
                lblNoRecipes.Visible = false;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string[] durs = new string[] { "Dur7", "Dur14", "Dur43" };
            string[] filterNames = new string[] { "Strength", "Dexterity", "Endurance", "Speed", "Constitution", "Focus", "Perception" };
            List<string> colList = new List<string>();

            string filter1 = "";
            string filter2 = "";
            string sortBy1 = "";
            string dur1 = durs[ratio];
            switch (sortby)
            {
                case 7:
                    filter1 = $"Strength >= 0 AND Dexterity >= 0";
                    filter2 = $"P.Strength >= 0 AND P.Dexterity >= 0";
                    sortBy1 = $"Strength DESC, Dexterity DESC";
                    break;
                default:
                    filter1 = $"{filterNames[sortby]} >= 0";
                    filter2 = $"P.{filterNames[sortby]} >= 0";
                    sortBy1 = $"{filterNames[sortby]} DESC";
                    break;
            }

            string qry = string.Empty;
            int minBase = 0;
            int minAdd = 0;

            switch (ratio)
            {
                case 0:
                    minBase = servings * 6;
                    minAdd = servings;
                    break;
                case 1:
                    minBase = servings * 13;
                    minAdd = servings;
                    break;
                case 2:
                    minBase = servings * 4;
                    minAdd = servings * 3;
                    break;
            }

            List<string> t1 = db.GetStringList($"SELECT TOP 50 Base + '|' + Additive FROM Pairs P LEFT OUTER JOIN StockView v1 ON v1.IngredientName=P.Base LEFT OUTER JOIN StockView v2 ON v2.IngredientName=P.Additive WHERE v1.Quantity >= 30 AND v2.Quantity >= 5 AND v1.GenDisable=0 AND v2.GenDisable=0 and {filter2} ORDER BY {sortBy1}");
            Dictionary<string, App_Code.Pair> pairCache = new Dictionary<string, App_Code.Pair>();
            if (t1 == null)
            {
                gvRecipes.Visible = false;
                lblNoRecipes.Visible = true;
                return;
            }

            PairData[] pairDatas = new PairData[t1.Count];
            int[] inputSet = new int[pairDatas.Length];
            for (int i = 0; i < t1.Count; i++)
            {
                string key = t1[i];
                if (!pairCache.ContainsKey(key))
                {
                    pairCache.Add(key, App_Code.Pair.FetchPair(key));
                }

                pairDatas[i] = new PairData(pairCache[key], ratio);
                inputSet[i] = i;
            }

            ConcurrentBag<Recipe> candidates = new ConcurrentBag<Recipe>();

            for (int i = 0; i < level + 1; i++)
            {
                Combinations<int> combos = new Combinations<int>(inputSet, i + 1);
                Parallel.ForEach(combos, c => ValidateRecipe(candidates, pairDatas, sortby, statFilters, boosted, c));
            }
            List<Recipe> cand = new List<Recipe>(candidates);
            cand.Sort((a, b) => a.SortVal.CompareTo(b.SortVal));
            cand.Reverse();
            cand = cand.Take(200).ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("Recipe", typeof(string));
            dt.Columns.Add("Cost", typeof(int));
            dt.Columns.Add("STR", typeof(int));
            dt.Columns.Add("DEX", typeof(int));
            dt.Columns.Add("END", typeof(int));
            dt.Columns.Add("SPD", typeof(int));
            dt.Columns.Add("CON", typeof(int));
            dt.Columns.Add("FOC", typeof(int));
            dt.Columns.Add("PER", typeof(int));
            dt.Columns.Add("Dur", typeof(int));
            dt.Columns.Add("Time", typeof(TimeSpan));

            List<string> UpdatePairs = new List<string>();

            foreach (Recipe r in cand)
            {
                r.Prep(pairDatas, ratio);
                DataRow row = dt.NewRow();

                row["recipe"] = r.ToString();
                float[] results = r.Attributes;
                for (int j = 0; j < 8; j++)
                    row[j + 2] = (int)results[j];

                row["cost"] = r.cost;
                row["time"] = new TimeSpan(0, 0, (int)results[7]);
                dt.Rows.Add(row);

                foreach (int pi in r.PairID)
                {
                    PairData pdc = pairDatas[pi];
                    if (!UpdatePairs.Contains(pdc.pair.Key))
                        UpdatePairs.Add(pdc.pair.Key);
                }
            }

            if (dt.Rows.Count > 0)
            {
                Session["RecipeTable"] = dt;

                string sortExpression = cboSort.Text;
                switch (cboSort.SelectedIndex)
                {
                    case 7:
                        sortExpression = $"STR DESC, DEX DESC";
                        break;
                    default:
                        sortExpression = $"{cboSort.Text} DESC";
                        break;
                }
                dt.DefaultView.Sort = sortExpression;
                gvRecipes.DataSource = Session["RecipeTable"];
                gvRecipes.DataBind();

                gvRecipes.Visible = true;
                lblNoRecipes.Visible = false;

                DataTable dup = new DataTable();
                dup.Columns.Add("Pair", typeof(string));
                dup.Columns.Add("LastUpdated", typeof(DateTime));

               
                foreach (string s in UpdatePairs)
                {
                    App_Code.Pair p = pairCache[s];
                    TimeSpan elapsed = DateTime.Now - p.LastUpdated;
                    if (elapsed > new TimeSpan(5, 0, 0, 0))
                    {
                        DataRow row = dup.NewRow();
                        row["Pair"] = s;
                        row["LastUpdated"] = p.LastUpdated;
                        dup.Rows.Add(row);
                    }
                }

                Session["UpdatePairs"] = dup;
                gvUpdatePairs.DataSource = dup;
                gvUpdatePairs.DataBind();
            }
            else
            {
                Session["RecipeTable"] = dt;
                gvRecipes.Visible = false;
                lblNoRecipes.Visible = true;
            }
            lblRecipeCount.Text = $"Recipes Shown: {dt.Rows.Count}";
        }

        protected void cboSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Session["RecipeTable"] == null || gvRecipes.Rows.Count == 0)
                return;

            string sortExpression = cboSort.Text;
            switch (cboSort.SelectedIndex)
            {
                case 7:
                    sortExpression = $"STR DESC, DEX DESC";
                    break;
                default:
                    sortExpression = $"{cboSort.Text} DESC";
                    break;
            }
            DataTable dt = Session["RecipeTable"] as DataTable;
            dt.DefaultView.Sort = sortExpression;

            gvRecipes.DataSource = Session["RecipeTable"];
            gvRecipes.DataBind();

        }

        protected void gvRecipes_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = Session["RecipeTable"] as DataTable;
            if (dt != null)
            {
                //Sort the data.
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                gvRecipes.DataSource = dt;
                gvRecipes.DataBind();
            }
            else
                Response.Redirect("~/");

            DataTable dt2 = Session["UpdatePairs"] as DataTable;
            if (dt2 != null && dt2.Rows.Count > 0)
            {
                //Sort the data.
                gvUpdatePairs.DataSource = dt2;
                gvUpdatePairs.DataBind();
            }
        }

        private string GetSortDirection(string column)
        {

            // By default, set the sort direction to ascending.
            string sortDirection = "ASC";

            // Retrieve the last column that was sorted.
            string sortExpression = Session["SortExpression"] as string;

            if (sortExpression != null)
            {
                // Check if the same column is being sorted.
                // Otherwise, the default value can be returned.
                if (sortExpression == column)
                {
                    string lastDirection = Session["SortDirection"] as string;
                    if (lastDirection == "DESC")
                        sortDirection = "ASC";
                    else
                        sortDirection = "DESC";
                }
            }

            // Save new values in ViewState.
            Session["SortDirection"] = sortDirection;
            Session["SortExpression"] = column;

            return sortDirection;
        }

        protected void gvUpdatePairs_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = Session["RecipeTable"] as DataTable;
            if (dt != null && dt.Rows.Count > 0)
            {
                gvRecipes.DataSource = dt;
                gvRecipes.DataBind();
            }
            else
                Response.Redirect("~/");

            DataTable dt2 = Session["UpdatePairs"] as DataTable;
            if (dt2 != null && dt2.Rows.Count > 0)
            {
                //Sort the data.
                dt2.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                gvUpdatePairs.DataSource = dt2;
                gvUpdatePairs.DataBind();
            }
        }

        private void ValidateRecipe(ConcurrentBag<Recipe> candidates, PairData[] pairDatas, int sortby, bool[] attrFilters, bool boosted, IList<int> c)
        {
            Recipe rdc = new Recipe();
            for (int j = 0; j < c.Count; j++)
                rdc.PairID.Add(c[j]);
            CalcStats(rdc, pairDatas, sortby);

            if (!rdc.isPositive(attrFilters, boosted))
                return;
            if (!rdc.isUnique(pairDatas))
                return;
            if (!rdc.isValid(pairDatas))
                return;

            candidates.Add(rdc);
        }

        private void CalcStats(Recipe rdc, PairData[] pdc, int sortby)
        {
            for (int i = 0; i < 8; i++)
            {
                float f = 0.0f;
                foreach (int p in rdc.PairID)
                {

                    f += Util.SSqr(pdc[p].Attributes[i]);
                }
                rdc.Attributes[i] = Util.SSqrt(f);
            }
            rdc.SortVal = 0.0f;
            foreach (int p in rdc.PairID)
            {
                switch (sortby)
                {
                    case 7:
                        rdc.SortVal = Util.SSqr(pdc[p].Attributes[0] + pdc[p].Attributes[1]);
                        break;
                    default:
                        rdc.SortVal += Util.SSqr(pdc[p].Attributes[sortby]);
                        break;
                }
            }
            rdc.SortVal = Util.SSqrt(rdc.SortVal);
        }
    }

}