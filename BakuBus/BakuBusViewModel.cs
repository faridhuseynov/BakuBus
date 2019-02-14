using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BakuBus
{
    public class BakuBusViewModel : ViewModelBase
    {
        private int selectedBusIndex=0;
        public int SelectedBusIndex { get => selectedBusIndex; set => Set(ref selectedBusIndex, value); }

        //Bus List for the combo box
        private ObservableCollection<string> busList;
        public ObservableCollection<string> BusList { get => busList; set => Set(ref busList, value); }

        private ObservableCollection<Bus> buses;
        public ObservableCollection<Bus> Buses { get => buses; set => Set(ref buses, value); }
        //private ObservableCollection<Bus> buses2;
        //public ObservableCollection<Bus> Buses2 { get => buses2; set => Set(ref buses2, value); }


        public HttpClient httpClient { get; set; } = new HttpClient();
        public string apiUri = @"https://www.bakubus.az/az/ajax/apiNew1";

        async void GetBusListAsync()
        {
            var json = await httpClient.GetStringAsync(apiUri);
            var result =   JsonConvert.DeserializeObject(json) as JObject;
            foreach (var item in result["BUS"])
            {
                //check if it is not the test bus. some of the buses are just for test, no need to add them
                var check = item["@attributes"]["DISPLAY_ROUTE_CODE"].ToString();

                if (check != "T")
                {
                    Bus NewBus = new Bus();
                    //bus number
                    NewBus.Name = check;

                    NewBus.Coordinates = new Microsoft.Maps.MapControl.WPF.Location();

                    //bus lattitude
                    NewBus.Coordinates.Latitude = double.Parse(item["@attributes"]["LATITUDE"].ToString());

                    //bus longitude
                    NewBus.Coordinates.Longitude = double.Parse(item["@attributes"]["LONGITUDE"].ToString());

                    Application.Current.Dispatcher.Invoke((System.Action)delegate
                    {
                        Buses.Add(NewBus);
                    });
                    
                    if (!BusList.Contains(NewBus.Name) && NewBus.Name != "H1")
                        BusList.Add(NewBus.Name);
                }
            }
        }
        public BakuBusViewModel()
        {
            Buses = new ObservableCollection<Bus>();
            BusList = new ObservableCollection<string>();
            BusList.Add("Choose...");
            BusList.Add("Airport Express");
            SelectedBusIndex = 0;
            //Timer timer = new Timer(param=> { GetBusListAsync(); }, null, 0, 2);

            GetBusListAsync();
            //int a = 0;
            //while (a<2)
            //{
            //    Timer timer = new Timer(param => { SelectedBusIndex++; }, null, 0, 1);
            //    a++;
            //}
        }
    }
}
