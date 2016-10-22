using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using System.IO;
using HtmlAgilityPack;

namespace QuartzPullDataWindowService
{
    public class JobDemo : IJob
    {

        public void Execute(IJobExecutionContext context)
        {

            GetArticleInfo();

        }


        public void GetArticleInfo()
        {

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();

            //获取博客园精华文章
            doc = web.Load("http://www.cnblogs.com/pick/");

            HtmlNode htmlNode = doc.GetElementbyId("post_list");


            string path = "D:\\" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt";//AppDomain.CurrentDomain.BaseDirectory + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt";


            //判断文件路径是否存在，不存在则创建文件
            if (!System.IO.File.Exists(path))
            {
                FileStream ss = System.IO.File.Create(path);
                ss.Dispose();
            }


            StreamWriter sw = File.AppendText(path);

     


            //获取post_list下面的子节点
            foreach (var child in htmlNode.ChildNodes)
            {

                if (child.Attributes["class"] != null && child.Attributes["class"].Value == "post_item")
                {
                    ///如果用child.SelectSingleNode("//*[@class=\"titlelnk\"]").InnerText这样的方式查询，是永远以整个document为基准来查询，
                    ///这点就不好，理应以当前child节点的html为基准才对。

                    HtmlNode hn = HtmlNode.CreateNode(child.OuterHtml);//定位当前子节点

                    Article model = new Article();
                    model.Recommend = String.Format("推荐：{0}", hn.SelectSingleNode("//*[@class=\"diggnum\"]").InnerText);
                    model.Title = string.Format("标题:{0}", hn.SelectSingleNode("//*[@class=\"titlelnk\"]").InnerText);
                    model.Summary = string.Format("摘要:{0}", hn.SelectSingleNode("//*[@class=\"post_item_summary\"]").InnerText);
                    model.Author = string.Format("作者:{0}", hn.SelectSingleNode("//*[@class=\"lightblue\"]").InnerText);


                    sw.Write(model.Recommend+"\r\n");
                    sw.Write(model.Title + "\r\n");
                    sw.Write(model.Summary + "\r\n");
                    sw.Write(model.Author + "\r\n");
                    sw.Write("--------------------------------------------------------------------------------\r\n");

                }

            }

            sw.Close();
            Console.WriteLine("写入成功！"+System.DateTime.Now);
        
        }


    }


    public class Article
    {
        public string Recommend { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Author { get; set; }

    }
}
