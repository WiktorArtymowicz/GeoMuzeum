using GeoMuzeum.DataService;
using System;
using System.Threading.Tasks;

namespace GeoMuzeum.View.ProjectHelpers
{
    public static class GeneratePin
    {
        public async static Task<int> Generate()
        {
            var userLogDataService = new UserLoginDataService();

            var seed = DateTime.Now.Ticks;
            var random = new Random(MixSeed(seed));

            int pin;
            do
            {
                pin = random.Next(1000, 9999);
            } while (await userLogDataService.CheckUserPin(pin));

            return pin;
        }

        private static int MixSeed(long seed)
        {
            return (int)((seed * DateTime.Today.Ticks) / (397 / 23 * new DateTime(3002, 12, 21).Ticks).GetHashCode());
        }
    }

}
