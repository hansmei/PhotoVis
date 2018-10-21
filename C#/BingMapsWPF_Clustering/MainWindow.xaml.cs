﻿using ClusterEngine;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using PhotoVis.Util;
using PhotoVis.Data;
using PhotoVis.Data.DatabaseTables;

namespace PhotoVis
{
    public partial class MainWindow : Window
    {
        private List<Entity> _mockData;
        private ClusterOptions _options;
        private bool _generatingData;
        private BackgroundWorker _worker;

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

        // A collection of key/value pairs containing the event name 
        // and the text block to display the event to.
        Dictionary<string, TextBlock> eventBlocks = new Dictionary<string, TextBlock>();
        // A collection of key/value pairs containing the event name  
        // and the number of times the event fired.
        Dictionary<string, int> eventCount = new Dictionary<string, int>();


        public MainWindow()
        {
            InitializeComponent();

            _options = new MyClusterOptions(20);

            //Background worker for populating data
            _worker = new BackgroundWorker();
            _worker.DoWork += (s, a) =>
            {
                PopulateImages((int)a.Argument);
            };
            _worker.RunWorkerCompleted += (s, a) =>
            {
                _generatingData = false;
                StatusTbx.Text += "Loaded database.\r\n";
                this.ShowAllDataPointClustered();
            };
            _worker.RunWorkerAsync(1);
            
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
            //PushpinLayer.Children.Clear();

            //int size;

            //if (string.IsNullOrWhiteSpace(EntitySize.Text) ||
            //    !int.TryParse(EntitySize.Text, out size))
            //{
            //    StatusTbx.Text += "Invalid mock data size.\r\n";
            //    return;
            //}

            //StatusTbx.Text += "Generating Mock data.\r\n";

            //_worker.RunWorkerAsync(size);
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

        private void ShowAllData()
        {
            PushpinLayer.Children.Clear();

            if (_generatingData)
            {
                StatusTbx.Text += "Database is still loading.\r\n";
                return;
            }

            if (_mockData == null)
            {
                StatusTbx.Text += "No database data found.\r\n";
                return;
            }

            foreach (var entity in _mockData)
            {
                PushpinLayer.Children.Add(_options.RenderEntity(entity));
            }

            StatusTbx.Text += "All data displayed without clustering.\r\n";

            LocationRect bounds = new LocationRect(_mockData.Select(s => s.Location).ToList());
            MyMap.SetView(bounds);
            MyMap.Focus();
        }

        private void ShowAllDataPointClustered()
        {
            PushpinLayer.Children.Clear();

            if (_generatingData)
            {
                StatusTbx.Text += "Database is still loading.\r\n";
                return;
            }

            if (_mockData == null)
            {
                StatusTbx.Text += "No database data found.\r\n";
                return;
            }

            //Create an instance of the Point Based clustering layer
            var layer = new PointBasedClusteredLayer(MyMap, _options);

            //Get the map layer from the clustered layer and add to map
            PushpinLayer.Children.Add(layer.GetMapLayer());

            //Add mock data to cluster layer
            layer.AddEntities(_mockData);

            StatusTbx.Text += "Point based clustering is enabled.\r\n";

            LocationRect bounds = new LocationRect(_mockData.Select(s => s.Location).ToList());
            MyMap.SetView(bounds);
            MyMap.Focus();
        }

        private void ShowAllDataGridClustered()
        {
            PushpinLayer.Children.Clear();

            if (_generatingData)
            {
                StatusTbx.Text += "Database is still loading.\r\n";
                return;
            }

            if (_mockData == null)
            {
                StatusTbx.Text += "No database data found.\r\n";
                return;
            }

            //Create an instance of the Grid Based clustering layer
            var layer = new GridBasedClusteredLayer(MyMap, _options);

            //Get the map layer from the clustered layer and add to map
            PushpinLayer.Children.Add(layer.GetMapLayer());

            //Add mock data to cluster layer
            layer.AddEntities(_mockData);

            StatusTbx.Text += "Grid based clustering is enabled.\r\n";

            LocationRect bounds = new LocationRect(_mockData.Select(s => s.Location).ToList());
            MyMap.SetView(bounds);
            MyMap.Focus();
        }


        private void PointClusterData_Clicked(object sender, RoutedEventArgs e)
        {
            this.ShowAllDataPointClustered();
        }

        private void GridClusterData_Clicked(object sender, RoutedEventArgs e)
        {
        }

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

        /// <summary>
        /// Method that generates mock Entity data
        /// </summary>
        private void PopulateImages(int numEntities)
        {
            if (_mockData != null)
            {
                _mockData.Clear();
            }
            else
            {
                _mockData = new List<Entity>();
            }

            //AssignmentIndexer.IndexImages();


            Dictionary<int, ImageAtLocation> imageList = new Dictionary<int, ImageAtLocation>();
            DataTable data = App.DB.GetValues(DTables.Images, null, new string[] { "*" });

            foreach (DataRow row in data.Rows)
            {
                ImageAtLocation img = new ImageAtLocation(row);
                imageList.Add(img.ID, img);

                _mockData.Add(img);
                //_mockData.Add(new CustomEntity()
                //{
                //    ID = img.Id,
                //    Location = new Location()
                //    {
                //        Latitude = img.Latitude,
                //        Longitude = img.Longitude,
                //    },
                //    Title = string.Format("Entity: {0}", img.Id)
                //});
            }
        }

        private void DisplayNextImage()
        {
            this.currentImageIndex++;
            if(this.currentImageIndex > this.selectedImages.Count - 1)
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
            if (_mockData == null)
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
            foreach (ImageAtLocation img in _mockData)
            {
                if(rect.Contains(img.Location.Latitude, img.Location.Longitude))
                {
                    this.selectedImages.Add(img);
                    if(n != 0)
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
                this.isOverlayActive = true;
                this.Overlay.Visibility = Visibility.Visible;
                int currentImageIndex = 0;

                this.preloadedImages = new BitmapImage[this.selectedImages.Count];
                
                // Start a background worker to preload all images
                BackgroundWorker imageLoader = new BackgroundWorker();
                imageLoader.DoWork += (s, a) =>
                {
                    bool success = this.PreloadImageSelection(a.Argument as List<ImageAtLocation>);
                    a.Cancel = !success;
                };
                imageLoader.RunWorkerCompleted += (s, a) =>
                {
                    if(a.Cancelled)
                        StatusTbx.Text += "Skipped loading images.\r\n";
                    else
                       StatusTbx.Text += "Loaded selected images.\r\n";
                };
                imageLoader.RunWorkerAsync(allButFirst);


                List<Location> locations = this.selectedImages.Select(s => s.Location).ToList();
                LocationRect bounds = new LocationRect(locations);

                double height = Measure(bounds.North, bounds.East, bounds.South, bounds.West);
                int zoom = GetZoomLevel(height, bounds.Center.Latitude, this.Height * 0.5, this.Width * 0.4);
                if (zoom > 19)
                    zoom = 19;
                if (zoom < 4)
                    zoom = 4;
                this.InnerMap.SetView(bounds.Center, zoom);

                // Show the first image
                this.ShowImage(currentImageIndex);
            }
            
        }

        private bool PreloadImageSelection(List<ImageAtLocation> images)
        {
            preloadedImages = new BitmapImage[images.Count + 1];
            int i = 1;
            foreach(ImageAtLocation image in images)
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
            this.FileName.Text = System.IO.Path.GetFileName(image.ImagePath);
            string format = "yyyy MM DD HH:mm:ss";
            this.ImageTaken.Text = image.TimeImageTaken.ToString();
            
            // Zoom the inner map
            this.InnerMap.Children.Clear();
            int i = 0;
            Pushpin activePin = null;
            foreach (ImageAtLocation img in this.selectedImages)
            {
                Pushpin pin = _options.RenderEntity(img);
                pin.Content = i + 1;
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
        }

        #endregion

        #region Events

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ImageAtLocation activeImage = this.selectedImages[this.currentImageIndex];
            string argument = "/select, \"" + activeImage.ImagePath + "\"";
            Process.Start("explorer.exe", argument); //System.IO.Path.GetDirectoryName(activeImage.ImagePath)
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if(e.Key == Key.LeftShift)
            {
                isShiftDown = true;
            }
            else if (e.Key == Key.Escape)
            {
                this.preloadedImages = null;
                this.Overlay.Visibility = Visibility.Collapsed;
                this.isOverlayActive = false;
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

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Key == Key.LeftShift)
            {
                this.SelectPushpins();
                this.isMouseDown = false;
                this.isShiftDown = false;
                this.mapPolygon.Locations.Clear();
                this.polygonPointLayer.Children.Clear();
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
                this.polygonStartPoint = e.GetPosition(this);

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
                this.polygonEndPoint = e.GetPosition(this);

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

    }
}
