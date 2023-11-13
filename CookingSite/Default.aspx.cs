using CookingSite.App_Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            string[] filterNames = new string[] { "Strength", "Dexterity", "Endurance", "Speed", "Constitution", "Focus", "Perception" };
            List<string> colList = new List<string>();
            string sortBy = filterNames[sortby];

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

            List<string> t1 = db.GetStringList($"SELECT TOP 50 Base + '|' + Additive FROM Pairs P LEFT OUTER JOIN StockView v1 ON v1.IngredientName=P.Base LEFT OUTER JOIN StockView v2 ON v2.IngredientName=P.Additive WHERE " + (invLimited ? $" v1.Quantity >= {minBase} AND v2.Quantity >= {minAdd}" : "1=1") + $" AND v1.GenDisable=0 AND v2.GenDisable=0 and {sortBy} >= 0 ORDER BY {sortBy} DESC");
            if (t1 == null)
            {
                gvRecipes.Visible = false;
                lblNoRecipes.Visible = true;
                return;
            }

            Recipe.pairCache = new Dictionary<string, App_Code.Pair>();
            foreach (string s1 in t1)
            {
                Recipe.pairCache.Add(s1, App_Code.Pair.FetchPair(s1));
            }

            List<Recipe> masterRecipeList = new List<Recipe>();
            List<Recipe> recipeList = new List<Recipe>();
            for (int i = 0; i <= level; i++)
            {
                Combinations<string> combinations = new Combinations<string>(t1, i + 1);
                long comb = combinations.Count;

                recipeList = new List<Recipe>();
                foreach (IList<string> c in combinations)
                {
                    Recipe recipe = new Recipe();
                    bool valid = true;
                    foreach (string s in c)
                    {
                        if (!recipe.Add(s, sortby))
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (!valid)
                        continue;
                    if (recipe.isValid())
                    {
                        if (chkPosOnly.Checked)
                        {
                            if (recipe.isPositive(statFilters, boosted))
                                recipeList.Add(recipe);
                        }
                        else
                            recipeList.Add(recipe);
                    }
                }
                masterRecipeList.AddRange(recipeList);
            }


            //List<App_Code.Pair> pairList = new List<App_Code.Pair>();
            //foreach (string t in t1)
            //    pairList.Add(App_Code.Pair.FetchPair(t));

            //List<Recipe> masterRecipeList = new List<Recipe>();
            //List<Recipe> recipeList = new List<Recipe>();

            //foreach (string s in t1)
            //{
            //    Recipe r = new Recipe(s, sortby);
            //    if (r.isValid() && r.isUnique())
            //    {
            //        if (chkPosOnly.Checked)
            //        {
            //            if (r.isPositive(statFilters, boosted))
            //                recipeList.Add(r);
            //        }
            //        else
            //            recipeList.Add(r);
            //    }
            //}

            //masterRecipeList.AddRange(recipeList);

            //if (level > 0)
            //{
            //    for (int i = 0; i < level; i++)
            //    {
            //        List<Recipe> newRecipeList = new List<Recipe>();
            //        foreach (string s in t1)
            //        {
            //            foreach (Recipe recipe in recipeList)
            //            {
            //                if (!recipe.Contains(s))
            //                {
            //                    Recipe r = new Recipe(recipe, s, sortby);
            //                    if (r.isValid() && r.isUnique())
            //                    {
            //                        if (chkPosOnly.Checked)
            //                        {
            //                            if (!newRecipeList.Contains(r) && r.isPositive(statFilters, boosted))
            //                            {
            //                                //bool b = r.isPositive(statFilters, boosted);
            //                                newRecipeList.Add(r);
            //                            }
            //                        }
            //                        else
            //                            newRecipeList.Add(r);
            //                    }
            //                }
            //            }
            //        }

            //        recipeList = new List<Recipe>(newRecipeList);

            //        recipeList.Sort((a, b) => a.SorterAttr.CompareTo(b.SorterAttr));
            //        recipeList.Reverse();
            //        recipeList = recipeList.Take(200).ToList();
            //        masterRecipeList.AddRange(recipeList);
            //    }
            //}

            masterRecipeList.Sort((a, b) => a.SorterAttr.CompareTo(b.SorterAttr));
            masterRecipeList.Reverse();
            masterRecipeList = masterRecipeList.Take(200).ToList();

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

            foreach (Recipe r in masterRecipeList)
            {
                DataRow row = dt.NewRow();
                row["recipe"] = r.ToWikiString(cboRatio.Text);
                float[] results = r.Attributes(ratio);
                for (int j = 0; j < 8; j++)
                    row[j + 2] = (int)results[j];

                row["cost"] = r.Cost(ratio);
                row["time"] = new TimeSpan(0, 0, (int)results[7]);
                dt.Rows.Add(row);

                foreach (App_Code.Pair p in r.pairs.Values)
                {
                    if (!UpdatePairs.Contains(p.Key))
                        UpdatePairs.Add(p.Key);
                }
            }

            if (dt.Rows.Count > 0)
            {
                Session["RecipeTable"] = dt;
                string sortExpression = cboSort.Text;
                dt.DefaultView.Sort = sortExpression + " DESC";
                gvRecipes.DataSource = Session["RecipeTable"];
                gvRecipes.DataBind();

                gvRecipes.Visible = true;
                lblNoRecipes.Visible = false;

                DataTable dup = new DataTable();
                dup.Columns.Add("Pair", typeof(string));
                dup.Columns.Add("LastUpdated", typeof(DateTime));

               
                foreach (string s in UpdatePairs)
                {
                    App_Code.Pair p = Recipe.pairCache[s];
                    TimeSpan elapsed = DateTime.Now - p.LastUpdated;
                    if (elapsed > new TimeSpan(2, 0, 0, 0))
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
            DataTable dt = Session["RecipeTable"] as DataTable;
            dt.DefaultView.Sort = sortExpression + " " + GetSortDirection(sortExpression);

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
                gvRecipes.DataSource = Session["RecipeTable"];
                gvRecipes.DataBind();
            }
            else
                Response.Redirect("~/");
        }

        private string GetSortDirection(string column)
        {

            // By default, set the sort direction to ascending.
            string sortDirection = "ASC";

            // Retrieve the last column that was sorted.
            string sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null)
            {
                // Check if the same column is being sorted.
                // Otherwise, the default value can be returned.
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;
                    if ((lastDirection != null) && (lastDirection == "DESC"))
                    {
                        sortDirection = "ASC";
                    }
                }
            }

            // Save new values in ViewState.
            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }

        protected void gvUpdatePairs_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = Session["UpdatePairs"] as DataTable;
            if (dt != null && dt.Rows.Count > 0)
            {
                //Sort the data.
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                gvRecipes.DataSource = Session["UpdatePairs"];
                gvRecipes.DataBind();
            }
            else
                Response.Redirect("~/");
        }
    }

}