using Godot;

public struct PlanetSettings
{
    public int Resolution;
    public float Radius;
    public Color Color;

    public PlanetSettings(int resolution, float radius, Color color)
    {
        Resolution = resolution;
        Radius = radius;
        Color = color;
    }
}