﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;


namespace Phase2 {

    enum Col {
        Parent3Pct = 1,
        Parent3,
        Parent2Pct,
        Parent2,
        Parent1Pct,
        Parent1,
        ConflictName, // Friendly Name
        ConflictDescription,
        ConflictingEntity, // As in Ethics Doc
        FamilyMember,
        FamilyRelationshipDescription,
        FamilyMemberConflictStatus,
        Source1,
        Source1Date,
        Source2,
        Source2Date,
        Source3,
        Source3Date,
        Notes,
        DatePublished
    };
    

    public class Conflict {
        public string Name;
        public string Description;
        public string Notes;
        public DateTime DateChanged;
    }

    public class BusinessConflict {
        public string Business;
        public string Conflict;
    }

    public class BusinessOwnership {
        public string Owner;
        public string Ownee;
        public string Percentage;
    }

    public class FamilyMemberBusiness {
        public string FamilyMember;
        public string Description;
        public string Business;
        public string ConflictStatus;
    }
    
    public class EthicsDocument {
        public string FamilyMember;
        public string Title;
        public string Link;
        public DateTime Date;

        public string Key() {
            return FamilyMember + Date.ToString();
        }

        public EthicsDocument(string familyMember, DateTime date, string link) {
            FamilyMember = familyMember;
            Date = date;
            Link = link;
        }
    }

    public class EthicsDocumentBusiness {
        public EthicsDocument EthicsDocument;
        public string Business;
    }

    public class Story {
        public string Conflict;
        public string MediaOutlet;
        public string Link;
        public DateTime Date;
        public string Headline;

        public Story(string conflict, string mediaOutlet, string link, DateTime date) {
            Conflict = conflict;
            MediaOutlet = mediaOutlet;
            Link = link;
            Date = date;

            Headline = "";
        }

        public string ToJson() {
            return "{ " +
                 "\"name\": \"" + Util.RemoveQuotes(MediaOutlet) + "\"," +
                 "\"link\": \"" + Util.RemoveQuotes(Link) + "\"," +
                 //"date=\"" + Date.ToString("d") + "\"" +
                 "\"date\": \"" + String.Format("{0:MM/dd/yyyy}", Date) + "\"" +
                 "}";
        }
    }
    
    

    class Phase2Loader {

        Dictionary<string, string> Businesss = new Dictionary<string, string>();
        Dictionary<string, Conflict> Conflicts = new Dictionary<string, Conflict>();

        Dictionary<string, string> MediaOutlets = new Dictionary<string, string>();

        List<Story> Stories = new List<Story>();
        Dictionary<string, string> StoryConflicts = new Dictionary<string, string>();

        Dictionary<string, EthicsDocument> EthicsDocuments = new Dictionary<string, EthicsDocument>();
        List<EthicsDocumentBusiness> EthicsDocumentBusiness = new List<EthicsDocumentBusiness>();

        List<FamilyMemberBusiness> FamilyMemberBusiness = new List<FamilyMemberBusiness>();

        List<BusinessConflict> BusinessConflicts = new List<BusinessConflict>();

        List<BusinessOwnership> BusinessOwnerships = new List<BusinessOwnership>();


        // Debugging Spreadheet
        List<string> RawConflictingEntities = new List<string>();
        List<string> RawBusinesses = new List<string>();
        List<string> RawFamilyMemberBusinesses = new List<string>();
        List<string> RawNotes = new List<string>();

        public Phase2Loader(String path) {
            var parentsFile = "Copy of Donald & Melania.html";

            var childrenFile = "Copy of Other Family.html";

            var conflicts = new List<Conflict>();
            //ImportPage(conflicts, path + "\\" + parentsFile);
            ImportPage(conflicts, path + "\\" + childrenFile);
            WriteSql("c:\\trump-conflicts\\Exporter\\Exporter\\db\\Phase2B\\");

            Console.ReadLine();
        }

        private void WriteSql(string path) {
            WriteBusinessScript(path, "4");
            WriteConflictScript(path, "5");

            WriteMediaOutletScript(path, "6");
            WriteStoryScript(path, "7");

            WriteEthicsDocumentScript(path, "8");

            WriteFamilyMemberBusinessScript(path, "9");
            WriteBusinessOwnershipScript(path, "91");

            //using (TextWriter tw = new StreamWriter(path + "DEBUG RawConflictingEntities.txt")) {
            //    foreach (String s in RawConflictingEntities)
            //        if (ConflictingEntity(s).Contains("<"))
            //            tw.WriteLine(s + ":::" + ConflictingEntity(s));
            //}

            //using (TextWriter tw = new StreamWriter(path + "DEBUG RawRawBusinesses.txt")) {
            //    foreach (String s in RawBusinesses) {
            //        var bus = BusUnit(s);
            //        if (bus.Contains("<"))
            //            tw.WriteLine(s + "      :" + BusUnit(s));
            //    }
            //}

            //using (TextWriter tw = new StreamWriter(path + "DEBUG RawFamilyMemberBusinesses.txt")) {
            //    foreach (String s in RawFamilyMemberBusinesses)
            //        if (FamilyDescription(s).Contains("<"))
            //            tw.WriteLine(s + ":::" + FamilyDescription(s));
            //}

            using (TextWriter tw = new StreamWriter(path + "DEBUG RawNotes.txt")) {
                foreach (String s in RawNotes)
                    if (Notes(s).Contains("<") || Notes(s).Contains("class"))
                        tw.WriteLine(s + ":::" + Notes(s));
            }
        }

        private void ImportPage(List<Conflict> conflicts, string file) {

            string text = File.ReadAllText(file, Encoding.UTF8);

            int rowNum = 0;
            int max = 1340;  // !!
            var rows = text.Split(new[] { "<tr " }, StringSplitOptions.None);
            foreach (string row in rows) {
                if (rowNum < max && rowNum > 1) {

                    // Bad date
                    if (file.Contains("Other Family") && rowNum == 948)
                        continue;


                    string theRow = row.Replace("<td></td>", "<td ></td>");
                    var cols = theRow.Split(new[] { "<td " }, StringSplitOptions.None);
                    if (cols.Length < 2)
                        return;

                    //Console.WriteLine(cols.Length.ToString());

                    AddBusinesss(cols);
                    AddConflict(cols);
                    AddStoriesAndEthicsDocuments(cols);
                    AddFamilyMemberBusiness(cols);
                    AddBusinessOwnerships(cols);
                }
                rowNum++;
            }
                       

            Console.WriteLine(Businesss.Count.ToString() + " BusinesUnits");
            Console.WriteLine(Conflicts.Count.ToString() + " Conflicts");
            Console.WriteLine(Stories.Count.ToString() + " Stories");
        }

        private void AddConflict(string[] cols) {
            var name = ConflictingEntity(cols[(int)Col.ConflictName]);
            if (name == "")
                return;

            //if (name.Contains("Class=")) {
            //    Console.WriteLine(name);
            //    return;
            //}


            Conflict conflict;

            // Add Conflict to list if it isn't already there.
            if (!Conflicts.ContainsKey(name)) {

                var description = ConflictDescription(cols[(int)Col.ConflictDescription]);

                RawNotes.Add(cols[(int)Col.Notes]);

                var note = Notes(cols[(int)Col.Notes]);
                var date = DateChanged(cols[(int)Col.DatePublished]);

                conflict = new Conflict();
                conflict.Name = name;
                conflict.Description = description;
                conflict.Notes = note;
                conflict.DateChanged = date;

                Conflicts.Add(name, conflict);
            } else
                conflict = Conflicts[name]; 

            
            // Add link between conflict and business
            var business = ConflictingEntity(cols[(int)Col.ConflictingEntity]);
            var busConflict = new BusinessConflict();
            busConflict.Conflict = conflict.Name;
            busConflict.Business = business;
            BusinessConflicts.Add(busConflict);
        }

        private void AddStoriesAndEthicsDocuments(string[] cols) {
            AddStoryAndEthicsDocument(cols[(int)Col.FamilyMember], cols[(int)Col.ConflictingEntity], cols[(int)Col.ConflictName], cols[(int)Col.Source1], cols[(int)Col.Source1Date]);
            AddStoryAndEthicsDocument(cols[(int)Col.FamilyMember], cols[(int)Col.ConflictingEntity], cols[(int)Col.ConflictName], cols[(int)Col.Source2], cols[(int)Col.Source2Date]);
            AddStoryAndEthicsDocument(cols[(int)Col.FamilyMember], cols[(int)Col.ConflictingEntity], cols[(int)Col.ConflictName], cols[(int)Col.Source3], cols[(int)Col.Source3Date]);
        }

        private void AddStoryAndEthicsDocument(string familyMemberCol, string conflictingEntityCol, string conflictCol, string linkCol, string dateCol) {

            RawConflictingEntities.Add(conflictingEntityCol);

            var fields = linkCol.Split(new[] { " href=" }, StringSplitOptions.None);
            if (fields.Length < 2)
                return;

            var familyMember = FamilyMember(familyMemberCol);
            var business = ConflictingEntity(conflictingEntityCol);
            var conflict = ConflictingEntity(conflictCol);

            var linkAndName = fields[1].Split('>');
            var mediaOutlet = linkAndName[1].Replace("</a", "");
            var link = linkAndName[0].Replace("\"", "");

            var dateStr = dateCol.Split('>')[1].Replace("</td", "");
            var dte = GetDate(dateStr);

            if (mediaOutlet == "")
                return;

            //if (link == "http://money.cnn.com/2017/01/23/news/donald-trump-resigns-business/")
            //    Console.WriteLine("X");

            // It is a story
                if (mediaOutlet != "Office of Government Ethics") {
                // Add a new MediaOutlet if neccessary
                if ((mediaOutlet != "") && (!MediaOutlets.ContainsKey(mediaOutlet)))
                    MediaOutlets.Add(mediaOutlet, mediaOutlet);

                if (mediaOutlet != "") {

                    bool alreadyThere = false; 
                    foreach (Story s in Stories) {
                        if (s.Link == link)
                            alreadyThere = true;
                    }

                    var story = new Story(conflict, mediaOutlet, link, dte);
                    if (!alreadyThere)
                        Stories.Add(story);
                }
            } else { // it is a Ethics Doc reference 
                // Add a new Ethics Doc if neccessary
                if ((mediaOutlet != "") && (!EthicsDocuments.ContainsKey(link))) {
                    EthicsDocuments.Add(link, new EthicsDocument(familyMember, dte, link));
                }

                var ethicsDocRef = new EthicsDocumentBusiness();
                ethicsDocRef.Business = business;
                EthicsDocuments.TryGetValue(link, out ethicsDocRef.EthicsDocument); 
                EthicsDocumentBusiness.Add(ethicsDocRef);
            }   
        }



        private void AddBusinessOwnerships(string[] cols) {
            AddBusinessOwnership(cols[(int)Col.ConflictingEntity], cols[(int)Col.Parent1], cols[(int)Col.Parent1Pct]);
            AddBusinessOwnership(cols[(int)Col.ConflictingEntity], cols[(int)Col.Parent2], cols[(int)Col.Parent2Pct]);
            AddBusinessOwnership(cols[(int)Col.ConflictingEntity], cols[(int)Col.Parent3], cols[(int)Col.Parent3Pct]);
        }

        private void AddBusinessOwnership(string owneeCol, string ownerCol, string percentageCol) {
            
            var owner = ConflictingEntity(ownerCol);
            var ownee = ConflictingEntity(owneeCol);
            var percentage = ConflictingEntity(percentageCol);
            
            if (owner == "")
                return;

            var own = new BusinessOwnership();
            own.Owner = owner;
            own.Ownee = ownee;
            own.Percentage = percentage;
            BusinessOwnerships.Add(own);
        }


        private void AddFamilyMemberBusiness(string[] cols) {
            
            var x = new FamilyMemberBusiness();
            x.Business = ConflictingEntity(cols[(int)Col.ConflictingEntity]);
            x.ConflictStatus = Category(cols[(int)Col.FamilyMemberConflictStatus]);
            x.FamilyMember = FamilyMember(cols[(int)Col.FamilyMember]);
            x.Description = FamilyDescription(cols[(int)Col.FamilyRelationshipDescription]);

            RawFamilyMemberBusinesses.Add(cols[(int)Col.FamilyRelationshipDescription]);

            if (x.Business != "")
                FamilyMemberBusiness.Add(x);
        }


        private void AddBusinesss(string [] cols) {
            RawBusinesses.Add(cols[(int)Col.Parent1]);
            RawBusinesses.Add(cols[(int)Col.Parent2]);
            RawBusinesses.Add(cols[(int)Col.Parent3]);
            RawBusinesses.Add(cols[(int)Col.ConflictingEntity]);

            AddBusiness(BusUnit(cols[(int)Col.Parent1]));
            AddBusiness(BusUnit(cols[(int)Col.Parent2]));
            AddBusiness(BusUnit(cols[(int)Col.Parent3]));

            AddBusiness(ConflictingEntity(cols[(int)Col.ConflictingEntity]));
        }

        private void AddBusiness(string unit) {
            if (unit == "")
                return;

            if (!Businesss.ContainsKey(unit))
                Businesss.Add(unit, Businesss.Count.ToString());
        }

        private void WriteBusinessScript(string path, string seq) {
            var strings = new List<string>();
            strings.Add("USE Trump");
            strings.Add("GO\r\n");

            foreach (String unit in Businesss.Keys)
                strings.Add("INSERT INTO Business VALUES ('" + unit + "')");

            using (TextWriter tw = new StreamWriter(path + seq + " INSERT Business.sql")) {
                foreach (String s in strings)
                    tw.WriteLine(s);
            }
         }

         private void WriteBusinessOwnershipScript(string path, string seq) {
            var strings = new List<string>();
            strings.Add("USE Trump");
            strings.Add("GO\r\n");

            foreach (BusinessOwnership own in BusinessOwnerships)
                strings.Add("INSERT INTO BusinessOwnership VALUES (" +
                    "(SELECT ID FROM Business WHERE Name = '" + own.Owner + "'), " +
                    "(SELECT ID FROM Business WHERE Name = '" + own.Ownee + "'), " +
                    "'" + own.Percentage + "')");

            using (TextWriter tw = new StreamWriter(path + seq + " INSERT BusinessOwnership.sql")) {
                foreach (String s in strings)
                    tw.WriteLine(s);
            }
          }

        private void WriteStoryScript(string path, string seq) {
            var strings = new List<string>();
            strings.Add("USE Trump");
            strings.Add("GO\r\n");

            int storyId = 0;
            foreach (Story story in Stories) {
                storyId++;

                strings.Add("INSERT INTO Story VALUES (" +
                    "(SELECT ID FROM MediaOutlet WHERE Name = '" + story.MediaOutlet + "'), " +
                    "2, '" + // StoryStatusUD
                    story.Link + "', " +  // Link
                    "'', " + // Headline
                    "'" + story.Date + "', " +
                    "'', " + // Text
                    "'', " + // Keywords
                    "'', " + // Authors
                    "'', " + // TopImage
                    "GetDate()," +
                    "2)" // Lynn Walsh 
                );

                if (story.Conflict != "") {
                    strings.Add("INSERT INTO StoryConflict VALUES (" +
                        storyId + ", " +
                        "(SELECT ID FROM Conflict WHERE Name = '" + story.Conflict + "'))"
                    );
                }
            }

            using (TextWriter tw = new StreamWriter(path + seq + " INSERT Story and StoryConflict.sql")) {
                foreach (String s in strings)
                    tw.WriteLine(s);
            }
        }

        private void WriteEthicsDocumentScript(string path, string seq) {
            var strings = new List<string>();
            strings.Add("USE Trump");
            strings.Add("GO\r\n");

            foreach (EthicsDocument doc in EthicsDocuments.Values) {
                strings.Add("INSERT INTO EthicsDocument VALUES (" +
                    "(SELECT ID FROM FamilyMember WHERE Name = '" + doc.FamilyMember + "'), " +
                    "'Ethics Document ', '" +
                    doc.Link + "', '" +
                    doc.Date.ToString() + "')"
                );
            }

            foreach (EthicsDocumentBusiness ethicsDocRef in EthicsDocumentBusiness) {
                strings.Add("INSERT INTO EthicsDocumentBusiness VALUES (" +
                    "(SELECT ID FROM EthicsDocument WHERE Link = '" + ethicsDocRef.EthicsDocument.Link + "'), " +
                    "(SELECT ID FROM Business WHERE Name = '" + ethicsDocRef.Business + "'))"
                );
            }

            using (TextWriter tw = new StreamWriter(path + seq + " INSERT EthicsDocumentBusiness.sql")) {
                foreach (String s in strings)
                    tw.WriteLine(s);
            }
        }


        private void WriteFamilyMemberBusinessScript(string path, string seq) {
            var strings = new List<string>();
            strings.Add("USE Trump");
            strings.Add("GO\r\n");

            foreach (FamilyMemberBusiness x in this.FamilyMemberBusiness) {
                strings.Add("INSERT INTO FamilyMemberBusiness VALUES (" +
                    "(SELECT ID FROM FamilyMember WHERE Name = '" + x.FamilyMember + "'), " +
                    "(SELECT ID FROM Business WHERE Name = '" + x.Business + "'), " +
                    "(SELECT ID FROM FamilyMemberConflictStatus WHERE Name = '" + x.ConflictStatus + "'), " +
                    "'" + x.Description + "')"
                );
            }

            using (TextWriter tw = new StreamWriter(path + seq + " INSERT FamilyMemberBusiness.sql")) {
                foreach (String s in strings)
                    tw.WriteLine(s);
            }
        }
        
        private void WriteConflictScript(string path, string seq) {
            var strings = new List<string>();
            strings.Add("USE Trump");
            strings.Add("GO\r\n");

            foreach (Conflict conflict in Conflicts.Values) {

                strings.Add("INSERT INTO Conflict VALUES ('" +
                    conflict.Name + "', '" +
                    conflict.Description + "', '" +
                    conflict.Notes + "', '" +
                    conflict.DateChanged.ToString() + "', " +
                    "GetDate(), 1)"
                );
            }

            foreach (BusinessConflict busConflict in BusinessConflicts)
                strings.Add("INSERT INTO BusinessConflict VALUES (" +
                    "" +
                    "(SELECT ID FROM Conflict WHERE Name = '" + busConflict.Conflict + "'), " +
                    "(SELECT ID FROM Business WHERE Name = '" + busConflict.Business + "'))"
                );

            using (TextWriter tw = new StreamWriter(path + seq + " INSERT Conflict and BusinesConflict.sql")) {
                foreach (String s in strings)
                    tw.WriteLine(s);
            }
        }

        private void WriteMediaOutletScript(string path, string seq) {
            var strings = new List<string>();
            strings.Add("USE Trump");
            strings.Add("GO\r\n");

            foreach (String outlet in MediaOutlets.Keys)
                strings.Add("INSERT INTO MediaOutlet VALUES ('" + outlet + "')");

            using (TextWriter tw = new StreamWriter(path + seq + " INSERT MediaOutlet.sql")) {
                foreach (String s in strings)
                    tw.WriteLine(s);
            }
        }

        private DateTime GetDate(string dte) {
            dte = dte
                .Replace("Sept.", "September");
            DateTime date = Convert.ToDateTime(dte.Replace(".", ""));
            return date;
        }
                

        public string ConflictDescription(string txt) {
            if (txt.Contains("ltr\">"))
                txt = txt.Substring(txt.LastIndexOf("ltr\">") + 5);

            if (txt.Contains("px;\">"))
                txt = txt.Substring(txt.LastIndexOf("px;\">") + 5);

            txt = txt.Replace("</div></td>", "");
            txt = txt.Replace("</td>", "");

            return
                txt.TrimEnd().TrimStart();
        }

        public string FamilyDescription(string txt) {
            if (txt.Contains("ltr\">"))
                txt = txt.Substring(txt.LastIndexOf("ltr\">") + 5);

            if (txt.Contains("px;\">"))
                txt = txt.Substring(txt.LastIndexOf("px;\">") + 5);

            txt = txt.Replace("</div></td>", "");
            txt = txt.Replace("</td>", "");

            return
                txt.TrimEnd().TrimStart();
        }

        public string FamilyMember(string txt) {
            var subs = txt.Split('>');
            return
                subs[1].Replace("</td", "")
                    .TrimEnd()
                    .TrimStart()
                    .Replace("ump, Jr", "ump Jr");
        }
        
        private string BusUnit(string txt) {
            if (txt.Contains("ltr\">"))
                txt = txt.Substring(txt.LastIndexOf("ltr\">") + 5);

            if (txt.Contains("px;\">"))
                txt = txt.Substring(txt.LastIndexOf("px;\">") + 5);

            txt = txt.Replace("</div></td>", "");
            txt = txt.Replace("</td>", "");

            txt = txt.TrimEnd().TrimStart();
            return 
                CleanEntity(txt);
        }

        private string ConflictingEntity(string txt) {
            
            if (txt.Contains("px;\">"))
                txt = txt.Substring(txt.LastIndexOf("px;\">") + 5);

            if (txt.Contains("ltr\">"))
                txt = txt.Substring(txt.LastIndexOf("ltr\">") + 5);

            txt = txt.Replace("</div></td>", "");
            txt = txt.Replace("</td>", "");

            var entity = txt.TrimEnd().TrimStart();
            
            return
                CleanEntity(entity);
        }

        private string CleanEntity(string txt) {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo
                .ToTitleCase(txt.ToLower())
                .Replace("Ny", "NY")
                .Replace("Llc", "LLC")
                .Replace("Nj", "NJ")
                .Replace(" Va", " VA");
        }

        private string Category(string txt) {
            var subs = txt.Split('>');
            var category = subs[1].Replace("</td", "").TrimEnd().TrimStart();

            if (string.IsNullOrEmpty(category))
                return "Active";  // Only one of these

            return char.ToUpper(category[0]) + category.Substring(1);
        }

        private string Notes(string txt) {
            if (txt.Contains("ltr\">"))
                txt = txt.Substring(txt.LastIndexOf("ltr\">") + 5);

            if (txt.Contains("px;\">"))
                txt = txt.Substring(txt.LastIndexOf("px;\">") + 5);

            txt = txt.Replace("</div></td>", "");
            txt = txt.Replace("</td>", "");

            if (txt.Contains("class=\"s7\">"))
                txt = "";

            return
                txt.TrimEnd().TrimStart();
        }

        private DateTime DateChanged(string txt) {
            var subs = txt.Split('>');
            var dte = subs[1].Replace("</td", "").TrimEnd().TrimStart();
            return
                Convert.ToDateTime(dte);
        }   
    }

    public class Util {
        public static string RemoveQuotes(String str) {
            if (str == null)
                return "";
            return str.Replace("\"", "");
        }

        // CSVs need internal quotes to be repeated
        public static string RepeatQuotes(String str) {
            if (str == null)
                return "";
            return str.Replace("\"", "\"\"");
        }
    }
}
