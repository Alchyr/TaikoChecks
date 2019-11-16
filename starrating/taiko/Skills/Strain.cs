using MapsetParser.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using static MapsetParser.objects.HitObject;

namespace TaikoChecks.starrating.taiko.Skills
{
    class Strain : MapsetParser.starrating.standard.Skill
    {

        private const double rhythm_change_base_threshold = 0.2;
        private const double rhythm_change_base = 2.0;

        protected override double SkillMultiplier => 1;
        protected override double StrainDecay => 0.3;

        private ColourSwitch lastColourSwitch = ColourSwitch.None;

        private int sameColourCount = 1;

        private Dictionary<HitObject, double> deltaTimes = new Dictionary<HitObject, double>();

        protected override double StrainValueOf(HitObject current)
        {
            double addition = 1;

            deltaTimes.Add(current, current.time - lastObject().time);

            // We get an extra addition if we are not a slider or spinner
            if (/*isHit(lastObject()) && isHit(current) && */getDeltaTime(current) < 1000)
            {
                if (hasColourChange(lastObject(), current))
                    addition += 0.75;

                if (hasRhythmChange(lastObject(), current))
                    addition += 1;
            }
            else
            {
                lastColourSwitch = ColourSwitch.None;
                sameColourCount = 1;
            }

            double additionFactor = 1;

            // Scale the addition factor linearly from 0.4 to 1 for DeltaTime from 0 to 50
            if (getDeltaTime(current) < 50)
                additionFactor = 0.4 + 0.6 * getDeltaTime(current) / 50;

            return additionFactor * addition;
        }

        private bool hasRhythmChange(HitObject lastObject, HitObject current)
        {
            // We don't want a division by zero if some random mapper decides to put two HitObjects at the same time.
            if (getDeltaTime(current) < 0.5 || getDeltaTime(lastObject) == 0)
                return false;

            double timeElapsedRatio = Math.Max(getDeltaTime(lastObject) / getDeltaTime(current), getDeltaTime(current) / getDeltaTime(lastObject));

            if (timeElapsedRatio >= 8)
                return false;

            double difference = Math.Log(timeElapsedRatio, rhythm_change_base) % 1.0;

            return difference > rhythm_change_base_threshold && difference < 1 - rhythm_change_base_threshold;
        }

        private bool hasColourChange(HitObject lastObject, HitObject current)
        {
            if (isKat(lastObject) == isKat(current))
            {
                sameColourCount++;
                return false;
            }

            var oldColourSwitch = lastColourSwitch;
            var newColourSwitch = sameColourCount % 2 == 0 ? ColourSwitch.Even : ColourSwitch.Odd;

            lastColourSwitch = newColourSwitch;
            sameColourCount = 1;

            // We only want a bonus if the parity of the color switch changes
            return oldColourSwitch != ColourSwitch.None && oldColourSwitch != newColourSwitch;
        }

        private enum ColourSwitch
        {
            None,
            Even,
            Odd
        }

        private bool isHit(HitObject h)
        {
            return h.type.HasFlag(HitObject.Type.Circle);
        }

        private bool isKat(HitObject h)
        {
            return h.hitSound.HasFlag(HitSound.Clap) || h.hitSound.HasFlag(HitSound.Whistle);
        }
        private HitObject lastObject()
        {
            return previousObjects.Last();
        }
        private double getDeltaTime(HitObject h)
        {
            return deltaTimes.GetValueOrDefault(h, 0);
        }
    }
}
