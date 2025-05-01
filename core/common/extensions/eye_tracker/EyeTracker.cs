using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Single;

public partial class EyeTracker : XRController3D
{
    [Signal] public delegate void GazeSampleEventHandler(Camera3D camera, Vector3 gazeRayRotation, Vector3 hitPoint);

    [Export] public bool Enabled { get; set; } = true;
    [Export] public float RayLength { get; set; } = 100.0f;
    [Export] public XRCamera3D Camera { get; set; }
    
    [ExportGroup("Gaze Dot")]
    [Export] public bool ShowGazeDot { get; set; } = true;
    [Export] public float GazeDotDistance { get; set; } = 10.0f;
    [Export] public float GazeDotRadius { get; set; } = 0.025f;
    
    private Node3D _gazeDot;
    private IGazeable _prevGazeable;
    
    private Matrix<float> _rotationMatrix;
    private bool _calibrated = false;
    
    public override void _Ready()
    {
        _gazeDot = GetNode<Node3D>("GazeDot");
        _gazeDot.Visible = ShowGazeDot;
        _gazeDot.Position = new Vector3(0, 0, -GazeDotDistance);
        
        if (_gazeDot is MeshInstance3D meshInstance)
        {
            if (meshInstance.Mesh is QuadMesh quadMesh)
            {
                quadMesh.Size = Vector2.One * GazeDotRadius;
            }
        }
        
        CoreRadio.Instance.ToggleGazeDot += ToggleGazeDot;
    }
    
    public override void _ExitTree()
    {
        CoreRadio.Instance.ToggleGazeDot -= ToggleGazeDot;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 viewDir = -GlobalBasis.Z.Normalized();
        if (_calibrated)
        {
            viewDir = CorrectViewDir(viewDir);
        }
        _gazeDot.GlobalPosition = Camera.GlobalPosition + viewDir * GazeDotDistance;

        PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(Camera.GlobalPosition, Camera.GlobalPosition + viewDir * RayLength, 1 << 10);
        Dictionary result = spaceState.IntersectRay(query);

        if (!Godot.GodotObject.IsInstanceValid((Godot.GodotObject)_prevGazeable)) _prevGazeable = null;

        if (result.Count <= 0)
        {
            _prevGazeable?.OnGazeExit();
            _prevGazeable = null;
            EmitSignal("GazeSample", Camera, viewDir, new Vector3(float.NaN, float.NaN, float.NaN));
            return;
        }
        
        Node collider = result["collider"].As<Node>();
        
        if (collider is IGazeable gazeable)
        {
            if (gazeable != _prevGazeable)
            {
                _prevGazeable?.OnGazeExit();
                gazeable.OnGazeEnter();
                _prevGazeable = gazeable;
            }
            else
            {
                gazeable.OnGazeStay(delta);
            }
        }
        else
        {
            _prevGazeable?.OnGazeExit();
            _prevGazeable = null;
        }
        
        EmitSignal("GazeSample", Camera, viewDir.Normalized(), result["position"]);
    }

    public void Calibrate(List<Vector3> samples, List<Vector3> targets)
    {
        int n = samples.Count;

        if (n < 3)
        {
            GD.PrintErr("Too few samples");
            return;
        }
        if (n != targets.Count)
        {
            GD.PrintErr("Sample and target count mismatch");
            return;
        }

        DenseMatrix S = DenseMatrix.Create(3, n, 0f);
        DenseMatrix T = DenseMatrix.Create(3, n, 0f);

        for (int i = 0; i < n; i++)
        {
            Vector3 s = samples[i].Normalized();
            Vector3 t = targets[i].Normalized();
            S[0, i] = s.X; S[1, i] = s.Y; S[2, i] = s.Z;
            T[0, i] = t.X; T[1, i] = t.Y; T[2, i] = t.Z;
        }

        Matrix<float> M = T * S.Transpose();
        Svd<float> svd = M.Svd();
        Matrix<float> U = svd.U;
        Matrix<float> Vt = svd.VT;

        _rotationMatrix = U * Vt;

        if (_rotationMatrix.Determinant() < 0)
        {
            U.SetColumn(2, U.Column(2).Multiply(-1));
            _rotationMatrix = U * Vt;
        }

        _calibrated = true;
    }
    
    private Vector3 CorrectViewDir(Vector3 viewDir)
    {
        Vector3 localDir = Camera.GlobalBasis.Inverse() * viewDir.Normalized();
        DenseVector v = DenseVector.OfArray(new float[] { localDir.X, localDir.Y, localDir.Z });
        
        Vector<float> corrected = _rotationMatrix * v;
        Vector3 correctedLocal = new Vector3(corrected[0], corrected[1], corrected[2]).Normalized();
        
        return Camera.GlobalBasis * correctedLocal;
    }


    private void ToggleGazeDot()
    {
        _gazeDot.Visible = !_gazeDot.Visible;
    }
}
