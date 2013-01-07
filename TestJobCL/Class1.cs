using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Xunit;
using System.Linq;
using FluentAssertions;

namespace TestJobCL
{
    public class TestClass
    {
        private readonly string firstText;
        private string secondText ;

        public TestClass(string url)
        {
            firstText = new WebClient().DownloadString(url);
        }

        public string GetFirstLink()
        {
            var rareChars = firstText.GroupBy(a => a)
                       .Where(b => b.Count() < 10)
                       .Select(c => c.Key)
                       .ToList();
            return string.Join("", firstText.Where(rareChars.Contains));
        }
        public string GetMessage()
        {
            secondText = new WebClient().DownloadString(GetFirstLink());

            var pairs = new SortedDictionary<int, char>();

            const string keys = @"\d+\D";
            const string nums = @"\d+";

            for (var match = new Regex(keys).Match(secondText); match.Success; match = match.NextMatch())
            {
                var value = match.Value;
                var number = new Regex(nums).Match(value).Value;
                pairs.Add(int.Parse(number), value.Last());
            }

            return string.Join("", pairs.Values);
        }
    }

    public class TestClassTests
    {
        private const string url = "https://raw.github.com/gist/4b6e920fbb7cc49fed48/ef591fd9774339de82041665708f9add9ccd5a11/gistfile1.txt";
        readonly TestClass test = new TestClass(url);

        [Fact]
        public void show_result()
        {
            var result = test.GetFirstLink();
            Console.Out.WriteLine(result);
            result.Length.Should().Be(103);
        }

        [Fact]
        public void should_show_message()
        {
            var result = test.GetMessage();
            result.Should().Be("There is nothing I love more than watching someone being ejected from a spaceship");
        }
    }
}
