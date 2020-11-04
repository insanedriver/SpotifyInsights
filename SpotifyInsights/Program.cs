using Newtonsoft.Json;
using SpotifyInsights.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyInsights
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting application");

            Execute().Wait();

            Console.WriteLine("Ending application");
            Console.ReadKey();
        }

        private static async Task Execute()
        {
            try
            {
                string json = await ReadConfig();
                Input input = JsonConvert.DeserializeObject<Input>(json);

                Console.WriteLine("Enter OAuth Token: ");
                input.Token = Console.ReadLine();

                await CallSpotifyAPI(input);
                PrintInfo(input.Artists);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task<string> ReadConfig()
        {
            using (StreamReader r = new StreamReader(Directory.GetCurrentDirectory() + @"/input.json"))
            {
                return await r.ReadToEndAsync();
            }
        }

        private static async Task CallSpotifyAPI(Input config)
        {
            foreach (var artist in config.Artists)
            {
                Console.WriteLine(string.Format("Getting information about: {0} - {1}", artist.Name, artist.Uri));

                string url = config.Endpoint + "?ids=" + artist.Uri.Replace("spotify:artist:", "");

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    httpRequest.Headers.Add("Accept", "application/json");
                    httpRequest.Headers.Add("Authorization", "Bearer " + config.Token);

                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage response = await client.SendAsync(httpRequest))
                        {
                            string jsonResponse = await response.Content.ReadAsStringAsync();
                            var spotifyResponse = JsonConvert.DeserializeObject<SpotifyArtistResponse>(jsonResponse);

                            if (spotifyResponse.artists != null)
                            {
                                var spotifyArtist = spotifyResponse.artists.FirstOrDefault();

                                if (spotifyArtist != null)
                                {
                                    artist.Name = spotifyArtist.name;
                                    artist.Followers = spotifyArtist.followers.total;
                                    artist.Popularity = spotifyArtist.popularity;
                                    artist.MonthlyListeners = await GetMonthlyListeners(spotifyArtist.external_urls.spotify);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Unable to return information");
                                Console.WriteLine(jsonResponse);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static async Task PrintInfo(List<Artist> artists)
        {
            try
            {
                var artistsOrdered = artists.OrderByDescending(p => p.Popularity).ToList();
                string path = Directory.GetCurrentDirectory() + @"\SpotifyStatistics.csv";

                Console.WriteLine("Generating file at " + path);

                if (File.Exists(path))
                    File.Delete(path);

                using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    await sw.WriteLineAsync("Name, Popularity, MonthlyListeners, Followers");

                    foreach (var artist in artistsOrdered)
                    {
                        await sw.WriteLineAsync(string.Format("{0}, {1}, {2}, {3}",
                                                        artist.Name,
                                                        artist.Popularity,
                                                        artist.MonthlyListeners,
                                                        artist.Followers
                                                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail to generate the file.");
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private static async Task<int> GetMonthlyListeners(string url)
        {
            int monthlyListeners = 0;

            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.SendAsync(httpRequest))
                    {
                        string getResponse = null;

                        if (response.IsSuccessStatusCode)
                        {
                            getResponse = await response.Content.ReadAsStringAsync();

                            string monthly1 = getResponse.Substring(getResponse.IndexOf("monthly_listeners\":"), 35);

                            int positionColon = monthly1.IndexOf(":");
                            int positionComma = monthly1.IndexOf(",");

                            string strMonthlyListeners = monthly1.Substring(positionColon + 1, positionComma - positionColon - 1);
                            return Convert.ToInt32(strMonthlyListeners);
                        }
                        else
                        {
                            Console.WriteLine("Unable to return Artist Page information");
                        }
                    }
                }
            }

            return monthlyListeners;
        }
    }
}
