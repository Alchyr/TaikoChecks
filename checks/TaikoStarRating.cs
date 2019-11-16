using MapsetParser.objects;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using System.Collections.Generic;

namespace TaikoChecks.checks
{
    [Check]
    public class TaikoStarRating : GeneralCheck
    {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata()
        {
            Modes = new Beatmap.Mode[]
            {
                Beatmap.Mode.Taiko
            },

            Category = "Spread",
            Message = "Approximate Taiko Star Rating.",
            Author = "Alchyr",

            Documentation = new Dictionary<string, string>()
            {
                {
                    "Purpose",
                    @"
                    I'm too lazy to properly implement SR as a pull request to MapsetParser."
                },
                {
                    "Note",
                    @"
                    Accuracy decreases as Star Rating increases."
                }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates()
        {
            return new Dictionary<string, IssueTemplate>()
            {
                { "Recalculated Star Rating",
                    new IssueTemplate(Issue.Level.Info,
                        "{0} is approximately {1} stars.",
                        "Difficulty: ", "Star rating: ")
                    .WithCause(
                        "Taiko Difficulty")
                    }
            };
        }

        public override IEnumerable<Issue> GetIssues(BeatmapSet aBeatmapSet)
        {
            foreach (Beatmap beatmap in aBeatmapSet.beatmaps)
            {
                if (beatmap.generalSettings.mode == Beatmap.Mode.Taiko)
                {
                    Common.CalculateTaikoSR(beatmap);

                    yield return new Issue(GetTemplate("Recalculated Star Rating"), beatmap, beatmap.ToString(), beatmap.starRating);
                }
            }
        }
    }
}
