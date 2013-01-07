using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Xunit;
using System.Linq;
using FluentAssertions;

namespace TestJobCL
{
    public class TestCl
    {
        public string GetFirstLink(string text)
        {
            var rareChars = text.GroupBy(a => a)
                       .Where(b => b.Count() < 10)
                       .Select(c => c.Key)
                       .ToList();
            return string.Join("", text.Where(rareChars.Contains));
        }

        public string GetMessage(string text)
        {
            const string pairsPattern = @"\d+\D";

            var matchCollection = Regex.Matches(text, pairsPattern);

            var pairs = (from Match match in matchCollection select match.Value)
                .Select(item => new
                                    {
                                        key = int.Parse(item.Substring(0, item.Length - 1)),
                                        value = item.Last()
                                    })
                .ToList();

            return string.Join("", pairs.OrderBy(p => p.key).Select(p => p.value).ToList());
        }
    }

    public class TestClTests
    {
        private const string url = "https://raw.github.com/gist/4b6e920fbb7cc49fed48/ef591fd9774339de82041665708f9add9ccd5a11/gistfile1.txt";
        private readonly TestCl test = new TestCl();
        private readonly WebClient webClient = new WebClient();

        [Fact]
        public void show_result()
        {
            var result = test.GetFirstLink(webClient.DownloadString(url));
            Console.Out.WriteLine(result);
            result.Length.Should().Be(103);
        }

        [Fact]
        public void should_show_message()
        {
            var link = test.GetFirstLink(webClient.DownloadString(url));
            var result = test.GetMessage(webClient.DownloadString(link));

            result.Should().Be("There is nothing I love more than watching someone being ejected from a spaceship");
        }
    }
}
