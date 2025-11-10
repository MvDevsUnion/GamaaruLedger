using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace GamaaruLedger;

class Program
{
    static async Task Main()
    {
        //string baseUrl = "https://majlis.gov.mv";
        string listUrl = "https://majlis.gov.mv/en/20-parliament/parliament-works/type/1?year=2025";

        using (var client = new HttpClient())
        {
            var html = await client.GetStringAsync(listUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // select all bill rows/links
            var billLinks = doc.DocumentNode.DescendantsAndSelf()
                .Where(x => x.HasAttributes && x.GetAttributeValue("class", "") == "card-list-link").ToList();

            if (billLinks == null)
            {
                Console.WriteLine("No bills found.");
                return;
            }

            Console.WriteLine($"{billLinks.Count} bills found.");

            foreach (var link in billLinks)
            {
                string billUrl = link.GetAttributeValue("href", "");
                string billName = link
                    .DescendantsAndSelf()
                    .Where(x => x.HasAttributes && x.GetAttributeValue("class", "") == "card-main-title mt-4")
                    .Select(x => x.InnerText.Trim())
                    .FirstOrDefault();

                string billStatus = link
                    .DescendantsAndSelf()
                    .Where(x => x.HasAttributes && x.GetAttributeValue("class", "") == "card-sub-title")
                    .Select(x => x.InnerText.Trim())
                    .FirstOrDefault();

                Console.WriteLine($"Bill: {billName}");
                Console.WriteLine($"Status: {billStatus}");

                if (billStatus == "Passed at Parliament")
                {
                    Console.WriteLine($"Checking bill: {billName}");

                    var billHtml = await client.GetStringAsync(billUrl);
                    var billDoc = new HtmlDocument();
                    billDoc.LoadHtml(billHtml);

                    var allbils = billDoc.DocumentNode.DescendantsAndSelf()
                        .Where(x => x.HasAttributes && x.GetAttributeValue("class", "") == "xlist-link")
                        .ToList();

                    var fileLink = allbils.Where(x => x.InnerText.Contains("Bill passed at parliament")).FirstOrDefault();
                    if (fileLink != null)
                    {
                        //the html pharsing method was giving me a headach so i will just regex this
                        //who gon stop me?
                        string pattern = @"https://majlis\.gov\.mv/storage/action_files/[^\s'""]+";

                        var match = Regex.Match(fileLink.InnerHtml, pattern);

                        if (match.Success)
                        {
                            string fileUrl = match.Value;

                            string ext = Path.GetExtension(fileUrl);

                            string safeBillName = string.Concat(billName.Split(Path.GetInvalidFileNameChars())).Trim();

                            string fileName = safeBillName + "._2025" + ext;

                            Console.WriteLine($"Downloading {fileName} ...");
                            var bytes = await client.GetByteArrayAsync(fileUrl);
                            await File.WriteAllBytesAsync(fileName, bytes);
                            Console.WriteLine($"Saved {fileName}");
                        }
                        else { Console.WriteLine("Error extracting url"); }
                    }
                    else { Console.WriteLine("No passed bill file found."); }
                }
                else { Console.WriteLine("Not passed yet."); }
            }
            Console.WriteLine("Done");
        }
    }
}
