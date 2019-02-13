using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BakuBus
{
    public class BakuBusViewModel:ViewModelBase
    {
        private ObservableCollection<Bus> buses;
        public ObservableCollection<Bus> Buses {  get => buses; set =>Set(ref buses, value); }

        public HttpClient httpClient { get; set; }
        public string apiUri = @"https://www.bakubus.az/az/ajax/apiNew1";

        public BakuBusViewModel()
        {
            Buses = new ObservableCollection<Bus>();
            var json = httpClient.GetAsync(apiUri);
            var result = JsonConvert.DeserializeObject(json.ToString());
            foreach (var item in result["BUS"])
            {

            }
        }
    }
}
