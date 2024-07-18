using Godot;
using System;

[Tool]
public partial class Planet : Node3D
{
    [Export(PropertyHint.Range, "2,256")]
    public int Resolution { get; set; } = 10;

    [Export]
    public bool AutoUpdate { get; set; } = true;

    private ShapeSettings _shapeSettings;
    [Export]
    public Resource ShapeSettingsResource
    {
        get => _shapeSettings;
        set
        {
            if (value is ShapeSettings ss)
            {
                _shapeSettings = ss;
            }
            else
            {
                GD.PushError("Invalid ShapeSettings resource");
            }
        }
    }

    private ColorSettings _colorSettings;
    [Export]
    public Resource ColorSettingsResource
    {
        get => _colorSettings;
        set
        {
            if (value is ColorSettings cs)
            {
                _colorSettings = cs;
                if (Engine.IsEditorHint() && AutoUpdate)
                {
                    GeneratePlanet();
                }
            }
            else
            {
                GD.PushError("Invalid ColorSettings resource");
            }
        }
    }   

    private ShapeGenerator shapeGenerator;
    private MeshInstance3D[] meshInstances = new MeshInstance3D[6];
    private TerrainFace[] terrainFaces = new TerrainFace[6];

    // Add these fields to track changes
    private int lastResolution;
    private ShapeSettings lastShapeSettings;
    private ColorSettings lastColorSettings;

    public override void _Ready()
    {
        GeneratePlanet();
        UpdateLastValues();
    }

    private void GenerateSettingsResources()
    {
        if (ShapeSettingsResource == null)
        {
            ShapeSettingsResource = new ShapeSettings();
        }
        if (ColorSettingsResource == null)
        {
            ColorSettingsResource = new ColorSettings();
        }
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint() && AutoUpdate && SettingsChanged())
        {
            GeneratePlanet();
            UpdateLastValues();
        }
    }

    private bool SettingsChanged()
    {
        return Resolution != lastResolution ||
               _shapeSettings != lastShapeSettings ||
               _colorSettings != lastColorSettings ||
               (_shapeSettings != null && !_shapeSettings.Equals(lastShapeSettings)) ||
               (_colorSettings != null && !_colorSettings.Equals(lastColorSettings));
    }

    private void UpdateLastValues()
    {
        lastResolution = Resolution;
        lastShapeSettings = _shapeSettings?.Clone() as ShapeSettings;
        lastColorSettings = _colorSettings?.Clone() as ColorSettings;
    }

    private void Initialize()
    {
        GenerateSettingsResources();
        shapeGenerator = new ShapeGenerator(_shapeSettings);
        Vector3[] directions = { Vector3.Up, Vector3.Down, Vector3.Left, Vector3.Right, Vector3.Forward, Vector3.Back };

        for (int i = 0; i < 6; i++)
        {
            if (meshInstances[i] == null)
            {
                meshInstances[i] = new MeshInstance3D();
                AddChild(meshInstances[i]);
                meshInstances[i].Mesh = new ArrayMesh();
                meshInstances[i].MaterialOverride = new StandardMaterial3D();
            }

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshInstances[i].Mesh as ArrayMesh, Resolution, directions[i]);
        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    private void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    private void GenerateColors()
    {
        foreach (MeshInstance3D m in meshInstances)
        {
            (m.MaterialOverride as StandardMaterial3D).AlbedoColor = _colorSettings.PlanetColor;
        }
    }
}