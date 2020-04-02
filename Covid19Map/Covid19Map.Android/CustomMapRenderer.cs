using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Covid19Map;
using Covid19Map.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;
using Android.Graphics;
using System;
using Android.Locations;
using Xamarin.Forms.Maps;
using System.Collections.Generic;
using Android.Widget;
using static Android.Gms.Maps.GoogleMap;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace Covid19Map.Droid
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        List<CustomPin> customPins;
        private bool _mapDrawn;
        public CustomMapRenderer(Context context) : base(context)
        {
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Cleanup();
            }

            base.Dispose(disposing);
        }


        private void Cleanup()
        {
            if (base.NativeMap == null) return;
            //base.NativeMap.MarkerClick -= HandleMarkerClick;
            this.NativeMap.InfoWindowClick -= OnInfoWindowClick;
            base.NativeMap.Clear();
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                //base.NativeMap.MarkerClick -= HandleMarkerClick;
                //this.NativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                customPins = formsMap.CustomPins;
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            // OnMapReady is called twice, not entirely certain why, known issue
            //if (_mapDrawn) return;

            base.OnMapReady(map);

            //base.NativeMap.MarkerClick += HandleMarkerClick;
            //this.NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);
            /*
            foreach (var formsPin in customPins)
            {
                var markerWithIcon = new MarkerOptions();
                markerWithIcon.SetPosition(new LatLng(formsPin.Position.Latitude, formsPin.Position.Longitude));
                markerWithIcon.SetTitle(formsPin.Label);
                markerWithIcon.SetSnippet(formsPin.Address);
                //markerWithIcon.SetRotation(100);

                var m = NativeMap.AddMarker(markerWithIcon);

                var polylineOptions = new PolylineOptions();
                polylineOptions.InvokeColor(0x66FF0000);

                polylineOptions.Add(new LatLng(formsPin.Position.Latitude, formsPin.Position.Longitude));


                base.NativeMap.AddPolyline(polylineOptions);
                m.ShowInfoWindow();
            }

            _mapDrawn = true;
            */
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);
            marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));

            var polylineOptions = new PolylineOptions();
            polylineOptions.InvokeColor(0x66FF0000);

            polylineOptions.Add(new LatLng(pin.Position.Latitude, pin.Position.Longitude));


            base.NativeMap.AddPolyline(polylineOptions);

            return marker;
        }

        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var customPin = GetCustomPin(e.Marker);
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            if (!string.IsNullOrWhiteSpace(customPin.Url))
            {
                var url = Android.Net.Uri.Parse(customPin.Url);
                var intent = new Intent(Intent.ActionView, url);
                intent.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            }
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;

                var customPin = GetCustomPin(marker);
                if (customPin == null)
                {
                    //throw new Exception("Custom pin not found");
                }

                //if (customPin.Name.Equals("Xamarin"))
                //{
                    view = inflater.Inflate(Resource.Layout.XamarinMapInfoWindow, null);
                //}
                //else
                //{
                 //   view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);
                //}

                var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle);
                var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

                if (infoTitle != null)
                {
                    infoTitle.Text = marker.Title;
                }
                if (infoSubtitle != null)
                {
                    infoSubtitle.Text = marker.Snippet;
                }

                return view;
            }
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        CustomPin GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            if (customPins != null)
            { 
                foreach (var pin in customPins)
                {
                    if (pin.Position == position)
                    {
                        return pin;
                    }
                }
            }
            return null;
        }

        private static void HandleMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            e.Handled = true;
            if (!Equals(e.Marker, null))
                e.Marker.ShowInfoWindow();
        }
    }
}
