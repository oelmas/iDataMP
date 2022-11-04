using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using GMap.NET.MapProviders;
using MissionPlanner.GCSViews;
using MissionPlanner.Utilities;
using ZedGraph;

// GE xml alt reader

namespace MissionPlanner.Controls
{
    public partial class ElevationProfile : Form
    {
        private readonly FlightPlanner.altmode altmode = FlightPlanner.altmode.Relative;
        private readonly int distance;
        private List<PointLatLngAlt> gelocs = new List<PointLatLngAlt>();
        private double homealt;
        private PointPairList list1 = new PointPairList();
        private PointPairList list2 = new PointPairList();
        private readonly PointPairList list3 = new PointPairList();

        private readonly PointPairList list4terrain = new PointPairList();
        private readonly List<PointLatLngAlt> planlocs = new List<PointLatLngAlt>();
        private List<PointLatLngAlt> srtmlocs = new List<PointLatLngAlt>();

        public ElevationProfile(List<PointLatLngAlt> locs, double homealt, FlightPlanner.altmode altmode)
        {
            InitializeComponent();

            this.altmode = altmode;

            planlocs = locs;

            for (var a = 0; a < planlocs.Count; a++)
                if (planlocs[a] == null || (planlocs[a].Tag != null && planlocs[a].Tag.Contains("ROI")))
                {
                    planlocs.RemoveAt(a);
                    a--;
                }

            if (planlocs.Count <= 1)
            {
                CustomMessageBox.Show("Please plan something first", Strings.ERROR);
                return;
            }

            // get total distance
            distance = 0;
            PointLatLngAlt lastloc = null;
            foreach (var loc in planlocs)
            {
                if (loc == null)
                    continue;

                if (lastloc != null) distance += (int)loc.GetDistance(lastloc);
                lastloc = loc;
            }

            this.homealt = homealt;

            var frm = Common.LoadingBox("Loading", "using alt data");

            //gelocs = getGEAltPath(planlocs);

            srtmlocs = getSRTMAltPath(planlocs);

            frm.Close();

            Tracking.AddPage(GetType().ToString(), Text);
        }

        private void ElevationProfile_Load(object sender, EventArgs e)
        {
            if (planlocs.Count <= 1)
            {
                Close();
                return;
            }

            // GE plot
            /*
            double a = 0;
            double increment = (distance / (float)(gelocs.Count - 1));

            foreach (PointLatLngAlt geloc in gelocs)
            {
                if (geloc == null)
                    continue;

                list2.Add(a * CurrentState.multiplierdist, Convert.ToInt32(geloc.Alt * CurrentState.multiplieralt));

                Console.WriteLine("GE " + geloc.Lng + "," + geloc.Lat + "," + geloc.Alt);

                a += increment;
            }
            */
            // Planner Plot
            double a = 0;
            var count = 0;
            PointLatLngAlt lastloc = null;
            foreach (var planloc in planlocs)
            {
                if (planloc == null)
                    continue;

                if (lastloc != null) a += planloc.GetDistance(lastloc);

                // deal with at mode
                if (altmode == FlightPlanner.altmode.Terrain)
                {
                    list1 = list4terrain;
                    break;
                }

                if (altmode == FlightPlanner.altmode.Relative)
                {
                    // already includes the home alt
                    list1.Add(a * CurrentState.multiplierdist, planloc.Alt * CurrentState.multiplieralt, 0,
                        planloc.Tag);
                }
                else
                {
                    // abs
                    // already absolute
                    list1.Add(a * CurrentState.multiplierdist, planloc.Alt * CurrentState.multiplieralt, 0,
                        planloc.Tag);
                }

                lastloc = planloc;
                count++;
            }

            // draw graph
            CreateChart(zg1);
        }

        private List<PointLatLngAlt> getSRTMAltPath(List<PointLatLngAlt> list)
        {
            var answer = new List<PointLatLngAlt>();

            PointLatLngAlt last = null;

            double disttotal = 0;

            foreach (var loc in list)
            {
                if (loc == null)
                    continue;

                if (last == null)
                {
                    last = loc;
                    if (altmode == FlightPlanner.altmode.Terrain)
                        loc.Alt -= srtm.getAltitude(loc.Lat, loc.Lng).alt;
                    continue;
                }

                var dist = last.GetDistance(loc);

                if (altmode == FlightPlanner.altmode.Terrain)
                    loc.Alt -= srtm.getAltitude(loc.Lat, loc.Lng).alt;

                var points = (int)(dist / 10) + 1;

                var deltalat = last.Lat - loc.Lat;
                var deltalng = last.Lng - loc.Lng;
                var deltaalt = last.Alt - loc.Alt;

                var steplat = deltalat / points;
                var steplng = deltalng / points;
                var stepalt = deltaalt / points;

                var lastpnt = last;

                for (var a = 0; a <= points; a++)
                {
                    var lat = last.Lat - steplat * a;
                    var lng = last.Lng - steplng * a;
                    var alt = last.Alt - stepalt * a;

                    var newpoint = new PointLatLngAlt(lat, lng, srtm.getAltitude(lat, lng).alt, "");

                    var subdist = lastpnt.GetDistance(newpoint);

                    disttotal += subdist;

                    // srtm alts
                    list3.Add(disttotal * CurrentState.multiplierdist,
                        Convert.ToInt32(newpoint.Alt * CurrentState.multiplieralt));

                    // terrain alt
                    list4terrain.Add(disttotal * CurrentState.multiplierdist,
                        Convert.ToInt32((newpoint.Alt + alt) * CurrentState.multiplieralt));

                    lastpnt = newpoint;
                }

                answer.Add(new PointLatLngAlt(loc.Lat, loc.Lng, srtm.getAltitude(loc.Lat, loc.Lng).alt, ""));

                last = loc;
            }

            return answer;
        }

        private List<PointLatLngAlt> getGEAltPath(List<PointLatLngAlt> list)
        {
            double alt = 0;
            double lat = 0;
            double lng = 0;

            var pos = 0;

            var answer = new List<PointLatLngAlt>();

            //http://code.google.com/apis/maps/documentation/elevation/
            //http://maps.google.com/maps/api/elevation/xml
            var coords = "";

            foreach (var loc in list)
            {
                if (loc == null)
                    continue;

                coords = coords + loc.Lat.ToString(new CultureInfo("en-US")) + "," +
                         loc.Lng.ToString(new CultureInfo("en-US")) + "|";
            }

            coords = coords.Remove(coords.Length - 1);

            if (list.Count < 2 || coords.Length > 2048 - 256)
            {
                CustomMessageBox.Show("Too many/few WP's or to Big a Distance " + distance / 1000 + "km",
                    Strings.ERROR);
                return answer;
            }

            try
            {
                using (
                    var xmlreader =
                    new XmlTextReader("https://maps.google.com/maps/api/elevation/xml?path=" + coords + "&samples=" +
                                      (distance / 100).ToString(new CultureInfo("en-US")) +
                                      "&sensor=false&key=" + GoogleMapProviderBase.APIKey))
                {
                    while (xmlreader.Read())
                    {
                        xmlreader.MoveToElement();
                        switch (xmlreader.Name)
                        {
                            case "elevation":
                                alt = double.Parse(xmlreader.ReadString(), new CultureInfo("en-US"));
                                Console.WriteLine("DO it " + lat + " " + lng + " " + alt);
                                var loc = new PointLatLngAlt(lat, lng, alt, "");
                                answer.Add(loc);
                                pos++;
                                break;
                            case "lat":
                                lat = double.Parse(xmlreader.ReadString(), new CultureInfo("en-US"));
                                break;
                            case "lng":
                                lng = double.Parse(xmlreader.ReadString(), new CultureInfo("en-US"));
                                break;
                        }
                    }
                }
            }
            catch
            {
                CustomMessageBox.Show("Error getting GE data", Strings.ERROR);
            }

            return answer;
        }

        public void CreateChart(ZedGraphControl zgc)
        {
            var myPane = zgc.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Elevation above ground";
            myPane.XAxis.Title.Text = "Distance (" + CurrentState.DistanceUnit + ")";
            myPane.YAxis.Title.Text = "Elevation (" + CurrentState.AltUnit + ")";

            LineItem myCurve;

            myCurve = myPane.AddCurve("Planned Path", list1, Color.Red, SymbolType.None);
            //myCurve = myPane.AddCurve("Google", list2, Color.Green, SymbolType.None);
            myCurve = myPane.AddCurve("DEM", list3, Color.Blue, SymbolType.None);

            foreach (var pp in list1)
            {
                // Add a another text item to to point out a graph feature
                var text = new TextObj((string)pp.Tag, pp.X, pp.Y);
                // rotate the text 90 degrees
                text.FontSpec.Angle = 90;
                text.FontSpec.FontColor = Color.White;
                // Align the text such that the Right-Center is at (700, 50) in user scale coordinates
                text.Location.AlignH = AlignH.Right;
                text.Location.AlignV = AlignV.Center;
                // Disable the border and background fill options for the text
                text.FontSpec.Fill.IsVisible = false;
                text.FontSpec.Border.IsVisible = false;
                myPane.GraphObjList.Add(text);
            }

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;

            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = distance * CurrentState.multiplierdist;

            // Make the Y axis scale red
            myPane.YAxis.Scale.FontSpec.FontColor = Color.Red;
            myPane.YAxis.Title.FontSpec.FontColor = Color.Red;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            // Don't display the Y zero line
            myPane.YAxis.MajorGrid.IsZeroLine = true;
            // Align the Y axis labels so they are flush to the axis
            myPane.YAxis.Scale.Align = AlignP.Inside;
            // Manually set the axis range
            //myPane.YAxis.Scale.Min = -1;
            //myPane.YAxis.Scale.Max = 1;

            // Fill the axis background with a gradient
            //myPane.Chart.Fill = new Fill(Color.White, Color.LightGray, 45.0f);

            // Calculate the Axis Scale Ranges
            try
            {
                zg1.AxisChange();
            }
            catch
            {
            }
        }
    }
}