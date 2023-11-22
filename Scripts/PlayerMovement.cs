using Godot;

public partial class PlayerMovement : CharacterBody3D
{
    [Export]
    public bool isDummy{ get; set; } = true;
    [Export]
    public bool canAttack { get; set; } = true;
    [Export]
    public int speed { get; set; } = 14;

    [Export]
    public int jumpHeight{ get; set; } = 14;
    [Export]
    public int FallAcceleration { get; set; } = 75;

    [Export]
    public int camSensitivity { get; set; } = 1;

    [Export]
    public float cameraLimit { get; set; } = 1.2f;
    [Export]
    public float jumpCooldown { get; set; } = 1f;
    float currentJumpCooldown = 1f;

    private Vector3 _targetVelocity = Vector3.Zero;
    Camera3D cam;

    float attackTime = 0f;

    public float stunTime = 0f;
    float shoveTime = 0f;

    [Export]
    public float shoveDuration { get; set; } = 14;
    [Export]
    public float shoveStun { get; set; } = 14;
    [Export]
    public float lightAttDuration { get; set; } = 1f;
    [Export]
    public float parryDuration { get; set; } = 14;
    float parryTime = 0f;


    public override void _Ready()
    {
        cam = GetNode<Camera3D>("Camera3D");
    }
    public override void _PhysicsProcess(double delta)
    {
        attackTime -= (float)delta;
        currentJumpCooldown -= (float)delta;
        stunTime -= (float)delta;
        shoveTime -= (float)delta;
        parryTime -= (float)delta;
        if(stunTime < 0f)
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
            if (direction != Vector3.Zero)
            {
                direction = direction.Normalized();
            }

            if (Input.IsActionPressed("jump") && currentJumpCooldown < 0)
            {
                direction.Y += 1.0f;
                currentJumpCooldown = jumpCooldown;
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
            if (canAttack && !isDummy)
            {
                if (Input.IsActionJustPressed("light_attack"))
                {
                    attackTime = lightAttDuration;
                }
                if (Input.IsActionJustPressed("heavy_attack"))
                {

                }
                if (Input.IsActionJustPressed("shove"))
                {
                    shoveTime = shoveDuration;
                }
                if (Input.IsActionJustPressed("parry"))
                {
                    parryTime = parryDuration;
                }
            }

        }
        else
        {
            var direction = Vector3.Zero;
            direction.Y += 1.0f;
            _targetVelocity.Y = direction.Y;
            Velocity = _targetVelocity.Rotated(Vector3.Up.Normalized(), Rotation.Y);
            MoveAndSlide();
        }
        if (!isDummy)
        {
            if (attackTime > 0 && GetMeta("Attacking").AsBool() == false)
            {
                SetMeta("Attacking", true);
                GD.Print("Attacking");
            }
            else if (attackTime < 0 && GetMeta("Attacking").AsBool() == true)
            {
                SetMeta("Attacking", false);
                GD.Print("Not Attacking");
            }
            if (shoveTime < shoveDuration && shoveTime >= shoveDuration - shoveStun)
            {
                SetMeta("Shoving", true);
                canAttack = false;
                GD.Print("Shoving");
            }
            else if (shoveTime < shoveDuration - shoveStun && shoveTime >= 0)
            {
                SetMeta("Shoving", false);
                GD.Print("Shove Recover");
            }
            else if (shoveTime < 0)
            {
                SetMeta("Shoving", false);
                canAttack = true;
            }

            if (parryTime > 0 && GetMeta("Blocking").AsBool() == false)
            {
                SetMeta("Blocking", true);
                GD.Print("Parrying");
            }
            else if (parryTime < 0 && GetMeta("Blocking").AsBool() == true)
            {
                SetMeta("Blocking", false);
                GD.Print("Not Parrying");
            }
        }
        
    }

    public override void _Process(double delta)
    {

    }
    public override void _Input(InputEvent motionUnknown)
    {
        if(!isDummy)
        {
            InputEventMouseMotion motion = motionUnknown as InputEventMouseMotion;
            Node3D pivot = GetNode<Node3D>("Pivot");
            if (motion != null)
            {
                Rotate(Vector3.Up, -((motion.Relative.X / 100)));
                if (pivot.Rotation.X > -1.2f && pivot.Rotation.X < 1.2f)
                {
                    pivot.Rotate(Vector3.Right, -((motion.Relative.Y / 100)));
                }
                else
                {
                    if (pivot.Rotation.X <= -1.2f)
                    {

                        pivot.Rotate(Vector3.Right, 0.01f);
                    }
                    else
                    {
                        pivot.Rotate(Vector3.Right, -0.01f);
                    }
                }
            }
        }
        
    }
}