using MapsetParser.objects;
using TaikoChecks.starrating.taiko;

namespace TaikoChecks
{
    public class Common
    {
        public static void CalculateTaikoSR(Beatmap beatmap)
        {
            beatmap.starRating = TaikoDifficultyCalculator.Calculate(beatmap);
        }
    }

    /*TODO Checks:
     * X = done
    
    Finishers with small gap at lower star rating (Finishers in the middle of streams)

    Ninja circles (500 bpm+ scroll speed, and must be more than twice as fast as previous object?) X
    Drum Sampleset unusable unless custom sound set?
    Base Slider Velocity 1.4 - Warning

    Rest time

    Snap usage

    SV Changes - should be almost none on Kantan/Futsuu, very limited on Muzu, less limited on Oni, no check on higher diffs.

    OD Settings

    HP Settings (Based on object count and difficulty)
    */
}