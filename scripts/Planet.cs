using Godot;
using System;

[Tool]
public partial class Planet : Node3D
{
	private int _res = 10;

    [Export]
    public int Resolution
    {
        get => _res;
        set
        {
            // Update speed and reset the rotation.
            _res = value;
			GD.PushWarning("Setting res to " + value);
			_Ready();
        }
    }

    private MeshInstance3D[] _meshInstances;
    private TerrainFace[] _terrainFaces;

    public override void _Ready()
    {
        Initialize();
        GenerateMesh();
        var resSlider = GetNode<Slider>("../Camera3D/SettingsMenu/VBoxContainer/ResSlider");
        resSlider.Value = _res;
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
                meshInstance.Name = "mesh";
                AddChild(meshInstance);

                var material = new StandardMaterial3D();
                meshInstance.MaterialOverride = material;

                _meshInstances[i] = meshInstance;
                _meshInstances[i].Mesh = new ArrayMesh();
            }

            _terrainFaces[i] = new TerrainFace((ArrayMesh)_meshInstances[i].Mesh, Resolution, directions[i]);
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainFace face in _terrainFaces)
        {
            face.ConstructMesh();
        }
    }

	void _on_res_slider_drag_ended(bool value_changed)
	{
		// DEPRECATED FIGURE OUT HOW TO DELETE
	}

	void _on_res_slider_value_changed(float value)
	{
        int newResolution = (int)value;
        Resolution = newResolution;	
	}

    // enable wireframe drawing if checked, disable if not
    void _on_wireframe_box_toggled(bool toggled_on)
    {
        if (toggled_on)
        {
		    GetViewport().DebugDraw = Viewport.DebugDrawEnum.Wireframe;
        } else {
            GetViewport().DebugDraw = Viewport.DebugDrawEnum.Disabled;
        }
    }
}
