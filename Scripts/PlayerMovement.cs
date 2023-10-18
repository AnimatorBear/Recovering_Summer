using Godot;

public partial class PlayerMovement : CharacterBody3D
{
    [Export]
    public int speed { get; set; } = 14;

    [Export]
    public int jumpHeight{ get; set; } = 14;
    [Export]
    public int FallAcceleration { get; set; } = 75;

    [Export]
    public int camSensitivity { get; set; } = 1;

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
        // We create a local variable to store the input direction.
        var direction = Vector3.Zero;

        // We check for each move input and update the direction accordingly.
        if (Input.IsActionPressed("move_right"))
        {
            direction.X += 1.0f;
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction.X -= 1.0f;
        }
        if (Input.IsActionPressed("move_back"))
        {
            // Notice how we are working with the vector's X and Z axes.
            // In 3D, the XZ plane is the ground plane.
            direction.Z += 1.0f;
        }
        if (Input.IsActionPressed("move_forward"))
        {
            direction.Z -= 1.0f;
        }
        if (Input.IsActionPressed("jump"))
        {
            direction.Y += 1.0f;
        }
        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
        }
        //Translate();
        _targetVelocity.X = direction.X * speed;
        _targetVelocity.Z = direction.Z * speed;

        // Vertical velocity
        if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
        {
            _targetVelocity.Y -= FallAcceleration * (float)delta;
        }
        else
        {
            _targetVelocity.Y = direction.Y * jumpHeight;
        }

        // Moving the character
        Velocity = _targetVelocity.Rotated(Vector3.Up.Normalized(), Rotation.Y);
        MoveAndSlide();
    }

    public override void _Process(double delta)
    {

    }
    public override void _Input(InputEvent motionUnknown)
    {
        InputEventMouseMotion motion = motionUnknown as InputEventMouseMotion;
        if (motion != null)
        {
            Rotate(Vector3.Up, -((motion.Relative.X / 100)));
            GetNode<Node3D>("Pivot").Rotate(Vector3.Right, -((motion.Relative.Y / 100)));
        }
    }
}