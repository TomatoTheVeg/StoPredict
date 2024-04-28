using System.Text;

DateTime begin = new DateTime(2018, 1, 1);



//Console.WriteLine(begin.ToString("MM/dd/yyyy"));

Dictionary<string, string> body = new Dictionary<string, string>(){
    {"job", "WeatherArchive" },
    {"term", "hourly" },
    {"lat", "40.585426" },
    {"lon", "28.990322" },
    {"date", begin.ToString("MM/dd/yyyy") },
    {"parameters", "temperature|precipitation|cloudcover|pressure|wind_speed|dewpoint" }
};


HttpClient client = new HttpClient();

StreamWriter stream = new StreamWriter(new FileStream("weather2018.csv", FileMode.Create));
//Console.WriteLine("Time,Temperature(°C),Precipitation(mm),Cloudcover(%),Pressure(hPa),Wind Speed(km / h),Dewpoint(°C)");
stream.WriteLine("Time,Temperature(°C),Precipitation(mm),Cloudcover(%),Pressure(hPa),Wind Speed(km / h),Dewpoint(°C)");


for (int i =0;i<365; ++i)
{
     var responce = await client.PostAsync("https://oneweather.org/api/", new FormUrlEncodedContent(body));
    //responce.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
    string answer = await responce.Content.ReadAsStringAsync();
 //   Console.WriteLine(answer.Substring(50, 130));
    
    int startLink = answer.IndexOf("https");
    int endLink = answer.IndexOf(".csv");
    string csvURL = answer.Substring(startLink, endLink - startLink + 4);
    var responceCSV = await client.GetAsync(csvURL);
    string answerCSV = await responceCSV.Content.ReadAsStringAsync();
    begin = begin.AddDays(1);
    body.Remove("date");
    body.Add("date", begin.ToString("MM/dd/yyyy"));
    //Console.WriteLine(begin.ToString("dd/MM/yyyy"));
    //Console.WriteLine(csvURL);
   
    answerCSV = answerCSV.Substring(answerCSV.IndexOf("\n") + 1);
    string[] strings = answerCSV.Split('\n');
    foreach (string s in strings)
    {
        if (s != "") { 
            string[] values = s.Split(',');
            string[] date = values[0].Split(new char[] { '-', ' ', ':'});
            for (int t = 0; t < 6; ++t)
            {
                var correctDateBuilder = new StringBuilder();

                string correctDate = correctDateBuilder.Append(date[1]).Append(" ").Append(date[2]).Append(" ").Append(date[0]).Append(" ").Append(date[3]).Append(":").Append(t).Append(0).ToString();
                //Console.WriteLine(correctDate);
                stream.Write(correctDate);
                for (int j = 1; j < values.Length; ++j)
                {
                    //Console.Write(',');
                    stream.Write(',');
                    //Console.Write(values[j]);
                    stream.Write(values[j]);
                }
                //Console.WriteLine();
                stream.Write('\n');
            }
        }
    
    }
    //Console.WriteLine();
    //Console.Write(answerCSV.Substring(answerCSV.IndexOf("\n")+1));
    //Console.WriteLine(answerCSV);
    //Thread.Sleep(200);
}
stream.Close();
