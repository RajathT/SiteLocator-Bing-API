using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using Microsoft.Maps.MapControl.WPF.Design;

namespace GeoNBCSiteLocator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // The user defined polygon to add to the map.
        MapPolygon newPolygon = null;
        // The map layer containing the polygon points defined by the user.
        MapLayer polygonPointLayer = new MapLayer();

        public MainWindow()
        {
            InitializeComponent();
            //Set focus to map
            MapWithPolygon.Focus();
            SetUpNewPolygon();
            // Adds location points to the polygon for every single mouse click
            MapWithPolygon.MouseDoubleClick += new MouseButtonEventHandler(MapWithPolygon_MouseDoubleClick);

            // Adds the layer that contains the polygon points
            NewPolygonLayer.Children.Add(polygonPointLayer);
            MapWithPolygon.ViewChangeOnFrame += new EventHandler<MapEventArgs>(MyMap_ViewChangeOnFrame);

        }
        void MyMap_ViewChangeOnFrame(object sender, MapEventArgs e)
        {
            Cord.Text = String.Empty;
            //Gets the map that raised this event
            Map map = (Map)sender;
            //Gets the bounded rectangle for the current frame
            LocationRect bounds = map.BoundingRectangle;
            //Update the current latitude and longitude
            Cord.Text += String.Format("Northwest: {0:F5}, Southeast: {1:F5} (Current)",bounds.Northwest, bounds.Southeast);
        }
        private void SetUpNewPolygon()
        {
            newPolygon = new MapPolygon();
            // Defines the polygon fill details
            newPolygon.Locations = new LocationCollection();
            newPolygon.Stroke = new SolidColorBrush(Colors.Green);
            newPolygon.StrokeThickness = 3;
            newPolygon.Opacity = 0.8;
            //Set focus back to the map so that +/- work for zoom in/out
            MapWithPolygon.Focus();
        }

        private void MapWithPolygon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            e.Handled = true;
            // Creates a location for a single polygon point and adds it to
            // the polygon's point location list.
            Point mousePosition = e.GetPosition(this);
            //Convert the mouse coordinates to a location on the map
            Location polygonPointLocation = MapWithPolygon.ViewportPointToLocation(mousePosition);
            
            newPolygon.Locations.Add(polygonPointLocation);

            // A visual representation of a polygon point.
            Rectangle r = new Rectangle();
            r.Fill = new SolidColorBrush(Colors.Red);
            r.Stroke = new SolidColorBrush(Colors.Yellow);
            r.StrokeThickness = 1;
            r.Width = 8;
            r.Height = 8;
            Pushpin pin = new Pushpin();
            pin.Location = polygonPointLocation;

            // Adds the pushpin to the map.
            polygonPointLayer.Children.Add(pin);
            // Adds a small square where the user clicked, to mark the polygon point.
            polygonPointLayer.AddChild(r, polygonPointLocation);
            //Set focus back to the map so that +/- work for zoom in/out
            MapWithPolygon.Focus();

        }

        private void btnCreatePolygon_Click(object sender, RoutedEventArgs e)
        {
            //If there are two or more points, add the polygon layer to the map
            if (newPolygon.Locations.Count >= 2)
            {
                // Removes the polygon points layer.
                polygonPointLayer.Children.Clear();

                // Adds the filled polygon layer to the map.
                NewPolygonLayer.Children.Add(newPolygon);
                SetUpNewPolygon();
            }
        }
    }
} 
