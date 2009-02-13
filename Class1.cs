using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using KismetLogger.KismetDataTableAdapters;


namespace KismetLogger
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		private static bool oui;
		private static Hashtable myhash= new Hashtable();
        public static Hashtable enchash = new Hashtable();

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//XmlSerializer xs = new XmlSerializer(typeof(detectionrun));
			//FileStream fs = new FileStream("c:\\Kismet-Mar-31-2006-3.xml", FileMode.Open);
			//XmlSerializer xs2 = new XmlSerializer(typeof(gpsrun));
			//FileStream fs2 = new FileStream("c:\\Kismet-Mar-31-2006-3gps.xml", FileMode.Open);
			//detectionrun ds = new detectionrun();
			//ds=(detectionrun) xs.Deserialize(fs);
			string thedir=Application.StartupPath + "\\logs";
			
			//string thedir="c:\\documents and settings\\luke\\my documents\\kismetlogs\\";
            string theconnectionstring = "Data Source=" + Application.StartupPath + "\\kismetdata.db;Version=3;Cache Size=3000;New=False;Compress=False;Synchronous=Off";
			//gpsrun ds2 = new gpsrun();
			//ds2=(gpsrun) xs2.Deserialize(fs2);
			DirectoryInfo dir= new DirectoryInfo(thedir);
            Console.WriteLine(dir.ToString());
			XmlSerializer xs2 = new XmlSerializer(typeof(gpsrun));
			//gpsfiles thegpsfiles = new gpsfiles();
			gpsdata thegpsdata= new gpsdata();
			bool kml=false;
			bool gps=false;
			bool xml=false;
		    bool dot = false;
		    bool clean = false;
		    bool allgps = false;
		    bool wigledata = false;
		    bool newoui = false;
			//bool oui=false;
			string theouifilename= Application.StartupPath+"\\oui.txt";
			FileInfo fi = new FileInfo(theouifilename);
			if(fi.Exists)
			{
				//Console.WriteLine("Found oui.txt file, enabling oui.txt lookup");
				oui=true;
				CreateOUITable();
			}
			else
			{
				Console.WriteLine("No oui.txt file found, disabling oui.txt lookup");	
				oui=false;

			}
			if(args.Length == 0)
			{
				Console.WriteLine("No arguments - processing gps and xml files and creating kml file");
				kml = true;
				gps = true;
				xml = true;
				clean=false;

			}
			else
			{
                if(args[0].IndexOf("d")!=-1)
                {
                    dot = true;
                }
                if(args[0].IndexOf("o") !=-1)
                {
                    newoui = true;
                }
				if(args[0].IndexOf("x")!=-1)
				{
					xml = true;
				}
				if(args[0].IndexOf("g")!=-1)
				{
					gps = true;
				}
				if(args[0].IndexOf("k")!=-1)
				{
					kml = true;
				}
                if (args[0].IndexOf("c") != -1)
                {
                    clean = true;
                }
				if(args[0].IndexOf("a") !=-1)
				{
				    allgps = true;
				}
                if(args[0].IndexOf("w") !=-1)
                {
                    wigledata = true;
                }
			}
            //-g
			if(gps)
			{
				//ProcessGPSFiles(theconnectionstring, dir, xs2,  thegpsdata,  thegpsfiles);
                ProcessGPSFilesNew(dir, xs2);
			}
			//-x
			if(xml)
			{
                    ProcessCSVFilesNew(dir, enchash);
                    ProcessXMLFilesNew(dir);
			}            
            if(dot)
            {
                //CreateDotFile();
                RenameFiles();
            }
			//-k
			if(kml)
			{                
                CreateKmlFileNew(thegpsdata, theconnectionstring);
			}
            if(clean)
            {
                CleanGPSFiles(dir);
            }
            if(newoui)
            {
                DoNewOuis();
            }
            if(allgps)
            {                
                CleanGPSFiles(dir);
                ProcessHugeGPSFiles(xs2);
                
            }
            if(wigledata)
            {
                DoWigleData();
            }
		}

        private static void RenameFiles()
        {
            string thedir = Application.StartupPath + "\\logs";
            DirectoryInfo dir = new DirectoryInfo(thedir);
            string oldfilename = "";
            string newname;
            foreach (FileInfo f in dir.GetFiles("Kismet-*-*-*.gps"))
            {
                oldfilename = f.Name;
                Console.WriteLine(oldfilename);
                string[] thestrings;
                thestrings = oldfilename.Split(new char[] { '-' });
                newname=thestrings[0]+"-" + thestrings[3] + MonthLookup(thestrings[1]) + thestrings[2] + "-" + thestrings[4];
                Console.WriteLine(newname);
                f.MoveTo(thedir + "\\" + newname);
            }
        }

        private static string MonthLookup(string p)
        {
            switch(p)
            {
                case "Jan":
                    return "01";
                case "Feb":
                    return "02";
                case "Mar":
                    return "03";
                case "Apr":
                    return "04";
                case "May":
                    return "05";
                case "Jun":
                    return "06";
                case "Jul":
                    return "07";
                case "Aug":
                    return "08";
                case "Sep":
                    return "09";
                case "Oct":
                    return "10";
                case "Nov":
                    return "11";
                case "Dec":
                    return "12";
                default:
                    return "Error";

            }
        }

        private static void DoNewOuis()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = string.Format("{0}\\kismetdata.db", Application.StartupPath);
            builder.SyncMode = SynchronizationModes.Off;
            builder.CacheSize = 5000;
            DataTable t;
            DataTable y;
            SQLiteConnection connection = new SQLiteConnection(builder.ToString());
            //SQLiteCommand command = connection.CreateCommand();
            connection.Open();
            ClientsTableAdapter theclientstableadaptor = new ClientsTableAdapter();
            DatanewTableAdapter thedatatableadaptor = new DatanewTableAdapter();
            y = thedatatableadaptor.GetDataByUnknownOui();
            t = theclientstableadaptor.GetDataByUnknownOui();
            DataTableReader thereader = t.CreateDataReader();
            DataTableReader datareader = y.CreateDataReader();
            string thenewoui = "";
            int clientcount = 0;
            int datacount = 0;
            for (int i = 0; i < t.Rows.Count; i++)
            {                
                thereader.Read();
                string thebssid = thereader["bssid"].ToString();
                //Console.WriteLine(thebssid);
                thenewoui = LookupOuiData(thebssid);
                if (thenewoui != "Unknown")
                {
                    clientcount++;
                    theclientstableadaptor.UpdateQuery(thenewoui, thebssid);
                }                
            }
            Console.WriteLine("Client Ouis updated= " + clientcount);
            for (int i = 0; i < y.Rows.Count; i++)
            {
                datareader.Read();
                string thebssid = datareader["bssid"].ToString();                
                thenewoui = LookupOuiData(thebssid);
                if(thenewoui != "Unknown")
                {
                    datacount++;
                    Console.WriteLine(thebssid);
                    thedatatableadaptor.UpdateQuery(thenewoui, thebssid);
                }
            }
            Console.WriteLine("Data Ouis updated= " + datacount);
        }

        private static void ProcessCSVFilesNew(DirectoryInfo dir, Hashtable enchash)
        {
            if (enchash == null) throw new ArgumentNullException("enchash");
            encchanges theencchanges = new encchanges();
            DataTable t;
            FilesTableAdapter thefilesadaptor = new FilesTableAdapter();
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = string.Format("{0}\\kismetdata.db", Application.StartupPath);
            builder.SyncMode = SynchronizationModes.Off;
            builder.CacheSize = 5000;
            //DataTable t;
            SQLiteConnection connection = new SQLiteConnection(builder.ToString());
            //SQLiteCommand command = connection.CreateCommand();
            connection.Open();
            SQLiteCommand addcmd = connection.CreateCommand();
                        //DbCommand updatecmd = connection.CreateCommand();
            string theaddcmd;
            theaddcmd = "INSERT INTO encchanges values (?, ?, ?, ?)";
            //updatecmd.CommandText="Update [GPSData] set [source]= @thesource, [timesec]=@thetimesec, [timeusec]=@thetimeusec, [lat] = @thelat, [lon] = @thelon, [alt] = @thealt, [speed] = @thespeed, [heading] = @theheading, [fix] = @thefix, [signal] = @thesignal, [quality] = @thequality, [noise] = @thenoise where [bssid]=@thebssid";
            addcmd.CommandText = theaddcmd;
            SQLiteParameter bssid = addcmd.CreateParameter();
            SQLiteParameter date = addcmd.CreateParameter();
            SQLiteParameter oldenc = addcmd.CreateParameter();
            SQLiteParameter newenc = addcmd.CreateParameter();
            
            addcmd.Parameters.Add(bssid);
            addcmd.Parameters.Add(date);
            addcmd.Parameters.Add(oldenc);
            addcmd.Parameters.Add(newenc);
            
               
            foreach (FileInfo f in dir.GetFiles("Kismet*.csv"))
            {
                t = thefilesadaptor.GetDataByFilename(f.Name);
                if (t.Rows.Count == 0)
                {

                    //thegpsfiles.AddNew();
                    //thegpsfiles.Filename=f.Name;
                    //thegpsfiles.Save();
                    thefilesadaptor.InsertQuery(f.Name, 0, "None", 0, 0);
                    StreamReader sr = File.OpenText(Application.StartupPath + "\\logs\\" + f.Name);
                    string input;
                    Console.WriteLine(f.Name);
                    //string aoui = "";

                    //string[] theoui;
                    //string thecompany = "";
                    sr.ReadLine();
                    using (DbTransaction dbtrans = connection.BeginTransaction())
                    {
                    while ((input = sr.ReadLine()) != null)
                    {
                        if (input.Length != 0)
                        {
                            //Console.WriteLine(input);	
                            string[] thestrings;
                            thestrings = input.Split(new char[] { ';' });
                            //Console.WriteLine(thestrings.Length);
                            if (!enchash.ContainsKey(thestrings[3]))
                            {
                                enchash.Add(thestrings[3], thestrings[7]);
                            }
                            else
                            {
                                string temp = enchash[thestrings[3]].ToString();

                                if (temp != thestrings[7])
                                {
                                    enchash.Remove(thestrings[3]);
                                    
                                    bssid.Value = thestrings[3];
                                    date.Value = thestrings[20];
                                    oldenc.Value = temp;
                                    newenc.Value = thestrings[7];

                                    addcmd.ExecuteNonQuery();
                                    enchash.Add(thestrings[3], thestrings[7]);


                                }
                            }
                        }

                    }
                    dbtrans.Commit();
                }
            }
            
        }
            
            
        }

        private static void DoWigleData()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = string.Format("{0}\\kismetdata.db", Application.StartupPath);
            builder.SyncMode = SynchronizationModes.Off;
            builder.CacheSize = 5000;
            //DataTable t;
            SQLiteConnection connection = new SQLiteConnection(builder.ToString());
            //SQLiteCommand command = connection.CreateCommand();
            connection.Open();
            StreamReader sr = File.OpenText(Application.StartupPath + "\\out.txt");
            string input;
            DatanewTableAdapter thedatatableadaptor = new DatanewTableAdapter();
            GPSDatanewTableAdapter thegpsdatatableadaptor = new GPSDatanewTableAdapter();
            DataTable t;
            while ((input = sr.ReadLine()) != null)
            {
                if (input.Length != 0)
                {
                    //Console.WriteLine(input);	
                    string[] thestrings;
                    
                    thestrings = input.Split(new char[] {'~'});
                    try
                    {
                        //t = thegpsdatatableadaptor.GetDataByGpsData(thestrings[0].ToUpper());
                        t = thedatatableadaptor.GetDataByBssid(thestrings[0].ToUpper());
                        DataTableReader thereader = t.CreateDataReader();
                        thereader.Read();
                        if (!thereader.HasRows)
                        {
                            Console.WriteLine("Insert into gpsdata values('" + (thestrings[0].ToUpper()) +
                                              "\',NULL,NULL,NULL," +
                                              thestrings[10] + "," + thestrings[11] +
                                              ",NULL,NULL,NULL,\'3\',-256,NULL,NULL);");
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    //Console.WriteLine(thestrings[0]);
                }
            }
        }

/*
		private static void AnalyzeClients(string theconnectionstring)
		{
			
			thedata therealdata = new thedata();
			clientdata theclientdata = new clientdata();
			therealdata.ConnectionString=theconnectionstring;
			theclientdata.ConnectionString=theconnectionstring;
			
			therealdata.LoadAll();
			therealdata.Rewind();
			do
			{
				theclientdata.Where.Bssid.Value=therealdata.BSSID;
				theclientdata.Where.Bssid.Operator=WhereParameter.Operand.Equal;
				theclientdata.Query.Load();
				{
					if(theclientdata.RowCount > 1)
					{
						Console.WriteLine(therealdata.BSSID+" "+ theclientdata.RowCount);
					}
				}
			
			}while(therealdata.MoveNext());
		}
*/

		private static void CreateOUITable()
		{
	
			//string  theconnectionstring = "Data Source=c:\\documents and settings\\luke\\my documents\\visual studio projects\\kismetlogger\\bin\\debug\\kismetdata.db;Version=3;New=False;Compress=False;Synchronous=On";
			
			
			StreamReader sr = File.OpenText(Application.StartupPath+ "\\oui.txt");
			string input;
			string aoui="";
			
			string thecompany="";
			//Console.WriteLine("Reading values from oui.txt");
			while ((input=sr.ReadLine())!=null)
			{
                
				if(input.Length != 0)
				{
					//Console.WriteLine(input);	
                    string[] thestrings;
                    
					thestrings=input.Split(new char[]{'\t'});
					//Console.WriteLine(thestrings.Length);
					if(input.IndexOf("hex") !=-1)
					{
                        string[] theoui;
						theoui=thestrings[0].Split(new char[]{' '});
						aoui=theoui[0];
						thecompany=thestrings[2];
					}					
					
				}
				else
				{
					if(aoui.Length !=0)
					{
						try
						{
							myhash.Add(aoui, thecompany);
						}
						catch (Exception)
						{
						    //Console.WriteLine("Already in database");
						}
					}
										
				}
			}
			/*
			Console.WriteLine("Now adding values to database");
			IDictionaryEnumerator myEnumerator = theht.GetEnumerator();
			while ( myEnumerator.MoveNext() )
			{
					theouitable.AddNew();
					theouitable.Oui=myEnumerator.Key.ToString();
					theouitable.Company_name=myEnumerator.Value.ToString();
					theouitable.Save();	
				
			}
			*/
		}

/*
		private static void CleanGPSFilesOld(DirectoryInfo dir)
		{
			foreach (FileInfo f in dir.GetFiles("Kismet*.gps"))
			{
				string input;
				StreamReader fs2 = File.OpenText("c:\\documents and settings\\luke\\my documents\\kismetlogs\\" + f.Name);
				//Console.WriteLine("Processing " + f.Name);
				// GP:SD:TR:AC:KL:OG
				Regex theregex = new Regex(@"GP:SD:TR:AC:KL:OG", RegexOptions.Compiled);
				Regex theregex2 = new Regex(@"bssid");
				while((input=fs2.ReadLine()) != null)
				{
					
					if(!theregex.IsMatch(input))
					{
						if(theregex2.IsMatch(input))
						{
							Console.WriteLine(input);
						}
					}
				}
				
			}
		}
*/

        public static void CreateKmlFile(gpsdata thegpsdata, string theconnectionstring)
        {
            thedata newdata = new thedata();
            //clientdata thecd = new clientdata();
            thegpsdata.FlushData();
            clientdata theclientdata = new clientdata();
            //Decimal thebestlon;
            //Decimal thebestlat;
            //Decimal thebestsignal;
            Console.WriteLine("Creating KML File");
            theclientdata.ConnectionString = theconnectionstring;
            newdata.ConnectionString = theconnectionstring;
            thegpsdata.ConnectionString = theconnectionstring;
            //thecd.ConnectionString=theconnectionstring;
            newdata.Where.NetType.Value = "infrastructure";
            ///newdata.Where.NetType.Operator = WhereParameter.Operand.Equal;
            newdata.Where.Encryption.Value = "None";
            ///newdata.Where.Encryption.Operator = WhereParameter.Operand.NotEqual;
            //string firstquery = "Select * from data where nettype=\"infrastructure\" and encryption=\"None\"";

            //Kisdata.Where.BSSID.Value="00:0F:3D:3E:41:9A";
            //Kisdata.Where.BSSID.Operator=WhereParameter.Operand.Equal;
            //Console.WriteLine(thegpsdata.Query.GenerateSQL());
            newdata.Query.Load();
            XmlTextWriter myXmlTextWriter = new XmlTextWriter(Application.StartupPath + "\\kismet.kml", null);
            myXmlTextWriter.Formatting = Formatting.Indented;
            myXmlTextWriter.WriteStartDocument(false);
            myXmlTextWriter.WriteStartElement("Folder");
            myXmlTextWriter.WriteElementString("name", null, "KismetLogger");
            myXmlTextWriter.WriteStartElement("Folder");
            myXmlTextWriter.WriteElementString("name", null, "Encrypted");
            //Console.WriteLine(newdata.RowCount);
            if (newdata.RowCount != 0)
            {
                newdata.Rewind();
                do
                {
                    thegpsdata.Where.Bssid.Value = newdata.BSSID;
                    ///thegpsdata.Where.Bssid.Operator = WhereParameter.Operand.Equal;
                    //Kisdata.Where.BSSID.Value="00:0F:3D:3E:41:9A";
                    //Kisdata.Where.BSSID.Operator=WhereParameter.Operand.Equal;
                    //Console.WriteLine(thegpsdata.Query.GenerateSQL());
                    thegpsdata.Query.Load();
                    //thebestlat = 0;
                    //thebestlon = 0;
                    //thebestsignal=-257;

                    if (thegpsdata.RowCount != 0)
                    {

                        thegpsdata.Rewind();
                        


                        string clientstring = "";
                        myXmlTextWriter.WriteStartElement("Placemark");
                        myXmlTextWriter.WriteElementString("name", null, newdata.ESSID);
                        myXmlTextWriter.WriteElementString("styleUrl", null, "#track");
                        myXmlTextWriter.WriteStartElement("description");
                        theclientdata.Where.Bssid.Value = newdata.BSSID;
                        theclientdata.Query.Load();
                        if (theclientdata.RowCount != 0)
                        {
                            theclientdata.Rewind();
                            clientstring = "<p>Number of clients=" + theclientdata.RowCount + "<p>";
                            do
                            {
                                clientstring += "<p>" + theclientdata.Clientmac + "-" + theclientdata.Oui + "</p>";

                            } while (theclientdata.MoveNext());
                        }

                        myXmlTextWriter.WriteCData(newdata.BSSID + "-" + newdata.Oui + "<p><b>First Seen: </b><br>" + newdata.FirstTime + "<br>" + "<BR><b> Last Seen </b>" + newdata.LastTime + clientstring);
                        myXmlTextWriter.WriteEndElement();
                        myXmlTextWriter.WriteStartElement("Style");
                        myXmlTextWriter.WriteStartElement("Icon");
                        myXmlTextWriter.WriteElementString("href", null, "root://icons/palette-3.png");
                        myXmlTextWriter.WriteElementString("x", null, "96");
                        myXmlTextWriter.WriteElementString("y", null, "160");
                        myXmlTextWriter.WriteElementString("w", null, "32");
                        myXmlTextWriter.WriteElementString("h", null, "32");
                        myXmlTextWriter.WriteEndElement();
                        myXmlTextWriter.WriteEndElement();
                        myXmlTextWriter.WriteStartElement("Point");                        
                        myXmlTextWriter.WriteElementString("coordinates", null, thegpsdata.Lon + "," + thegpsdata.Lat);
                        myXmlTextWriter.WriteEndElement();
                        myXmlTextWriter.WriteEndElement();
                    }

                } while (newdata.MoveNext());
            }
            myXmlTextWriter.WriteEndElement();
            newdata.Where.NetType.Value = "infrastructure";
            newdata.Where.Encryption.Value = "None";
            newdata.Query.Load();
            myXmlTextWriter.WriteStartElement("Folder");
            myXmlTextWriter.WriteElementString("name", null, "Open");
            if (newdata.RowCount != 0)
            {
                newdata.Rewind();
                do
                {
                    thegpsdata.Where.Bssid.Value = newdata.BSSID;
                    thegpsdata.Query.Load();
                    
                    if (thegpsdata.RowCount != 0)
                    {
                        thegpsdata.Rewind();
                        
                        string clientstring = "";
                        myXmlTextWriter.WriteStartElement("Placemark");
                        myXmlTextWriter.WriteElementString("name", null, newdata.ESSID);
                        myXmlTextWriter.WriteElementString("styleUrl", null, "#track");
                        myXmlTextWriter.WriteStartElement("description");
                        theclientdata.Where.Bssid.Value = newdata.BSSID;
                        ///theclientdata.Where.Bssid.Operator = WhereParameter.Operand.Equal;
                        theclientdata.Query.Load();
                        if (theclientdata.RowCount != 0)
                        {
                            theclientdata.Rewind();
                            clientstring = "<p>Number of clients=" + theclientdata.RowCount + "<p>";
                            do
                            {
                                clientstring += "<p>" + theclientdata.Clientmac + "-" + theclientdata.Oui + "</p>";

                            } while (theclientdata.MoveNext());
                        }

                        myXmlTextWriter.WriteCData(newdata.BSSID + "-" + newdata.Oui + "<p><b>First Seen: </b><br>" + newdata.FirstTime + "<br>" + "<BR><b> Last Seen </b>" + newdata.LastTime + clientstring);
                        myXmlTextWriter.WriteEndElement();
                        myXmlTextWriter.WriteStartElement("Style");
                        if (newdata.Encryption == "None")
                        {
                            myXmlTextWriter.WriteStartElement("Icon");
                            myXmlTextWriter.WriteElementString("href", null, "root://icons/palette-3.png");
                            myXmlTextWriter.WriteElementString("x", null, "64");
                            myXmlTextWriter.WriteElementString("y", null, "96");
                            myXmlTextWriter.WriteElementString("w", null, "32");
                            myXmlTextWriter.WriteElementString("h", null, "32");
                            myXmlTextWriter.WriteEndElement();
                        }

                        myXmlTextWriter.WriteEndElement();
                        myXmlTextWriter.WriteStartElement("Point");                      
                        myXmlTextWriter.WriteElementString("coordinates", null, thegpsdata.Lon + "," + thegpsdata.Lat);
                        myXmlTextWriter.WriteEndElement();
                        myXmlTextWriter.WriteEndElement();
                    }

                } while (newdata.MoveNext());
            }
            myXmlTextWriter.WriteEndElement();
            myXmlTextWriter.Flush();
            myXmlTextWriter.Close();
        } 
        public static void CreateKmlFileNew(gpsdata thegpsdata, string theconnectionstring)
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = string.Format("{0}\\kismetdata.db", Application.StartupPath);
            builder.SyncMode = SynchronizationModes.Off;
            builder.CacheSize = 5000;
            DatanewTableAdapter thedatanewtableadaptor = new DatanewTableAdapter();
            ClientsTableAdapter theclientstableadaptor = new ClientsTableAdapter();
            GPSDatanewTableAdapter thegpsdatadapator = new GPSDatanewTableAdapter();
            
            SQLiteConnection connection = new SQLiteConnection(builder.ToString());
            connection.Open();
            
            Console.WriteLine("Creating KML File");
            
            DataTable t;

            DataTable gpst;
            DataTable clientt;

            string thelat;
            string thelong;
            XmlTextWriter myXmlTextWriter = new XmlTextWriter(Application.StartupPath + "\\kismet.kml", null);
            myXmlTextWriter.Formatting = Formatting.Indented;
            myXmlTextWriter.WriteStartDocument(false);
            myXmlTextWriter.WriteStartElement("Folder");
            myXmlTextWriter.WriteElementString("name", null, "KismetLogger");
            string[] names = new string[26] { "None","WEP","WEP,CCMP","WEP,TKIP","WEP,TKIP,CCMP","WEP,TKIP,WPA","WEP,TKIP,WPA,AES-CCM","WEP,TKIP,WPA,AES-CCM,CCMP","WEP,TKIP,WPA,PSK","WEP,TKIP,WPA,PSK,AES-CCM","WEP,TKIP,WPA,PSK,AES-CCM,CCMP","WEP,WEP104,TKIP,WPA","WEP,WEP104,TKIP,WPA,AES-CCM","WEP,WEP104,TKIP,WPA,PSK,AES-CCM","WEP,WEP104,WPA","WEP,WEP104,WPA,CCMP","WEP,WEP40,TKIP,WPA","WEP,WEP40,TKIP,WPA,AES-CCM","WEP,WEP40,TKIP,WPA,PSK","WEP,WEP40,WEP104,TKIP,WPA","WEP,WEP40,WEP104,TKIP,WPA,AES-CCM","WEP,WEP40,WPA,AES-CCM","WEP,WPA,AES-CCM","WEP,WPA,AES-CCM,CCMP","WEP,WPA,PSK,AES-CCM","WEP,WPA,PSK,AES-CCM,CCMP"};
            for (int a = 0; a < 26; a++)
            {
                t = thedatanewtableadaptor.GetDataByEncType(names[a]);
                DataTableReader thedatareader = t.CreateDataReader();
                myXmlTextWriter.WriteStartElement("Folder");
                myXmlTextWriter.WriteElementString("name", null,names[a]);
                Console.WriteLine(names[a] + " " + t.Rows.Count);
                for (int i = 0; i < t.Rows.Count; i++)
                {
                    thedatareader.Read();
                    string thebssid = thedatareader["bssid"].ToString();
                    gpst = thegpsdatadapator.GetDataByGpsData(thebssid);
                    if (gpst.Rows.Count != 0)
                    {
                        string clientstring = "";
                        myXmlTextWriter.WriteStartElement("Placemark");
                        myXmlTextWriter.WriteElementString("name", null, thedatareader["Essid"].ToString());
                        myXmlTextWriter.WriteElementString("styleUrl", null, "#track");
                        myXmlTextWriter.WriteStartElement("description");
                        DataTableReader thegpsreader = gpst.CreateDataReader();
                        thegpsreader.Read();
                        thelat = thegpsreader["lat"].ToString();
                        //Console.WriteLine(thelat);
                        
                        thelong = thegpsreader["lon"].ToString();
                        //Console.WriteLine(thelong);
                        clientt = theclientstableadaptor.GetDataByBssid(thebssid);
                        if (clientt.Rows.Count !=0)
                        {
                            DataTableReader clientreader = clientt.CreateDataReader();
                            clientstring = "<p>Number of clients=" + clientt.Rows.Count + "<p>";
                            for (int j = 0; j < clientt.Rows.Count; j++)
                            {
                                clientreader.Read();
                                string theclientmac = clientreader["Clientmac"].ToString();
                                string theclientoui = clientreader["oui"].ToString();
                                clientstring += "<p>" + theclientmac + "-" + theclientoui + "</p>";
                            }
                        }
                        myXmlTextWriter.WriteCData(string.Format("{0}-{1}<p><b>First Seen: </b><br>{2}<br><BR><b> Last Seen </b>{3}{4}", thedatareader["bssid"], thedatareader["Oui"], thedatareader["FirstTime"], thedatareader["LastTime"], clientstring));
                        myXmlTextWriter.WriteEndElement();
                        myXmlTextWriter.WriteStartElement("Style");
                        myXmlTextWriter.WriteStartElement("Icon");
                        myXmlTextWriter.WriteElementString("href", null, "root://icons/palette-3.png");
                        myXmlTextWriter.WriteElementString("x", null, "96");
                        myXmlTextWriter.WriteElementString("y", null, "160");
                        myXmlTextWriter.WriteElementString("w", null, "32");
                        myXmlTextWriter.WriteElementString("h", null, "32");
                        myXmlTextWriter.WriteEndElement();
                        myXmlTextWriter.WriteEndElement();
                        myXmlTextWriter.WriteStartElement("Point");
                        myXmlTextWriter.WriteElementString("coordinates", null, thelong + "," + thelat);
                        myXmlTextWriter.WriteEndElement();
                        myXmlTextWriter.WriteEndElement();
                    }
                }
                myXmlTextWriter.WriteEndElement();
                myXmlTextWriter.Flush();
            }
            
            myXmlTextWriter.Close();
        }

        private static void ProcessCSVFiles(string theconnectionstring, DirectoryInfo dir)
        {
            thedata Kisdata = new thedata();
            gpsdata thegpsdata = new gpsdata();
            Kisdata.ConnectionString = theconnectionstring;
            thegpsdata.ConnectionString = theconnectionstring;
            int count = 0;
            double timestamp = 1161883162;
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            // Add the number of seconds in UNIX timestamp to be converted.
            dateTime = dateTime.AddSeconds(timestamp);

            // The dateTime now contains the right date/time so to format the string,
            // use the standard formatting methods of the DateTime object.
            // string printDate = dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();

            // Print the date and time
            Console.WriteLine(dateTime);
            thegpsdata.LoadAll();
            do
            {
                if (!Kisdata.LoadByPrimaryKey(thegpsdata.Bssid))
                {
                    count++;
                    //Console.WriteLine(thegpsdata.Bssid);

                }
            } while (thegpsdata.MoveNext());
            Console.WriteLine(count);
            
            foreach (FileInfo f in dir.GetFiles("Kismet*.csv"))
            {
                StreamReader sr = File.OpenText(Application.StartupPath + "\\logs\\" + f.Name);
                string input;
                Console.WriteLine(f.Name);
            
                sr.ReadLine();
                while ((input = sr.ReadLine()) != null)
                {
                    if (input.Length != 0)
                    {
                        string[] thestrings;
                        thestrings = input.Split(new char[] { ';' });
                        
                        if(!Kisdata.LoadByPrimaryKey(thestrings[3]))
                        {
                            Console.WriteLine(thestrings[2]);
                            Console.WriteLine(thestrings[3]);
                        }
                        
                    }
                    
                }
            }
            

        }

        /*
		private static void ProcessXMLFiles(string theconnectionstring, DirectoryInfo dir)
		{
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = string.Format("{0}\\kismetdata.db", Application.StartupPath);
            builder.SyncMode = SynchronizationModes.Off;
            builder.CacheSize = 5000;
            //DataTable t;
            FilesTableAdapter thegpsfilesadaptor = new FilesTableAdapter();
            SQLiteConnection connection = new SQLiteConnection(builder.ToString());
            //SQLiteCommand command = connection.CreateCommand();
            connection.Open();
			gpsfiles thegpsfiles = new gpsfiles();
			clientdata theClientData = new clientdata();	
			thedata Kisdata = new thedata();
            ssidchanges thessidchanges = new ssidchanges();
            thessidchanges.ConnectionString = theconnectionstring;
			Kisdata.ConnectionString=theconnectionstring;
			theClientData.ConnectionString=theconnectionstring;
			thegpsfiles.ConnectionString=theconnectionstring;
		    SQLiteCommand insertssidchanges = connection.CreateCommand();

            SQLiteCommand adddatacmd = connection.CreateCommand();
            SQLiteCommand updatedatacmd = connection.CreateCommand();
			XmlSerializer xs = new XmlSerializer(typeof(detectionrun));
			
			
			//ObjectContainer db = Db4o.OpenFile("c:\\testdb.db");
			foreach (FileInfo f in dir.GetFiles("Kismet*.xml"))
			{
				DataTable t;
				    
				    t = thegpsfilesadaptor.GetDataByFilename(f.Name);
                if (t.Rows.Count == 0)
                    {
				
    				//thegpsfiles.AddNew();
					//thegpsfiles.Filename=f.Name;
					//thegpsfiles.Save();
                    InsertFilename(thegpsfilesadaptor, f); 
                    Console.WriteLine("Processing " + f.Name);
					FileStream fs = new FileStream(f.DirectoryName + "\\"+ f.Name, FileMode.Open);
					try
					{
                        detectionrun ds;
						ds=(detectionrun) xs.Deserialize(fs);
						DateTime dbfirsttime = new DateTime();
						DateTime xmlfirsttime = new DateTime();
						DateTime dblasttime = new DateTime();
						DateTime xmllasttime= new DateTime();
						//clientdata theClientData = new clientdata();	
						//thedata Kisdata = new thedata();
						//Kisdata.ConnectionString=theconnectionstring;
						//theClientData.ConnectionString=theconnectionstring;
						int thedatalength;
						thedatalength=ds.wirelessnetwork.Length;
					
						for(int i=0; i<thedatalength; i++)
						{
						
							//Network network1=new Network(ds.wirelessnetwork[i].BSSID, ds.wirelessnetwork[i].SSID);
							//db.Set(network1);
							if(Kisdata.LoadByPrimaryKey(ds.wirelessnetwork[i].BSSID))
							{
								
								IFormatProvider format =new CultureInfo("en-US");
								string [] expectedformats= {"ddd MMM  d HH:mm:ss yyyy", "ddd MMM d HH:mm:ss yyyy"};

								// Wed Mar 29 14:52:56 2006
								try
								{
									dbfirsttime=DateTime.ParseExact(Kisdata.FirstTime,expectedformats,format, DateTimeStyles.AllowWhiteSpaces);
									xmlfirsttime=DateTime.ParseExact(ds.wirelessnetwork[i].firsttime, expectedformats,format, DateTimeStyles.AllowWhiteSpaces);
									dblasttime=DateTime.ParseExact(Kisdata.LastTime,expectedformats,format,DateTimeStyles.AllowWhiteSpaces);
									xmllasttime=DateTime.ParseExact(ds.wirelessnetwork[i].lasttime, expectedformats,format, DateTimeStyles.AllowWhiteSpaces);
								}
								catch(FormatException ex)
								{
									Console.WriteLine(ex);
								}
							    if ((Kisdata.ESSID != ds.wirelessnetwork[i].SSID && ds.wirelessnetwork[i].SSID != null) )
							    {
                                    Console.WriteLine("Old SSID " + Kisdata.ESSID + " New SSID " + ds.wirelessnetwork[i].SSID);
                                    thessidchanges.AddNew();
                                    thessidchanges.Bssid = Kisdata.BSSID;
                                    thessidchanges.Oldname = Kisdata.ESSID;
                                    thessidchanges.Newname = ds.wirelessnetwork[i].SSID;
                                    thessidchanges.Date = ds.wirelessnetwork[i].lasttime;
                                    thessidchanges.Save();
                                    
							        Kisdata.ESSID = ds.wirelessnetwork[i].SSID;
							    }
							    else
							    {
							    }
							    Kisdata.Totalpacketscrypt+=ds.wirelessnetwork[i].packets.crypt;
								Kisdata.Totalpacketsdata+=ds.wirelessnetwork[i].packets.data;
								Kisdata.Totalpacketsdupeiv+=ds.wirelessnetwork[i].packets.dupeiv;
								Kisdata.Totalpacketsllc+=ds.wirelessnetwork[i].packets.LLC;
								Kisdata.Totalpacketstotal+=ds.wirelessnetwork[i].packets.total;
								Kisdata.Totalpacketsweak+=ds.wirelessnetwork[i].packets.weak;
								Kisdata.Cloaked=ds.wirelessnetwork[i].cloaked;
								if (Kisdata.GPSMaxLat > ds.wirelessnetwork[i].gpsinfo.maxlat)
									Kisdata.GPSMaxLat = ds.wirelessnetwork[i].gpsinfo.maxlat;
								if (Kisdata.GPSMaxLon > ds.wirelessnetwork[i].gpsinfo.maxlon)
									Kisdata.GPSMaxLon=ds.wirelessnetwork[i].gpsinfo.maxlon;
								if(Kisdata.GPSMinLat < ds.wirelessnetwork[i].gpsinfo.minlat)
									Kisdata.GPSMinLat=ds.wirelessnetwork[i].gpsinfo.minlat;
								if(Kisdata.GPSMinLon < ds.wirelessnetwork[i].gpsinfo.minlon)
									Kisdata.GPSMinLon=ds.wirelessnetwork[i].gpsinfo.minlon;
								if(xmlfirsttime  < dbfirsttime)
								{
									Kisdata.FirstTime=ds.wirelessnetwork[i].firsttime;
								}
								if(xmllasttime > dblasttime)
								{
									Kisdata.LastTime=ds.wirelessnetwork[i].lasttime;
								}
							
								//if(ds.wirelessnetwork[i].wirelessclient != null)
								//{
								//AddorUpdateClientsSqlite(ds, i, theClientData);
								//}						
								Kisdata.Save();
							}
							else
							{
								AddDataTableSqlite(Kisdata, ds, i);

								//if(ds.wirelessnetwork[i].wirelessclient != null)
								//{
								//AddorUpdateClientsSqlite(ds, i, theClientData);
								//}
								//Kisdata.Save();
							}
							if(ds.wirelessnetwork[i].wirelessclient != null)
							{
								AddorUpdateClientsSqlite(ds, i, theClientData);
                                //AddorUpdateClientsSqliteNew(ds, i, theClientData);
							    
							}						
							Kisdata.Save();

						}
					}
					catch(Exception ex)
					{
						Console.WriteLine(ex);
					}
					
				}
				
			}
			//db.Close();
		}
         */
        private static void ProcessXMLFilesNew(DirectoryInfo dir)
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = string.Format("{0}\\kismetdata.db", Application.StartupPath);
            builder.SyncMode = SynchronizationModes.Off;
            builder.CacheSize = 5000;
            //DataTable t;

            FilesTableAdapter thegpsfilesadaptor = new FilesTableAdapter();
            DatanewTableAdapter thedatanewtableadaptor = new DatanewTableAdapter();
            statsTableAdapter thestatsadaptor = new statsTableAdapter();
            ClientsTableAdapter theclientstableadaptor = new ClientsTableAdapter();
            SQLiteConnection connection = new SQLiteConnection(builder.ToString());
            //SQLiteCommand command = connection.CreateCommand();
            connection.Open();
            //gpsfiles thegpsfiles = new gpsfiles();
            //clientdata theClientData = new clientdata();
            //thedata Kisdata = new thedata();
            //ssidchanges thessidchanges = new ssidchanges();
            //thessidchanges.ConnectionString = theconnectionstring;
            //Kisdata.ConnectionString = theconnectionstring;
            //theClientData.ConnectionString = theconnectionstring;
            //thegpsfiles.ConnectionString = theconnectionstring;
            SQLiteCommand insertssidchanges = connection.CreateCommand();
            int networknew = 0;
            int networkseen = 0;
            int clientnew = 0;
            int clientseen = 0;
            int networktotal = 0;
            int clienttotal = 0;
            SQLiteCommand insertdatacmd = connection.CreateCommand();
            SQLiteCommand updatedatacmd = connection.CreateCommand();

            insertssidchanges.CommandText = "Insert into ssidchanges values(?,?,?,?)";
            insertdatacmd.CommandText =
                "Insert into data values(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            updatedatacmd.CommandText =
                @"UPDATE [Data] SET 
				[oui]=?,
				[totalpacketsweak]=?,
				[totalpacketstotal]=?,
				[totalpacketsllc]=?,
				[totalpacketsdupeiv]=?,
				[totalpacketsdata]=?,
				[totalpacketscrypt]=?,
				[Network]=?,
				[NetType]=?,
				[ESSID]=?,
				[Info]=?,
				[Channel]=?,
				[Cloaked]=?,
				[Encryption]=?,
				[Decrypted]=?,
				[MaxRate]=?,
				[MaxSeenRate]=?,
				[Beacon]=?,
				[LLC]=?,
				[Data]=?,
				[Crypt]=?,
				[Weak]=?,
				[Total]=?,
				[Carrier]=?,
				[Encoding]=?,
				[FirstTime]=?,
				[LastTime]=?,
				[BestQuality]=?,
				[BestSignal]=?,
				[BestNoise]=?,
				[GPSMinLat]=?,
				[GPSMinLon]=?,
				[GPSMinAlt]=?,
				[GPSMinSpd]=?,
				[GPSMaxLat]=?,
				[GPSMaxLon]=?,
				[GPSMaxAlt]=?,
				[GPSMaxSpd]=?,
				[GPSBestLat]=?,
				[GPSBestLon]=?,
				[GPSBestAlt]=?,
				[Datasize]=?,
				[IPType]=?,
				[IP]=?
			WHERE
				[BSSID]=?";

            SQLiteParameter ssidbssid = insertssidchanges.CreateParameter();
            SQLiteParameter ssiddate = insertssidchanges.CreateParameter();
            SQLiteParameter ssidoldname = insertssidchanges.CreateParameter();
            SQLiteParameter ssidnewname = insertssidchanges.CreateParameter();
            insertssidchanges.Parameters.Add(ssidbssid);
            insertssidchanges.Parameters.Add(ssiddate);
            insertssidchanges.Parameters.Add(ssidoldname);
            insertssidchanges.Parameters.Add(ssidnewname);

            SQLiteParameter dataoui = insertdatacmd.CreateParameter();
            SQLiteParameter updateoui = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(dataoui);
            updatedatacmd.Parameters.Add(updateoui);
            SQLiteParameter Totalpacketsweak = insertdatacmd.CreateParameter();
            SQLiteParameter updateTotalpacketsweak = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Totalpacketsweak);
            updatedatacmd.Parameters.Add(updateTotalpacketsweak);
            SQLiteParameter Totalpacketstotal = insertdatacmd.CreateParameter();
            SQLiteParameter updateTotalpacketstotal = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Totalpacketstotal);
            updatedatacmd.Parameters.Add(updateTotalpacketstotal);
            SQLiteParameter Totalpacketsllc = insertdatacmd.CreateParameter();
            SQLiteParameter updateTotalpacketsllc = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Totalpacketsllc);
            updatedatacmd.Parameters.Add(updateTotalpacketsllc);
            SQLiteParameter Totalpacketsdupeiv = insertdatacmd.CreateParameter();
            SQLiteParameter updateTotalpacketsdupeiv = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Totalpacketsdupeiv);
            updatedatacmd.Parameters.Add(updateTotalpacketsdupeiv);
            SQLiteParameter Totalpacketsdata = insertdatacmd.CreateParameter();
            SQLiteParameter updateTotalpacketsdata = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Totalpacketsdata);
            updatedatacmd.Parameters.Add(updateTotalpacketsdata);
            SQLiteParameter Totalpacketscrypt = insertdatacmd.CreateParameter();
            SQLiteParameter updateTotalpacketscrypt = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Totalpacketscrypt);
            updatedatacmd.Parameters.Add(updateTotalpacketscrypt);
            SQLiteParameter Network = insertdatacmd.CreateParameter();
            SQLiteParameter updateNetwork = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Network);
            updatedatacmd.Parameters.Add(updateNetwork);
            SQLiteParameter updateNetType = updatedatacmd.CreateParameter();
            SQLiteParameter NetType = insertdatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(NetType);
            updatedatacmd.Parameters.Add(updateNetType);
            SQLiteParameter ESSID = insertdatacmd.CreateParameter();
            SQLiteParameter updateESSID = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(ESSID);
            updatedatacmd.Parameters.Add(updateESSID);
            SQLiteParameter BSSID = insertdatacmd.CreateParameter();

            insertdatacmd.Parameters.Add(BSSID);
            SQLiteParameter Info = insertdatacmd.CreateParameter();
            SQLiteParameter updateInfo = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Info);
            updatedatacmd.Parameters.Add(updateInfo);
            SQLiteParameter Channel = insertdatacmd.CreateParameter();
            SQLiteParameter updateChannel = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Channel);
            updatedatacmd.Parameters.Add(updateChannel);
            SQLiteParameter updateCloaked = updatedatacmd.CreateParameter();
            SQLiteParameter Cloaked = insertdatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Cloaked);
            updatedatacmd.Parameters.Add(updateCloaked);
            SQLiteParameter Encryption = insertdatacmd.CreateParameter();
            SQLiteParameter updateEncryption = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Encryption);
            updatedatacmd.Parameters.Add(updateEncryption);
            SQLiteParameter updateDecrypted = updatedatacmd.CreateParameter();
            SQLiteParameter Decrypted = insertdatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Decrypted);
            updatedatacmd.Parameters.Add(updateDecrypted);
            SQLiteParameter MaxRate = insertdatacmd.CreateParameter();
            SQLiteParameter updateMaxRate = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(MaxRate);
            updatedatacmd.Parameters.Add(updateMaxRate);
            SQLiteParameter MaxSeenRate = insertdatacmd.CreateParameter();
            SQLiteParameter updateMaxSeenRate = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(MaxSeenRate);
            updatedatacmd.Parameters.Add(updateMaxSeenRate);
            SQLiteParameter Beacon = insertdatacmd.CreateParameter();
            SQLiteParameter updateBeacon = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Beacon);
            updatedatacmd.Parameters.Add(updateBeacon);
            SQLiteParameter LLC = insertdatacmd.CreateParameter();
            SQLiteParameter updateLLC = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(LLC);
            updatedatacmd.Parameters.Add(updateLLC);
            SQLiteParameter Data = insertdatacmd.CreateParameter();
            SQLiteParameter updateData = updatedatacmd.CreateParameter();

            insertdatacmd.Parameters.Add(Data);
            updatedatacmd.Parameters.Add(updateData);
            SQLiteParameter Crypt = insertdatacmd.CreateParameter();
            SQLiteParameter updateCrypt = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Crypt);
            updatedatacmd.Parameters.Add(updateCrypt);
            SQLiteParameter Weak = insertdatacmd.CreateParameter();
            SQLiteParameter updateWeak = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Weak);
            updatedatacmd.Parameters.Add(updateWeak);
            SQLiteParameter Total = insertdatacmd.CreateParameter();
            SQLiteParameter updateTotal = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Total);
            updatedatacmd.Parameters.Add(updateTotal);
            SQLiteParameter Carrier = insertdatacmd.CreateParameter();
            SQLiteParameter updateCarrier = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Carrier);
            updatedatacmd.Parameters.Add(updateCarrier);
            SQLiteParameter Encoding = insertdatacmd.CreateParameter();
            SQLiteParameter updateEncoding = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Encoding);
            updatedatacmd.Parameters.Add(updateEncoding);
            SQLiteParameter FirstTime = insertdatacmd.CreateParameter();
            SQLiteParameter updateFirstTime = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(FirstTime);
            updatedatacmd.Parameters.Add(updateFirstTime);
            SQLiteParameter LastTime = insertdatacmd.CreateParameter();
            SQLiteParameter updateLastTime = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(LastTime);
            updatedatacmd.Parameters.Add(updateLastTime);
            SQLiteParameter BestQuality = insertdatacmd.CreateParameter();
            SQLiteParameter updateBestQuality = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(BestQuality);
            updatedatacmd.Parameters.Add(updateBestQuality);
            SQLiteParameter BestSignal = insertdatacmd.CreateParameter();
            SQLiteParameter updateBestSignal = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(BestSignal);
            updatedatacmd.Parameters.Add(updateBestSignal);
            SQLiteParameter BestNoise = insertdatacmd.CreateParameter();
            SQLiteParameter updateBestNoise = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(BestNoise);
            updatedatacmd.Parameters.Add(updateBestNoise);
            SQLiteParameter GPSMinLat = insertdatacmd.CreateParameter();
            SQLiteParameter updateGPSMinLat = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(GPSMinLat);
            updatedatacmd.Parameters.Add(updateGPSMinLat);
            SQLiteParameter GPSMinLon = insertdatacmd.CreateParameter();
            SQLiteParameter updateGPSMinLon = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(GPSMinLon);
            updatedatacmd.Parameters.Add(updateGPSMinLon);
            SQLiteParameter GPSMinAlt = insertdatacmd.CreateParameter();
            SQLiteParameter updateGPSMinAlt = updatedatacmd.CreateParameter();

            insertdatacmd.Parameters.Add(GPSMinAlt);
            updatedatacmd.Parameters.Add(updateGPSMinAlt);
            SQLiteParameter GPSMinSpd = insertdatacmd.CreateParameter();
            SQLiteParameter updateGPSMinSpd = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(GPSMinSpd);
            updatedatacmd.Parameters.Add(updateGPSMinSpd);
            SQLiteParameter GPSMaxLat = insertdatacmd.CreateParameter();
            SQLiteParameter updateGPSMaxLat = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(GPSMaxLat);
            updatedatacmd.Parameters.Add(updateGPSMaxLat);
            SQLiteParameter GPSMaxLon = insertdatacmd.CreateParameter();
            SQLiteParameter updateGPSMaxLon = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(GPSMaxLon);
            updatedatacmd.Parameters.Add(updateGPSMaxLon);
            SQLiteParameter GPSMaxAlt = insertdatacmd.CreateParameter();
            SQLiteParameter updateGPSMaxAlt = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(GPSMaxAlt);
            updatedatacmd.Parameters.Add(updateGPSMaxAlt);
            SQLiteParameter GPSMaxSpd = insertdatacmd.CreateParameter();
            SQLiteParameter updateGPSMaxSpd = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(GPSMaxSpd);
            updatedatacmd.Parameters.Add(updateGPSMaxSpd);
            SQLiteParameter GPSBestLat = insertdatacmd.CreateParameter();
            SQLiteParameter updateGPSBestLat = updatedatacmd.CreateParameter();

            insertdatacmd.Parameters.Add(GPSBestLat);
            updatedatacmd.Parameters.Add(updateGPSBestLat);
            SQLiteParameter GPSBestLon = insertdatacmd.CreateParameter();
            SQLiteParameter updateGPSBestLon = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(GPSBestLon);
            updatedatacmd.Parameters.Add(updateGPSBestLon);
            SQLiteParameter GPSBestAlt = insertdatacmd.CreateParameter();
            SQLiteParameter updateGPSBestAlt = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(GPSBestAlt);
            updatedatacmd.Parameters.Add(updateGPSBestAlt);
            SQLiteParameter Datasize = insertdatacmd.CreateParameter();
            SQLiteParameter updateDatasize = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(Datasize);
            updatedatacmd.Parameters.Add(updateDatasize);
            SQLiteParameter IPType = insertdatacmd.CreateParameter();
            SQLiteParameter updateIPType = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(IPType);
            updatedatacmd.Parameters.Add(updateIPType);
            SQLiteParameter IP = insertdatacmd.CreateParameter();
            SQLiteParameter updateIP = updatedatacmd.CreateParameter();
            insertdatacmd.Parameters.Add(IP);
            updatedatacmd.Parameters.Add(updateIP);

            SQLiteParameter updateBSSID = updatedatacmd.CreateParameter();
            updatedatacmd.Parameters.Add(updateBSSID);


            SQLiteCommand insertclients = connection.CreateCommand();
            SQLiteCommand updateclients = connection.CreateCommand();
            insertclients.CommandText = "Insert into clients values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            updateclients.CommandText =
                @"UPDATE [Clients] SET 
				[Clientipaddress]=?,
				[oui]=?,
				[clientencryption]=?,
				[clientfirsttime]=?,
				[clientgpsminlat]=?,
				[clientgpsminlon]=?,
				[clientgpsmaxlat]=?,
				[clientgpsmaxlon]=?,
				[clientgpsminalt]=?,
				[clientgpsmaxalt]=?,
				[clientgpsminspeed]=?,
				[clientgpsmaxspeed]=?,
				[clientdatasize]=?,
				[clientmaxrate]=?,
				[clientmaxseenrate]=?,
				[clientlasttime]=?,
				[clientpacketscrypt]=?,
				[clientpacketsdata]=?,
				[clientpacketsweak]=?,
				[Number]=?,
				[Type]=?,
				[wep]=?
			WHERE
				[bssid]=? AND 
				[clientmac]=?";
            SQLiteParameter Clientipaddress = insertclients.CreateParameter();
            SQLiteParameter updateClientipaddress = updateclients.CreateParameter();
            SQLiteParameter insertoui = insertclients.CreateParameter();
            SQLiteParameter clientupdateoui = updateclients.CreateParameter();
            SQLiteParameter bssid = insertclients.CreateParameter();

            SQLiteParameter clientmac = insertclients.CreateParameter();

            SQLiteParameter clientencryption = insertclients.CreateParameter();
            SQLiteParameter updateclientencryption = updateclients.CreateParameter();
            SQLiteParameter clientfirsttime = insertclients.CreateParameter();
            SQLiteParameter updateclientfirsttime = updateclients.CreateParameter();
            SQLiteParameter clientgpsminlat = insertclients.CreateParameter();
            SQLiteParameter clientgpsminlon = insertclients.CreateParameter();
            SQLiteParameter clientgpsmaxlat = insertclients.CreateParameter();
            SQLiteParameter clientgpsmaxlon = insertclients.CreateParameter();
            SQLiteParameter clientgpsminalt = insertclients.CreateParameter();
            SQLiteParameter clientgpsmaxalt = insertclients.CreateParameter();
            SQLiteParameter clientgpsminspeed = insertclients.CreateParameter();
            SQLiteParameter clientgpsmaxspeed = insertclients.CreateParameter();
            SQLiteParameter clientdatasize = insertclients.CreateParameter();
            SQLiteParameter clientmaxrate = insertclients.CreateParameter();
            SQLiteParameter clientmaxseenrate = insertclients.CreateParameter();
            SQLiteParameter clientlasttime = insertclients.CreateParameter();
            SQLiteParameter clientpacketscrypt = insertclients.CreateParameter();
            SQLiteParameter clientpacketsdata = insertclients.CreateParameter();
            SQLiteParameter clientpacketsweak = insertclients.CreateParameter();
            SQLiteParameter Number = insertclients.CreateParameter();
            SQLiteParameter Type = insertclients.CreateParameter();
            SQLiteParameter wep = insertclients.CreateParameter();


            SQLiteParameter updateclientgpsminlat = updateclients.CreateParameter();
            SQLiteParameter updateclientgpsminlon = updateclients.CreateParameter();
            SQLiteParameter updateclientgpsmaxlat = updateclients.CreateParameter();
            SQLiteParameter updateclientgpsmaxlon = updateclients.CreateParameter();
            SQLiteParameter updateclientgpsminalt = updateclients.CreateParameter();
            SQLiteParameter updateclientgpsmaxalt = updateclients.CreateParameter();
            SQLiteParameter updateclientgpsminspeed = updateclients.CreateParameter();
            SQLiteParameter updateclientgpsmaxspeed = updateclients.CreateParameter();
            SQLiteParameter updateclientdatasize = updateclients.CreateParameter();
            SQLiteParameter updateclientmaxrate = updateclients.CreateParameter();
            SQLiteParameter updateclientmaxseenrate = updateclients.CreateParameter();
            SQLiteParameter updateclientlasttime = updateclients.CreateParameter();
            SQLiteParameter updateclientpacketscrypt = updateclients.CreateParameter();
            SQLiteParameter updateclientpacketsdata = updateclients.CreateParameter();
            SQLiteParameter updateclientpacketsweak = updateclients.CreateParameter();
            SQLiteParameter updateNumber = updateclients.CreateParameter();
            SQLiteParameter updateType = updateclients.CreateParameter();
            SQLiteParameter updatewep = updateclients.CreateParameter();
            SQLiteParameter updatebssid = insertclients.CreateParameter();
            SQLiteParameter updateclientmac = updateclients.CreateParameter();


            insertclients.Parameters.Add(Clientipaddress);
            insertclients.Parameters.Add(insertoui);
            insertclients.Parameters.Add(bssid);
            insertclients.Parameters.Add(clientmac);
            insertclients.Parameters.Add(clientencryption);
            insertclients.Parameters.Add(clientfirsttime);
            insertclients.Parameters.Add(clientgpsminlat);
            insertclients.Parameters.Add(clientgpsminlon);
            insertclients.Parameters.Add(clientgpsmaxlat);
            insertclients.Parameters.Add(clientgpsmaxlon);
            insertclients.Parameters.Add(clientgpsminalt);
            insertclients.Parameters.Add(clientgpsmaxalt);
            insertclients.Parameters.Add(clientgpsminspeed);
            insertclients.Parameters.Add(clientgpsmaxspeed);
            insertclients.Parameters.Add(clientdatasize);
            insertclients.Parameters.Add(clientmaxrate);
            insertclients.Parameters.Add(clientmaxseenrate);
            insertclients.Parameters.Add(clientlasttime);
            insertclients.Parameters.Add(clientpacketscrypt);
            insertclients.Parameters.Add(clientpacketsdata);
            insertclients.Parameters.Add(clientpacketsweak);
            insertclients.Parameters.Add(Number);
            insertclients.Parameters.Add(Type);
            insertclients.Parameters.Add(wep);


            updateclients.Parameters.Add(updateClientipaddress);
            updateclients.Parameters.Add(clientupdateoui);

            updateclients.Parameters.Add(updateclientencryption);
            updateclients.Parameters.Add(updateclientfirsttime);
            updateclients.Parameters.Add(updateclientgpsminlat);
            updateclients.Parameters.Add(updateclientgpsminlon);
            updateclients.Parameters.Add(updateclientgpsmaxlat);
            updateclients.Parameters.Add(updateclientgpsmaxlon);
            updateclients.Parameters.Add(updateclientgpsminalt);
            updateclients.Parameters.Add(updateclientgpsmaxalt);
            updateclients.Parameters.Add(updateclientgpsminspeed);
            updateclients.Parameters.Add(updateclientgpsmaxspeed);
            updateclients.Parameters.Add(updateclientdatasize);
            updateclients.Parameters.Add(updateclientmaxrate);
            updateclients.Parameters.Add(updateclientmaxseenrate);
            updateclients.Parameters.Add(updateclientlasttime);
            updateclients.Parameters.Add(updateclientpacketscrypt);
            updateclients.Parameters.Add(updateclientpacketsdata);
            updateclients.Parameters.Add(updateclientpacketsweak);
            updateclients.Parameters.Add(updateNumber);
            updateclients.Parameters.Add(updateType);
            updateclients.Parameters.Add(updatewep);
            updateclients.Parameters.Add(updatebssid);
            updateclients.Parameters.Add(updateclientmac);

            XmlSerializer xs = new XmlSerializer(typeof (detectionrun));


            //ObjectContainer db = Db4o.OpenFile("c:\\testdb.db");
            DataTable t;
            DateTime dbfirsttime = new DateTime();
            DateTime xmlfirsttime = new DateTime();
            DateTime dblasttime = new DateTime();
            DateTime xmllasttime = new DateTime();
            FileInfo[] fis = dir.GetFiles("Kismet*.xml");
            Array.Sort(fis, new CompareFileTime());
            

            detectionrun ds;
            //for (int ij = 0; ij < fis.Length; ij++)
            //{
            foreach (FileInfo f in dir.GetFiles("Kismet*.xml"))
            {
                //FileInfo f = fis[ij];
                t = thegpsfilesadaptor.GetDataByFilename(f.Name);
                if (t.Rows.Count == 0)
                {
                    //thegpsfiles.AddNew();
                    //thegpsfiles.Filename=f.Name;
                    //thegpsfiles.Save();
                    InsertFilename(thegpsfilesadaptor, f);
                    Console.WriteLine("Processing " + f.Name);
                    FileStream fs = new FileStream(f.DirectoryName + "\\" + f.Name, FileMode.Open);
                    try
                    {
                        ds = (detectionrun) xs.Deserialize(fs);
                        //clientdata theClientData = new clientdata();	
                        //thedata Kisdata = new thedata();
                        //Kisdata.ConnectionString=theconnectionstring;
                        //theClientData.ConnectionString=theconnectionstring;
                        int thedatalength;
                        thedatalength = ds.wirelessnetwork.Length;
                        networktotal = thedatalength;
                        IFormatProvider format = new CultureInfo("en-US");
                        for (int i = 0; i < thedatalength; i++)
                        {
                            //Network network1=new Network(ds.wirelessnetwork[i].BSSID, ds.wirelessnetwork[i].SSID);
                            //db.Set(network1);
                            t = thedatanewtableadaptor.GetDataByBssid(ds.wirelessnetwork[i].BSSID);

                            if (t.Rows.Count != 0)
                            {
                                networkseen++;
                                DataTableReader thereader = t.CreateDataReader();
                                thereader.Read();
                                string firsttime = thereader["firsttime"].ToString();
                                string lasttime = thereader["lasttime"].ToString();
                                if (firsttime == "")
                                {
                                    Console.WriteLine(firsttime);
                                    Console.WriteLine(lasttime);
                                }

                                string[] expectedformats = {"ddd MMM  d HH:mm:ss yyyy", "ddd MMM d HH:mm:ss yyyy"};

                                // Wed Mar 29 14:52:56 2006
                                try
                                {
                                    dbfirsttime =
                                        DateTime.ParseExact(firsttime, expectedformats, format,
                                                            DateTimeStyles.AllowWhiteSpaces);
                                    xmlfirsttime =
                                        DateTime.ParseExact(ds.wirelessnetwork[i].firsttime, expectedformats, format,
                                                            DateTimeStyles.AllowWhiteSpaces);
                                    dblasttime =
                                        DateTime.ParseExact(lasttime, expectedformats, format,
                                                            DateTimeStyles.AllowWhiteSpaces);
                                    xmllasttime =
                                        DateTime.ParseExact(ds.wirelessnetwork[i].lasttime, expectedformats, format,
                                                            DateTimeStyles.AllowWhiteSpaces);
                                }
                                catch (FormatException ex)
                                {
                                    Console.WriteLine(ex);
                                }
                                if ((thereader["essid"].ToString() != ds.wirelessnetwork[i].SSID) &&
                                    ds.wirelessnetwork[i].SSID != null)
                                {
                                    Console.WriteLine("Old SSID " + thereader["essid"] + " New SSID " +
                                                      ds.wirelessnetwork[i].SSID);
                                    ssidbssid.Value = thereader["bssid"];
                                    //thessidchanges.AddNew();
                                    ssiddate.Value = ds.wirelessnetwork[i].lasttime;
                                    ssidoldname.Value = thereader["essid"].ToString();
                                    ssidnewname.Value = ds.wirelessnetwork[i].SSID;
                                    insertssidchanges.ExecuteNonQuery();

                                    updateESSID.Value = ds.wirelessnetwork[i].SSID;

                                    //Kisdata.ESSID = ds.wirelessnetwork[i].SSID;
                                }
                                else
                                {
                                    updateESSID.Value = thereader["ESSID"];
                                }
                                updateTotalpacketscrypt.Value =
                                    Convert.ToInt32(thereader["Totalpacketscrypt"]) +
                                    ds.wirelessnetwork[i].packets.crypt;
                                updateTotalpacketsdata.Value = Convert.ToInt32(thereader["Totalpacketsdata"]) +
                                                               ds.wirelessnetwork[i].packets.data;
                                updateTotalpacketsdupeiv.Value = Convert.ToInt32(thereader["Totalpacketsdupeiv"]) +
                                                                 ds.wirelessnetwork[i].packets.dupeiv;
                                updateTotalpacketsllc.Value = Convert.ToInt32(thereader["Totalpacketsllc"]) +
                                                              ds.wirelessnetwork[i].packets.LLC;
                                updateTotalpacketstotal.Value = Convert.ToInt32(thereader["Totalpacketstotal"]) +
                                                                ds.wirelessnetwork[i].packets.total;
                                updateTotalpacketsweak.Value = Convert.ToInt32(thereader["Totalpacketsweak"]) +
                                                               ds.wirelessnetwork[i].packets.weak;
                                updateCloaked.Value = ds.wirelessnetwork[i].cloaked;
                                updateoui.Value = thereader["oui"];
                                updateNetwork.Value = thereader["Network"];
                                updateEncryption.Value = enchash[ds.wirelessnetwork[i].BSSID];
                                updateNetType.Value = thereader["NetType"];
                                updateMaxRate.Value = ds.wirelessnetwork[i].maxrate;
                                updateMaxSeenRate.Value = ds.wirelessnetwork[i].maxseenrate;
                                if (Convert.ToInt16(thereader["GPSMaxLat"]) > ds.wirelessnetwork[i].gpsinfo.maxlat)
                                {
                                    updateGPSMaxLat.Value = ds.wirelessnetwork[i].gpsinfo.maxlat;
                                }
                                else
                                {
                                    updateGPSMaxLat.Value = thereader["GPSMaxLat"];
                                }
                                if (Convert.ToInt16(thereader["GPSMaxLon"]) > ds.wirelessnetwork[i].gpsinfo.maxlon)
                                {
                                    updateGPSMaxLon.Value = ds.wirelessnetwork[i].gpsinfo.maxlon;
                                }
                                else
                                {
                                    updateGPSMaxLon.Value = thereader["GPSMaxLon"];
                                }
                                if (Convert.ToInt16(thereader["GPSMinLat"]) < ds.wirelessnetwork[i].gpsinfo.minlat)
                                {
                                    updateGPSMinLat.Value = ds.wirelessnetwork[i].gpsinfo.minlat;
                                }
                                else
                                {
                                    updateGPSMinLat.Value = thereader["GPSMinLat"];
                                }
                                updateGPSMinLon.Value = Convert.ToInt16(thereader["GPSMinLon"]) <
                                                        ds.wirelessnetwork[i].gpsinfo.minlon
                                                            ? ds.wirelessnetwork[i].gpsinfo.minlon
                                                            : thereader["GPSMinLon"];
                                if (xmlfirsttime < dbfirsttime)
                                {
                                    updateFirstTime.Value = ds.wirelessnetwork[i].firsttime;
                                }
                                else
                                {
                                    updateFirstTime.Value = thereader["FirstTime"];
                                }
                                if (xmllasttime > dblasttime)
                                {
                                    updateLastTime.Value = ds.wirelessnetwork[i].lasttime;
                                }
                                else
                                {
                                    updateLastTime.Value = thereader["LastTime"];
                                }
                                updateBSSID.Value = ds.wirelessnetwork[i].BSSID;

                                //{"The database file is locked\r\nUnable to close due to unfinalised statements"}
                                //updateESSID.Value=thereader["ESSID"];
                                //if(ds.wirelessnetwork[i].wirelessclient != null)
                                //{
                                //AddorUpdateClientsSqlite(ds, i, theClientData);
                                //}	
                                thereader.Close();
                                try
                                {
                                    updatedatacmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                            else
                            {
                                //AddDataTableSqlite(Kisdata, ds, i);
                                networknew++;
                                //AddDataTableNew(ds, i, insertdatacmd);
                                dataoui.Value = LookupOuiData(ds.wirelessnetwork[i].BSSID);
                                Cloaked.Value = ds.wirelessnetwork[i].cloaked;
                                FirstTime.Value = ds.wirelessnetwork[i].firsttime;
                                LastTime.Value = ds.wirelessnetwork[i].lasttime;
                                BSSID.Value = ds.wirelessnetwork[i].BSSID;
                                Network.Value = ds.wirelessnetwork[i].number;
                                NetType.Value = ds.wirelessnetwork[i].type;
                                Channel.Value = ds.wirelessnetwork[i].channel;
                                Encryption.Value = enchash[ds.wirelessnetwork[i].BSSID];
                                Totalpacketscrypt.Value = ds.wirelessnetwork[i].packets.crypt;
                                Totalpacketsdata.Value = ds.wirelessnetwork[i].packets.data;
                                Totalpacketsdupeiv.Value = ds.wirelessnetwork[i].packets.dupeiv;
                                Totalpacketsllc.Value = ds.wirelessnetwork[i].packets.LLC;
                                Totalpacketstotal.Value = ds.wirelessnetwork[i].packets.total;
                                Totalpacketsweak.Value = ds.wirelessnetwork[i].packets.weak;
                                MaxRate.Value = ds.wirelessnetwork[i].maxrate;
                                MaxSeenRate.Value = ds.wirelessnetwork[i].maxseenrate;
                                ESSID.Value = ds.wirelessnetwork[i].SSID ?? "no ssid";
                                GPSMaxLat.Value = ds.wirelessnetwork[i].gpsinfo.maxlat;
                                GPSMaxLon.Value = ds.wirelessnetwork[i].gpsinfo.maxlon;
                                GPSMinLat.Value = ds.wirelessnetwork[i].gpsinfo.minlat;
                                GPSMinLon.Value = ds.wirelessnetwork[i].gpsinfo.minlon;
                                insertdatacmd.ExecuteNonQuery();

                                
                            }
                            if (ds.wirelessnetwork[i].wirelessclient != null)
                            {
                                clienttotal += ds.wirelessnetwork[i].wirelessclient.Length;
                                for (int j = 0; j < ds.wirelessnetwork[i].wirelessclient.Length; j++)
                                {
                                    t =
                                        theclientstableadaptor.GetDataByClientBssid(
                                            ds.wirelessnetwork[i].wirelessclient[j].clientmac,
                                            ds.wirelessnetwork[i].BSSID);
                                    if (t.Rows.Count == 0)
                                    {
                                       
                                        clientnew++;
                                        bssid.Value = ds.wirelessnetwork[i].BSSID;
                                        insertoui.Value =
                                            LookupOuiData(ds.wirelessnetwork[i].wirelessclient[j].clientmac);
                                        clientmac.Value = ds.wirelessnetwork[i].wirelessclient[j].clientmac;
                                        clientdatasize.Value = ds.wirelessnetwork[i].wirelessclient[j].clientdatasize;
                                        clientencryption.Value =
                                            ds.wirelessnetwork[i].wirelessclient[j].clientencryption;
                                        clientfirsttime.Value = ds.wirelessnetwork[i].wirelessclient[j].firsttime;
                                        clientlasttime.Value = ds.wirelessnetwork[i].wirelessclient[j].lasttime;
                                        if (ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo != null)
                                        {
                                            clientgpsmaxalt.Value =
                                                ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxalt;
                                            clientgpsmaxlat.Value =
                                                ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxlat;
                                            clientgpsmaxlon.Value =
                                                ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxlon;
                                            clientgpsminalt.Value =
                                                ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminalt;
                                            clientgpsminlat.Value =
                                                ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminlat;
                                            clientgpsminlon.Value =
                                                ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminlon;
                                        }
                                        clientmaxrate.Value = ds.wirelessnetwork[i].wirelessclient[j].clientmaxrate;
                                        if (ds.wirelessnetwork[i].wirelessclient[j].clientipaddress != null)
                                        {
                                            Clientipaddress.Value =
                                                ds.wirelessnetwork[i].wirelessclient[j].clientipaddress;
                                        }
                                        clientmaxseenrate.Value =
                                            ds.wirelessnetwork[i].wirelessclient[j].clientmaxseenrate;
                                        clientpacketscrypt.Value =
                                            ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientcrypt;
                                        clientpacketsdata.Value =
                                            ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientdata;
                                        clientpacketsweak.Value =
                                            ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientweak;

                                        insertclients.ExecuteNonQuery();

                                        //theClientData.Save();
                                    }
                                    else
                                    {
                                        clientseen++;
                                        DataTableReader thereader = t.CreateDataReader();
                                        thereader.Read();
                                        
                                        updateclientdatasize.Value = Convert.ToInt32(thereader["clientdatasize"]) +
                                                                     ds.wirelessnetwork[i].wirelessclient[j].
                                                                         clientdatasize;
                                        updateoui.Value = LookupOuiData(ds.wirelessnetwork[i].wirelessclient[j].clientmac);
                                        updateclientencryption.Value =
                                            ds.wirelessnetwork[i].wirelessclient[j].clientencryption;
                                        //IFormatProvider format = new CultureInfo("en-US");
                                        string[] expectedformats = {
                                                                       "ddd MMM  d HH:mm:ss yyyy",
                                                                       "ddd MMM d HH:mm:ss yyyy"
                                                                   };
                                        string firsttime = thereader["clientfirsttime"].ToString();
                                        string lasttime = thereader["clientlasttime"].ToString();
                                        
                                        dbfirsttime =
                                            DateTime.ParseExact(firsttime, expectedformats, format,
                                                                DateTimeStyles.AllowWhiteSpaces);
                                        xmlfirsttime =
                                            DateTime.ParseExact(ds.wirelessnetwork[i].wirelessclient[j].firsttime,
                                                                expectedformats, format, DateTimeStyles.AllowWhiteSpaces);
                                        dblasttime =
                                            DateTime.ParseExact(lasttime, expectedformats, format,
                                                                DateTimeStyles.AllowWhiteSpaces);
                                        xmllasttime =
                                            DateTime.ParseExact(ds.wirelessnetwork[i].wirelessclient[j].lasttime,
                                                                expectedformats, format, DateTimeStyles.AllowWhiteSpaces);

                                        if (xmlfirsttime < dbfirsttime)
                                        {
                                            updateclientfirsttime.Value = ds.wirelessnetwork[i].firsttime;
                                        }
                                        else
                                        {
                                            updateclientfirsttime.Value = thereader["clientfirsttime"];
                                        }
                                        if (xmllasttime > dblasttime)
                                        {
                                            updateclientlasttime.Value = ds.wirelessnetwork[i].lasttime;
                                        }
                                        else
                                        {
                                            updateclientlasttime.Value = thereader["clientlasttime"];
                                        }

                                        
                                        if (ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo != null)
                                        {
                                            updateclientgpsmaxalt.Value =
                                                ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxalt;
                                            updateclientgpsmaxlat.Value =
                                                ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxlat;
                                            updateclientgpsmaxlon.Value =
                                                ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxlon;
                                            updateclientgpsminalt.Value =
                                                ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminalt;
                                            updateclientgpsminlat.Value =
                                                ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminlat;
                                            updateclientgpsminlon.Value =
                                                ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminlon;
                                        }
                                        updateclientmaxrate.Value =
                                            ds.wirelessnetwork[i].wirelessclient[j].clientmaxrate;
                                        if (ds.wirelessnetwork[i].wirelessclient[j].clientipaddress != null)
                                        {
                                            updateClientipaddress.Value =
                                                ds.wirelessnetwork[i].wirelessclient[j].clientipaddress;
                                        }
                                        updateclientmaxseenrate.Value =
                                            ds.wirelessnetwork[i].wirelessclient[j].clientmaxseenrate;
                                        updateclientpacketscrypt.Value =
                                            Convert.ToInt32(thereader["clientpacketscrypt"]) +
                                            ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientcrypt;
                                        updateclientpacketsdata.Value =
                                            Convert.ToInt32(thereader["clientpacketsdata"]) +
                                            ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientdata;
                                        updateclientpacketsweak.Value =
                                            Convert.ToInt32(thereader["clientpacketsweak"]) +
                                            ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientweak;
                                        //theClientData.Save();
                                        updateclients.ExecuteNonQuery();
                                        t = null;
                                        thereader.Close();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    thestatsadaptor.InsertQuery(f.Name, networktotal, networknew, networkseen, clienttotal, clientnew,
                                                clientseen);

                    networknew = 0;
                    networkseen = 0;
                    networktotal = 0;
                    clienttotal = 0;
                    clientnew = 0;
                    clientseen = 0;
                    fs.Close();
                }
            }
        

	    connection.Close();
        }

	    private static void InsertFilename(FilesTableAdapter thegpsfilesadaptor, FileSystemInfo f)
	    {
	        try
	        {
	            string[] split = null;
	            split = f.Name.Split(new char[] {'-'});
	            string temp = split[4].Substring(0, (split[4].Length - 4));
	            int thenumber = Convert.ToInt16(temp);

	            thegpsfilesadaptor.InsertQuery(f.Name, Convert.ToInt16(split[3]), split[1],
	                                           Convert.ToInt16(split[2]), thenumber);
	        }
	        catch(Exception)
	        {
	            thegpsfilesadaptor.InsertQuery(f.Name, 0, "None", 0, 0);
	        }
	    }
        private static void CreateDotFile()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = string.Format("{0}\\kismetdata.db", Application.StartupPath);
            builder.SyncMode = SynchronizationModes.Off;
            builder.CacheSize = 5000;
            SQLiteConnection connection = new SQLiteConnection(builder.ToString());
            connection.Open();
            SQLiteCommand addcmd = connection.CreateCommand();
            addcmd.CommandText = "Select * from dog where bssid=\"00:0C:E5:45:C5:BD\"";
            SQLiteDataReader t = addcmd.ExecuteReader();
            Console.WriteLine("digraph networks {");
            while (t.Read())
            {
                Console.WriteLine("\""+t["clientmac"]+ "\""+ " -> "+ "\""+ t["bssid"]+"\"");
                //Console.WriteLine(t["data.bssid"]);    
            }
            Console.WriteLine("}");
            t.Close();
            
        }
	    private static void AddDataTableNew(detectionrun ds, int i, SQLiteCommand insertdata)
        {
            insertdata.Parameters["Cloaked"].Value = ds.wirelessnetwork[i].cloaked;
            insertdata.Parameters["FirstTime"].Value = ds.wirelessnetwork[i].firsttime;
            insertdata.Parameters["LastTime"].Value = ds.wirelessnetwork[i].lasttime;
            insertdata.Parameters["BSSID"].Value = ds.wirelessnetwork[i].BSSID;
            insertdata.Parameters["Network"].Value = ds.wirelessnetwork[i].number;
            insertdata.Parameters["NetType"].Value = ds.wirelessnetwork[i].type;
            insertdata.Parameters["Channel"].Value = ds.wirelessnetwork[i].channel;
            insertdata.Parameters["Encryption"].Value = ds.wirelessnetwork[i].encryption;
            insertdata.Parameters["Totalpacketscrypt"].Value = ds.wirelessnetwork[i].packets.crypt;
            insertdata.Parameters["Totalpacketsdata"].Value = ds.wirelessnetwork[i].packets.data;
            insertdata.Parameters["Totalpacketsdupeiv"].Value = ds.wirelessnetwork[i].packets.dupeiv;
            insertdata.Parameters["Totalpacketsllc"].Value = ds.wirelessnetwork[i].packets.LLC;
            insertdata.Parameters["Totalpacketstotal"].Value = ds.wirelessnetwork[i].packets.total;
            insertdata.Parameters["Totalpacketsweak"].Value = ds.wirelessnetwork[i].packets.weak;

            insertdata.Parameters["ESSID"].Value = ds.wirelessnetwork[i].SSID ?? "no ssid";
            insertdata.Parameters["GPSMaxLat"].Value = ds.wirelessnetwork[i].gpsinfo.maxlat;
            insertdata.Parameters["GPSMaxLon"].Value = ds.wirelessnetwork[i].gpsinfo.maxlon;
            insertdata.Parameters["GPSMinLat"].Value = ds.wirelessnetwork[i].gpsinfo.minlat;
            insertdata.Parameters["GPSMinLon"].Value = ds.wirelessnetwork[i].gpsinfo.minlon;
            insertdata.ExecuteNonQuery();
        }

//        private static void AddGPSOrphans()
//        {
//            string theconnectionstring = "Data Source=" + Application.StartupPath + "\\kismetdata.db;Version=3;New=False;Compress=False;Synchronous=Off";
//            thedata Kisdata = new thedata();
//            Kisdata.ConnectionString = theconnectionstring;
//            
//            //Console.WriteLine("Network add");
//            //Console.WriteLine(ds.wirelessnetwork[i].BSSID);
//            gpsdata thegpsdata = new gpsdata();
//            thegpsdata.ConnectionString = theconnectionstring;
//            thegpsdata.LoadAll();
//            do
//            {
//                if (!Kisdata.LoadByPrimaryKey(thegpsdata.Bssid))
//                {
//                    Kisdata.AddNew();
//                    Kisdata.Oui = LookupOuiData(thegpsdata.Bssid);
//                    Kisdata.BSSID = thegpsdata.Bssid;
//                    Kisdata.FirstTime = ConvertToDate(Convert.ToDouble(thegpsdata.Timesec));
//                    Kisdata.LastTime = Kisdata.FirstTime;
//                    Kisdata.ESSID = "no ssid from gps file";
//                    Kisdata.Save();
//                    //Console.WriteLine(thegpsdata.Bssid);
//
//                }
//            } while (thegpsdata.MoveNext());
//            /*
//            Kisdata.Oui = LookupOuiData(ds.wirelessnetwork[i].BSSID);
//            Kisdata.Cloaked = ds.wirelessnetwork[i].cloaked;
//            Kisdata.FirstTime = ds.wirelessnetwork[i].firsttime;
//            Kisdata.LastTime = ds.wirelessnetwork[i].lasttime;
//            Kisdata.BSSID = ds.wirelessnetwork[i].BSSID;
//            Kisdata.Network = ds.wirelessnetwork[i].number;
//            Kisdata.NetType = ds.wirelessnetwork[i].type;
//            Kisdata.Channel = ds.wirelessnetwork[i].channel;
//            Kisdata.Encryption = ds.wirelessnetwork[i].encryption;
//            Kisdata.Totalpacketscrypt = ds.wirelessnetwork[i].packets.crypt;
//            Kisdata.Totalpacketsdata = ds.wirelessnetwork[i].packets.data;
//            Kisdata.Totalpacketsdupeiv = ds.wirelessnetwork[i].packets.dupeiv;
//            Kisdata.Totalpacketsllc = ds.wirelessnetwork[i].packets.LLC;
//            Kisdata.Totalpacketstotal = ds.wirelessnetwork[i].packets.total;
//            Kisdata.Totalpacketsweak = ds.wirelessnetwork[i].packets.weak;
//
//            if (ds.wirelessnetwork[i].SSID != null)
//            {
//                //Console.WriteLine(ds.wirelessnetwork[i].SSID);
//                Kisdata.ESSID = ds.wirelessnetwork[i].SSID;
//            }
//            else
//            {
//                Kisdata.ESSID = "no ssid";
//            }
//            Kisdata.GPSMaxLat = ds.wirelessnetwork[i].gpsinfo.maxlat;
//            Kisdata.GPSMaxLon = ds.wirelessnetwork[i].gpsinfo.maxlon;
//            Kisdata.GPSMinLat = ds.wirelessnetwork[i].gpsinfo.minlat;
//            Kisdata.GPSMinLon = ds.wirelessnetwork[i].gpsinfo.minlon;
//            Kisdata.Save();
//             */
//        }
        private static void CleanGPSFiles(DirectoryInfo dir)
        {

            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = string.Format("{0}\\kismetdata.db", Application.StartupPath);
            builder.SyncMode = SynchronizationModes.Off;
            builder.CacheSize = 5000;
            //DataTable t;
            SQLiteConnection connection = new SQLiteConnection(builder.ToString());
            //SQLiteCommand command = connection.CreateCommand();
            connection.Open();
            //GPSDatanewTableAdapter gpstableadaptor = new GPSDatanewTableAdapter();
            FilesTableAdapter thegpsfilesadaptor = new FilesTableAdapter();
            StreamWriter sw = new StreamWriter(Application.StartupPath+"\\"+"allgpslogs.txt");
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>");
            sw.WriteLine("<!DOCTYPE gps-run SYSTEM \"http://kismetwireless.net/kismet-gps-2.9.1.dtd\">");
            sw.WriteLine("<gps-run gps-version=\"5\" start-time=\"Sun Apr  2 13:57:26 2006\">");

            foreach (FileInfo f in dir.GetFiles("Kismet*.gps"))
            {
                
                string input;
                //FileStream fs2 = new FileStream(f.DirectoryName + "\\" + f.Name, FileMode.Open);
                StreamReader fs2 = File.OpenText(f.DirectoryName + "\\" + f.Name);

                InsertFilename(thegpsfilesadaptor, f);
                //Console.WriteLine("Processing " + f.Name);
                // GP:SD:TR:AC:KL:OG
                Regex theregex = new Regex(@"GP:SD:TR:AC:KL:OG", RegexOptions.Compiled);
                Regex theregex2 = new Regex(@"bssid");
                while ((input = fs2.ReadLine()) != null)
                {

                    if (!theregex.IsMatch(input))
                    {
                        if (theregex2.IsMatch(input))
                        {
                            sw.WriteLine(input);
                        }
                    }
                }

                
            }
            sw.WriteLine("</gps-run>");
            sw.Close();
            connection.Close();
        }
        private static string ConvertToDate(double p)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            // Add the number of seconds in UNIX timestamp to be converted.
            dateTime = dateTime.AddSeconds(p);
            return dateTime.ToString();
            // The dateTime now contains the right date/time so to format the string,
            // use the standard formatting methods of the DateTime object.
            // string printDate = dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();

        }
        private static void ProcessHugeGPSFiles(XmlSerializer xs2)
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = string.Format("{0}\\kismetdata.db", Application.StartupPath);
            builder.SyncMode = SynchronizationModes.Off;
            builder.CacheSize = 5000;

            SQLiteConnection connection = new SQLiteConnection(builder.ToString());
            connection.Open();

            GPSDatanewTableAdapter thegpsdataadaptor = new GPSDatanewTableAdapter();
            Hashtable gpshashadd = new Hashtable();
            SQLiteCommand addcmd = connection.CreateCommand();
            string theaddcmd;
            theaddcmd = "INSERT INTO GPSData values (?, ?, ?, ?,?,?,?,?,?,?,?,?,?)";
            addcmd.CommandText = theaddcmd;
            SQLiteParameter bssid = addcmd.CreateParameter();
            SQLiteParameter source = addcmd.CreateParameter();
            SQLiteParameter timesec = addcmd.CreateParameter();
            SQLiteParameter timeusec = addcmd.CreateParameter();
            SQLiteParameter lat = addcmd.CreateParameter();
            SQLiteParameter lon = addcmd.CreateParameter();
            SQLiteParameter alt = addcmd.CreateParameter();
            SQLiteParameter spd = addcmd.CreateParameter();
            SQLiteParameter heading = addcmd.CreateParameter();
            SQLiteParameter fix = addcmd.CreateParameter();
            SQLiteParameter signal = addcmd.CreateParameter();
            SQLiteParameter quality = addcmd.CreateParameter();
            SQLiteParameter noise = addcmd.CreateParameter();


            addcmd.Parameters.Add(bssid);


            addcmd.Parameters.Add(source);

            addcmd.Parameters.Add(timesec);

            addcmd.Parameters.Add(timeusec);

            addcmd.Parameters.Add(lat);

            addcmd.Parameters.Add(lon);

            addcmd.Parameters.Add(alt);

            addcmd.Parameters.Add(spd);

            addcmd.Parameters.Add(heading);

            addcmd.Parameters.Add(fix);

            addcmd.Parameters.Add(signal);

            addcmd.Parameters.Add(quality);

            addcmd.Parameters.Add(noise);

                    FileStream fs2 = new FileStream(Application.StartupPath + "\\" + "allgpslogs.txt", FileMode.Open);
                    //Console.WriteLine("Processing " + f.Name);

                    try
                    {
                        gpsrun ds2;
                        int thecount;
                        ds2 = (gpsrun)xs2.Deserialize(fs2);

                        
                        thecount = ds2.Items.Length;
                        Console.WriteLine(thecount);
                        for (int i = 0; i < thecount; i++)
                        {
                            if (ds2.Items[i].bssid != "GP:SD:TR:AC:KL:OG")
                            {
                                    if (!gpshashadd.ContainsKey(ds2.Items[i].bssid))
                                    {
                                        if (ds2.Items[i].lat != 0 && ds2.Items[i].signal != 0)
                                        {
                                            gpshashadd.Add(ds2.Items[i].bssid, i);
                                        }
                                    }
                                    else
                                    {

                                        try
                                        {
                                            decimal thehashsignal;
                                            thehashsignal =
                                                ds2.Items[Convert.ToInt32(gpshashadd[ds2.Items[i].bssid])].signal;

                                            if ((thehashsignal < ds2.Items[i].signal) && ds2.Items[i].fix == "3" && ds2.Items[i].signal != 0)
                                            {
                                                if (ds2.Items[i].lat != 0)
                                                {
                                                    gpshashadd.Remove(ds2.Items[i].bssid);
                                                    gpshashadd.Add(ds2.Items[i].bssid, i);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(i);
                                            Console.WriteLine(ex);
                                        }


                                    }


                                    /*
                                        
                                    */





                                    /*
                                    thegpsdatatableadaptor.InsertQuery(ds2.Items[i].bssid, ds2.Items[i].source,
                                                                       ds2.Items[i].timesec, ds2.Items[i].timeusec,
                                                                       ds2.Items[i].lat, ds2.Items[i].lon,
                                                                       ds2.Items[i].alt, ds2.Items[i].spd,
                                                                       ds2.Items[i].heading, ds2.Items[i].fix,
                                                                       ds2.Items[i].signal, ds2.Items[i].quality,
                                                                       ds2.Items[i].noise);
                                         
                                         
                                        
                                        
                                    thegpsdata.AddNew();
                                    thegpsdata.Bssid = ds2.Items[i].bssid;
                                    thegpsdata.Timesec = ds2.Items[i].timesec;
                                    thegpsdata.Timeusec = ds2.Items[i].timeusec;
                                    thegpsdata.Source = ds2.Items[i].source;
                                    thegpsdata.Lat = ds2.Items[i].lat;
                                    thegpsdata.Lon = ds2.Items[i].lon;
                                    thegpsdata.Alt = ds2.Items[i].alt;
                                    thegpsdata.Speed = ds2.Items[i].spd;
                                    thegpsdata.Heading = ds2.Items[i].heading;
                                    thegpsdata.Fix = ds2.Items[i].fix;
                                    thegpsdata.Signal = ds2.Items[i].signal;
                                    thegpsdata.Quality = ds2.Items[i].quality;
                                    thegpsdata.Noise = ds2.Items[i].noise;
                                    thegpsdata.Save();
                                     */
                                //}
                                

                            }

                        }
                        Console.WriteLine("Saving data");

                        try
                        {
                            using (DbTransaction dbtrans = connection.BeginTransaction())
                            {
                                foreach (DictionaryEntry de in gpshashadd)
                                {
                                    int theindex = Convert.ToInt32(de.Value);
                                    bssid.Value = ds2.Items[theindex].bssid;
                                    timesec.Value = ds2.Items[theindex].timesec;
                                    timeusec.Value = ds2.Items[theindex].timeusec;
                                    source.Value = ds2.Items[theindex].source;
                                    lat.Value = ds2.Items[theindex].lat;
                                    lon.Value = ds2.Items[theindex].lon;
                                    alt.Value = ds2.Items[theindex].alt;
                                    spd.Value = ds2.Items[theindex].spd;
                                    heading.Value = ds2.Items[theindex].heading;
                                    fix.Value = ds2.Items[theindex].fix;
                                    signal.Value = ds2.Items[theindex].signal;
                                    quality.Value = ds2.Items[theindex].quality;
                                    noise.Value = ds2.Items[theindex].noise;
                                    addcmd.ExecuteNonQuery();
                                }

                                try
                                {
                                    dbtrans.Commit();
                                }
                                catch (Exception ex)
                                {
                                    
                                   Console.WriteLine(ex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);

                        //Console.WriteLine("Bad gps file " + f.Name);

                    }
                    finally
                    {
                        fs2.Close();
                        gpshashadd.Clear();
                        connection.Close();
                        
                        //cmd.Dispose();
                    }
        }
        private static void ProcessGPSFilesNew(DirectoryInfo dir, XmlSerializer xs2)
        {
            //thegpsdata = new gpsdata();
            //thegpsfiles = new gpsfiles();
            //thegpsfiles.ConnectionString = theconnectionstring;
            //thegpsdata.ConnectionString = theconnectionstring;
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = string.Format("{0}\\kismetdata.db", Application.StartupPath);
            builder.SyncMode = SynchronizationModes.Off;
            builder.CacheSize = 5000;
            //DataTable t;
            SQLiteConnection connection = new SQLiteConnection(builder.ToString());
            //SQLiteCommand command = connection.CreateCommand();
            connection.Open();
            //GPSDatanewTableAdapter gpstableadaptor = new GPSDatanewTableAdapter();
            FilesTableAdapter thefilesdaaptor = new FilesTableAdapter();
            //GPSDatanewTableAdapter thegpsdataadaptor = new GPSDatanewTableAdapter();
            Hashtable gpshashadd = new Hashtable();
            //Hashtable gpshashupdate = new Hashtable();
            SQLiteCommand addcmd = connection.CreateCommand();
            SQLiteCommand updatecmd = connection.CreateCommand();
                //DbCommand updatecmd = connection.CreateCommand();
                string theaddcmd;
                    theaddcmd = "INSERT INTO GPSData values (?, ?, ?, ?,?,?,?,?,?,?,?,?,?)";
                    //updatecmd.CommandText="Update [GPSData] set [source]= @thesource, [timesec]=@thetimesec, [timeusec]=@thetimeusec, [lat] = @thelat, [lon] = @thelon, [alt] = @thealt, [speed] = @thespeed, [heading] = @theheading, [fix] = @thefix, [signal] = @thesignal, [quality] = @thequality, [noise] = @thenoise where [bssid]=@thebssid";
                    updatecmd.CommandText =
                    @"UPDATE [GPSData] SET 
				[source]=?,
				[timesec]=?,
				[timeusec]=?,
				[lat]=?,
				[lon]=?,
				[alt]=?,
				[speed]=?,
				[heading]=?,
				[fix]=?,
				[signal]=?,
				[quality]=?,
				[noise]=?
			WHERE
				[bssid]=?";        
                    addcmd.CommandText = theaddcmd;
                    SQLiteParameter bssid = addcmd.CreateParameter();
                    SQLiteParameter source = addcmd.CreateParameter();
                    SQLiteParameter timesec = addcmd.CreateParameter();
                    SQLiteParameter timeusec = addcmd.CreateParameter();
                    SQLiteParameter lat = addcmd.CreateParameter();
                    SQLiteParameter lon = addcmd.CreateParameter();
                    SQLiteParameter alt = addcmd.CreateParameter();
                    SQLiteParameter spd = addcmd.CreateParameter();
                    SQLiteParameter heading = addcmd.CreateParameter();
                    SQLiteParameter fix = addcmd.CreateParameter();
                    SQLiteParameter signal = addcmd.CreateParameter();
                    SQLiteParameter quality = addcmd.CreateParameter();
                    SQLiteParameter noise = addcmd.CreateParameter();
                    addcmd.Parameters.Add(bssid);                  
                    addcmd.Parameters.Add(source);
                    addcmd.Parameters.Add(timesec);
                    addcmd.Parameters.Add(timeusec);
                    addcmd.Parameters.Add(lat);
                    addcmd.Parameters.Add(lon);
                    addcmd.Parameters.Add(alt);
                    addcmd.Parameters.Add(spd);
                    addcmd.Parameters.Add(heading);
                    addcmd.Parameters.Add(fix);
                    addcmd.Parameters.Add(signal);
                    addcmd.Parameters.Add(quality);
                    addcmd.Parameters.Add(noise);

            SQLiteParameter thebssid = updatecmd.CreateParameter();
            SQLiteParameter thesource = updatecmd.CreateParameter();
            SQLiteParameter thetimesec = updatecmd.CreateParameter();
            SQLiteParameter thetimeusec = updatecmd.CreateParameter();
            SQLiteParameter thelat = updatecmd.CreateParameter();
            SQLiteParameter thelon = updatecmd.CreateParameter();
            SQLiteParameter thealt = updatecmd.CreateParameter();
            SQLiteParameter thespd = updatecmd.CreateParameter();
            SQLiteParameter theheading = updatecmd.CreateParameter();
            SQLiteParameter thefix = updatecmd.CreateParameter();
            SQLiteParameter thesignal = updatecmd.CreateParameter();
            SQLiteParameter thequality = updatecmd.CreateParameter();
            SQLiteParameter thenoise = updatecmd.CreateParameter();
                    
                    updatecmd.Parameters.Add(thesource);
                    updatecmd.Parameters.Add(thetimesec);
                    updatecmd.Parameters.Add(thetimeusec);
                    updatecmd.Parameters.Add(thelat);
                    updatecmd.Parameters.Add(thelon);
                    updatecmd.Parameters.Add(thealt);
                    updatecmd.Parameters.Add(thespd);
                    updatecmd.Parameters.Add(theheading);
                    updatecmd.Parameters.Add(thefix);
                    updatecmd.Parameters.Add(thesignal);
                    updatecmd.Parameters.Add(thequality);
                    updatecmd.Parameters.Add(thenoise);
                    updatecmd.Parameters.Add(thebssid);
                    
            
                foreach (FileInfo f in dir.GetFiles("Kismet*.gps"))
                {
                    //Console.WriteLine(dir.ToString());
                    DataTable t;
                    t = thefilesdaaptor.GetDataByFilename(f.Name);
                    if (t.Rows.Count == 0)
                    {
                        FileStream fs2 = new FileStream(f.DirectoryName + "\\" + f.Name, FileMode.Open);

                        InsertFilename(thefilesdaaptor, f);
                        Console.WriteLine("Processing " + f.Name);
                        
                        try
                        {
                            gpsrun ds2;
                            int thecount;
                            ds2 = (gpsrun) xs2.Deserialize(fs2);
                            // thegpsdataadaptor.Update("FF:FF:FF:00:00:00","")
                            //thegpsfiles.AddNew();
                            //thegpsfiles.Filename = f.Name;
                            //thegpsfiles.Save();
                            // Processing Kismet-Apr-17-2006-3.gps
                            // Object reference not set to an instance of an object.
                            thecount = ds2.Items.Length;
                            Console.WriteLine(thecount);
                            GPSDatanewTableAdapter thegpsdatatableadaptor = new GPSDatanewTableAdapter();
                            for (int i = 0; i < thecount; i++)
                            {
                                if (ds2.Items[i].bssid != "GP:SD:TR:AC:KL:OG")
                                {
                                    
                                    t = thegpsdatatableadaptor.GetDataByGpsData(ds2.Items[i].bssid);
                                    if (t.Rows.Count == 0)
                                    {
                                        //Console.WriteLine(i);
                                        if (!gpshashadd.ContainsKey(ds2.Items[i].bssid))
                                        {
                                            if (ds2.Items[i].lat != 0 && ds2.Items[i].signal !=0)
                                            {
                                                gpshashadd.Add(ds2.Items[i].bssid, i);
                                            }
                                        }
                                        else
                                        {
                                            
                                            try
                                            {
                                                decimal thehashsignal;
                                                thehashsignal =
                                                    ds2.Items[Convert.ToInt32(gpshashadd[ds2.Items[i].bssid])].signal;

                                                if ((thehashsignal < ds2.Items[i].signal) && ds2.Items[i].fix=="3" && ds2.Items[i].signal !=0)
                                                {
                                                    //Console.WriteLine(thehashsignal + "," + ds2.Items[i].signal);
                                                    
                                                    if (ds2.Items[i].lat != 0)
                                                    {
                                                        //Console.WriteLine("Removing " + i);

                                                        gpshashadd.Remove(ds2.Items[i].bssid);
                                                        gpshashadd.Add(ds2.Items[i].bssid, i);
                                                    }
                                                }
                                            }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(i);
                                            Console.WriteLine(ex);
                                        }
                                                
                                                    
                                        }
                                        
                                        
                                        /*
                                        
                                        */




                                        
                                        /*
                                        thegpsdatatableadaptor.InsertQuery(ds2.Items[i].bssid, ds2.Items[i].source,
                                                                           ds2.Items[i].timesec, ds2.Items[i].timeusec,
                                                                           ds2.Items[i].lat, ds2.Items[i].lon,
                                                                           ds2.Items[i].alt, ds2.Items[i].spd,
                                                                           ds2.Items[i].heading, ds2.Items[i].fix,
                                                                           ds2.Items[i].signal, ds2.Items[i].quality,
                                                                           ds2.Items[i].noise);
                                         
                                         
                                        
                                        
                                        thegpsdata.AddNew();
                                        thegpsdata.Bssid = ds2.Items[i].bssid;
                                        thegpsdata.Timesec = ds2.Items[i].timesec;
                                        thegpsdata.Timeusec = ds2.Items[i].timeusec;
                                        thegpsdata.Source = ds2.Items[i].source;
                                        thegpsdata.Lat = ds2.Items[i].lat;
                                        thegpsdata.Lon = ds2.Items[i].lon;
                                        thegpsdata.Alt = ds2.Items[i].alt;
                                        thegpsdata.Speed = ds2.Items[i].spd;
                                        thegpsdata.Heading = ds2.Items[i].heading;
                                        thegpsdata.Fix = ds2.Items[i].fix;
                                        thegpsdata.Signal = ds2.Items[i].signal;
                                        thegpsdata.Quality = ds2.Items[i].quality;
                                        thegpsdata.Noise = ds2.Items[i].noise;
                                        thegpsdata.Save();
                                         */
                                    }                                        
                                    else
                                    {
                                        //Update
                                        
                                        DataTableReader thereader = t.CreateDataReader();
                                        thereader.Read();
                                        int databasesignal = Convert.ToInt16(thereader[10]);
                                        
                                        //string databasebssid = thereader[0].ToString();
                                        if (ds2.Items[i].signal > databasesignal)
                                        {
                                            try
                                            {
                                                //Console.WriteLine("updating");
                                                thebssid.Value = ds2.Items[i].bssid;
                                                thesource.Value = ds2.Items[i].source;
                                                thetimesec.Value = ds2.Items[i].timesec;
                                                thetimeusec.Value = ds2.Items[i].timeusec;                                                
                                                thelat.Value = ds2.Items[i].lat;
                                                thelon.Value = ds2.Items[i].lon;
                                                thealt.Value = ds2.Items[i].alt;
                                                thespd.Value = ds2.Items[i].spd;
                                                theheading.Value = ds2.Items[i].heading;
                                                thefix.Value = ds2.Items[i].fix;
                                                thesignal.Value = ds2.Items[i].signal;
                                                thequality.Value = ds2.Items[i].quality;
                                                thenoise.Value = ds2.Items[i].noise;
                                                updatecmd.ExecuteNonQuery();
                                            }
                                            catch(Exception ex)
                                            {
                                                Console.WriteLine(ex);
                                            }
                                            //Add value to hashtable
                                            //Console.WriteLine("Chicken");
                                        }

                                    }
                                    
                                }
                                
                            }
                            Console.WriteLine("Saving data");
                            using (DbTransaction dbtrans = connection.BeginTransaction())
                            {
                                foreach (DictionaryEntry de in gpshashadd)
                                {
                                    int theindex = Convert.ToInt32(de.Value);
                                    bssid.Value = ds2.Items[theindex].bssid;
                                    timesec.Value = ds2.Items[theindex].timesec;
                                    timeusec.Value = ds2.Items[theindex].timeusec;
                                    source.Value = ds2.Items[theindex].source;
                                    lat.Value = ds2.Items[theindex].lat;
                                    lon.Value = ds2.Items[theindex].lon;
                                    alt.Value = ds2.Items[theindex].alt;
                                    spd.Value = ds2.Items[theindex].spd;
                                    heading.Value = ds2.Items[theindex].heading;
                                    fix.Value = ds2.Items[theindex].fix;
                                    signal.Value = ds2.Items[theindex].signal;
                                    quality.Value = ds2.Items[theindex].quality;
                                    noise.Value = ds2.Items[theindex].noise;
                                    addcmd.ExecuteNonQuery();
                                }
                                dbtrans.Commit();
                            }   

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);

                            Console.WriteLine("Bad gps file " + f.Name);
                            
                        }
                        finally
                        {
                            fs2.Close();
                            gpshashadd.Clear(); 
                            //cmd.Dispose();
                            
                        }
                        fs2.Close();   
                    }
                    
               }
            connection.Close();
        }
        /*
        private static void ProcessGPSFiles(string theconnectionstring, DirectoryInfo dir, XmlSerializer xs2, gpsdata thegpsdata, gpsfiles thegpsfiles)
        {
            thegpsdata = new gpsdata();
            thegpsfiles = new gpsfiles();
            thegpsfiles.ConnectionString = theconnectionstring;
            thegpsdata.ConnectionString = theconnectionstring;

            foreach (FileInfo f in dir.GetFiles("Kismet*.gps"))
            {
                //Console.WriteLine(dir.ToString());
                FileStream fs2 = new FileStream(f.DirectoryName + "\\" + f.Name, FileMode.Open);
                Console.WriteLine("Processing " + f.Name);
                if (!thegpsfiles.LoadByPrimaryKey(f.Name))
                {
                    try
                    {
                        gpsrun ds2;
                        int thecount;
                        ds2 = (gpsrun) xs2.Deserialize(fs2);

                        thegpsfiles.AddNew();
                        thegpsfiles.Filename = f.Name;
                        thegpsfiles.Save();

                        thecount = ds2.Items.Length;
                        Console.WriteLine(thecount);
                        for (int i = 0; i < thecount; i++)
                        {
                            if (ds2.Items[i].bssid != "GP:SD:TR:AC:KL:OG")
                            {
                                //thegpsdata.Where.Bssid.Value=ds2.Items[i].bssid;
                                //thegpsdata.Where.Bssid.Operator=WhereParameter.Operand.Equal;
                                //Kisdata.Where.BSSID.Value="00:0F:3D:3E:41:9A";
                                //Kisdata.Where.BSSID.Operator=WhereParameter.Operand.Equal;
                                //Console.WriteLine(thegpsdata.Query.GenerateSQL());
                                //thegpsdata.Query.Load();

                                if (!thegpsdata.LoadByPrimaryKey(ds2.Items[i].bssid))
                                {
                                    //Console.WriteLine(i);

                                    thegpsdata.AddNew();
                                    thegpsdata.Bssid = ds2.Items[i].bssid;
                                    thegpsdata.Timesec = ds2.Items[i].timesec;
                                    thegpsdata.Timeusec = ds2.Items[i].timeusec;
                                    thegpsdata.Source = ds2.Items[i].source;
                                    thegpsdata.Lat = ds2.Items[i].lat;
                                    thegpsdata.Lon = ds2.Items[i].lon;
                                    thegpsdata.Alt = ds2.Items[i].alt;
                                    thegpsdata.Speed = ds2.Items[i].spd;
                                    thegpsdata.Heading = ds2.Items[i].heading;
                                    thegpsdata.Fix = ds2.Items[i].fix;
                                    thegpsdata.Signal = ds2.Items[i].signal;
                                    thegpsdata.Quality = ds2.Items[i].quality;
                                    thegpsdata.Noise = ds2.Items[i].noise;
                                    thegpsdata.Save();
                                }
                                else
                                {
                                    if ((thegpsdata.Signal <= ds2.Items[i].signal) && ds2.Items[i].fix=="3")
                                    {
                                        thegpsdata.Timesec = ds2.Items[i].timesec;
                                        thegpsdata.Timeusec = ds2.Items[i].timeusec;
                                        thegpsdata.Source = ds2.Items[i].source;
                                        thegpsdata.Lat = ds2.Items[i].lat;
                                        thegpsdata.Lon = ds2.Items[i].lon;
                                        thegpsdata.Alt = ds2.Items[i].alt;
                                        thegpsdata.Speed = ds2.Items[i].spd;
                                        thegpsdata.Heading = ds2.Items[i].heading;
                                        thegpsdata.Fix = ds2.Items[i].fix;
                                        thegpsdata.Signal = ds2.Items[i].signal;
                                        thegpsdata.Quality = ds2.Items[i].quality;
                                        thegpsdata.Noise = ds2.Items[i].noise;
                                        thegpsdata.Save();
                                    }
                                    
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine(ex.ToString());
                        Console.WriteLine("Bad gps file " + f.Name);
                    }
                }
            }
        }
        */

	    public static void AddDataTableSqlite(_Data Kisdata, detectionrun ds, int i)
		{
			Kisdata.AddNew();
			//Console.WriteLine("Network add");
			//Console.WriteLine(ds.wirelessnetwork[i].BSSID);

            Kisdata.Oui=LookupOuiData(ds.wirelessnetwork[i].BSSID);
			Kisdata.Cloaked=ds.wirelessnetwork[i].cloaked;
			Kisdata.FirstTime=ds.wirelessnetwork[i].firsttime;
			Kisdata.LastTime=ds.wirelessnetwork[i].lasttime;
			Kisdata.BSSID=ds.wirelessnetwork[i].BSSID;
			Kisdata.Network=ds.wirelessnetwork[i].number;
			Kisdata.NetType=ds.wirelessnetwork[i].type;
			Kisdata.Channel=ds.wirelessnetwork[i].channel;						
			Kisdata.Encryption=ds.wirelessnetwork[i].encryption;
			Kisdata.Totalpacketscrypt=ds.wirelessnetwork[i].packets.crypt;
			Kisdata.Totalpacketsdata=ds.wirelessnetwork[i].packets.data;
			Kisdata.Totalpacketsdupeiv=ds.wirelessnetwork[i].packets.dupeiv;
			Kisdata.Totalpacketsllc=ds.wirelessnetwork[i].packets.LLC;
			Kisdata.Totalpacketstotal=ds.wirelessnetwork[i].packets.total;
			Kisdata.Totalpacketsweak=ds.wirelessnetwork[i].packets.weak;

	        Kisdata.ESSID = ds.wirelessnetwork[i].SSID ?? "no ssid";
	        Kisdata.GPSMaxLat=ds.wirelessnetwork[i].gpsinfo.maxlat;
			Kisdata.GPSMaxLon=ds.wirelessnetwork[i].gpsinfo.maxlon;
			Kisdata.GPSMinLat=ds.wirelessnetwork[i].gpsinfo.minlat;
			Kisdata.GPSMinLon=ds.wirelessnetwork[i].gpsinfo.minlon;
			Kisdata.Save();
		}
		private static string LookupOuiData(string bssid)
		{

			if(!oui)
			{
				return "Unknown";
			}
			else
			{
				string temp;
				string temp2;
				string result;
				temp=bssid.Substring(0, 8);
				temp2=temp.Replace(':','-');
				result = myhash[temp2] as string;
				if (result != null)
				{
					return result;
				}
				else
				{
					return "Unknown";
				}

			}
		}
/*
		private static string CreateOuiData(string bssid)
		{
			string temp;
			string temp2;
			temp=bssid.Substring(0, 8);
			temp2=temp.Replace(':','-');
			ouidata theouitable= new ouidata();
			string  theconnectionstring = "Data Source="+ Application.StartupPath + "\\kismetdata.db;Version=3;New=False;Compress=False;Synchronous=Off";
			theouitable.ConnectionString=theconnectionstring;
			if(theouitable.LoadByPrimaryKey(temp2))
			{
				return theouitable.Company_name;
			}
			else
			{
				return "Unknown";
			}

		}
*/


		private static void AddorUpdateClientsSqlite(detectionrun ds, int i, _Clients theClientData)
		{		
				for(int j=0; j<ds.wirelessnetwork[i].wirelessclient.Length; j++)
				{

					if(!theClientData.LoadByPrimaryKey(ds.wirelessnetwork[i].BSSID, ds.wirelessnetwork[i].wirelessclient[j].clientmac))
					{
						theClientData.AddNew();
						//Console.WriteLine("Client Add");
						
						theClientData.Bssid=ds.wirelessnetwork[i].BSSID;
						theClientData.Oui=LookupOuiData(ds.wirelessnetwork[i].wirelessclient[j].clientmac);
						theClientData.Clientmac=ds.wirelessnetwork[i].wirelessclient[j].clientmac;
						theClientData.Clientdatasize=ds.wirelessnetwork[i].wirelessclient[j].clientdatasize;
						theClientData.Clientencryption=ds.wirelessnetwork[i].wirelessclient[j].clientencryption;
						theClientData.Clientfirsttime=ds.wirelessnetwork[i].wirelessclient[j].firsttime;
						theClientData.Clientlasttime=ds.wirelessnetwork[i].wirelessclient[j].lasttime;
						if(ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo != null)
						{
							theClientData.Clientgpsmaxalt=ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxalt;
							theClientData.Clientgpsmaxlat=ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxlat;
							theClientData.Clientgpsmaxlon=ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxlon;
							theClientData.Clientgpsminalt=ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminalt;
							theClientData.Clientgpsminlat=ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminlat;
							theClientData.Clientgpsminlon=ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminlon;
						}
						theClientData.Clientmaxrate=ds.wirelessnetwork[i].wirelessclient[j].clientmaxrate;
						if(ds.wirelessnetwork[i].wirelessclient[j].clientipaddress != null)
						{
							theClientData.Clientipaddress=ds.wirelessnetwork[i].wirelessclient[j].clientipaddress;
						}
						theClientData.Clientmaxseenrate=ds.wirelessnetwork[i].wirelessclient[j].clientmaxseenrate;
						theClientData.Clientpacketscrypt=ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientcrypt;
						theClientData.Clientpacketsdata=ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientdata;
						theClientData.Clientpacketsweak=ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientweak;
						theClientData.Save();
					}
					else
					{
						//Console.WriteLine("Client Update");
						theClientData.Clientdatasize+=ds.wirelessnetwork[i].wirelessclient[j].clientdatasize;
						theClientData.Clientencryption=ds.wirelessnetwork[i].wirelessclient[j].clientencryption;
                        
						theClientData.Clientfirsttime=ds.wirelessnetwork[i].wirelessclient[j].firsttime;
						theClientData.Clientlasttime=ds.wirelessnetwork[i].wirelessclient[j].lasttime;
						if(ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo != null)
						{
							theClientData.Clientgpsmaxalt=ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxalt;
							theClientData.Clientgpsmaxlat=ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxlat;
							theClientData.Clientgpsmaxlon=ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxlon;
							theClientData.Clientgpsminalt=ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminalt;
							theClientData.Clientgpsminlat=ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminlat;
							theClientData.Clientgpsminlon=ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminlon;
						}
						theClientData.Clientmaxrate=ds.wirelessnetwork[i].wirelessclient[j].clientmaxrate;
						if(ds.wirelessnetwork[i].wirelessclient[j].clientipaddress != null)
						{
							theClientData.Clientipaddress=ds.wirelessnetwork[i].wirelessclient[j].clientipaddress;
						}
						theClientData.Clientmaxseenrate=ds.wirelessnetwork[i].wirelessclient[j].clientmaxseenrate;
						theClientData.Clientpacketscrypt+=ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientcrypt;
						theClientData.Clientpacketsdata+=ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientdata;
						theClientData.Clientpacketsweak+=ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientweak;
						theClientData.Save();
					}
								
				}
			
		}
        private static void AddorUpdateClientsSqliteNew(ref detectionrun ds, int i, ref int clienttotal, ref int clientnew, ref int clientseen)
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = string.Format("{0}\\kismetdata.db", Application.StartupPath);
            builder.SyncMode = SynchronizationModes.Off;
            builder.CacheSize = 5000;

            
            SQLiteConnection connection = new SQLiteConnection(builder.ToString());
            //SQLiteCommand command = connection.CreateCommand();
            connection.Open();
            SQLiteCommand insertclients = connection.CreateCommand();
            SQLiteCommand updateclients = connection.CreateCommand();
            insertclients.CommandText = "Insert into clients values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            updateclients.CommandText=@"UPDATE [Clients] SET 
				[Clientipaddress]=?,
				[oui]=?,
				[clientencryption]=?,
				[clientfirsttime]=?,
				[clientgpsminlat]=?,
				[clientgpsminlon]=?,
				[clientgpsmaxlat]=?,
				[clientgpsmaxlon]=?,
				[clientgpsminalt]=?,
				[clientgpsmaxalt]=?,
				[clientgpsminspeed]=?,
				[clientgpsmaxspeed]=?,
				[clientdatasize]=?,
				[clientmaxrate]=?,
				[clientmaxseenrate]=?,
				[clientlasttime]=?,
				[clientpacketscrypt]=?,
				[clientpacketsdata]=?,
				[clientpacketsweak]=?,
				[Number]=?,
				[Type]=?,
				[wep]=?
			WHERE
				[bssid]=? AND 
				[clientmac]=?";
            SQLiteParameter Clientipaddress = insertclients.CreateParameter();
            SQLiteParameter updateClientipaddress = updateclients.CreateParameter();
            SQLiteParameter insertoui = insertclients.CreateParameter();
            SQLiteParameter updateoui = updateclients.CreateParameter();
            SQLiteParameter bssid = insertclients.CreateParameter();
            
            SQLiteParameter clientmac = insertclients.CreateParameter();
            
            SQLiteParameter clientencryption = insertclients.CreateParameter();
            SQLiteParameter updateclientencryption = updateclients.CreateParameter();
            SQLiteParameter clientfirsttime = insertclients.CreateParameter();
            SQLiteParameter updateclientfirsttime = updateclients.CreateParameter();
            SQLiteParameter clientgpsminlat=insertclients.CreateParameter();
			SQLiteParameter clientgpsminlon=insertclients.CreateParameter();
			SQLiteParameter clientgpsmaxlat=insertclients.CreateParameter();
			SQLiteParameter clientgpsmaxlon=insertclients.CreateParameter();
            SQLiteParameter clientgpsminalt=insertclients.CreateParameter();
			SQLiteParameter clientgpsmaxalt=insertclients.CreateParameter();
			SQLiteParameter clientgpsminspeed=insertclients.CreateParameter();
			SQLiteParameter clientgpsmaxspeed=insertclients.CreateParameter();
			SQLiteParameter clientdatasize=insertclients.CreateParameter();
			SQLiteParameter clientmaxrate=insertclients.CreateParameter();
			SQLiteParameter clientmaxseenrate=insertclients.CreateParameter();
			SQLiteParameter clientlasttime=insertclients.CreateParameter();
			SQLiteParameter clientpacketscrypt=insertclients.CreateParameter();
			SQLiteParameter clientpacketsdata=insertclients.CreateParameter();
			SQLiteParameter clientpacketsweak=insertclients.CreateParameter();
			SQLiteParameter Number=insertclients.CreateParameter();
			SQLiteParameter Type=insertclients.CreateParameter();
            SQLiteParameter wep = insertclients.CreateParameter();


            SQLiteParameter updateclientgpsminlat = updateclients.CreateParameter();
            SQLiteParameter updateclientgpsminlon = updateclients.CreateParameter();
            SQLiteParameter updateclientgpsmaxlat = updateclients.CreateParameter();
            SQLiteParameter updateclientgpsmaxlon = updateclients.CreateParameter();
            SQLiteParameter updateclientgpsminalt = updateclients.CreateParameter();
            SQLiteParameter updateclientgpsmaxalt = updateclients.CreateParameter();
            SQLiteParameter updateclientgpsminspeed = updateclients.CreateParameter();
            SQLiteParameter updateclientgpsmaxspeed = updateclients.CreateParameter();
            SQLiteParameter updateclientdatasize = updateclients.CreateParameter();
            SQLiteParameter updateclientmaxrate = updateclients.CreateParameter();
            SQLiteParameter updateclientmaxseenrate = updateclients.CreateParameter();
            SQLiteParameter updateclientlasttime = updateclients.CreateParameter();
            SQLiteParameter updateclientpacketscrypt = updateclients.CreateParameter();
            SQLiteParameter updateclientpacketsdata = updateclients.CreateParameter();
            SQLiteParameter updateclientpacketsweak = updateclients.CreateParameter();
            SQLiteParameter updateNumber = updateclients.CreateParameter();
            SQLiteParameter updateType = updateclients.CreateParameter();
            SQLiteParameter updatewep = updateclients.CreateParameter();
            SQLiteParameter updatebssid = insertclients.CreateParameter();
            SQLiteParameter updateclientmac = updateclients.CreateParameter();
            



            insertclients.Parameters.Add(Clientipaddress);
				insertclients.Parameters.Add(insertoui);
				insertclients.Parameters.Add(bssid);
				insertclients.Parameters.Add(clientmac);
            insertclients.Parameters.Add(clientencryption);
            insertclients.Parameters.Add(clientfirsttime);
				insertclients.Parameters.Add(clientgpsminlat);
				insertclients.Parameters.Add(clientgpsminlon);
				insertclients.Parameters.Add(clientgpsmaxlat);
				insertclients.Parameters.Add(clientgpsmaxlon);
				insertclients.Parameters.Add(clientgpsminalt);
				insertclients.Parameters.Add(clientgpsmaxalt);
				insertclients.Parameters.Add(clientgpsminspeed);
				insertclients.Parameters.Add(clientgpsmaxspeed);
				insertclients.Parameters.Add(clientdatasize);
				insertclients.Parameters.Add(clientmaxrate);
				insertclients.Parameters.Add(clientmaxseenrate);
				insertclients.Parameters.Add(clientlasttime);
				insertclients.Parameters.Add(clientpacketscrypt);
				insertclients.Parameters.Add(clientpacketsdata);
				insertclients.Parameters.Add(clientpacketsweak);
				insertclients.Parameters.Add(Number);
				insertclients.Parameters.Add(Type);
            insertclients.Parameters.Add(wep);


            updateclients.Parameters.Add(updateClientipaddress);
            updateclients.Parameters.Add(updateoui);
            
            updateclients.Parameters.Add(updateclientencryption);
            updateclients.Parameters.Add(updateclientfirsttime);
            updateclients.Parameters.Add(updateclientgpsminlat);
            updateclients.Parameters.Add(updateclientgpsminlon);
            updateclients.Parameters.Add(updateclientgpsmaxlat);
            updateclients.Parameters.Add(updateclientgpsmaxlon);
            updateclients.Parameters.Add(updateclientgpsminalt);
            updateclients.Parameters.Add(updateclientgpsmaxalt);
            updateclients.Parameters.Add(updateclientgpsminspeed);
            updateclients.Parameters.Add(updateclientgpsmaxspeed);
            updateclients.Parameters.Add(updateclientdatasize);
            updateclients.Parameters.Add(updateclientmaxrate);
            updateclients.Parameters.Add(updateclientmaxseenrate);
            updateclients.Parameters.Add(updateclientlasttime);
            updateclients.Parameters.Add(updateclientpacketscrypt);
            updateclients.Parameters.Add(updateclientpacketsdata);
            updateclients.Parameters.Add(updateclientpacketsweak);
            updateclients.Parameters.Add(updateNumber);
            updateclients.Parameters.Add(updateType);
            updateclients.Parameters.Add(updatewep);
            updateclients.Parameters.Add(updatebssid);
            updateclients.Parameters.Add(updateclientmac);
            ClientsTableAdapter theclientstableadaptor = new ClientsTableAdapter();
            DataTable t;
            clienttotal += ds.wirelessnetwork[i].wirelessclient.Length;
            for (int j = 0; j < ds.wirelessnetwork[i].wirelessclient.Length; j++)
            {
                t=theclientstableadaptor.GetDataByClientBssid(ds.wirelessnetwork[i].wirelessclient[j].clientmac, ds.wirelessnetwork[i].BSSID);
                if(t.Rows.Count == 0)
                {
                    //theClientData.AddNew();
                    //Console.WriteLine("Client Add");
                    clientnew++;
                    bssid.Value = ds.wirelessnetwork[i].BSSID;
                    insertoui.Value = LookupOuiData(ds.wirelessnetwork[i].wirelessclient[j].clientmac);
                    clientmac.Value = ds.wirelessnetwork[i].wirelessclient[j].clientmac;
                    clientdatasize.Value = ds.wirelessnetwork[i].wirelessclient[j].clientdatasize;
                    clientencryption.Value = ds.wirelessnetwork[i].wirelessclient[j].clientencryption;
                    clientfirsttime.Value = ds.wirelessnetwork[i].wirelessclient[j].firsttime;
                    clientlasttime.Value = ds.wirelessnetwork[i].wirelessclient[j].lasttime;
                    if (ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo != null)
                    {
                        clientgpsmaxalt.Value = ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxalt;
                        clientgpsmaxlat.Value = ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxlat;
                        clientgpsmaxlon.Value = ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxlon;
                        clientgpsminalt.Value = ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminalt;
                        clientgpsminlat.Value = ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminlat;
                        clientgpsminlon.Value = ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminlon;
                    }
                    clientmaxrate.Value = ds.wirelessnetwork[i].wirelessclient[j].clientmaxrate;
                    if (ds.wirelessnetwork[i].wirelessclient[j].clientipaddress != null)
                    {
                        Clientipaddress.Value = ds.wirelessnetwork[i].wirelessclient[j].clientipaddress;
                    }
                    clientmaxseenrate.Value = ds.wirelessnetwork[i].wirelessclient[j].clientmaxseenrate;
                    clientpacketscrypt.Value = ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientcrypt;
                    clientpacketsdata.Value = ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientdata;
                    clientpacketsweak.Value = ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientweak;

                    insertclients.ExecuteNonQuery();
                    
                    //theClientData.Save();
                }
                else
                {
                    clientseen++;
                    DataTableReader thereader = t.CreateDataReader();
                    thereader.Read();
                    //Console.WriteLine("Client Update");
                    updateoui.Value = LookupOuiData(ds.wirelessnetwork[i].wirelessclient[j].clientmac);
                    updateclientdatasize.Value = Convert.ToInt32(thereader["clientdatasize"]) + ds.wirelessnetwork[i].wirelessclient[j].clientdatasize;
                    updateclientencryption.Value = ds.wirelessnetwork[i].wirelessclient[j].clientencryption;
                    IFormatProvider format = new CultureInfo("en-US");
                    string[] expectedformats = { "ddd MMM  d HH:mm:ss yyyy", "ddd MMM d HH:mm:ss yyyy" };
                    string firsttime = thereader["clientfirsttime"].ToString();
                    string lasttime = thereader["clientlasttime"].ToString();
                    DateTime dbfirsttime;
                    DateTime xmlfirsttime;
                    DateTime dblasttime;
                    DateTime xmllasttime;
                    // Wed Mar 29 14:52:56 2006
                        dbfirsttime = DateTime.ParseExact(firsttime, expectedformats, format, DateTimeStyles.AllowWhiteSpaces);
                        xmlfirsttime = DateTime.ParseExact(ds.wirelessnetwork[i].wirelessclient[j].firsttime, expectedformats, format, DateTimeStyles.AllowWhiteSpaces);
                        dblasttime = DateTime.ParseExact(lasttime, expectedformats, format, DateTimeStyles.AllowWhiteSpaces);
                        xmllasttime = DateTime.ParseExact(ds.wirelessnetwork[i].wirelessclient[j].lasttime, expectedformats, format, DateTimeStyles.AllowWhiteSpaces);
                    
                    if (xmlfirsttime < dbfirsttime)
                    {
                        updateclientfirsttime.Value = ds.wirelessnetwork[i].firsttime;
                    }
                    else
                    {
                        updateclientfirsttime.Value = thereader["clientfirsttime"];
                    }
                    if (xmllasttime > dblasttime)
                    {
                        updateclientlasttime.Value = ds.wirelessnetwork[i].lasttime;
                    }
                    else
                    {
                        updateclientlasttime.Value = thereader["clientlasttime"];
                    }
                                
                    //theClientData.Clientfirsttime = ds.wirelessnetwork[i].wirelessclient[j].firsttime;
                    //theClientData.Clientlasttime = ds.wirelessnetwork[i].wirelessclient[j].lasttime;
                    if (ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo != null)
                    {
                        updateclientgpsmaxalt.Value = ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxalt;
                        updateclientgpsmaxlat.Value = ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxlat;
                        updateclientgpsmaxlon.Value = ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientmaxlon;
                        updateclientgpsminalt.Value = ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminalt;
                        updateclientgpsminlat.Value = ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminlat;
                        updateclientgpsminlon.Value = ds.wirelessnetwork[i].wirelessclient[j].clientgpsinfo.clientminlon;
                    }
                    updateclientmaxrate.Value = ds.wirelessnetwork[i].wirelessclient[j].clientmaxrate;
                    if (ds.wirelessnetwork[i].wirelessclient[j].clientipaddress != null)
                    {
                        updateClientipaddress.Value = ds.wirelessnetwork[i].wirelessclient[j].clientipaddress;
                    }
                    updateclientmaxseenrate.Value = ds.wirelessnetwork[i].wirelessclient[j].clientmaxseenrate;
                    updateclientpacketscrypt.Value = Convert.ToInt32(thereader["clientpacketscrypt"])+ ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientcrypt;
                    updateclientpacketsdata.Value = Convert.ToInt32(thereader["clientpacketsdata"])+ ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientdata;
                    updateclientpacketsweak.Value = Convert.ToInt32(thereader["clientpacketsweak"])+ds.wirelessnetwork[i].wirelessclient[j].clientpackets.clientweak;
                    //theClientData.Save();
                    updateclients.ExecuteNonQuery();
                    
                    thereader.Close();
                }

            }
        

        }
	}
	class thedata : _Data
	{
		
	}
	class clientdata: _Clients
	{
		
	}
	class gpsdata:_GPSData
	{
		
	}
	
	class ouidata:_OUIData
	{
		
	}
    class ssidchanges : _ssidchanges
    {
    }
    class encchanges : _encchanges
    {
    }

	public class Network
	{
	    readonly string _bssid;
	    readonly string _essid;
		public Network(string bssid, string essid)
		{
			_bssid=bssid;
			_essid=essid;

		}
		public string Bssid
		{
			get
			{
				return _bssid;
			}
		}
		public string Essid
		{
			get
			{
				return _essid;
			}
		}
	}
    public class CompareFileTime : IComparer
    {
        #region IComparer<FileInfo> Members

        #endregion

        public int Compare(object x, object y)
        {
            FileInfo x1 = (FileInfo) x;
            FileInfo y1 = (FileInfo) y;
            return x1.LastAccessTime.CompareTo(y1.LastAccessTime);
        }
    }
}
