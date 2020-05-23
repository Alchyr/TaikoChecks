using MapsetParser.objects;
using MapsetParser.statics;
using MapsetVerifierFramework;
using MapsetVerifierFramework.objects;
using MapsetVerifierFramework.objects.attributes;
using MapsetVerifierFramework.objects.metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TaikoChecks.checks.settings
{
    [Check]
    public class CheckBaseSV : BeatmapCheck
    {

        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata()
        {
            Modes = new Beatmap.Mode[]
            {
                Beatmap.Mode.Taiko
            },
            Category = "Settings",
            Message = "Base Slider Velocity.",
            Author = "Alchyr",

            Documentation = new Dictionary<string, string>()
            {
                {
                    "Purpose",
                    @"
                    The base slider velocity should be 1.40 throughout all difficulties of a mapset."
                },
                {
                    "Reasoning",
                    @"
                    This is to ensure optimal quantity of notes on the playfield, as well as the optimal distance of separation between different notes."
                }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates()
        {
            return new Dictionary<string, IssueTemplate>()
            {
                { "Base Slider Velocity",
                    new IssueTemplate(Issue.Level.Warning,
                        "{0} has a base slider velocity of {1}.",
                        "timestamp - ", "rate")
                    .WithCause(
                        "The base slider velocity should be 1.40 throughout all difficulties of a mapset, if there is no justified reason to change it.") }
            };
        }

        public override IEnumerable<Issue> GetIssues(Beatmap aBeatmap)
        {
            if (aBeatmap.difficultySettings.sliderMultiplier != 1.4f)
                yield return new Issue(GetTemplate("Base Slider Velocity"), aBeatmap,
                    aBeatmap.ToString(), aBeatmap.difficultySettings.sliderMultiplier);
        }
    }
}
