using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using AccuWeatherData;
using System.Xml.Serialization;
using System.IO; // reference to our Data DLL


namespace AccuWeatherGetStationID
{
    /// <summary>
    /// This will attempt to retrieve station information via city info
    /// </summary>
    class Program
    {

        static void Main(string[] args)
        {
         
               
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("AccuWeather Point Data Tester");
                Console.WriteLine("=============================");
                Console.WriteLine("A. Query Location Name");
                Console.WriteLine("B. Current Conditions for Point");
                Console.WriteLine("C. 72 hour Forecast for Point");
                Console.WriteLine("D. Day Part Forecast for 7 Days");
                Console.WriteLine("Q. Quit");
                Console.WriteLine("-----------------------------");
                Console.Write("Your entry?: ");
                while (true)
                {
                    var keyPress = Console.ReadKey();
                    if (keyPress.KeyChar == 'a' || keyPress.KeyChar == 'A')
                    {
                        Console.WriteLine(); Console.WriteLine();
                        QueryLocations(); 
                        break;
                    }
                    if (keyPress.KeyChar == 'b' || keyPress.KeyChar == 'B')
                    {
                        Console.WriteLine(); Console.WriteLine();
                        QueryCurrentConditions();
                        break;
                    }
                    if (keyPress.KeyChar == 'c' || keyPress.KeyChar == 'C') 
                    {
                        Console.WriteLine(); Console.WriteLine();                           
                        QueryHourlyForecastPointData();
                        break;
                    }
                    if (keyPress.KeyChar == 'd' || keyPress.KeyChar == 'D')
                    {
                        Console.WriteLine(); Console.WriteLine();
                        QueryDaypartForecastPointData();
                        break;
                    }
                    if (keyPress.KeyChar == '~')
                    {
                        LoadLocationList();
                        break;
                    }

                    if (keyPress.KeyChar == '`')
                    {
                        SaveLocationList();
                        break;
                    }

                    if (keyPress.KeyChar == 'q' || keyPress.Key == ConsoleKey.Escape)
                    {
                        Environment.Exit(0);
                    }

                        
                }

            }
            
        }

        static void QueryCurrentConditions()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("ACCUWEATHER CURRENT OBSERVATION QUERY");
                Console.WriteLine("=====================================");
                Console.Write("Enter AccuWeather Location ID ('Q' returns to main menu): ");
                string accuWeatherLocationId = null;
                while (String.IsNullOrWhiteSpace(accuWeatherLocationId))
                {
                    accuWeatherLocationId = Console.ReadLine();

                    if (String.IsNullOrWhiteSpace(accuWeatherLocationId)) Console.Write("Invalid ID, try again!: ");
                }
                if (accuWeatherLocationId.ToUpper().Equals("Q")) break;  
                string result = AccuWeatherStationID.GetCurrentConditions(accuWeatherLocationId);
                Console.WriteLine("WEATHER REPORT FOR {0}...",accuWeatherLocationId);
                DeserializePointDataJSON(result);
            }
        }

        static void QueryHourlyForecastPointData()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("ACCUWEATHER HOURLY FORECAST QUERY");
                Console.WriteLine("==================================");
                Console.Write("Enter AccuWeather Location ID ('Q' returns to main menu): ");
                string accuWeatherLocationId = null;
                while (String.IsNullOrWhiteSpace(accuWeatherLocationId))
                {
                    accuWeatherLocationId = Console.ReadLine();
                    
                    if (String.IsNullOrWhiteSpace(accuWeatherLocationId)) Console.Write("Invalid ID, try again!: ");
                }
                if (accuWeatherLocationId.ToUpper().Equals("Q")) break;  
                string result = AccuWeatherStationID.GetHourlyForecastData(accuWeatherLocationId, 72);
                Console.WriteLine("WEATHER REPORT FOR {0}...", accuWeatherLocationId);
                DeserializeHourlyPointForecastDataJSON(result);
            }
        }

        static void QueryDaypartForecastPointData()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("ACCUWEATHER DAY PART FORECAST QUERY");
                Console.WriteLine("===================================");
                Console.Write("Enter AccuWeather Location ID ('Q' returns to main menu): ");
                string accuWeatherLocationId = null;
                while (String.IsNullOrWhiteSpace(accuWeatherLocationId))
                {
                    accuWeatherLocationId = Console.ReadLine();
                    if (String.IsNullOrWhiteSpace(accuWeatherLocationId)) Console.Write("Invalid ID, try again!: ");
                    
                }
                if (accuWeatherLocationId.ToUpper().Equals("Q")) break;
                string result = AccuWeatherStationID.GetDaypartForecastData(accuWeatherLocationId, 5);
                Console.WriteLine("WEATHER REPORT FOR {0}...", accuWeatherLocationId);
                DeserializeDaypartPointForecastDataJSON(result);
            }
        }
        
        static void QueryLocations()
        {
            // search for city by name
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("ACCUWEATHER LOCATION SEARCH TOOL");
                Console.WriteLine("================================");
                Console.Write("Enter City or Location ('Q' returns to main menu): ");
                string location = null;
                while (String.IsNullOrWhiteSpace(location))
                {
                    location = Console.ReadLine();

                    if (String.IsNullOrWhiteSpace(location)) Console.Write("Invalid location name, try again!: ");
                }
                if (location.ToUpper().Equals("Q")) break;
                string result = AccuWeatherStationID.GetLocation(location, "", true);
                Console.WriteLine("RESULT FROM QUERY IS...");
                DeserializeLocationJSON(result);
            }
        }

        static void PrintErrorMessage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("There was an error processing your data.  Please try again!");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

        }

        static void DeserializePointDataJSON(string rawJsonText)
        {
            if (String.IsNullOrWhiteSpace(rawJsonText))
            {
                PrintErrorMessage();
                return;
            }
            var myJSON = JsonConvert.DeserializeObject<List<LocationPointCurrentData>>(rawJsonText);
            foreach (var pointData in myJSON)
            {
                
                var tempF = pointData.Temperature.Imperial.Value;
                var tempC = pointData.Temperature.Metric.Value;
                var windDir = pointData.Wind.Direction.English;
                var windSpeed = pointData.Wind.Speed.Imperial.Value;
                var feelsLikeF = pointData.RealFeelTemperature.Imperial.Value;
                var feelsLikeC = pointData.RealFeelTemperature.Metric.Value;

                Console.WriteLine("TEMP: {0}°F / {1}°C", tempF, tempC);
                Console.WriteLine("ACCUWEATHER REAL FEEL: {0}°F / {1}°C", feelsLikeF, feelsLikeC);
                Console.WriteLine("WIND: {0} at {1} mph", windDir, windSpeed);

            }
        }

        static void DeserializeHourlyPointForecastDataJSON(string rawJsonText)
        {
            if (String.IsNullOrWhiteSpace(rawJsonText))
            {
                PrintErrorMessage();
                return;
            }
            var myJSON = JsonConvert.DeserializeObject<List<LocationPointHourlyForecastData>>(rawJsonText);
            foreach (var pointData in myJSON)
            {
                var timeStamp = pointData.DateTime;
                var tempF = pointData.Temperature.Value; var tempUnit = pointData.Temperature.Unit;
                var feelsLikeF = pointData.RealFeelTemperature.Value; var feelsLikeUnit = pointData.RealFeelTemperature.Unit;
                var iconPhrase = pointData.IconPhrase;

                //var tempC = pointData.Temperature.Metric.Value;
                //var chancePrecip = pointData.

                Console.WriteLine("TIME {0}: Weather: {1}  Temp: {2}°{3}  RealFeel: {4}°{5}", 
                    timeStamp, iconPhrase, tempF, tempUnit, feelsLikeF, feelsLikeUnit);
                //Console.WriteLine("ACCUWEATHER REAL FEEL: {0}°F / {1}°C", feelsLikeF, feelsLikeC);
                //Console.WriteLine("WIND: {0} at {1} mph", windDir, windSpeed);

            }
        }

        static void DeserializeDaypartPointForecastDataJSON(string rawJsonText)
        {
            if (String.IsNullOrWhiteSpace(rawJsonText))
            {
                PrintErrorMessage();
                return;
            }
            var myJSON = JsonConvert.DeserializeObject<DaypartRoot>(rawJsonText);
            string headline = myJSON.Headline.Text;
            var forecasts = myJSON.DailyForecasts;
            foreach (var pointData in forecasts)
            {
                var timeStamp = pointData.Date;
                var maxtempF = pointData.Temperature.Maximum.Value; var tempUnit = pointData.Temperature.Maximum.Unit;
                var feelsLikeF = pointData.RealFeelTemperature.Maximum.Value; var feelsLikeUnit = pointData.RealFeelTemperature.Maximum.Unit;
                var iconPhrase = pointData.Day.LongPhrase;

                //var tempC = pointData.Temperature.Metric.Value;
                //var chancePrecip = pointData.

                Console.WriteLine("TIME {0}: Weather: {1}  Temp: {2}°{3}  RealFeel: {4}°{5}",
                    timeStamp, iconPhrase, maxtempF, tempUnit, feelsLikeF, feelsLikeUnit);
                //Console.WriteLine("ACCUWEATHER REAL FEEL: {0}°F / {1}°C", feelsLikeF, feelsLikeC);
                //Console.WriteLine("WIND: {0} at {1} mph", windDir, windSpeed);

            }
        }

        static void DeserializeLocationJSON(string rawJsonText)
        {
            if (String.IsNullOrWhiteSpace(rawJsonText))
            {
                PrintErrorMessage();
                return;
            }
            var myJSON = JsonConvert.DeserializeObject<List<Location>>(rawJsonText);
            foreach (var location in myJSON)
            {
                string locName = location.EnglishName;
                    string locCountry = location.Country.EnglishName;
                    string locRegion = location.AdministrativeArea.LocalizedName;
                    string locSubRegion = null;
                    if (location.SupplementalAdminAreas.Length > 0) locSubRegion = location.SupplementalAdminAreas[0].EnglishName; // may throw an exception!
                    string locSearchId = location.Key;
                    float locLongitude = location.GeoPosition.Longitude;
                    float locLatitude = location.GeoPosition.Latitude;
                    double locHeight = location.GeoPosition.Elevation.Imperial.Value;
                    string locVizId = location.LocalizedName;
                int locRank = location.Rank;
                string locType = location.Type;
                var locPop = location.Details.Population;
                var locLatLon = String.Format("{0},{1} {2}", locLatitude.ToString(), locLongitude.ToString(),locHeight);
                
                     
                    //Console.WriteLine("{0}, {2} is in Country: {1} \n\tAccuWeatherID: {4}\tGeoLocation: {6}°,{5}° Elevation: {7} ft.\n\tCounty={3}\tRank={9]\tType={10}",
                    //                 locName,
                    //                 locCountry,
                    //                 locRegion,
                    //                 locSubRegion,
                    //                 locSearchId,
                    //                 locLongitude,
                    //                 locLatitude,
                    //                 locHeight,
                    //                 locVizId,
                    //                 locRank.ToString(),
                    //                 locType);
                Console.WriteLine("LOCATION\tCOUNTRY\tREGION\tSUBREGION\tRANK\tTYPE\tPOP.");
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}",
                    locName, locCountry,locRegion,locSubRegion,locRank,locType,locPop,locSearchId,locLatLon);
            }
         


        }

        static void LoadLocationList()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Preferences.Locations));
            var locations = new Preferences.Locations();
            using (StreamReader reader = new StreamReader("Locations.xml"))
            {
                locations = (Preferences.Locations)serializer.Deserialize(reader);

            }

            var locationList = locations.location;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("LOCATION LIST XML LOAD TOOL");
            Console.WriteLine("===========================");
            
            
            Console.WriteLine("CITY\tREGION\tSUBREGION\tID\tELEVATION");
            foreach (var location in locationList)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}",
                    location.name, location.region, location.sub_region,location.id,location.elevation);


            }
            Console.WriteLine();
            return;

        }

        static void SaveLocationList()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Preferences.Locations));
            var locations = new Preferences.Locations();
            using (StreamReader reader = new StreamReader("Locations.xml"))
            {
                locations = (Preferences.Locations)serializer.Deserialize(reader);

            }

            var locationList = locations.location;

            // change some data so it looks like we did something
            // let's Malonchefy the names! :)

            foreach (var location in locationList)
            {
                location.name = "Mal" + location.name;
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("LOCATION LIST XML SAVE TOOL");
            Console.WriteLine("===========================");

            // write list back out to a new XML file
            
            // we can reuse the XmlSerializer Object created at the top
            using (StreamWriter sw = new StreamWriter("Locations2.xml"))
            {
                serializer.Serialize(sw, locations);
            }
            
            return;

        }

    }

    public class AccuWeatherStationID
    {
        // constants that will eventually go into XML Preferences file
        private const string ACCU_API_KEY = "5d487e4b2da5453a8ca390ab3d3f26fc";
        private const string baseUri = "http://api.accuweather.com";
        private const string restSearchPath = "locations/v1";
        private const string restCurrentConditionsPath = "currentconditions/v1";
        private const string restForecastPath = "forecasts/v1";
        private const string restHourlyForecastPath = "hourly";
        private const string restDaypartForecastPath = "daily";
        private const string restSearchParam = "search?";
        private const string restSearchVar = "q=";
        private const string restApiKeyVar = "apikey=";
        private const string detailsKey = "details=";

        private static Preferences.Preferences _preferences;
        
        // load preferences
        private static bool LoadPreferences()
        {
            
            // check if Preferences is instantiated.  If not read preferences
            if (_preferences == null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Preferences.Preferences));
                _preferences = new Preferences.Preferences();
                try
                {
                    using (StreamReader reader = new StreamReader("Preferences.xml"))
                    {
                        _preferences = (Preferences.Preferences)serializer.Deserialize(reader);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Read error: {0}",e.Message);
                    _preferences = null;
                    return false;
                }

            }

            return true;

        }


        // search location by name; name, state; name, country; zip or postal code
        public static string GetLocation(string cityName, string countryCode, bool details)
        {
            
            // check if preferences are loaded
            // if not, load them
            // if can't load, exit method
            bool success = LoadPreferences();
            if (success == false) return null;
            
            // uri should be in format
            // http://api.accuweather.com/locations/v1/US/search?q=Boston&apikey=5d487e4b2da5453a8ca390ab3d3f26fc&details=true

            string result = null;

            string uri = _preferences.domain_uri + "/" +
                         _preferences.location_request_path + "/" +
                         _preferences.api_version + "/" +
                         _preferences.location_search_request_path + "?" +
                         _preferences.search_query_param + "=" +
                         cityName + "&" +
                         _preferences.api_query_param + "=" +
                         _preferences.api_key + "&" +
                         _preferences.details_query_param + "=" +
                         details;
                

            IFetchTextData data = new FetchTextDataHttp();
            result = data.FetchData(uri);

            return result;
        }

        
        public static string GetCurrentConditions(string accuWeatherLocationId)
        {

            // check if preferences are loaded
            // if not, load them
            // if can't load, exit method
            bool success = LoadPreferences();
            if (success == false) return null;
            
            
            //uri should be in format
            // http://api.accuweather.com/currentconditions/v1/348735.json?apikey=5d487e4b2da5453a8ca390ab3d3f26fc&details=true
            // where 348735 is the AccuWeather location ID

            string result = null;
            bool details = true; // should pass this as a parameter

            string uri = _preferences.domain_uri + "/" +
                         _preferences.current_obs_request_path + "/" +
                         _preferences.api_version + "/" +
                         accuWeatherLocationId + ".json" + "?" +
                         _preferences.api_query_param + "=" +
                         _preferences.api_key + "&" +
                         _preferences.details_query_param + "=" +
                         details;
            
            
            IFetchTextData data = new FetchTextDataHttp();
            result = data.FetchData(uri);

            return result;
        }

        public static string GetHourlyForecastData(string accuWeatherLocationId, int forecastLength)
        {

            // check if preferences are loaded
            // if not, load them
            // if can't load, exit method
            bool success = LoadPreferences();
            if (success == false) return null;

            bool details = true; // should move to method param
            
            //uri should be in format
            // http://api.accuweather.com/forecasts/v1/hourly/120hour/348735.json?apikey=5d487e4b2da5453a8ca390ab3d3f26fc&details=true
            // where 348735 is the AccuWeather location ID,
            // 120hr is the number of hours to forecast (1,12,24,72,120,240)

            int[] validForecastTime = {1,12,24,72,120,240};

            // compare entered forecast length against valid ones. Choose forecast length closest to one entered without going over
            for (int i=0; i<validForecastTime.Length; i++)
            {
                if (forecastLength == validForecastTime[i]) break; // there's a match.
                if (forecastLength < validForecastTime[i])
                {
                    if (i == 0) forecastLength = validForecastTime[0];
                    if (i > 0) forecastLength = validForecastTime[i - 1]; // pick the value directly below the one entered
                    break;
                }
            }

            string result = null;
            string uri = _preferences.domain_uri + "/" +
                         _preferences.forecast_request_path + "/" +
                         _preferences.api_version + "/" +
                         _preferences.hourly_forecast_request_path + "/" +
                         forecastLength + "hour" + "/" +
                         accuWeatherLocationId + ".json" + "?" +
                         _preferences.api_query_param + "=" +
                         _preferences.api_key + "&" +
                         _preferences.details_query_param + "=" +
                         details;
                
                
                //uri = baseUri + "/" + restForecastPath + "/" + restHourlyForecastPath + "/" +
                //forecastLength + "hour" + "/" + accuWeatherLocationId + ".json?" + restApiKeyVar + ACCU_API_KEY +
                //"&" + detailsKey + true;

            IFetchTextData data = new FetchTextDataHttp();
            result = data.FetchData(uri);

            return result;
        }

        public static string GetDaypartForecastData(string accuWeatherLocationId, int forecastLength)
        {

            // check if preferences are loaded
            // if not, load them
            // if can't load, exit method
            bool success = LoadPreferences();
            if (success == false) return null;

            bool details = true; // should move to method param
            
            //uri should be in format
            // http://api.accuweather.com/forecasts/v1/daily/1day/348735.json?apikey=5d487e4b2da5453a8ca390ab3d3f26fc&details=true
            // where 348735 is the AccuWeather location ID,
            // 1day is the number of days to forecast (1,5,10,15,25)

            int[] validForecastTime = { 1, 5, 10, 15, 25 };

            // compare entered forecast length against valid ones. Choose forecast length closest to one entered without going over
            for (int i = 0; i < validForecastTime.Length; i++)
            {
                if (forecastLength == validForecastTime[i]) break; // there's a match.
                if (forecastLength < validForecastTime[i])
                {
                    if (i == 0) forecastLength = validForecastTime[0];
                    if (i > 0) forecastLength = validForecastTime[i - 1]; // pick the value directly below the one entered
                    break;
                }
            }

            string result = null;
            string uri = _preferences.domain_uri + "/" +
                         _preferences.forecast_request_path + "/" +
                         _preferences.api_version + "/" +
                         _preferences.daypart_forecast_request_path + "/" +
                         forecastLength + "day" + "/" +
                         accuWeatherLocationId + ".json" + "?" +
                         _preferences.api_query_param + "=" +
                         _preferences.api_key + "&" +
                         _preferences.details_query_param + "=" +
                         details;
                
                

            IFetchTextData data = new FetchTextDataHttp();
            result = data.FetchData(uri);

            return result;
        }




        // constructor
        public AccuWeatherStationID()
        {
            //this.countryCode = "US";
            

        }

        

    }

    /// <summary>
    /// This class represents a Fetch over HTTP
    /// </summary>
    public class FetchTextDataHttp : IFetchTextData
    {
        // using System.Net;

        /// <summary>
        /// Implements IFetchData interface
        /// </summary>
        /// <param name="uri">URI or URL</param>
        /// <returns>Text returned from fetch</returns>
        public string FetchData(string uri)
        {
            string fetchData = null;

            try
            {
                WebClient client = new WebClient();
                fetchData = client.DownloadString(uri);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Program error: {0}", ex.Message);
            }

            return fetchData;
        }

    }

    /// <summary>
    /// Interface that defines one method used to fetch text data
    /// it can then be implemented via Net Http, Ftp or through disk access, etc.
    /// </summary>
    public interface IFetchTextData
    {
        string FetchData(string uri);

    }
}
