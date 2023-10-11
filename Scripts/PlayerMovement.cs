using Godot;

public partial class PlayerMovement : CharacterBody3D
{
    [Export]
    public int speed { get; set; } = 14;

    [Export]
    public int jumpHeight{ get; set; } = 14;
    [Export]
    public int FallAcceleration { get; set; } = 75;

    private Vector3 _targetVelocity = Vector3.Zero;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here.
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
            GetNode<Node3D>("Pivot").LookAt(Position + direction, Vector3.Up);
        }
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
        Velocity = _targetVelocity;
        MoveAndSlide();
    }

    public override void _Process(double delta)
    {

    }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.A)
            {
                GD.Print("A was pressed");

            }
        }
    }
}