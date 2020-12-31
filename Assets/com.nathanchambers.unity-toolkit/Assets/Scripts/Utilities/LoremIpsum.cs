using System;
using System.Text;

public static class LoremIpsum {

    private static Random _random = new Random();

    public static readonly string[] terms = new string[] {
        "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
        "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
        "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"
    };

    public static string Generate(int paragraphs, int sentences, int words) {
        StringBuilder result = new StringBuilder();
        for(int p = 0; p < paragraphs; p++) {
            for(int s = 0; s < sentences; s++) {
                for(int w = 0; w < words; w++) {
                    if (w > 0) {
                        result.Append(" ");
                    }
                    result.Append(terms[_random.Next(terms.Length)]);
                }
                result.Append(". ");
            }
        }
        return result.ToString();
    }
}
