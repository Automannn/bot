using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;
using NSoup;
using System.Text;
using System.Net;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Bot_Application3.Dialogs
{

    public class Init_DB
    {
        public MySqlConnection get_Init_DB()
        {
            string constir = "server=localhost; User Id=root; password=chenkaihai; database=robot";
            MySqlConnection mycon = new MySqlConnection(constir);
            return mycon;
        }

    }



    public class Intention
    {

        //@@@intention_type

        public string intention_study = "study";
        public string intention_conversition = "conversition";
        public string intention_action = "action";
        public string intention_check = "check";
        public string intention_train = "train";
    }

    public class Train
    {
        string current_Ssubject = "";
        string current_Predicate = "";
        string current_Accusative = "";
        int current_Ssubject_id = 0;
        int current_Predicate_id = 0;
        int current_Accusative_id = 0;

        public int train_intentid(string ikoktest, List<string> allSsubject, List<string> allPredicate, List<string> allAccustive)
        {
            int theneed = 0;
            foreach (string every in allPredicate)
            {
                MatchCollection mc_train_words_confirm = Regex.Matches(ikoktest, "(?<Ssubjective>.{0,3})" + every + "(?<accusative>.{1,3})[\\.;。；]*");

                bool confirm = Regex.IsMatch(ikoktest, "(?<Ssubjective>.{0,3})" + every + "(?<accusative>.{1,3})[\\.;。；]*");
                if (confirm)
                {
                    current_Predicate = every;
                    foreach (Match item in mc_train_words_confirm)
                    {
                        current_Ssubject = item.Groups["Ssubjective"].Value;
                        current_Accusative = item.Groups["accusative"].Value;
                    }
                    //改变基本元素的权值
                    MySqlConnection conn = new Init_DB().get_Init_DB();
                    conn.Open();
                    MySqlCommand mycmd_1 = new MySqlCommand("update Ssubject set authority = authority+1 where value = '"
                        + current_Ssubject + "';", conn);
                    MySqlCommand mycmd_2 = new MySqlCommand("update predicate set authority= authority +1 where value = '"
                        + current_Predicate + "';", conn);
                    MySqlCommand mycmd_3 = new MySqlCommand("update accusative set authority= authority +1 where value = '"
                        + current_Accusative + "';", conn);
                    if (mycmd_1.ExecuteNonQuery() > 0)
                    {
                        MySqlDataReader mr1 = new MySqlCommand("select id from Ssubject where value = '"
                       + current_Ssubject + "';", conn).ExecuteReader();
                        while (mr1.Read())
                        {
                            current_Ssubject_id = Int16.Parse(mr1[0].ToString());
                        }
                        mr1.Close();
                    }
                    if (mycmd_3.ExecuteNonQuery() > 0)
                    {
                        MySqlDataReader mr2 = new MySqlCommand("select id from accusative where value = '"
                        + current_Accusative + "';", conn).ExecuteReader();
                        while (mr2.Read())
                        {
                            current_Accusative_id = Int16.Parse(mr2[0].ToString());
                        }
                        mr2.Close();

                    }
                    if (mycmd_2.ExecuteNonQuery() > 0)
                    {
                        MySqlDataReader mr3 = new MySqlCommand("select id from predicate where value = '"
                        + current_Predicate + "';", conn).ExecuteReader();
                        while (mr3.Read())
                        {
                            current_Predicate_id = Int16.Parse(mr3[0].ToString());
                        }
                        mr3.Close();
                    }



                    //改变对应关系的权值
                    MySqlConnection conn1 = new Init_DB().get_Init_DB();
                    conn1.Open();
                    if (current_Ssubject_id != 0)
                    {
                        MySqlCommand currentcmd_1 = new MySqlCommand("select * from s_p where s_id = " + current_Ssubject_id + " and p_id = " + current_Predicate_id, conn1);
                        if (currentcmd_1.ExecuteNonQuery() > 0)
                        {
                            MySqlCommand thecmd = new MySqlCommand("update s_p set authority = authority+1  where s_id =" + current_Ssubject_id + " and p_id =" + current_Predicate_id, conn1);
                            thecmd.ExecuteNonQuery();
                        }
                        else
                        {
                            MySqlCommand thecmd = new MySqlCommand("insert into s_p(s_id,p_id)  values(" + current_Ssubject_id + "," + current_Predicate_id + ")", conn1);
                            thecmd.ExecuteNonQuery();
                        }
                    }
                    if (current_Accusative_id != 0)
                    {
                        MySqlCommand currentcmd_2 = new MySqlCommand("select p_id,a_id from p_a where p_id =" + current_Predicate_id + " and a_id =" + current_Accusative_id, conn1);
                        if (currentcmd_2.ExecuteNonQuery() > 0)
                        {
                            MySqlCommand thecmd = new MySqlCommand("update p_a set authority = authority+1  where p_id =" + current_Predicate_id + " and a_id = " + current_Accusative_id, conn1);
                            thecmd.ExecuteNonQuery();
                        }
                        else
                        {
                            MySqlCommand thecmd = new MySqlCommand("insert into p_a(p_id,a_id)  values(" + current_Predicate_id + "," + current_Accusative_id + ")", conn1);
                            thecmd.ExecuteNonQuery();
                        }
                    }
                    MySqlDataReader mr = new MySqlCommand("select id from s_p where s_id =" + current_Ssubject_id + " and p_id =" + current_Predicate_id, conn1).ExecuteReader();
                    while (mr.Read())
                    {
                        theneed = Int16.Parse(mr[0].ToString());
                    }
                    mr.Close();
                    conn1.Close();
                }
                return theneed;
            }
            return theneed;
        }
    }

        public class Current
        {
            public string current_Ssubject = "";
            public int current_Intent_id = 0;
        }

        public class Talking_unit
        {
            public int height = 0;
            public int type = 0;
            public string question = "";
            public string answer = "";
            public string order = "";

            public void initTalking_unit()
            {
                this.height = 0;
                this.type = 0;
                this.question = "";
                this.answer = "";
                this.order = "";
            }

        }

        public class GetIntent
        {

            public string getMain(string data, List<string> l1, List<string> l2, List<string> l3)
            {
                string theneed = "";
                foreach (string temp in l2)
                {
                    bool is1 = Regex.IsMatch(data, ".+" + temp + ".+");
                    if (is1)
                    {
                        foreach (string temp1 in l1)
                        {
                            bool is2 = Regex.IsMatch(data, "" + temp1 + ".*" + temp + ".+");
                            if (is2)
                            {
                                foreach (string temp2 in l3)
                                {
                                    bool is3 = Regex.IsMatch(data, "" + temp1 + ".*" + temp + ".*" + temp2);
                                    if (is3)
                                    {
                                        theneed = "@" + temp + "@" + temp2;
                                        return theneed;
                                    }
                                }
                            }
                        }
                    }
                }
                theneed = data;
                return theneed;
            }
        }


        [Serializable]
        public class RootDialog : IDialog<object>
        {
            string current_Ssubject = "";
            string current_Predicate = "";
            string current_Accusative = "";
            int current_Ssubject_id = 0;
            int current_Predicate_id = 0;
            int current_Accusative_id = 0;

            int current_intent_id = 0;

            string thecache = "";
            string intention = "";
            int talking_depth = 0;
            Talking_unit talking_unit = new Talking_unit();
            List<string> allSsubjects = new List<string>();
            List<string> allPredicate = new List<string>();
            List<string> allAccusative = new List<string>();
            List<Talking_unit> talking_units = new List<Talking_unit>();

            public Task StartAsync(IDialogContext context)
            {
                context.Wait(MessageReceivedAsync);

                return Task.CompletedTask;
            }

            private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
            {

                var activity = await result as Activity;
                string ikoktest = activity.Text;
                //byte[] utf8bytes = System.Text.Encoding.Default.GetBytes(ikoktest);
                //byte[] utf8bytes2 = System.Text.Encoding.Convert(System.Text.Encoding.Default, System.Text.Encoding.UTF8, utf8bytes);
                //utf8bytes2.ToString();



                //当前文有重复的基本意图的时候，取到最近的基本意图
                if (intention.Split(',').Length > 1 && !intention.Equals("")) intention = intention.Substring(intention.LastIndexOf(",") + 1);

                //@@@intention_head

                MatchCollection mc_study_words = Regex.Matches(ikoktest, ".*教.*你(一些)*(部分)*(一点)*(?<100>.{1,2})词");
                bool is_study = Regex.IsMatch(ikoktest, ".*教.*你(一些)*(部分)*(一点)*(?<100>.{1,2})词");
                if (is_study)
                {
                    //当前文已经有基本意图的时候，为后文追加的基本意图进行格式的准备
                    //if (intention != null && !intention.Equals("")) intention += ",";
                    intention = new Intention().intention_study;
                    talking_depth = 0;
                }

                MatchCollection mc_action = Regex.Matches(ikoktest, "[.]*放(一首){0,1}(?<musician>.*)(的){0,1}歌[\\.。!]*");
                bool is_action = Regex.IsMatch(ikoktest, "[.]*放(一首){0,1}(.*)(的){0,1}歌[\\.。!]*");
                if (is_action)
                {
                    //当前文已经有基本意图的时候，为后文追加的基本意图进行格式的准备
                    //if (intention != null && !intention.Equals("")) intention += ",";
                    intention = new Intention().intention_action;
                    talking_depth = 0;
                }

                MatchCollection mc_check = Regex.Matches(ikoktest, "(.*查(一下){0,1}(?<check>.+)(是什么){0,1}|(?<check>.+)是(什么|啥))|(?<check>^怎么{0,1}样{0,1}.+)");
                bool is_check = Regex.IsMatch(ikoktest, "(.*查(一下){0,1}(?<check>.+)(是什么){0,1}|(?<check>.+)是(什么|啥))|(?<check>^怎么{0,1}样{0,1}.+)");
                if (is_check)
                {
                    //当前文已经有基本意图的时候，为后文追加的基本意图进行格式的准备
                    //if (intention != null && !intention.Equals("")) intention += ",";
                    intention = new Intention().intention_check;
                    talking_depth = 0;
                }

                MatchCollection mc_train = Regex.Matches(ikoktest, ".*(训练模式|训练你)");
                bool is_train = Regex.IsMatch(ikoktest, ".*(训练模式|训练你)");
                if (is_train)
                {
                    //当前文已经有基本意图的时候，为后文追加的基本意图进行格式的准备
                    //if (intention != null && !intention.Equals("")) intention += ",";
                    intention = new Intention().intention_train;
                    talking_depth = 0;
                }


                if (intention.Equals("train") || intention == "train")
                {

                    foreach (Match item in mc_train)
                    {
                        await context.PostAsync("请问您是要开启训练模式吗？");
                    }
                    int temp = talking_depth + 1;
                    if (temp == 1)
                    {

                        MatchCollection mc_study_words_confirm = Regex.Matches(ikoktest, "^[对是好嗯]+的*");
                        bool confirm = Regex.IsMatch(ikoktest, "^[对是好嗯]+的*");
                        if (confirm)
                        {
                            MySqlConnection conn = new Init_DB().get_Init_DB();
                            conn.Open();
                            MySqlCommand mycmd_s = new MySqlCommand("select value from Ssubject", conn);
                            MySqlCommand mycmd_p = new MySqlCommand("select value from predicate", conn);
                            MySqlCommand mycmd_a = new MySqlCommand("select value from accusative", conn);
                            string result_s = "";
                            string result_p = "";
                            string result_a = "";
                            MySqlDataReader reader = null;
                            reader = mycmd_s.ExecuteReader();
                            while (reader.Read())
                            {
                                allSsubjects.Add(reader[0].ToString());
                                result_s += reader[0].ToString() + ",";
                            }
                            reader.Close();
                            reader = mycmd_p.ExecuteReader();
                            while (reader.Read())
                            {
                                allPredicate.Add(reader[0].ToString());
                                result_p += reader[0].ToString() + ",";
                            }
                            reader.Close();
                            reader = mycmd_a.ExecuteReader();
                            while (reader.Read())
                            {
                                allAccusative.Add(reader[0].ToString());
                                result_a += reader[0].ToString() + ",";
                            }

                            result_s = result_s.Remove(result_s.LastIndexOf(","), 1);
                            result_p = result_p.Remove(result_s.LastIndexOf(","), 1);
                            result_a = result_a.Remove(result_s.LastIndexOf(","), 1);
                            talking_depth++;
                            await context.PostAsync("好的，我已经学会的主语有： " + result_s + "。谓语有：" + result_p + "。宾语有：" + result_a + "。您可以在此基础上训练我，也可以继续让我学习哦~~");
                            reader.Close();
                            conn.Close();

                        }
                    }
                    if (temp == 2)
                    {
                        foreach (string every in allPredicate)
                        {
                            await context.PostAsync(every);
                            MatchCollection mc_train_words_confirm = Regex.Matches(ikoktest, "(?<Ssubjective>.{0,3})" + every + "(?<accusative>.{1,3})[\\.;。；]*");

                            bool confirm = Regex.IsMatch(ikoktest, "(?<Ssubjective>.{0,3})" + every + "(?<accusative>.{1,3})[\\.;。；]*");
                            if (confirm)
                            {
                                current_Predicate = every;
                                foreach (Match item in mc_train_words_confirm)
                                {
                                    current_Ssubject = item.Groups["Ssubjective"].Value;
                                    current_Accusative = item.Groups["accusative"].Value;
                                }
                                //改变基本元素的权值
                                MySqlConnection conn = new Init_DB().get_Init_DB();
                                conn.Open();
                                MySqlCommand mycmd_1 = new MySqlCommand("update Ssubject set authority = authority+1 where value = '"
                                    + current_Ssubject + "';", conn);
                                MySqlCommand mycmd_2 = new MySqlCommand("update predicate set authority= authority +1 where value = '"
                                    + current_Predicate + "';", conn);
                                MySqlCommand mycmd_3 = new MySqlCommand("update accusative set authority= authority +1 where value = '"
                                    + current_Accusative + "';", conn);
                                if (mycmd_1.ExecuteNonQuery() > 0)
                                {
                                    MySqlDataReader mr = new MySqlCommand("select id from Ssubject where value = '"
                                   + current_Ssubject + "';", conn).ExecuteReader();
                                    while (mr.Read())
                                    {
                                        current_Ssubject_id = Int16.Parse(mr[0].ToString());
                                    }
                                    mr.Close();
                                }
                                if (mycmd_3.ExecuteNonQuery() > 0)
                                {
                                    MySqlDataReader mr = new MySqlCommand("select id from accusative where value = '"
                                    + current_Accusative + "';", conn).ExecuteReader();
                                    while (mr.Read())
                                    {
                                        current_Accusative_id = Int16.Parse(mr[0].ToString());
                                    }
                                    mr.Close();

                                }
                                if (mycmd_2.ExecuteNonQuery() > 0)
                                {
                                    MySqlDataReader mr = new MySqlCommand("select id from predicate where value = '"
                                    + current_Predicate + "';", conn).ExecuteReader();
                                    while (mr.Read())
                                    {
                                        current_Predicate_id = Int16.Parse(mr[0].ToString());
                                    }
                                    mr.Close();
                                }



                                //改变对应关系的权值
                                MySqlConnection conn1 = new Init_DB().get_Init_DB();
                                conn1.Open();
                                if (current_Ssubject_id != 0)
                                {
                                    MySqlCommand currentcmd_1 = new MySqlCommand("select * from s_p where s_id = " + current_Ssubject_id + " and p_id = " + current_Predicate_id, conn1);
                                    if (currentcmd_1.ExecuteNonQuery() > 0)
                                    {
                                        MySqlCommand thecmd = new MySqlCommand("update s_p set authority = authority+1  where s_id =" + current_Ssubject_id + " and p_id =" + current_Predicate_id, conn1);
                                        thecmd.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        MySqlCommand thecmd = new MySqlCommand("insert into s_p(s_id,p_id)  values(" + current_Ssubject_id + "," + current_Predicate_id + ")", conn1);
                                        thecmd.ExecuteNonQuery();
                                    }
                                }
                                if (current_Accusative_id != 0)
                                {
                                    MySqlCommand currentcmd_2 = new MySqlCommand("select p_id,a_id from p_a where p_id =" + current_Predicate_id + " and a_id =" + current_Accusative_id, conn1);
                                    if (currentcmd_2.ExecuteNonQuery() > 0)
                                    {
                                        MySqlCommand thecmd = new MySqlCommand("update p_a set authority = authority+1  where p_id =" + current_Predicate_id + " and a_id = " + current_Accusative_id, conn1);
                                        thecmd.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        MySqlCommand thecmd = new MySqlCommand("insert into p_a(p_id,a_id)  values(" + current_Predicate_id + "," + current_Accusative_id + ")", conn1);
                                        thecmd.ExecuteNonQuery();
                                    }
                                }
                                conn1.Close();
                                await context.PostAsync("本次训练完成！");
                                break;
                            }
                        }

                    }




                }





                if (intention.Equals("check") || intention == "check")
                {

                    foreach (Match item in mc_check)
                    {
                        await context.PostAsync("请问您是要查" + item.Groups["check"].ToString() + "吗？");
                        thecache = item.Groups["check"].ToString();
                        await context.PostAsync(thecache);
                    }
                    int temp = talking_depth + 1;
                    if (temp == 1)
                    {

                        MatchCollection mc_study_words_confirm = Regex.Matches(ikoktest, "^[对是好嗯]+的*");
                        bool confirm_check = Regex.IsMatch(ikoktest, "^[对是好嗯]+的*");
                        if (confirm_check)
                        {

                            await context.PostAsync("正在为您执行命令....");
                            talking_depth++;
                            if (thecache != null && !thecache.Equals(""))
                            {
                                await context.PostAsync("正在为您查询....");
                                //thecache=System.Web.HttpUtility.UrlEncode(thecache, Encoding.UTF8);
                                if (Regex.IsMatch(thecache, "^怎么{0,1}样{0,1}"))
                                {

                                    org.jsoup.nodes.Document docsource = org.jsoup.Jsoup.connect("https://jingyan.baidu.com/search?word=" + thecache).get();
                                    // await context.PostAsync(HtmlString);
                                    //NSoup.Nodes.Document doc = NSoup.NSoupClient.Connect("https://jingyan.baidu.com/search?word=" + thecache).Get();
                                    //await context.PostAsync(doc.ToString());
                                    //NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(HtmlString);
                                    //await context.PostAsync("runrurun2"+doc.Html());
                                    // WebClient webClient = new WebClient();
                                    //webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36");

                                    // String HtmlString = Encoding.GetEncoding("utf-8").GetString(webClient.DownloadData("https://jingyan.baidu.com/search?word=" + thecache));
                                    NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(docsource.body().toString());
                                    NSoup.Select.Elements e = doc.Select(".search-list").Select("dl").First.Select("dt").Select("a");
                                    await context.PostAsync(e.ToString());
                                    string url = e.Attr("href");
                                    String aim_Html = org.jsoup.Jsoup.connect("https://jingyan.baidu.com" + url).get().toString();
                                    NSoup.Nodes.Document doc_1 = NSoup.NSoupClient.Parse(aim_Html);
                                    string aimcontent = doc_1.Select("div.exp-content-block").Select("li[class^=exp-content-list]").Text;
                                    aimcontent.Replace("步骤阅读", "");
                                    aimcontent.Replace("END", "");
                                    Regex.Replace(aimcontent, "\\s[0-9]{0,2}\\s", "\\n\\s[0-9]{0,2}\\s");
                                    await context.PostAsync(aimcontent);
                                }
                                else
                                {
                                    thecache = thecache.Replace("是什么", "");
                                    String HtmlString = Encoding.GetEncoding("utf-8").GetString(new WebClient().DownloadData("https://baike.baidu.com/item/" + thecache));
                                    NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(HtmlString);
                                    await context.PostAsync(doc.Select(".lemma-summary").First.Text());
                                }
                                //NSoup.Nodes.Document doc = NSoup.NSoupClient.Connect("https://baike.baidu.com/item/JJ").Get();
                                //Console.Write(doc.Text());
                                await context.PostAsync("查询成功，欢迎下次光临");

                            }
                        }

                    }




                }




                if (intention.Equals("action") || intention == "action")
                {

                    foreach (Match item in mc_action)
                    {
                        await context.PostAsync("您要放" + item.Groups["musician"].ToString() + "歌吗？");
                    }
                    int temp = talking_depth + 1;
                    if (temp == 1)
                    {

                        MatchCollection mc_study_words_confirm = Regex.Matches(ikoktest, "[对是好嗯]+的*");
                        bool confirm = Regex.IsMatch(ikoktest, "[对是好嗯]+的*");
                        if (confirm)
                        {
                            await context.PostAsync("正在为您执行命令....");
                            Process myProcess = new Process();
                            // try{
                            myProcess.StartInfo.UseShellExecute = false;
                            myProcess.StartInfo.FileName = "C:\\Program Files (x86)\\kuwo\\kuwomusic\\8.5.2.0_UG6\\bin\\KwMusic.exe";
                            myProcess.StartInfo.CreateNoWindow = true;
                            myProcess.Start();
                            //}
                            //catch (Exception e){

                            //}
                        }

                    }




                }





                if (intention.Equals("study") || intention == "study")
                {

                    foreach (Match item in mc_study_words)
                    {
                        await context.PostAsync("您要教我" + item.Groups[100].ToString() + "词吗？");
                    }
                    int temp = talking_depth + 1;
                    if (temp == 1)
                    {

                        MatchCollection mc_study_words_confirm = Regex.Matches(ikoktest, "^[对是好嗯]+的*");
                        bool confirm = Regex.IsMatch(ikoktest, "^[对是好嗯]+的*");
                        if (confirm)
                        {
                            talking_depth++;
                            await context.PostAsync("请问它们将要在句子中作什么成分？我暂时只能学习主语，谓语和宾语哟~~");
                        }

                    }

                    if (temp == 2)
                    {

                        MatchCollection mc_study_words_confirm = Regex.Matches(ikoktest, "^(?<grammer>[主谓宾])语{0,1}");

                        bool confirm = Regex.IsMatch(ikoktest, "^(?<grammer>[主谓宾])语{0,1}");
                        if (confirm)
                        {
                            foreach (Match item in mc_study_words_confirm) { thecache = item.Groups["grammer"].Value; }
                            talking_depth++;
                            await context.PostAsync("好的，我知道了。。您请说吧！");
                        }

                    }

                    if (temp == 3)
                    {
                        MatchCollection mc_study_words_content = Regex.Matches(ikoktest, "([^\\d]{1,2})[,，\\.。；;]*");
                        string aim_table = "";
                        if (thecache.Equals("主")) aim_table = "Ssubject";
                        if (thecache.Equals("谓")) aim_table = "predicate";
                        if (thecache.Equals("宾")) aim_table = "accusative";
                        if (aim_table != null && !aim_table.Equals(""))
                            foreach (Match item in mc_study_words_content)
                            {
                                await context.PostAsync("您说的词语是:" + item.Groups[1].Value + ",正在学习....");
                                MySqlConnection conn = new Init_DB().get_Init_DB();
                                conn.Open();
                                Regex reg = new Regex("[,，\\.。；;]");
                                string content = reg.Replace(item.Groups[1].Value, "");
                                MySqlCommand mycmd = new MySqlCommand("insert into " + aim_table + "(value) values('" + content + "')", conn);
                                if (mycmd.ExecuteNonQuery() > 0)
                                {
                                    await context.PostAsync("已成功学习！");
                                }
                                conn.Close();
                            }
                        intention = "";
                    }


                }

                //@@@intention_body
                if (intention == "" || intention.Equals(""))
                {
                    await context.PostAsync("不知道您要干什么呢~~需要学习吗？");
                    int temp = talking_depth + 1;
                    if (temp == 1)
                    {
                        current_intent_id = new Train().train_intentid(ikoktest, allSsubjects, allPredicate, allAccusative);
                        MatchCollection mc_study_words_confirm = Regex.Matches(ikoktest, "^[对是好嗯]+的*");
                        bool confirm = Regex.IsMatch(ikoktest, "^[对是好嗯]+的*");
                        if (confirm)
                        {
                            MySqlConnection conn = new Init_DB().get_Init_DB();
                            conn.Open();
                            MySqlDataReader mr = new MySqlCommand("select id from intent where value = '"
                                    + thecache + "';", conn).ExecuteReader();
                            while (mr.Read())
                            {
                                Int16.Parse(mr[0].ToString());
                            }
                            mr.Close();
                            intention = "studying";
                            await context.PostAsync("好的，请遵照提示配置您的对话模板。");
                            await context.PostAsync("请输入您的命令类型：（a）对话型命令或者(b)功能型！默认为对话型");
                        }

                    }

                }

                if (intention == "studying" || intention.Equals("studying"))
                {



                    MatchCollection mc_study_words_confirm = Regex.Matches(ikoktest, "b");
                    bool confirm = Regex.IsMatch(ikoktest, "b");

                    int temp = talking_depth + 1;
                    if (temp == 1)
                    {
                        if (confirm)
                        {
                            talking_unit.type = 1;
                            talking_depth++;
                            await context.PostAsync("请输入您要完成的功能：（目前已经实现的功能有：打开，关闭，查询，订购。更多功能正在开发中！请根据提示回复必要的信息已实现具体的功能！）");
                        }
                        else
                        {
                            talking_depth++;
                            await context.PostAsync("请输入您的问题");
                        }
                    }

                    if (temp == 2)
                    {

                        MatchCollection mc_function_confirm = Regex.Matches(ikoktest, "^(?<function>打开|关闭|查询|订购)(?=\\s)");
                        bool confirm1 = Regex.IsMatch(ikoktest, "^(?<function>打开|关闭|查询|订购)(?=\\s)");
                        if (confirm1)
                        {
                            foreach (Match item in mc_function_confirm) { thecache = item.Groups["function"].Value; }
                            talking_unit.order = thecache;
                            talking_units.Add(talking_unit);
                            talking_unit.initTalking_unit();
                            await context.PostAsync("是否需要进一步的操作？");
                            talking_depth = 3;
                        }
                        else
                        {
                            string question = new GetIntent().getMain(ikoktest, allSsubjects, allPredicate, allAccusative);
                            talking_unit.question = question;
                            talking_depth++;
                            await context.PostAsync("请输入该问题的回答");
                        }

                    }
                    if (temp == 3)
                    {
                        string answer = ikoktest;
                        talking_unit.answer = answer;
                        talking_depth++;
                        await context.PostAsync("是否需要进一步的操作？");
                    }
                    if (temp == 4)
                    {
                        MatchCollection mc_confirm = Regex.Matches(ikoktest, "^[对是好嗯]+的*");
                        bool confirm2 = Regex.IsMatch(ikoktest, "^[对是好嗯]+的*");
                        if (confirm2)
                        {
                            talking_units.Add(talking_unit);
                            talking_unit.initTalking_unit();
                            await context.PostAsync("好的，请遵照提示配置您的对话模板。");
                            await context.PostAsync("请输入您的命令类型：（a）对话型命令或者(b)功能型！默认为对话型");
                            talking_depth = 0;
                        }
                        else
                        {
                            MySqlConnection conn1 = new Init_DB().get_Init_DB();
                            conn1.Open();

                            //将记录存到数据库中
                            foreach (Talking_unit item in talking_units)
                            {

                                int i = 0;
                                new MySqlCommand("insert into talking_unit(depth,intended_id,type,question,answer,order) values(" + i + "," + current_intent_id + "," + item.type + "," + item.question + "," + item.answer + "," + item.order + ")", conn1);
                                i++;
                            }
                            conn1.Close();

                        }
                    }
                }


                await context.PostAsync("本次的基本意图是:" + intention);
                await context.PostAsync("本次的谈话深度是:" + talking_depth);

                // if (activity.Text.Contains("你好"))
                // {
                //     await context.PostAsync("你好，老铁");
                // }
                // else if (activity.Text.Contains("你叫什么名字"))
                // {
                //     await context.PostAsync("你就叫我特浪铺吧。");
                // }
                // else if (activity.Text.Contains("你有对象吗"))
                // {
                //     await context.PostAsync("不要问这么悲伤的问题啊！  扎心了 老铁。。。");
                // }
                // else {
                //     await context.PostAsync("你在用脸滚键盘么。。。你发的什么我看不懂");
                // }
                //  int length = (activity.Text ?? string.Empty).Length;
                //  await context.PostAsync($"you sent {activity.Text}which was{length} characters");
                //  context.Wait(MessageReceivedAsync);
            }
        }
    
}