using Godot;
using System;

public class NoiseFilter
{
    private NoiseSettings settings;
    private FastNoiseLite noise;

    public NoiseFilter(NoiseSettings settings)
    {
        this.settings = settings;
        this.noise = new FastNoiseLite();
        noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.BaseRoughness;
        float amplitude = 1;

        for (int i = 0; i < settings.NumLayers; i++)
        {
            float v = noise.GetNoise3D(
                (point.X + settings.Centre.X) * frequency,
                (point.Y + settings.Centre.Y) * frequency,
                (point.Z + settings.Centre.Z) * frequency
            );
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= settings.BaseRoughness;
            amplitude *= settings.Persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - settings.MinValue);
        return noiseValue * settings.Strength;
    }
}