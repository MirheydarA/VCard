using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace VCard
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("How many VCard do you create?");

                int count = Convert.ToInt32(Console.ReadLine());   

                if (count < 0)
                {
                    Console.WriteLine("Number is not corrcet format!");
                    return;
                }

                string url = $"https://randomuser.me/api/?results={count}";
                using HttpClient client = new HttpClient();
                using HttpResponseMessage responseMessage = await client.GetAsync(url);
                responseMessage.EnsureSuccessStatusCode();

                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                var userResponse = JsonConvert.DeserializeObject<User.Rootobject>(responseBody);

                List<VCard> vcards = new List<VCard>();
                int id = 1;

                foreach (var user in userResponse.results)
                {
                    VCard vcard = new VCard
                    {
                        Id = id++,
                        Firstname = user.name.first,
                        Surname = user.name.last,
                        Email = user.email,
                        Phone = user.phone,
                        Country = user.location.country,
                        City = user.location.city
                    };

                    vcards.Add(vcard);
                }

                string folderPath = @"C:\Users\ASUS\VCards";

                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }

                foreach (var vcard in vcards)
                {
                    string vcardString = ConvertToVCardString(vcard);
                    string fileName = $"{vcard.Firstname[0]}{vcard.Surname[0]}_{Guid.NewGuid()}.vcf";
                    string filePath = System.IO.Path.Combine(folderPath, fileName);
                    System.IO.File.WriteAllText(filePath, vcardString);
                    Console.WriteLine($"vCard {vcard.Id} created and  saved to {filePath} folder.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured: {e.Message}");
            }
        }


        public static string ConvertToVCardString(VCard vcard)
        {
            return $"BEGIN:VCARD\n" +
                   $"VERSION:3.0\n" +
                   $"N:{vcard.Surname}, {vcard.Firstname}\n" +
                   $"EMAIL:{vcard.Email}\n" +
                   $"TEL:{vcard.Phone}\n" +
                   $"ADR:;;{vcard.City}, {vcard.Country}\n" +
                   $"END:VCARD\n";
        }
    }
}
