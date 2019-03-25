using System.Collections.Generic;

namespace BoxyBot.Seafight.Constants
{
    public static class BonusMapConstants
    {
        public const int VIRGO_MAX_WAVES = 10;
        public const int CAPRICORNUS_MAX_WAVES = 20;
        public const int SAGITTARIUS_MAX_WAVES = 30;
        public const int CANCER_MAX_WAVES = 40;
        public const int WHITE_MAX_WAVES = 10;
        public const int LEO_MAX_WAVES = 40;
        public const int LIBRA_MAX_WAVES = 25;
        public const int AQUARIUS_MAX_WAVES = 15;
        public const int TAURUS_MAX_WAVES = 10;
        public const int PUMPKIN_MAX_WAVES = 10;
        public const int WINTER_MAX_WAVES = 10;
        public const int MAX_WAVES = 10;
        private static Dictionary<int, int> maxBonusMapWaves;


        public static Dictionary<int, int> GetBonusmapMaxWaves()
        {
            if (maxBonusMapWaves == null)
            {
                maxBonusMapWaves = new Dictionary<int, int>
                {
                    { 106, VIRGO_MAX_WAVES },
                    { 107, CAPRICORNUS_MAX_WAVES },
                    { 108, SAGITTARIUS_MAX_WAVES },
                    { 112, CANCER_MAX_WAVES },
                    { 109, WHITE_MAX_WAVES },
                    { 110, WHITE_MAX_WAVES },
                    { 111, WHITE_MAX_WAVES },
                    { 113, LEO_MAX_WAVES },
                    { 114, LIBRA_MAX_WAVES },
                    { 116, AQUARIUS_MAX_WAVES },
                    { 115, TAURUS_MAX_WAVES },
                    { 117, PUMPKIN_MAX_WAVES },
                    { 103, WINTER_MAX_WAVES },
                    { 104, WINTER_MAX_WAVES },
                    { 100, VIRGO_MAX_WAVES },
                    { 101, CAPRICORNUS_MAX_WAVES },
                    { 102, SAGITTARIUS_MAX_WAVES },
                    { 105, CANCER_MAX_WAVES },
                    { 0, MAX_WAVES }
                };
            }
            return maxBonusMapWaves;
        }
    }
}
