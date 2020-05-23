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

namespace TaikoChecks.checks.compose
{
    [Check]
    public class CheckNinja : BeatmapCheck
    {
        private const double NINJA_RATE = 450.0;

        public override CheckMetadata GetMetadata() => new BeatmapCheckMetadata()
        {
            Modes = new Beatmap.Mode[]
            {
                Beatmap.Mode.Taiko
            },
            Category = "Compose",
            Message = "Ninja objects.",
            Author = "Alchyr",

            Documentation = new Dictionary<string, string>()
            {
                {
                    "Purpose",
                    @"
                    Ensuring that objects aren't completely unreadable due to extreme SV increases."
                },
                {
                    "Reasoning",
                    @"
                    Ninja objects are generally unrankable if not used in such a way that makes them easily predictable."
                }
            }
        };

        public override Dictionary<string, IssueTemplate> GetTemplates()
        {
            return new Dictionary<string, IssueTemplate>()
            {
                { "Ninja Object",
                    new IssueTemplate(Issue.Level.Warning,
                        "{0} Ninja object, with BPM equivalent rate of {1}",
                        "timestamp - ", "rate")
                    .WithCause(
                        "A circle has unreasonably high sv.") },
                { "Ninja Objects",
                    new IssueTemplate(Issue.Level.Warning,
                        "{0} Ninja objects, with BPM equivalent rate of {1}",
                        "timestamp - ", "rate")
                    .WithCause(
                        "Multiple circles have unreasonably high sv.") }
            };
        }

        public override IEnumerable<Issue> GetIssues(Beatmap aBeatmap)
        {
            //Combine consective ninja objects into single issues

            float baseSv = aBeatmap.difficultySettings.sliderMultiplier / 1.4f; //Relative to a standard 1.4 base sv.
            float baseRate = 0;
            float rate = 0;
            int timingIndex = -1;

            List<HitObject> ninjaObjects = new List<HitObject>(); //Ninja object - add to list. Non-ninja object: Yield new issue with all current objects in list, and clear list. Also report list after going through all objects.

            if (aBeatmap.timingLines.Count != 0)
            {
                foreach (HitObject hitObject in aBeatmap.hitObjects)
                {
                    while (timingIndex < aBeatmap.timingLines.Count - 1 && aBeatmap.timingLines[timingIndex + 1].offset <= hitObject.time)
                    {
                        float oldRate = rate;
                        ++timingIndex;
                        if (aBeatmap.timingLines[timingIndex].uninherited)
                        {
                            //get bpm
                            rate = baseRate = baseSv * 60000.0f / float.Parse(aBeatmap.timingLines[timingIndex].code.Split(',')[1], CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            rate = baseRate * aBeatmap.timingLines[timingIndex].svMult;
                        }

                        if (rate != oldRate)
                        {
                            if (ninjaObjects.Count > 1)
                            {
                                yield return new Issue(GetTemplate("Ninja Objects"), aBeatmap,
                                    Timestamp.Get(ninjaObjects.ToArray()), oldRate);
                            }
                            else if (ninjaObjects.Count == 1)
                            {
                                yield return new Issue(GetTemplate("Ninja Object"), aBeatmap,
                                    Timestamp.Get(ninjaObjects.First()), oldRate);
                            }
                            ninjaObjects.Clear();
                        }
                    }

                    if (rate > NINJA_RATE && rate >= baseRate * 2.5f)
                    {
                        if (hitObject.type.HasFlag(HitObject.Type.Circle))
                            ninjaObjects.Add(hitObject);
                    }
                    else
                    {
                        if (ninjaObjects.Count > 1)
                        {
                            yield return new Issue(GetTemplate("Ninja Objects"), aBeatmap,
                                Timestamp.Get(ninjaObjects.ToArray()), rate);
                        }
                        else if (ninjaObjects.Count == 1)
                        {
                            yield return new Issue(GetTemplate("Ninja Object"), aBeatmap,
                                Timestamp.Get(ninjaObjects.First()), rate);
                        }
                        ninjaObjects.Clear();
                    }
                }

                if (ninjaObjects.Count > 1)
                {
                    yield return new Issue(GetTemplate("Ninja Objects"), aBeatmap,
                        Timestamp.Get(ninjaObjects.ToArray()), rate);
                }
                else if (ninjaObjects.Count == 1)
                {
                    yield return new Issue(GetTemplate("Ninja Object"), aBeatmap,
                        Timestamp.Get(ninjaObjects.First()), rate);
                }
            }
        }
    }
}
