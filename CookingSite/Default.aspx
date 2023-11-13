<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CookingSite._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main>
        <table id="tblRecipes">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td class="centered" rowspan="2">
                                &nbsp;&nbsp;
                            </td>
                            <td class="centered">STR</td>
                            <td class="centered">DEX</td>
                            <td class="centered">END</td>
                            <td class="centered">SPD</td>
                            <td class="centered">CON</td>
                            <td class="centered">FOC </td>
                            <td class="centered">PER</td>
                            <td rowspan="2">&nbsp;&nbsp;</td>
                            <td rowspan="2">
                                Sort&nbsp;
                                <asp:DropDownList ID="cboSort" runat="server" Width="80" AutoPostBack="true" OnSelectedIndexChanged="cboSort_SelectedIndexChanged">
                                    <asp:ListItem>STR</asp:ListItem>
                                    <asp:ListItem>DEX</asp:ListItem>
                                    <asp:ListItem>END</asp:ListItem>
                                    <asp:ListItem>SPD</asp:ListItem>
                                    <asp:ListItem>CON</asp:ListItem>
                                    <asp:ListItem>FOC</asp:ListItem>
                                    <asp:ListItem>PER</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>&nbsp;&nbsp;</td>
                            <td rowspan="2">
                                Ratio&nbsp;
                                <asp:DropDownList ID="cboRatio" runat="server">
                                    <asp:ListItem>6:1</asp:ListItem>
                                    <asp:ListItem>13:1</asp:ListItem>
                                    <asp:ListItem>4:3</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>&nbsp;&nbsp;</td>
                            <td rowspan="2">
                                Cooking Level&nbsp;
                                <asp:DropDownList ID="cboCooklevel" runat="server">
                                    <asp:ListItem>0</asp:ListItem>
                                    <asp:ListItem>1</asp:ListItem>
                                    <asp:ListItem>2</asp:ListItem>
                                    <asp:ListItem>3</asp:ListItem>
                                    <asp:ListItem>4</asp:ListItem>
                                    <asp:ListItem>5</asp:ListItem>
                                    <asp:ListItem>6</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td rowspan="2">&nbsp;&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="centered">
                                <asp:CheckBox ID="chkSTR" runat="server" />
                            </td>
                            <td class="centered">
                                <asp:CheckBox ID="chkDEX" runat="server" />
                            </td>
                            <td class="centered">
                                <asp:CheckBox ID="chkEND" runat="server" />
                            </td>
                            <td class="centered">
                                <asp:CheckBox ID="chkSPD" runat="server" />
                            </td>
                            <td class="centered">
                                <asp:CheckBox ID="chkCON" runat="server" />
                            </td>
                            <td class="centered">
                                <asp:CheckBox ID="chkFOC" runat="server" />
                            </td>
                            <td class="centered">
                                <asp:CheckBox ID="chkPER" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>&nbsp;&nbsp;</td>
                            <td>
                                Multiplier&nbsp;<asp:TextBox ID="txtServings" runat="server" Width="50"></asp:TextBox>
                            </td>
                            <td>&nbsp;&nbsp;</td>
                            <td>
                                <asp:CheckBox ID="chkPosOnly" runat="server" Checked="true" ToolTip="Filter results by positive or 0 selected attributes only" />&nbsp;Pos Attr Only
                            </td>
                            <td>&nbsp;&nbsp;</td>
                            <td>
                                <asp:CheckBox ID="chkInvLimit" runat="server" Checked="true" ToolTip="Limit to qty of ingredients on hand" />&nbsp;Inv Limited
                            </td>
                            <td>&nbsp;&nbsp;</td>
                            <td>
                                <asp:CheckBox ID="chkBoosted" runat="server" Checked="false" ToolTip="Filter by selected stats > 0" />&nbsp;Boosted
                            </td>
                            <td rowspan="2">&nbsp;&nbsp;</td>
                            <td rowspan="2">
                               <asp:Button ID="Button1" runat="server" Text="Generate" OnClick="Button1_Click" UseSubmitBehavior="false" OnClientClick="this.disabled='true'; this.value='Please wait...';"/>
                            </td>
                            <td>&nbsp;&nbsp;</td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:GridView ID="gvRecipes" runat="server" AllowSorting="True" OnSorting="gvRecipes_Sorting" BorderWidth="1" BorderColor="Black" BorderStyle="Solid" CellPadding="5" AlternatingRowStyle-BackColor="#CCFFFF">
                                </asp:GridView>
                                <asp:Label ID="lblNoRecipes" runat="server" Font-Bold="True">No Recipes Found Matching Criteria</asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <br />
                                <asp:Label ID="lblRecipeCount" runat="server" Text="Recipes Shown: 0"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                Pairs that might need updating
                                <br />
                                <asp:GridView ID="gvUpdatePairs" runat="server" AllowSorting="True" OnSorting="gvUpdatePairs_Sorting" BorderWidth="1" BorderColor="Black" BorderStyle="Solid" CellPadding="5" AlternatingRowStyle-BackColor="#CCFFFF"></asp:GridView>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblDebug" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
    </main>

</asp:Content>
