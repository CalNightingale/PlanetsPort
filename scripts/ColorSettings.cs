using Godot;

public partial class ColorSettings : Resource
{
    [Export]
    public Color PlanetColor { get; set; } = Colors.White;

    // Add any other color-related properties here

    public override bool Equals(object obj)
    {
        if (obj is ColorSettings other)
        {
            return PlanetColor.Equals(other.PlanetColor);
            // Compare any other properties you've added
        }
        return false;
    }

    public ColorSettings Clone()
    {
        return new ColorSettings
        {
            PlanetColor = this.PlanetColor
            // Clone any other properties you've added
        };
    }
}