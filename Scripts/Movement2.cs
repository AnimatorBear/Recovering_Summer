using Godot;

public partial class Movement2 : CharacterBody3D
{
    [Export]
    public int speed { get; set; } = 14;

    [Export]
    public int jumpHeight { get; set; } = 14;
    [Export]
    public int FallAcceleration { get; set; } = 75;

    private Vector3 _targetVelocity = Vector3.Zero;
    Camera3D cam;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here.
        cam = GetNode<Camera3D>("Camera3D");
    }
    public override void _PhysicsProcess(double delta)
    {

    }

    public override void _Process(double delta)
    {

    }
    public override void _Input(InputEvent motionUnknown)
    {
        InputEventMouseMotion motion = motionUnknown as InputEventMouseMotion;
        if (motion != null)
        {
            Rotate(Vector3.Up, -(motion.Relative.X / 100));
            //Rotate(Vector3.Left, -(motion.Relative.Y / 100));
        }
    }
}
