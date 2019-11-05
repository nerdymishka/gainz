using System;

namespace NerdyMishka
{
    public interface IRandom : IRandomBytesGenerator
    {
        void NextBytes(byte[] buffer, int offset = 0, int? length = null);
        bool NextBoolean();

        double NextDouble();

        int NextInt32(int max = int.MaxValue);
        int NextInt32(int min, int max);

        long NextInt64(long max = long.MaxValue);
        long NextInt64(long min, long max);

        float NextFloat();

        void SetSeed(long seed);
        void SetSeed(int seed);
    }
}