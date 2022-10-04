namespace VStabiCloudReader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using VStabiParser.Interfaces;

    public class VStabiCloudReader : IVStabiReader
    {
        private readonly string username;
        private readonly string password;

        private Cookie sessionCookie;

        public VStabiCloudReader(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public VStabiCloudReader(Cookie sessionCookie)
        {
            this.sessionCookie = sessionCookie;
        }

        public Cookie SessionCookie()
        {
            return sessionCookie;
        }

        public async Task<string> Main()
        {
            var url = "cloud";
            return await GetData(url);
        }

        public async Task<string> SetupFile(string url)
        {
            return await GetFile(url);
        }

        public async Task<string> DeviceSettings(string sid)
        {
            var uri = new Uri("https://www.vstabi.info/node/1510");

            var cookieContainer = new CookieContainer();
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            httpClientHandler.CookieContainer = cookieContainer;

            var client = new HttpClient(httpClientHandler);

            cookieContainer.Add(uri, sessionCookie);

            List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();

            pairs.Add(new KeyValuePair<string, string>("Id", sid));
            pairs.Add(new KeyValuePair<string, string>("Refresh", "Refresh"));

            var content = new  FormUrlEncodedContent(pairs);

            var response = await client.PostAsync(uri, content);

            var contents = await response.Content.ReadAsStreamAsync();

            return new StreamReader(contents).ReadToEnd();
        }

        public async Task<string> Batteries(string controllerId)
        {
            var url = $"cloud?action=&sort=New+first&start=0&Aid={controllerId}&action=battlist";

            return await GetData(url);
        }

        public async Task<string> Battery(string id, string sid)
        {
            var url = $"cloud?action=edit_batt&start=0&Aid=All%20VBC-t&Sid={sid}&sort=New%20first&name={id}&list=batttlist";

            return await GetData(url);
        }

        public async Task<string> Devices()
        {
            var url = "devices";
            return await GetData(url);
        }

        public async Task<string> Flights(string controllerId, uint page = 0)
        {
            var url = $"cloud?action=flightlist&start=&start={page * 30}&model=All+models&Aid=All+VBC-t&sort=New+first&ftime=All+logs";

            return await GetData(url);
        }

        public async Task<string> Screenshots(string controllerId, uint page = 0)
        {
            var url = $"cloud?action=shotlist&start=&start={page * 20}&Sid=" + controllerId;

            return await GetData(url);
        }

        public async Task<string> FlightDetail(string sid, int flightNo)
        {
            var url = $"cloud?action=edit_flight&start=0&ftime=All logs&Aid=All VBC-t&Sid={sid}&sort=New first&model=All models&vbar=All VBar&flightno={flightNo}";

            return await GetData(url);
        }

        public async Task<string> Models(string controllerId)
        {
            var url = $"cloud?action=&sort=New+first&start=0&type=&Aid={controllerId}&ftime=All+logs&action=modellist";

            return await GetData(url);
        }

        public async Task<string> Setups()
        {
            var url = "cloud?action=&sort=New+first&start=0&Aid=All+VBC-t&action=setuplist";

            return await GetData(url);
        }

        public async Task<string> GetData(string url)
        {
            var uri = new Uri($"https://www.vstabi.info/en/{url}");

            var cookieContainer = new CookieContainer();
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            httpClientHandler.CookieContainer = cookieContainer;
            var client = new HttpClient(httpClientHandler);

            if (sessionCookie != null)
            {
                cookieContainer.Add(uri, sessionCookie);
            }
            else
            {
                var values = new Dictionary<string, string>
                {
                    {"form_id", "user_login"},
                    {"form_build_id", "form-74216942fdd7357e11c55504d6b90b37"},
                    {"name", username},
                    {"pass", password}
                };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("https://www.vstabi.info/en/user/login", content);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Login Failed");

                var body = await response.Content.ReadAsStringAsync();

                if (body.Contains("unrecognized username or password"))
                    throw new Exception("Login Failed");

                if (cookieContainer.Count > 0)
                {
                    sessionCookie = cookieContainer.GetCookies(uri)[0];
                }
            }

            uri = new Uri($"https://www.vstabi.info/en/{url}");

            var responses = await client.GetAsync(uri);

            var contents = await responses.Content.ReadAsStreamAsync();

            return new StreamReader(contents).ReadToEnd();
        }

        public async Task<string> GetFile(string url)
        {
            var uri = new Uri($"https://www.vstabi.info/en/{url}");

            var cookieContainer = new CookieContainer();
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            httpClientHandler.CookieContainer = cookieContainer;
            var client = new HttpClient(httpClientHandler);

            if (sessionCookie != null)
            {
                cookieContainer.Add(uri, sessionCookie);
            }
            else
            {
                var values = new Dictionary<string, string>
                {
                    {"form_id", "user_login"},
                    {"form_build_id", "form-74216942fdd7357e11c55504d6b90b37"},
                    {"name", username},
                    {"pass", password}
                };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("https://www.vstabi.info/en/user/login", content);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Login Failed");

                var body = await response.Content.ReadAsStringAsync();

                if (body.Contains("unrecognized username or password"))
                    throw new Exception("Login Failed");

                if (cookieContainer.Count > 0)
                {
                    sessionCookie = cookieContainer.GetCookies(uri)[0];
                }
            }

            uri = new Uri($"{url}");

            var responses = await client.GetAsync(uri);

            var contents = await responses.Content.ReadAsStreamAsync();

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                contents.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            return Convert.ToBase64String(bytes);
            //return new MemoryStream(Encoding.UTF8.GetBytes(base64));
        }
    }
}