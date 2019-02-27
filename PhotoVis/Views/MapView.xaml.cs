using ClusterEngine;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Data;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using PhotoVis.Util;
using PhotoVis.Interfaces;
using PhotoVis.ViewModel;
using PhotoVis.Data;
using PhotoVis.Data.DatabaseTables;

namespace PhotoVis.Views
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {

        #region Fields
        private ClusterOptions _options;
        private bool _generatingData;
        private BackgroundWorker _worker;

        private Point dragStartPoint;
        private Point offsetInGoogleStreetViewMarker;

        private bool isShiftDown = false;
        private bool isMouseDown = false;
        private bool isOverlayActive = false;
        private Point polygonStartPoint;
        private Point polygonEndPoint;
        private MapPolygon mapPolygon = new MapPolygon();
        MapLayer polygonPointLayer = new MapLayer();

        private List<ImageAtLocation> selectedImages = null;
        private int currentImageIndex = 0;
        private BitmapImage[] preloadedImages = null;

        private Stopwatch imageUpdateTimer;
        private Stopwatch unassignedImageUpdateTimer;

        // A collection of key/value pairs containing the event name 
        // and the text block to display the event to.
        Dictionary<string, TextBlock> eventBlocks = new Dictionary<string, TextBlock>();
        // A collection of key/value pairs containing the event name  
        // and the number of times the event fired.
        Dictionary<string, int> eventCount = new Dictionary<string, int>();

        #endregion

        public MapView()
        {
            InitializeComponent();
            _options = new MyClusterOptions(20);
            
            //App.MapVM.ImageLocations.CollectionChanged += ImageLocations_CollectionChanged;
            App.MapVM.FilterdImageLocations.CollectionChanged += ImageLocations_CollectionChanged;
            App.MapVM.FilterdUnassignedImageLocations.CollectionChanged += UnassignedImageLocations_CollectionChanged; ;
            this.PreparePolygon();
            
            // Adds the layer that contains the polygon points
            NewPolygonLayer.Children.Add(this.mapPolygon);
            NewPolygonLayer.Children.Add(this.polygonPointLayer);

            // Fires every animated frame from one location to another.
            MyMap.ViewChangeOnFrame +=
                new EventHandler<MapEventArgs>(MapWithEvents_ViewChangeOnFrame);
            // Fires when the map view location has changed.
            MyMap.TargetViewChanged +=
                new EventHandler<MapEventArgs>(MapWithEvents_TargetViewChanged);
            // Fires when the map view starts to move to its new target view.
            MyMap.ViewChangeStart +=
                new EventHandler<MapEventArgs>(MapWithEvents_ViewChangeStart);
            // Fires when the map view has reached its new target view.
            MyMap.ViewChangeEnd +=
                new EventHandler<MapEventArgs>(MapWithEvents_ViewChangeEnd);
            // Fires when a different mode button on the navigation bar is selected.
            MyMap.ModeChanged +=
                new EventHandler<MapEventArgs>(MapWithEvents_ModeChanged);
            // Fires when the mouse is double clicked
            MyMap.MouseDoubleClick +=
                new MouseButtonEventHandler(MapWithEvents_MouseDoubleClick);
            // Fires when the mouse wheel is used to scroll the map
            MyMap.MouseWheel +=
                new MouseWheelEventHandler(MapWithEvents_MouseWheel);
            // Fires when the left mouse button is depressed
            MyMap.MouseLeftButtonDown +=
                new MouseButtonEventHandler(MapWithEvents_MouseLeftButtonDown);
            // Fires when the left mouse button is released
            MyMap.MouseLeftButtonUp +=
                new MouseButtonEventHandler(MapWithEvents_MouseLeftButtonUp);
            // Fires when the mouse moves
            MyMap.MouseMove +=
                new MouseEventHandler(MapWithEvents_MouseMove);

            MyMap.KeyDown += MyMap_KeyDown;
            MyMap.Loaded += MyMap_Loaded;
            
            Application.Current.MainWindow.KeyDown += MainWindow_KeyDown;
            Application.Current.MainWindow.KeyUp += MainWindow_KeyUp;
            
            this.unassignedImageUpdateTimer = new Stopwatch();
            this.unassignedImageUpdateTimer.Start();

            this.imageUpdateTimer = new Stopwatch();
            this.imageUpdateTimer.Start();

            this.ShowAllData();
            //this.ShowAllDataPointClustered();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                isShiftDown = true;
            }
            else if (e.Key == Key.Escape)
            {
                this.ReturnToMap();
            }
            else if (e.Key == Key.Left && this.isOverlayActive)
            {
                this.DisplayPrevImage();
                e.Handled = true;
            }
            else if (e.Key == Key.Right && this.isOverlayActive)
            {
                this.DisplayNextImage();
                e.Handled = true;
            }
            else if (e.Key == Key.Up && this.isOverlayActive)
            {
                this.DisplayPrevImage();
                e.Handled = true;
            }
            else if (e.Key == Key.Down && this.isOverlayActive)
            {
                this.DisplayNextImage();
                e.Handled = true;
            }
        }

        private void ReturnToMap()
        {
            this.preloadedImages = null;
            this.Overlay.Visibility = Visibility.Collapsed;
            this.isOverlayActive = false;
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                this.SelectPushpins();
                this.isMouseDown = false;
                this.isShiftDown = false;
                this.mapPolygon.Locations.Clear();
                this.polygonPointLayer.Children.Clear();
            }
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            // Zoom to fit
            this.ZoomMapToFitPins(this.MyMap, App.MapVM.ImageLocations);
            MyMap.Focus();

            Task.Delay(1000).ContinueWith(
                t => this.Dispatcher.Invoke(new Action(() => ImageHelper.SnapshotMap(App.MapVM.ProjectId, this.MyMap)))
                );
        }

        private void AgeSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            App.MapVM.ApplyImageFilters();
            this.ReloadAssignedImages(true);
            this.ReloadUnassignedImages(true);
        }

        private void ImageLocations_CollectionChanged(object sender, EntityCollectionChangedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
                this.ReloadAssignedImages(e.ForceUpdate)
            ), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }
        
        private void UnassignedImageLocations_CollectionChanged(object sender, EntityCollectionChangedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
                this.ReloadUnassignedImages(e.ForceUpdate)
            ), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }


        #region Button Handlers

        private void MenuItem_ActivateDebug(object sender, RoutedEventArgs e)
        {
            if (this.debugContainer.Visibility == Visibility.Visible)
                this.debugContainer.Visibility = Visibility.Collapsed;
            else if (this.debugContainer.Visibility == Visibility.Collapsed)
                this.debugContainer.Visibility = Visibility.Visible;
        }

        private void RoadMode_Clicked(object sender, RoutedEventArgs e)
        {
            MyMap.Mode = new RoadMode();
            MyMap.Focus();
        }

        private void AerialMode_Clicked(object sender, RoutedEventArgs e)
        {
            MyMap.Mode = new AerialMode();
            MyMap.Focus();
        }

        private void HybridMode_Clicked(object sender, RoutedEventArgs e)
        {
            MyMap.Mode = new AerialMode(true);
            MyMap.Focus();
        }

        private void OpenStreetViewNewWindow_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.MyBrowserControl.ModifiedURL);
            //System.Diagnostics.Process.Start(this.StreetViewContainer.Source.ToString());
        }

        private void ShowAllData()
        {
            PushpinLayer.Children.Clear();

            if (_generatingData)
            {
                StatusTbx.Text += "Database is still loading.\r\n";
                return;
            }

            if (App.MapVM.FilterdImageLocations.Count == 0)
            {
                StatusTbx.Text += "No database data found.\r\n";
                return;
            }

            foreach (ImageAtLocation img in App.MapVM.FilterdImageLocations)
            {
                ColoredPushpin p = _options.RenderEntity(img);
                string strokeColor;
                p.FillColor = App.MapVM.GetPushpinColors(img, out strokeColor);
                p.StrokeColor = strokeColor;
                PushpinLayer.Children.Add(p);
            }

            //StatusTbx.Text += "All data displayed without clustering.\r\n";
            
        }

        private void ShowAllDataPointClustered()
        {
            PushpinLayer.Children.Clear();

            if (_generatingData)
            {
                StatusTbx.Text += "Database is still loading.\r\n";
                return;
            }

            if (App.MapVM.FilterdImageLocations.Count == 0)
            {
                StatusTbx.Text += "No database data found.\r\n";
                return;
            }

            //Create an instance of the Point Based clustering layer
            var layer = new PointBasedClusteredLayer(MyMap, _options);

            //Get the map layer from the clustered layer and add to map
            PushpinLayer.Children.Add(layer.GetMapLayer());

            //Add mock data to cluster layer
            layer.AddEntities(App.MapVM.FilterdImageLocations.AsEntities());

            //StatusTbx.Text += "Point based clustering is enabled.\r\n";
            
        }

        //private void ShowAllDataGridClustered()
        //{
        //    PushpinLayer.Children.Clear();

        //    if (_generatingData)
        //    {
        //        StatusTbx.Text += "Database is still loading.\r\n";
        //        return;
        //    }

        //    if (App.MapVM.ImageLocations.Count == 0)
        //    {
        //        StatusTbx.Text += "No database data found.\r\n";
        //        return;
        //    }

        //    //Create an instance of the Grid Based clustering layer
        //    var layer = new GridBasedClusteredLayer(MyMap, _options);

        //    //Get the map layer from the clustered layer and add to map
        //    PushpinLayer.Children.Add(layer.GetMapLayer());

        //    //Add mock data to cluster layer
        //    layer.AddEntities(App.MapVM.ImageLocations.AsEntities());

        //    StatusTbx.Text += "Grid based clustering is enabled.\r\n";

        //}

        private void ZoomMapToFitPins(Map map, IEnumerable<ImageAtLocation> images)
        {
            List<Location> locations = images.Where(s => s.Location != null).Select(s => s.Location).ToList();
            LocationRect bounds;
            if (locations.Count == 0)
            {
                bounds = new LocationRect(new Location(59.917, 10.728, 0), 10000, 10000);// Start zooming in on Norway
            }
            else
            {
                bounds = new LocationRect(locations);
            }

            double height = Measure(bounds.North, bounds.East, bounds.South, bounds.West);
            int zoom = GetZoomLevel(height, bounds.Center.Latitude, map.ActualHeight, map.ActualWidth);
            if (zoom > 19)
                zoom = 19;
            if (zoom < 0)
                zoom = 19;
            if (zoom < 4)
                zoom = 4;
            map.SetView(bounds.Center, zoom);
        }

        private void ReloadAssignedImages(bool forceUpdate)
        {
            long time = this.imageUpdateTimer.ElapsedMilliseconds;
            if (time > 3000 || forceUpdate)
            {
                this.ShowAllData();
                //this.ShowAllDataPointClustered();
                this.imageUpdateTimer.Restart();
            }
        }

        private void ReloadUnassignedImages(bool forceUpdate)
        {
            long time = this.unassignedImageUpdateTimer.ElapsedMilliseconds;
            if(time > 3000 || forceUpdate)
            {
                this.UnassignedImagesControl.ItemsSource = App.MapVM.FilterdUnassignedImageLocations;
                this.UnassignedImagesControl.Items.Refresh();
                this.unassignedImageUpdateTimer.Restart();
            }
        }

        //private void PointClusterData_Clicked(object sender, RoutedEventArgs e)
        //{
        //    this.ShowAllDataPointClustered();
        //}

        //private void GridClusterData_Clicked(object sender, RoutedEventArgs e)
        //{
        //}

        #endregion

        #region Private Helper Methods

        private void PreparePolygon()
        {
            mapPolygon = new MapPolygon();
            // Defines the polygon fill details
            mapPolygon.Locations = new LocationCollection();
            mapPolygon.Fill = new SolidColorBrush(Colors.Gray);
            mapPolygon.Stroke = new SolidColorBrush(Colors.Green);
            mapPolygon.StrokeThickness = 3;
            mapPolygon.Opacity = 0.6;
        }

        ///// <summary>
        ///// Method that generates mock Entity data
        ///// </summary>
        //private void PopulateImages(int projectId)
        //{
        //    if (App.MapVM.ImageLocations != null)
        //    {
        //        _imageData.Clear();
        //    }
        //    else
        //    {
        //        _imageData = new List<Entity>();
        //    }
            
        //    Dictionary<int, ImageAtLocation> imageList = new Dictionary<int, ImageAtLocation>();
        //    Dictionary<string, object> whereProject = new Dictionary<string, object>();
        //    whereProject.Add(DImageAtLocation.ProjectId, projectId);
        //    DataTable data = App.DB.GetValues(DTables.Images, whereProject, new string[] { "*" });

        //    foreach (DataRow row in data.Rows)
        //    {
        //        ImageAtLocation img = new ImageAtLocation(row);
        //        imageList.Add(img.ID, img);
        //        _imageData.Add(img);
        //    }
        //}

        private void DisplayNextImage()
        {
            this.currentImageIndex++;
            if (this.currentImageIndex > this.selectedImages.Count - 1)
            {
                this.currentImageIndex = 0;
            }
            this.ShowImage(this.currentImageIndex);
        }
        private void DisplayPrevImage()
        {
            this.currentImageIndex--;
            if (this.currentImageIndex < 0)
                this.currentImageIndex = this.selectedImages.Count - 1;
            this.ShowImage(this.currentImageIndex);
        }

        private void SelectPushpins()
        {
            if (App.MapVM != null && App.MapVM.ImageLocations.Count == 0)
                return;

            Location corner1 = MyMap.ViewportPointToLocation(this.polygonStartPoint);
            Location corner2 = MyMap.ViewportPointToLocation(this.polygonEndPoint);

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            if (minX > corner1.Latitude)
                minX = corner1.Latitude;
            if (minX > corner2.Latitude)
                minX = corner2.Latitude;
            if (minY > corner1.Longitude)
                minY = corner1.Longitude;
            if (minY > corner2.Longitude)
                minY = corner2.Longitude;

            if (maxX < corner1.Latitude)
                maxX = corner1.Latitude;
            if (maxX < corner2.Latitude)
                maxX = corner2.Latitude;
            if (maxY < corner1.Longitude)
                maxY = corner1.Longitude;
            if (maxY < corner2.Longitude)
                maxY = corner2.Longitude;

            Rect rect = new Rect(
                minX,
                minY,
                maxX - minX,
                maxY - minY
                );


            //PushpinLayer.Children.Clear();
            this.selectedImages = new List<ImageAtLocation>();
            List<ImageAtLocation> allButFirst = new List<ImageAtLocation>();
            int n = 0;
            foreach (ImageAtLocation img in App.MapVM.ImageLocations)
            {
                if (rect.Contains(img.Location.Latitude, img.Location.Longitude))
                {
                    this.selectedImages.Add(img);
                    if (n != 0)
                        allButFirst.Add(img);

                    n++;
                }
                else
                {
                    //PushpinLayer.Children.Add(_options.RenderEntity(img));
                }
            }

            if (this.selectedImages.Count > 0)
            {
                this.ShowSelectedImages(allButFirst);
            }

        }

        private void ShowSelectedImages(List<ImageAtLocation> allButFirst)
        {
            this.isOverlayActive = true;
            this.Overlay.Visibility = Visibility.Visible;
            this.Overlay.Focus();
            this.currentImageIndex = 0;

            this.preloadedImages = new BitmapImage[this.selectedImages.Count];

            if(allButFirst.Count > 0)
            {
                // Start a background worker to preload all images
                BackgroundWorker imageLoader = new BackgroundWorker();
                imageLoader.DoWork += (s, a) =>
                {
                    bool success = this.PreloadImageSelection(a.Argument as List<ImageAtLocation>);
                    a.Cancel = !success;
                };
                imageLoader.RunWorkerCompleted += (s, a) =>
                {
                    if (a.Cancelled)
                        StatusTbx.Text += "Skipped loading images.\r\n";
                    else
                        StatusTbx.Text += "Loaded selected images.\r\n";
                };
                imageLoader.RunWorkerAsync(allButFirst);
            }
            
            this.ZoomMapToFitPins(this.InnerMap, this.selectedImages);

            // Show the first image
            this.ShowImage(this.currentImageIndex);
        }

        private bool PreloadImageSelection(List<ImageAtLocation> images)
        {
            preloadedImages = new BitmapImage[images.Count + 1];
            int i = 1;
            foreach (ImageAtLocation image in images)
            {
                BitmapImage b1 = new BitmapImage();
                b1.BeginInit();
                b1.CacheOption = BitmapCacheOption.OnLoad;
                b1.UriSource = new Uri(image.ImagePath, UriKind.RelativeOrAbsolute);
                b1.DecodePixelHeight = 800;
                b1.EndInit();
                b1.Freeze();
                if (preloadedImages == null) // To avoid fatal error on early escapes
                    return false;
                preloadedImages[i] = b1;
                i++;
            }
            return true;
        }

        private double Measure(double lat1, double lon1, double lat2, double lon2)
        {
            // generally used geo measurement function
            var R = 6378.137; // Radius of earth in KM
            var dLat = lat2 * Math.PI / 180 - lat1 * Math.PI / 180;
            var dLon = lon2 * Math.PI / 180 - lon1 * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c;
            return d * 1000; // meters
        }

        private double Measure(Location l1, Location l2)
        {
            double lat1 = l1.Latitude;
            double lon1 = l1.Longitude;
            double lat2 = l2.Latitude;
            double lon2 = l2.Longitude;
            return Measure(lat1, lon1, lat2, lon2);
        }

        private int GetZoomLevel(double rangeInMeters, double latitude, double heightOfMapInPixels, double widthOfMapInPixels)
        {
            // we take the lower value of the bounding rect of the map, so the circle will be best seen
            double limitBoundPixels = Math.Min(heightOfMapInPixels, widthOfMapInPixels);
            double zoom = Math.Floor(Math.Log(156543.03392 * Math.Cos(latitude * Math.PI / 180) / (rangeInMeters / limitBoundPixels)) / Math.Log(2));

            return (int)zoom;
        }

        private void ShowImage(int imageIndex)
        {
            // Check for potential error
            if (this.selectedImages.Count <= imageIndex || imageIndex < 0)
            {
                this.ReturnToMap();
                return;
            }

            ImageAtLocation image = this.selectedImages[imageIndex];
            if (this.preloadedImages != null && this.preloadedImages[imageIndex] != null)
            {
                currentImage.Source = this.preloadedImages[imageIndex];
            }
            else
            {
                BitmapImage b1 = new BitmapImage();
                b1.BeginInit();
                b1.CacheOption = BitmapCacheOption.Default;
                b1.UriSource = new Uri(image.ImagePath, UriKind.RelativeOrAbsolute);
                b1.DecodePixelHeight = 800;
                b1.EndInit();
                b1.Freeze();
                currentImage.Source = b1;

                if (this.preloadedImages != null && imageIndex == 0)
                    this.preloadedImages[0] = b1;

            }

            this.imageCounter.Text = (imageIndex + 1) + "/" + this.selectedImages.Count;
            this.ImageFileName.Text = System.IO.Path.GetFileName(image.ImagePath);
            this.ImageSaveDate.Text = "";
            this.ImageTaken.Text = image.TimeImageTaken.ToString(App.RegionalCulture);
            this.Creator.Text = image.Creator.ToString();

            // Zoom the inner map
            this.InnerMap.Children.Clear();
            int i = 0;
            Pushpin activePin = null;
            foreach (ImageAtLocation img in this.selectedImages)
            {
                ColoredPushpin pin = _options.RenderEntity(img);
                string strokeColor;
                pin.FillColor = App.MapVM.GetPushpinColors(img, out strokeColor);
                pin.StrokeColor = strokeColor;

                pin.Tag = i + 1;
                if (i == imageIndex)
                {
                    ControlTemplate myTemplate = (ControlTemplate)FindResource("PushpinControlTemplate");
                    pin.Template = myTemplate;
                    pin.ApplyTemplate();
                    activePin = pin;
                }
                else
                {
                    this.InnerMap.Children.Add(pin);
                }
                i++;
            }
            // Make sure the active pin are added last
            this.InnerMap.Children.Add(activePin);

            if (image.HasLocation)
            {
                // Set the URL for the google street view
                List<Location> locations = this.selectedImages.Where(s => s.Location != null).Select(s => s.Location).ToList();
                LocationRect bounds = new LocationRect(locations);

                double height = Measure(bounds.North, bounds.East, bounds.South, bounds.West);
                //int zoom = GetZoomLevel(height, bounds.Center.Latitude, this.webView.ActualHeight, this.webView.ActualWidth);
                //if (zoom > 19)
                //    zoom = 19;
                //if (zoom < 0)
                //    zoom = 19;
                //if (zoom < 4)
                //    zoom = 4;

                //this.StreetViewContainer.NavigationCompleted += StreetViewContainer_NavigationCompleted;
                string googleStreetViewString =
                        string.Format(@"https://www.google.com/maps/@?api=1&map_action=pano&viewpoint={0},{1}&heading=-45&pitch=0&fov=80",
                        //string.Format(@"https://www.google.com/maps/@{0},{1},{2}z",
                        image.Location.Latitude.ToString(CultureInfo.InvariantCulture), image.Location.Longitude.ToString(CultureInfo.InvariantCulture));
                //this.StreetViewContainer.Navigate(new Uri(googleStreetViewString));
                this.MyBrowserControl.URL = googleStreetViewString;
            }
        }

        #endregion

        #region Events

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ImageAtLocation activeImage = this.selectedImages[this.currentImageIndex];
            string argument = "/select, \"" + activeImage.ImagePath + "\"";
            Process.Start("explorer.exe", argument); //System.IO.Path.GetDirectoryName(activeImage.ImagePath)
        }

        private void MyMap_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left && this.isOverlayActive)
            {
                this.DisplayPrevImage();
                e.Handled = true;
            }
            else if (e.Key == Key.Right && this.isOverlayActive)
            {
                this.DisplayNextImage();
                e.Handled = true;
            }
            else if (e.Key == Key.Up && this.isOverlayActive)
            {
                this.DisplayPrevImage();
                e.Handled = true;
            }
            else if (e.Key == Key.Down && this.isOverlayActive)
            {
                this.DisplayNextImage();
                e.Handled = true;
            }
        }

        void MapWithEvents_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            // Updates the count of single mouse clicks.
            ShowEvent("MapWithEvents_MouseLeftButtonUp");
            this.isMouseDown = false;
        }

        void MapWithEvents_MouseWheel(object sender, MouseEventArgs e)
        {
            // Updates the count of mouse drag boxes created.
            ShowEvent("MapWithEvents_MouseWheel");
        }

        void MapWithEvents_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            // Updates the count of mouse pans.
            ShowEvent("MapWithEvents_MouseLeftButtonDown");

            if (this.isShiftDown)
            {
                this.isMouseDown = true;
                e.Handled = true;

                // Creates a location for a single polygon point and adds it to
                // the polygon's point location list.
                this.polygonStartPoint = e.GetPosition(this.MyMap);

                //Convert the mouse coordinates to a location on the map
                Location polygonPointLocation = MyMap.ViewportPointToLocation(this.polygonStartPoint);
                mapPolygon.Locations.Add(polygonPointLocation);

                // A visual representation of a polygon point.
                Rectangle r = new Rectangle();
                r.Fill = new SolidColorBrush(Colors.Red);
                r.Stroke = new SolidColorBrush(Colors.Yellow);
                r.StrokeThickness = 1;
                r.Width = 8;
                r.Height = 8;

                // Adds a small square where the user clicked, to mark the polygon point.
                polygonPointLayer.AddChild(r, polygonPointLocation);
                //Set focus back to the map so that +/- work for zoom in/out
                MyMap.Focus();
            }
        }

        void MapWithEvents_MouseMove(object sender, MouseEventArgs e)
        {
            // Updates the count of mouse double clicks.
            ShowEvent("MapWithEvents_MouseMove");
            if (this.isShiftDown && this.isMouseDown)
            {
                this.polygonEndPoint = e.GetPosition(this.MyMap);

                //Convert the mouse coordinates to a location on the map
                Location upperRight = MyMap.ViewportPointToLocation(this.polygonStartPoint);
                Location lowerRight = MyMap.ViewportPointToLocation(new Point(this.polygonStartPoint.X, this.polygonEndPoint.Y));
                Location lowerLeft = MyMap.ViewportPointToLocation(this.polygonEndPoint);
                Location upperLeft = MyMap.ViewportPointToLocation(new Point(this.polygonEndPoint.X, this.polygonStartPoint.Y));

                mapPolygon.Locations.Clear();
                mapPolygon.Locations.Add(upperRight);
                mapPolygon.Locations.Add(lowerRight);
                mapPolygon.Locations.Add(lowerLeft);
                mapPolygon.Locations.Add(upperLeft);

                // Adds a small square where the user clicked, to mark the polygon point.
                polygonPointLayer.Children.Clear();
                //Set focus back to the map so that +/- work for zoom in/out
                MyMap.Focus();
            }
        }


        void MapWithEvents_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Updates the count of mouse double clicks.
            ShowEvent("MapWithEvents_MouseDoubleClick");
        }

        void MapWithEvents_ViewChangeEnd(object sender, MapEventArgs e)
        {
            //Updates the number of times the map view has changed.
            ShowEvent("ViewChangeEnd");
        }

        void MapWithEvents_ViewChangeStart(object sender, MapEventArgs e)
        {
            //Updates the number of times the map view started changing.
            ShowEvent("ViewChangeStart");
        }

        void MapWithEvents_ViewChangeOnFrame(object sender, MapEventArgs e)
        {
            // Updates the number of times a map view has changed 
            // during an animation from one location to another.
            ShowEvent("ViewChangeOnFrame");
        }
        void MapWithEvents_TargetViewChanged(object sender, MapEventArgs e)
        {
            // Updates the number of map view changes that occured during
            // a zoom or pan.
            ShowEvent("TargetViewChange");
        }

        void MapWithEvents_ModeChanged(object sender, MapEventArgs e)
        {
            // Updates the number of times the map mode changed.
            ShowEvent("ModeChanged");
        }

        void ShowEvent(string eventName)
        {
            // Updates the display box showing the number of times 
            // the wired events occured.
            if (!eventBlocks.ContainsKey(eventName))
            {
                TextBlock tb = new TextBlock();
                tb.Foreground = new SolidColorBrush(
                    Color.FromArgb(255, 128, 255, 128));
                tb.Margin = new Thickness(5);
                eventBlocks.Add(eventName, tb);
                eventCount.Add(eventName, 0);
                eventsPanel.Children.Add(tb);
            }

            eventCount[eventName]++;
            eventBlocks[eventName].Text = String.Format(
                "{0}: [{1} times] {2} (HH:mm:ss:ffff)",
                eventName, eventCount[eventName].ToString(), DateTime.Now.ToString());
        }

        #endregion

        #region Dragging

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ImageAtLocation imageAtLocation = ((Image)sender).Tag as ImageAtLocation;

            this.selectedImages = new List<ImageAtLocation>();
            this.selectedImages.Add(imageAtLocation);
            this.ShowSelectedImages(new List<ImageAtLocation>());
        }

        private void Images_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            dragStartPoint = e.GetPosition(null);
        }

        private void Images_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = dragStartPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ItemsControl control = sender as ItemsControl;
                Image inner = FindAnchestor<Image>((DependencyObject)e.OriginalSource);

                if (inner == null)
                    return;

                //// Find the data behind the ListViewItem
                //ImageAtLocation image = (Image)control.ItemContainerGenerator.
                //    ItemFromContainer(inner);
                ImageAtLocation image = inner.Tag as ImageAtLocation;

                // Initialize the drag & drop operation
                DataObject dragData = new DataObject("draggedImageWitoutLocation", image);
                DragDrop.DoDragDrop(control, dragData, DragDropEffects.Move);
            }
        }

        // Helper to search up the VisualTree
        private static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }


        private void GoogleStreetView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            //this.GoogleStreetViewMarker.CaptureMouse();
            //offsetInGoogleStreetViewMarker = e.GetPosition(null);
            
            //var offsetInEllipse = e.GetPosition(this.GoogleStreetViewMarker);
            //this.GoogleStreetViewTranslate.BeginAnimation(TranslateTransform.XProperty,
            //    new DoubleAnimation(this.GoogleStreetViewMarker.Width / 2 - offsetInEllipse.X, 0,
            //        new Duration(TimeSpan.FromSeconds(1))));
            //this.GoogleStreetViewTranslate.BeginAnimation(TranslateTransform.YProperty,
            //    new DoubleAnimation(this.GoogleStreetViewMarker.Height / 2 - offsetInEllipse.Y, 0,
            //        new Duration(TimeSpan.FromSeconds(1))));

            //MoveGoogleMarker(e);
        }

        //private void MoveGoogleMarker(MouseEventArgs e)
        //{
        //    var pos = e.GetPosition(null);
        //    Canvas.SetLeft(this.GoogleStreetViewMarker, pos.X - this.GoogleStreetViewMarker.Width / 2);
        //    Canvas.SetTop(this.GoogleStreetViewMarker, pos.Y - this.GoogleStreetViewMarker.Height / 2);
        //}

        private void GoogleStreetView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            dragStartPoint = e.GetPosition(null);
        }

        private void GoogleStreetView_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = dragStartPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                Image control = sender as Image;

                // Initialize the drag & drop operation
                DataObject dragData = new DataObject("googleStreetView", true);
                DragDrop.DoDragDrop(control, dragData, DragDropEffects.Move);
            }

            //if (e.LeftButton != MouseButtonState.Pressed || !this.GoogleStreetViewMarker.IsMouseCaptured)
            //    return;

            //MoveGoogleMarker(e);

        }

        private void MyMap_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("draggedImageWitoutLocation") ||
                sender == e.Source)
            {
                //e.Effects = DragDropEffects.None;
            }
        }

        private void MyMap_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("draggedImageWitoutLocation"))
            {
                int countBefore = App.MapVM.FilterdUnassignedImageLocations.Count;
                Location dropAt = this.MyMap.ViewportPointToLocation(e.GetPosition(this.MyMap));
                ImageAtLocation image = e.Data.GetData("draggedImageWitoutLocation") as ImageAtLocation;
                App.MapVM.UnassignedImageLocations.Remove(image);
                App.MapVM.UnassignedImageLocations.TriggerCollectionChanged(true); // Force updating the content
                App.MapVM.FilterdUnassignedImageLocations.Remove(image);
                App.MapVM.FilterdUnassignedImageLocations.TriggerCollectionChanged(true); // Force updating the content

                // Update the location add to images
                image.Location = dropAt;
                image.LocationSource = ImageAtLocation.LocationSourceType.Manual;

                // Write data back to the database
                image.SaveToDatabase();

                // Write a file to disk to help locate the image
                FileHelper.CreateCoordinateFile(image.ImagePath, dropAt);

                // Add to collection
                App.MapVM.ImageLocations.Add(image);
                App.MapVM.ImageLocations.TriggerCollectionChanged(true); // Force updating the content
                App.MapVM.FilterdImageLocations.Add(image);
                App.MapVM.FilterdImageLocations.TriggerCollectionChanged(true); // Force updating the content
                int countAfter = App.MapVM.FilterdUnassignedImageLocations.Count;
            }
            else if (e.Data.GetDataPresent("googleStreetView"))
            {
                Location dropAt = this.MyMap.ViewportPointToLocation(e.GetPosition(this.MyMap));

                string googleStreetViewString = 
                    string.Format(@"https://www.google.com/maps/@?api=1&map_action=pano&viewpoint={0},{1}", //&heading=-45&pitch=0&fov=80
                    dropAt.Latitude, dropAt.Longitude);
                System.Diagnostics.Process.Start(googleStreetViewString);

            }
        }
        #endregion

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            this.DisplayPrevImage();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            this.DisplayNextImage();
        }

        private void EscapeToMap_Click(object sender, RoutedEventArgs e)
        {
            this.ReturnToMap();
        }
    }
}
