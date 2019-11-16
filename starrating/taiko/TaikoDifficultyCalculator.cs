using MapsetParser.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaikoChecks.starrating.taiko.Skills;

namespace TaikoChecks.starrating.taiko
{
    class TaikoDifficultyCalculator
    {

        private const int sectionLength = 400;

        private const float star_scaling_factor = 0.04125f;


        /// <summary> Returns star rating. </summary>
        public static float Calculate(Beatmap aBeatmap)
        {
            if (aBeatmap.hitObjects.Count == 0)
                return 0;

            Strain s = new Strain();

            // First object cannot generate strain, so we offset this to account for that.
            double currentSectionEnd = sectionLength;

            s.previousObjects.Add(aBeatmap.hitObjects.First());

            foreach (HitObject hitObject in aBeatmap.hitObjects.Skip(1))
            {
                // Performed on the previous object, hence before Process.
                while (hitObject.time > currentSectionEnd)
                {
                    s.SaveCurrentPeak();
                    s.StartNewSectionFrom(currentSectionEnd);

                    currentSectionEnd += sectionLength;
                }

                s.Process(hitObject);
            }

            return (float) (s.DifficultyValue() * star_scaling_factor);
        }
    }
}
