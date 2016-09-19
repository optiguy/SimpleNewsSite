<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage_Frontend.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    Vis per side: 
    <div class="btn-group" role="group" aria-label="Antal per side">
        <asp:Button Text="5" OnCommand="Button_ChangePerPage" CommandArgument="5" CssClass="btn btn-default" runat="server" />
        <asp:Button Text="10" OnCommand="Button_ChangePerPage" CommandArgument="10" CssClass="btn btn-default" runat="server" />
        <asp:Button Text="25" OnCommand="Button_ChangePerPage" CommandArgument="25" CssClass="btn btn-default" runat="server" />
        <asp:Button Text="50" OnCommand="Button_ChangePerPage" CommandArgument="50" CssClass="btn btn-default" runat="server" />
        <asp:Button Text="100" OnCommand="Button_ChangePerPage" CommandArgument="100" CssClass="btn btn-default" runat="server" />
    </div>

    <asp:Repeater ID="Repeater_Frontpage" runat="server">
        <ItemTemplate>
            <section class="news_category">
                <h3><%# Eval("news_title") %></h3>
                <p><a href="News.aspx?category_id=<%# Eval("category_id") %>&amp;news_id=<%# Eval("news_id") %>"><%# Helpers.EvalTrimmed(Eval("news_content").ToString(), 250) %></a></p>
                <em>af: <%# Eval("user_name") %>, i kategorien: <%# Eval("category_title") %>, den. <%# ((DateTime)Eval("news_postdate")).ToString("dd. MMMM yyyy - HH:mm") %></em><hr />
            </section>
        </ItemTemplate>
    </asp:Repeater>

    <nav aria-label="Page navigation">
        <ul class="pagination">
            <asp:Literal Text="" ID="PaginationLinks" runat="server" />
        </ul>
    </nav>

</asp:Content>

