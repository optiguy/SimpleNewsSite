using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

public partial class _Default : System.Web.UI.Page
{
    private SqlConnection conn = new SqlConnection(Helpers.ConnectionString);
    private int articlesPerPage;
    private int articlesTotal;
    private int articlesPages;
    private int currentPage;
    private int numberOfLinksBeforeAndAfter = 2;

    protected void Page_Load(object sender, EventArgs e)
    {
        GetUserSettingPerPage(); //Metode til at hente en bruger indstillling om hvor mange artikler der skal vises
        CountArticles(); //Metode der tæller hvor mange artikler der er i databasen
        SetPageCount(); //Metode der udregner hvor mange sider vi skal bruge
        GetCurrentPage(); //Metode der henter den nuværende side
        MakePaginationLinks();//Metode der laver HTML'en til side links'ne

        if (IsPostBack) return;

        GetArticles();
    }

    private void GetArticles()
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"
            SELECT news_id, news_title, news_content, news_postdate, category_id, category_title, user_name 
	        FROM news
	        INNER JOIN categories ON categories.category_id = news.fk_categories_id
	        INNER JOIN users ON users.user_id = news.fk_users_id
	        ORDER BY news_postdate DESC
            OFFSET @skip ROWS
            FETCH NEXT @perPage ROWS ONLY";

        cmd.Parameters.AddWithValue("skip", ((currentPage - 1) * articlesPerPage));
        cmd.Parameters.AddWithValue("perPage", articlesPerPage);

        conn.Open();
        Repeater_Frontpage.DataSource = cmd.ExecuteReader();
        Repeater_Frontpage.DataBind();
        conn.Close();
    }

    private void GetUserSettingPerPage()
    {
        if (Request.Cookies["UserSettings"] == null)
        {
            articlesPerPage = 5;
        }
        else
        {
            int perPage;
            if (!int.TryParse(Request.Cookies["UserSettings"]["perPage"], out perPage))
            {
                perPage = 5;
            }
            articlesPerPage = perPage;
        }
    }

    private void GetCurrentPage()
    {
        currentPage = 1;
        if (Request.QueryString["articlePage"] != null)
        {
            if (!int.TryParse(Request.QueryString["articlePage"].ToString(), out currentPage))
            {
                currentPage = 1;
            }
        }
    }

    private void MakePaginationLinks()
    {
        string linksHtml = "";

        string disabledClassFirst = "";
        if (currentPage == 1)
        {
            disabledClassFirst = "class='disabled'";
        }
        linksHtml += string.Format("<li {1}><a href='?articlePage={0}' aria-label='First'><span aria-hidden='true'>&laquo;</span></a></li>", 1, disabledClassFirst);


        //if (currentPage != 1)
        //{
        //    linksHtml += string.Format("<li><a href='?articlePage={0}' aria-label='Previous'><span aria-hidden='true'>&laquo;</span></a></li>", currentPage - 1);
        //}

        for (int i = 0; i < articlesPages; i++)
        {
            if (((i + 1) >= (currentPage - numberOfLinksBeforeAndAfter)) && (i + 1) <= (currentPage + numberOfLinksBeforeAndAfter))
            {
                string extraClass = "";
                if ((i + 1) == currentPage)
                {
                    extraClass = "class='active'";
                }
                linksHtml += string.Format("<li {1}><a href='?articlePage={0}'>{0}</a></li>", i + 1, extraClass);
            }
        }

        //if (currentPage != articlesPages)
        //{
        //    linksHtml += string.Format("<li><a href='?articlePage={0}' aria-label='Next'><span aria-hidden='true'>&raquo;</span></a></li>", currentPage + 1);
        //}

        string disabledClassLast = "";
        if (currentPage == articlesPages)
        {
            disabledClassLast = "class='disabled'";
        }
        linksHtml += string.Format("<li {1}><a href='?articlePage={0}' aria-label='Last'><span aria-hidden='true'>&raquo;</span></a></li>", articlesPages, disabledClassLast);

        PaginationLinks.Text = linksHtml;
    }

    private void SetPageCount()
    {
        articlesPages = (int)Math.Ceiling((double)articlesTotal / (double)articlesPerPage);
    }

    private void CountArticles()
    {
        SqlCommand count = new SqlCommand();
        count.Connection = conn;
        count.CommandText = @"SELECT COUNT(news_id) FROM news";
        conn.Open();
        int.TryParse(count.ExecuteScalar().ToString(), out articlesTotal);
        conn.Close();
    }
    protected void Button_ChangePerPage(object sender, System.Web.UI.WebControls.CommandEventArgs e)
    {
        int perPage;
        if (!int.TryParse(e.CommandArgument.ToString(), out perPage))
        {
            perPage = 5;
        }
        articlesPerPage = perPage;

        HttpCookie myCookie = new HttpCookie("UserSettings");
        myCookie["perPage"] = perPage.ToString();
        myCookie.Expires = DateTime.Now.AddDays(1d); //Cookie overlever én dag
        Response.Cookies.Add(myCookie);

        GetArticles();
        SetPageCount();
        MakePaginationLinks();
    }
}