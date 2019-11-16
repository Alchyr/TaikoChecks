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

namespace TaikoChecks.checks.hitsounds
{
    [Check]
    public class CheckDrumHitsounds : GeneralCheck
    {
        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata()
        {
            Modes = new Beatmap.Mode[]
            {
                Beatmap.Mode.Taiko
            },
            Category = "Hit Sounds",
            Message = "Invalid Drum Sampleset Usage.",
            Author = "Alchyr",

            Documentation = new Dictionary<string, string>()
            {
                {
                    "Purpose",
                    @"
                    The Drum sampleset can only be used if custom samples are provided."
                },
                {
                    "Reasoning",
                    @"
                    The Drum sampleset provides no feedback, and thus should not be used unless samples are provided."
                }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates()
        {
            return new Dictionary<string, IssueTemplate>()
            {
                { "Drum Sampleset",
                    new IssueTemplate(Issue.Level.Problem,
                        "{0} uses Drum sampleset without custom samples.",
                        "difficulty")
                    .WithCause(
                        "The Drum sampleset may only be used with custom samples.") }
            };
        }

        public override IEnumerable<Issue> GetIssues(BeatmapSet aBeatmapSet)
        {
            bool hasDrumSample = false;
            foreach (string hsfile in aBeatmapSet.hitSoundFiles)
            {
                if (hsfile.Contains("drum-hit"))
                {
                    hasDrumSample = true;
                    break;
                }
            }

            if (!hasDrumSample)
            {
                foreach (Beatmap b in aBeatmapSet.beatmaps)
                {
                    foreach (TimingLine t in b.timingLines)
                    {
                        if (t.sampleset == Beatmap.Sampleset.Drum)
                        {
                            yield return new Issue(GetTemplate("Drum Sampleset"), b,
                                b.ToString());

                            break;
                        }
                    }
                }
            }
        }
    }
}
