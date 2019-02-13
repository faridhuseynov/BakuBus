using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BakuBus
{
    public class BakuBusViewModel : ViewModelBase
    {
        private ObservableCollection<string> busList;
        public ObservableCollection<string> BusList { get => busList; set => Set(ref busList, value); }

        private ObservableCollection<Bus> buses;
        public ObservableCollection<Bus> Buses { get => buses; set => Set(ref buses, value); }

        public HttpClient httpClient { get; set; } = new HttpClient();
        public string apiUri = @"https://www.bakubus.az/az/ajax/apiNew1";

        async void GetBusListAsync()
        {
            var json = await httpClient.GetStringAsync(apiUri);
            var result =   JsonConvert.DeserializeObject(json) as JObject;
            foreach (var item in result["BUS"])
            {
                Bus NewBus = new Bus();
                //bus number
                NewBus.Name = item["@atttibutes"]["DISPLAY_ROUTE_CODE"].ToString();

                //bus lattitude
                NewBus.Coordinates.Latitude = double.Parse(item["@atttibutes"]["LATITUDE"].ToString());

                //bus longitude
                NewBus.Coordinates.Longitude = double.Parse(item["@atttibutes"]["LONGITUDE"].ToString());

                Buses.Add(NewBus);
                if (!BusList.Contains(NewBus.Name))
                    BusList.Add(NewBus.Name);                
            }
        }
        public BakuBusViewModel()
        {
            Buses = new ObservableCollection<Bus>();
            BusesList = new ObservableCollection<string>();
            GetBusListAsync();
        }
    }
}
