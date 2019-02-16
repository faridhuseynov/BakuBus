using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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
        private int selectedBusIndex;
        public int SelectedBusIndex { get => selectedBusIndex; set => Set(ref selectedBusIndex, value); }

        //Bus List for the combo box
        private ObservableCollection<string> busList;
        public ObservableCollection<string> BusList { get => busList; set => Set(ref busList, value); }

        private ObservableCollection<Bus> buses;
        public ObservableCollection<Bus> Buses { get => buses; set => Set(ref buses, value); }

        private ObservableCollection<Bus> mapBuses;
        public ObservableCollection<Bus> MapBuses { get => mapBuses; set => Set(ref mapBuses, value); }


        public HttpClient httpClient { get; set; } = new HttpClient();
        public string apiUri = @"https://www.bakubus.az/az/ajax/apiNew1";

        async void GetBusListAsync(object sender)
        {
            var json = await httpClient.GetStringAsync(apiUri);
            var result =   JsonConvert.DeserializeObject(json) as JObject;
            foreach (var item in result["BUS"])
            {
                //check if it is not the test bus. some of the buses are just for test, no need to add them
                var check = item["@attributes"]["DISPLAY_ROUTE_CODE"].ToString();

                if (check != "T" && check != "0")
                {
                    Bus NewBus = new Bus();
                    //bus number
                    NewBus.Name = check;

                    NewBus.Coordinates = new Microsoft.Maps.MapControl.WPF.Location();

                    //bus lattitude
                    NewBus.Coordinates.Latitude = double.Parse(item["@attributes"]["LATITUDE"].ToString());

                    //bus longitude
                    NewBus.Coordinates.Longitude = double.Parse(item["@attributes"]["LONGITUDE"].ToString());

                    //Application.Current.Dispatcher.Invoke((System.Action)delegate
                    //{
                    //    Buses.Add(NewBus);
                    //});
                    Buses.Add(NewBus);

                    Application.Current.Dispatcher.Invoke((System.Action)delegate
                    {
                        if (!BusList.Contains(NewBus.Name) && NewBus.Name != "H1")
                            BusList.Add(NewBus.Name);
                    });
                  
                }
            }
           BusList=new ObservableCollection<string>(BusList.OrderByDescending(x=>x));
        }
        public BakuBusViewModel()
        {
            Buses = new ObservableCollection<Bus>();
            BusList = new ObservableCollection<string>();
            BusList.Add("Choose...");
            BusList.Add("Airport Express");
            SelectedBusIndex = 0;
            //Timer timer = new Timer(param=> { GetBusListAsync(); }, null, 0, 2);

            GetBusListAsync(new object());
            MapBuses = new ObservableCollection<Bus>(Buses);
            //Timer timer = new Timer(GetBusListAsync, null, 0, 5000);
        }

        private RelayCommand busSearchCommand;
        public RelayCommand BusSearchCommand
        {
            get => busSearchCommand ?? (busSearchCommand = new RelayCommand(
                            () =>
                            {
                                var bus = BusList[SelectedBusIndex];                                
                                if (bus == "Choose...")
                                {
                                    MapBuses = new ObservableCollection<Bus>(Buses);
                                }
                                else
                                {
                                    if (bus == "Airport Express")
                                        bus = "H1";
                                    MapBuses = new ObservableCollection<Bus>(Buses.Where(x => x.Name == bus));
                                }
                            }
                        ));
        }
    }
}
