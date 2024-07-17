using Godot;
using System;

[Tool]
public partial class Planet : Node3D
{
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

    private PlanetSettings _settings = new PlanetSettings(10, 1f, Colors.White);

    [Export]
    public int Resolution
    {
        get => _settings.Resolution;
        set
        {
            _settings.Resolution = value;
            GD.PushWarning($"Setting resolution to {value}");
            UpdatePlanet();
        }
    }

    [Export]
    public float Radius
    {
        get => _settings.Radius;
        set
        {
            _settings.Radius = value;
            GD.PushWarning($"Setting radius to {value}");
            UpdatePlanet();
        }
    }

    [Export]
    public Color Color
    {
        get => _settings.Color;
        set
        {
            _settings.Color = value;
            GD.PushWarning($"Setting color to {value}");
            UpdatePlanet();
        }
    }

    private MeshInstance3D[] _meshInstances;
    private TerrainFace[] _terrainFaces;

    public override void _Ready()
    {
        UpdatePlanet();
    }

    private void UpdatePlanet()
    {
        CallDeferred(nameof(DeferredUpdatePlanet));
    }

    private void DeferredUpdatePlanet()
    {
        Initialize();
        GenerateMesh();
        UpdateMenuVisuals();
    }

    void Initialize()
    {
        if (_meshInstances == null || _meshInstances.Length == 0)
        {
            _meshInstances = new MeshInstance3D[6];
        }
        _terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.Up, Vector3.Down, Vector3.Left, Vector3.Right, Vector3.Forward, Vector3.Back };

        for (int i = 0; i < 6; i++)
        {
            if (_meshInstances[i] == null)
            {
                MeshInstance3D meshInstance = new MeshInstance3D();
                meshInstance.Name = $"mesh_{i}";
                AddChild(meshInstance);
                _meshInstances[i] = meshInstance;
            }

            if (_meshInstances[i].Mesh == null)
            {
                _meshInstances[i].Mesh = new ArrayMesh();
            }

            _terrainFaces[i] = new TerrainFace((ArrayMesh)_meshInstances[i].Mesh, _settings.Resolution, _settings.Color, _settings.Radius, directions[i]);
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainFace face in _terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    void UpdateMenuVisuals()
    {
        var resSlider = GetNode<Slider>("../Camera3D/SettingsMenu/VBoxContainer/ResSlider");
        resSlider.Value = _settings.Resolution;
        var colorPicker = GetNode<ColorPickerButton>("../Camera3D/SettingsMenu/VBoxContainer/ColorPicker");
        colorPicker.Color = _settings.Color;
        var sizeSlider = GetNode<Slider>("../Camera3D/SettingsMenu/VBoxContainer/SizeSlider");
        sizeSlider.Value = _settings.Radius;
    }

    void _on_res_slider_value_changed(float value)
    {
        Resolution = (int)value;
    }

    void _on_wireframe_box_toggled(bool toggled_on)
    {
        GetViewport().DebugDraw = toggled_on ? Viewport.DebugDrawEnum.Wireframe : Viewport.DebugDrawEnum.Disabled;
    }

    void _on_color_picker_color_changed(Color color)
    {
        Color = color;
    }

    void _on_size_slider_value_changed(float newSize) 
    {
        Radius = newSize;
    }
}