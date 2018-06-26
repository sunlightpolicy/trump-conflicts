using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

using Newtonsoft.Json;



namespace Phase2 {

    public class media {
        public string url;
        public string caption;
        public string credit;

        public media(string url, string caption = "", string credit = "") {
            this.url = url;
            this.caption = caption;
            this.credit = credit;
        }
    }

    public class start_date {
        public string month;
        public string day;
        public string year;

        public start_date(string month, string day, string year) {
            this.month = month;
            this.day = day;
            this.year = year;
        }

        public start_date(DateTime date) {
            this.month = date.Month.ToString();
            this.day = date.Day.ToString();
            this.year = date.Year.ToString();
        }
    }

    public class text {
        public string headline;
        [Newtonsoft.Json.JsonProperty("text")]
        public string theText;

        public text(string headline, string text) {
            this.headline = headline;
            this.theText = text;
        }
    }

    public class Title {
        public media media;
        public text text;

        public Title(media media, text text) {
            this.media = media;
            this.text = text;
        }
    }

    public class Event {
        public media media;
        public start_date start_date;
        public text text;
        public string group;

        public Event(string url, string link, DateTime date, string text, string group, string mediaOutlet) {
            this.media = new media(url, "", mediaOutlet);
            this.start_date = new start_date(date);
            this.text = new text(text, link);
            this.group = group;
        }
    }

    public class TimelineJs {
        private string id;
        public Title title;
        public List<Event> events;

        public TimelineJs(string id, List<Event> events, Title title = null) {
            this.id = id;
            this.title = title;
            this.events = events;
        }

        public string GetId() {
            return id;
        }
    }


    public class TimelineJsImport {

        public static int MakeJson(string outputPath) {

            string connectionString = "Server=SCOTT-PC\\SQLExpress;Database=Trump;Trusted_Connection=True;";
                        
            int stories = 0;
            //string query = "SELECT Image, Link, Date, Description, Topic, MediaOutlet, Title FROM TopicView ORDER BY TopicID, Date";
            string query = "SELECT DISTINCT ID, Conflict, Image, Link, Date, ConflictDescription, 'Donald Trump' FamilyMember, MediaOutlet, Headline " +
                "FROM ConflictTimelineJsView WHERE Conflict <> '' " +
                "ORDER BY Conflict, Date";

            var timelines = new List<TimelineJs>();
            
            using (SqlConnection conn = new SqlConnection(connectionString)) {
                using (SqlCommand cmd = new SqlCommand(query, conn)) {
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    var oldConflict = "";
                    List<Event> events = null; 
                    while (reader.Read()) {

                        var conflict = reader["Conflict"].ToString();
                        if (oldConflict != conflict) {
                            var timeline = 
                                new TimelineJs(
                                    reader["Id"].ToString(),
                                    new List<Event>(), 
                                    new Title(null, new text(conflict, reader["conflictDescription"].ToString())));

                            timelines.Add(timeline);
                            events = timeline.events;
                            oldConflict = conflict;
                        }

                        var ev = new Event(
                            reader["Image"].ToString(),
                            ArticleLink(reader["MediaOutlet"].ToString(), reader["Link"].ToString(), reader["Headline"].ToString()),
                            DateTime.Parse(reader["Date"].ToString()),
                            reader["Headline"].ToString(),
                            reader["FamilyMember"].ToString(),
                            reader["MediaOutlet"].ToString()
                        );
                        events.Add(ev);
                        stories++;
                    }
                }
            }
            WriteTimelinesJsToJson(outputPath, timelines);

            return stories;
        }



        private static string ArticleLink(string mediaOutlet, string link, string headline) {

            if (headline != "")
                return "<a href='" + link + "' target='_blank'>" + mediaOutlet + " / " + headline + "</a>";

            if (link != "")
                return "<a href='" + link + "' target='_blank'>" + mediaOutlet + "</a>";

            return mediaOutlet;
            // "<a href='https://www.state.gov/e/eb/tfs/spi/ukrainerussia/' target='_blank'>Visit W3Schools.com!</a>"
        }




        private static void WriteTimelinesJsToJson(string outputPath, List<TimelineJs> timelines) {
            //"https://d3i6fh83elv35t.cloudfront.net/newshour/app/uploads/2014/10/LisaDesjardins_square-200x0-c-default.jpg"),

            //var title = new Title(
            //    //new media("http://kvie.org/wp-content/uploads/2017/12/pbs-newshour.png"),
            //    new media("/img/pbs-newshour.png"),
            //    new text("The giant timeline of everything Russia, Trump and the investigations", "By Lisa Desjardins"));

            //var timeline = new TimelineJS(events, title);
            foreach (TimelineJs timeline in timelines) {
                string json = JsonConvert.SerializeObject(timeline);
                var niceJson = Newtonsoft.Json.Linq.JToken.Parse(json).ToString();
                System.IO.File.WriteAllText(outputPath + timeline.GetId() + ".json", niceJson);
            }
        }
    }
}


