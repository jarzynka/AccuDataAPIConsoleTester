using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using AccuWeatherData; // reference to our Data DLL


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
                string result = AccuWeatherStationID.GetHourlyForecastData(accuWeatherLocationId, "72hour");
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
                string result = AccuWeatherStationID.GetDaypartForecastData(accuWeatherLocationId, "5day");
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
                    Console.WriteLine("{0}, {2} is in Country: {1} \n\tAccuWeatherID: {4}\tGeoLocation: {6}°,{5}° Elevation: {7} ft.",
                                     locName,
                                     locCountry,
                                     locRegion,
                                     locSubRegion,
                                     locSearchId,
                                     locLongitude,
                                     locLatitude,
                                     locHeight,
                                     locVizId);
                    
            }
         


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
        

        // search location by name; name, state; name, country; zip or postal code
        public static string GetLocation(string cityName, string countryCode, bool details)
        {
            // uri should be in format
            // http://api.accuweather.com/locations/v1/US/search?q=Boston&apikey=5d487e4b2da5453a8ca390ab3d3f26fc&details=true

            string result = null;

            string uri = baseUri + "/" + restSearchPath + "/" + countryCode + "/" + restSearchParam + restSearchVar +
                cityName + "&" + restApiKeyVar + ACCU_API_KEY + "&" + detailsKey + details;

            IFetchTextData data = new FetchTextDataHttp();
            result = data.FetchData(uri);

            return result;
        }

        
        public static string GetCurrentConditions(string accuWeatherLocationId)
        {
            //uri should be in format
            // http://api.accuweather.com/currentconditions/v1/348735.json?apikey=5d487e4b2da5453a8ca390ab3d3f26fc&details=true
            // where 348735 is the AccuWeather location ID

            string result = null;
            string uri = baseUri + "/" + restCurrentConditionsPath + "/" + accuWeatherLocationId + ".json?" + restApiKeyVar + ACCU_API_KEY +
                "&" + detailsKey + true;

            IFetchTextData data = new FetchTextDataHttp();
            result = data.FetchData(uri);

            return result;
        }

        public static string GetHourlyForecastData(string accuWeatherLocationId, string forecastLength)
        {
            //uri should be in format
            // http://api.accuweather.com/forecasts/v1/hourly/120hour/348735.json?apikey=5d487e4b2da5453a8ca390ab3d3f26fc&details=true
            // where 348735 is the AccuWeather location ID,
            // 120hr is the number of hours to forecast (1,12,24,72,120,240)

            string result = null;
            string uri = baseUri + "/" + restForecastPath + "/" + restHourlyForecastPath + "/" +
                forecastLength + "/" + accuWeatherLocationId + ".json?" + restApiKeyVar + ACCU_API_KEY +
                "&" + detailsKey + true;

            IFetchTextData data = new FetchTextDataHttp();
            result = data.FetchData(uri);

            return result;
        }

        public static string GetDaypartForecastData(string accuWeatherLocationId, string forecastLength)
        {
            //uri should be in format
            // http://api.accuweather.com/forecasts/v1/daily/1day/348735.json?apikey=5d487e4b2da5453a8ca390ab3d3f26fc&details=true
            // where 348735 is the AccuWeather location ID,
            // 1day is the number of days to forecast (1,5,10,15,25)

            string result = null;
            string uri = baseUri + "/" + restForecastPath + "/" + restDaypartForecastPath + "/" +
                forecastLength + "/" + accuWeatherLocationId + ".json?" + restApiKeyVar + ACCU_API_KEY +
                "&" + detailsKey + true;

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
