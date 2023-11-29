using Godot;

public partial class PlayerMovement : CharacterBody3D
{
    [Export]
    public int baseDamage {  get; set; }
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
    [Export]
    public ItemScript[] inventory { get; set; } = new ItemScript[6];

    int selectedItem = 0;

    [Export]
    public bool hasBackpack { get; set; }


    public override void _Ready()
    {
        //cam = GetNode<Camera3D>("Camera3D");
        inventory[0] = GetNode<Node3D>("Pivot").GetChild(1).GetChild(0).GetNode("Inventory1").GetChild(0) as ItemScript;
        inventory[1] = GetNode<Node3D>("Pivot").GetChild(1).GetChild(0).GetNode("Inventory2").GetChild(0) as ItemScript;
        inventory[2] = GetNode<Node3D>("Pivot").GetChild(1).GetChild(0).GetNode("Inventory3").GetChild(0) as ItemScript;
        inventory[4] = GetNode<Node3D>("Pivot").GetChild(1).GetChild(0).GetNode("Inventory5").GetChild(0) as ItemScript;
        inventory[5] = GetNode<Node3D>("Pivot").GetChild(1).GetChild(0).GetNode("Inventory6").GetChild(0) as ItemScript;
        GD.Print(inventory[0].Name);
        SetMeta("Damage", baseDamage);
    }
    public override void _PhysicsProcess(double delta)
    {
        #region Attacking, Movement
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
        #endregion
        if (!isDummy)
        {
            if (Input.IsActionJustPressed("InventoryOne") && selectedItem != 1)
            {
                ItemScript item = inventory[0].Duplicate() as ItemScript;
                GetNode<Node3D>("Pivot").GetChild(1).GetChild(0).GetNode("Inventory4").AddChild(item);
                inventory[3] = GetNode<Node3D>("Pivot").GetChild(1).GetChild(0).GetNode("Inventory4").GetChild(0) as ItemScript;


                SetMeta("Damage", baseDamage);
                foreach (ItemScript it in inventory)
                {
                    if(it != null)
                    {
                        it.GetParent<Node3D>().Visible = false;
                    }
                }
                if (inventory[0] != null)
                {
                    inventory[0].GetParent<Node3D>().Visible = true;
                    float currentDamage = ((float)GetMeta("Damage"));
                    SetMeta("Damage", currentDamage + inventory[0].extraDamage);
                }
                selectedItem = 1;
            }
            if (Input.IsActionJustPressed("InventoryTwo") && selectedItem != 2)
            {
                SetMeta("Damage", baseDamage);
                foreach (ItemScript it in inventory)
                {
                    if (it != null)
                    {
                        it.GetParent<Node3D>().Visible = false;
                    }
                }
                if (inventory[1] != null)
                {
                    inventory[1].GetParent<Node3D>().Visible = true;
                    float currentDamage = ((float)GetMeta("Damage"));
                    SetMeta("Damage", currentDamage + inventory[1].extraDamage);
                }
                selectedItem = 2;

            }
            if (Input.IsActionJustPressed("InventoryThree") && selectedItem != 3)
            {
                SetMeta("Damage", baseDamage);
                foreach (ItemScript it in inventory)
                {
                    if (it != null)
                    {
                        it.GetParent<Node3D>().Visible = false;
                    }
                }
                if (inventory[2] != null)
                {
                    inventory[2].GetParent<Node3D>().Visible = true;
                    float currentDamage = ((float)GetMeta("Damage"));
                    SetMeta("Damage", currentDamage + inventory[2].extraDamage);
                }
                selectedItem = 3;

            }
            if (Input.IsActionJustPressed("InventoryFour") && selectedItem != 4)
            {
                SetMeta("Damage",baseDamage);
                foreach (ItemScript it in inventory)
                {
                    if (it != null)
                    {
                        it.GetParent<Node3D>().Visible = false;
                    }
                }
                if (inventory[3] != null)
                {
                    inventory[3].GetParent<Node3D>().Visible = true;
                    float currentDamage = ((float)GetMeta("Damage"));
                    SetMeta("Damage", currentDamage + inventory[3].extraDamage);
                }
                selectedItem = 4;

            }
            if (Input.IsActionJustPressed("InventoryFive") && selectedItem != 5 && hasBackpack)
            {
                SetMeta("Damage", baseDamage);
                foreach (ItemScript it in inventory)
                {
                    if (it != null)
                    {
                        it.GetParent<Node3D>().Visible = false;
                    }
                }
                if (inventory[4] != null)
                {
                    inventory[4].GetParent<Node3D>().Visible = true;
                    float currentDamage = ((float)GetMeta("Damage"));
                    SetMeta("Damage", currentDamage + inventory[4].extraDamage);
                }
                selectedItem = 5;

            }
            if (Input.IsActionJustPressed("InventorySix") && selectedItem != 6 && hasBackpack)
            {
                SetMeta("Damage", baseDamage);
                foreach (ItemScript it in inventory)
                {
                    if (it != null)
                    {
                        it.GetParent<Node3D>().Visible = false;
                    }
                }
                if (inventory[5] != null)
                {
                    inventory[5].GetParent<Node3D>().Visible = true;
                    float currentDamage = ((float)GetMeta("Damage"));
                    SetMeta("Damage", currentDamage + inventory[5].extraDamage);
                }
                selectedItem = 6;

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