using System;

namespace LibrarianTests
{
    public static class Utils
    {
        public static bool AreEqual(double x, double y, double tolerance)
        {
            double difference = Math.Abs(x * tolerance);

            return Math.Abs(x - y) <= difference;
        }
    }
}
