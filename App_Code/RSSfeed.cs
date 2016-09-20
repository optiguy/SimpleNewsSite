using System.Data.SqlClient;
using System.Web;
using System.Xml;

/// <summary>
/// Used to generate RSS feed
/// </summary>
public class RSSfeed
{
    public static void CreateRSSFromCategory(int id = 0)
    {
        string fileName = "alle";
        XmlDocument RSSDocument;
        SqlConnection conn = new SqlConnection(Helpers.ConnectionString);

        //1.
        RSSDocument = new XmlDocument(); //Set as new document
        //Set header for document
        XmlProcessingInstruction xpi = RSSDocument.CreateProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");
        RSSDocument.AppendChild(xpi); //Add header as the first thing in the document.

        //2.
        XmlElement rss = RSSDocument.CreateElement("rss"); //Create root element
        rss.SetAttribute("version", "2.0"); //Set version attribute
        RSSDocument.AppendChild(rss); //Add RSS tag with all the other informations

        //3.
        XmlElement channel = RSSDocument.CreateElement("channel");
        rss.AppendChild(channel);

        //4. 
        string title = "Alle Nyheder";
        string link = "http://localhost:16278/Default.aspx";
        string description = "De nyeste nyheder på vore hjemmeside.";
        
        if (id != 0) //Use category instead
        {
            //If a category is selected, then get the information for that category
            SqlCommand cmd_cat = new SqlCommand("SELECT * from categories WHERE category_id = @category_id", conn);
            cmd_cat.Parameters.AddWithValue("category_id", id);
            conn.Open();
            SqlDataReader reader_cat = cmd_cat.ExecuteReader();
            while (reader_cat.Read())
            {
                title = reader_cat["category_title"].ToString().ToLower();
                fileName = title; //Save category title as filename
                link = string.Format("http://localhost:16278/Categories.aspx?category_id={0}", reader_cat["category_id"].ToString());
                description = reader_cat["category_description"].ToString();
            }
            conn.Close();
        }

        XmlElement channel_title = RSSDocument.CreateElement("title");
        channel_title.AppendChild(RSSDocument.CreateTextNode(title));
        channel.AppendChild(channel_title);

        XmlElement channel_link = RSSDocument.CreateElement("link");
        channel_link.AppendChild(RSSDocument.CreateTextNode(link));
        channel.AppendChild(channel_link);

        XmlElement channel_description = RSSDocument.CreateElement("description");
        channel_description.AppendChild(RSSDocument.CreateTextNode(description));
        channel.AppendChild(channel_description);

        SqlCommand cmd = new SqlCommand("SELECT TOP(25) fk_categories_id, news_id, news_title, news_content, news_postdate FROM news ORDER BY news_postdate DESC", conn);
        if (id != 0) //Use category instead
        {
            cmd = new SqlCommand("SELECT TOP(25) fk_categories_id, news_id, news_title, news_content, news_postdate FROM news WHERE fk_categories_id = @category_id ORDER BY news_postdate DESC", conn);
            cmd.Parameters.AddWithValue("category_id", id);
        }

        conn.Open();
        SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            //5.
            XmlElement item = RSSDocument.CreateElement("item");
            channel.AppendChild(item);

            //6.
            XmlElement item_title = RSSDocument.CreateElement("title");
            item_title.AppendChild(RSSDocument.CreateTextNode(reader["news_title"].ToString()));
            item.AppendChild(item_title);

            XmlElement item_link = RSSDocument.CreateElement("link");
            string url = string.Format("http://localhost:16278/News.aspx?category_id={0}&news_id={1}", reader["fk_categories_id"].ToString(), reader["news_id"].ToString());
            item_link.AppendChild(RSSDocument.CreateTextNode(url));
            item.AppendChild(item_link);

            XmlElement item_description = RSSDocument.CreateElement("description");
            item_description.AppendChild(RSSDocument.CreateTextNode(reader["news_content"].ToString()));
            item.AppendChild(item_description);
        }
        conn.Close();

        //8. Gem fil
        RSSDocument.Save(HttpContext.Current.Server.MapPath("~/feeds/") + fileName + ".xml"); //Save XML Document in website folder
    }
}