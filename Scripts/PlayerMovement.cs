using Godot;

public partial class PlayerMovement : CharacterBody3D
{
    [Export]
    public int baseDamage {  get; set; }
    [Export]
    public int maxStamina { get; set; } = 100;
    [Export]
    public int stamina { get; set; } = 100;
    [Export]
    public float staminaRegenTime { get; set; } = 0.05f;
    float currentStaminaRegenTime = 0;
    float timeTillStaminaRefill { get; set; }
    [Export]
    public bool isDummy{ get; set; } = true;
    [Export]
    public bool canAttack { get; set; } = true;
    [Export]
    public int speed { get; set; } = 14;

    float currentSpeed { get; set; }

    float speedDebuff { get; set; }

    [Export]
    public int jumpHeight{ get; set; } = 14;
    [Export]
    public int jumpStaminaUsage { get; set; } = 30;
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
    public float shoveStunTiming { get; set; } = 2;
    [Export]
    public float shoveStun{ get; set; } = 2;
    [Export]
    public float shoveDamage { get; set; } = 5;
    [Export]
    public int shoveStaminaUsage { get; set; } = 20;
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

    [Export]
    public bool canVirus { get; set; }
    int virusStage { get; set; } = 0;
    [Export]
    float[] timeTillNextStage { get; set; } = { 4.2f, 4, 4, 4,4 };
    float stageTime;

    float itemUseTime = 0;

    [Export]
    ObjectsInArea areaObjects { get; set; }

    bool firstHitShove = false;
    [Export]
    ObjectsInArea rayObjects { get; set; }
    float interactTime = 0;
    InteractionScript lastUsedScript;
    bool interacting;
    public int forceOpenSlot;

    public override void _Ready()
    {
        //cam = GetNode<Camera3D>("Camera3D");
        areaObjects = GetNode<Node3D>("Pivot").GetChild(1).GetChild(0).GetNode("EnemiesInArea") as ObjectsInArea;
        rayObjects = GetNode<Node3D>("Pivot").GetChild(1).GetChild(0).GetNode("CustomRaycast") as ObjectsInArea;
        inventory[0] = GetNode<Node3D>("Pivot").GetChild(1).GetChild(0).GetNode("Inventory1").GetChild(0) as ItemScript;
        inventory[1] = GetNode<Node3D>("Pivot").GetChild(1).GetChild(0).GetNode("Inventory2").GetChild(0) as ItemScript;
        inventory[2] = GetNode<Node3D>("Pivot").GetChild(1).GetChild(0).GetNode("Inventory3").GetChild(0) as ItemScript;
        inventory[4] = GetNode<Node3D>("Pivot").GetChild(1).GetChild(0).GetNode("Inventory5").GetChild(0) as ItemScript;
        inventory[5] = GetNode<Node3D>("Pivot").GetChild(1).GetChild(0).GetNode("Inventory6").GetChild(0) as ItemScript;
        GD.Print(inventory[0].Name);
        SetMeta("Damage", baseDamage);
        virusStage = 1;
        stageTime = timeTillNextStage[virusStage - 1];
        currentSpeed = speed;
        stamina = maxStamina;
        cam = GetNode<Node3D>("Pivot").GetChild(1).GetChild(0) as Camera3D;
    }
    public override void _PhysicsProcess(double delta)
    {
        #region Attacking, Movement
        attackTime -= (float)delta;
        currentJumpCooldown -= (float)delta;
        stunTime -= (float)delta;
        shoveTime -= (float)delta;
        parryTime -= (float)delta;
        currentStaminaRegenTime -= (float)delta;
        interactTime -= (float)delta;
        if(timeTillStaminaRefill > 0)
        {
            timeTillStaminaRefill -= (float)delta;
        }
        else
        {
            if(stamina < maxStamina && currentStaminaRegenTime < 0)
            {
                stamina++;
                currentStaminaRegenTime = staminaRegenTime;
            }
        }
        if (canVirus)
        {
            //GD.Print(stageTime);
            stageTime -= (float)delta;
        }

        if(stageTime < 0 && canVirus)
        {
            virusStage++;
            if (virusStage < 5)
            {
                GD.Print(virusStage);
                stageTime = timeTillNextStage[virusStage - 1];
                switch (virusStage)
                {
                    case 2:
                        speedDebuff += 0.1f;
                        break;
                    case 3:
                        GetNode<HealthScript>("MobDetector").maxHealth -= 10;
                        GetNode<HealthScript>("MobDetector").extraDamage  += 0.2f;
                        GetNode<HealthScript>("MobDetector").TakeDamage(1);
                        break;
                    case 4:
                        GetNode<HealthScript>("MobDetector").maxHealth -= 15;
                        GetNode<HealthScript>("MobDetector").extraDamage += 0.15f;
                        speedDebuff += 0.25f;
                        GetNode<HealthScript>("MobDetector").TakeDamage(1);
                        break;
                }

            }
            else
            {
                GetNode<HealthScript>("MobDetector").TakeDamage(500);
            }

        }
        // We create a local variable to store the input direction.
        var direction = Vector3.Zero;
        if (stunTime < 0f)
        {

            // We check for each move input and update the direction accordingly.
            if (Input.IsActionPressed("move_right"))
            {
                direction.X += 1.0f;
                interacting = false;
                interactTime = -1;
            }
            if (Input.IsActionPressed("move_left"))
            {
                direction.X -= 1.0f;
                interacting = false;
                interactTime = -1;
            }
            if (Input.IsActionPressed("move_back"))
            {
                // Notice how we are working with the vector's X and Z axes.
                // In 3D, the XZ plane is the ground plane.
                direction.Z += 1.0f;
                interacting = false;
                interactTime = -1;
            }
            if (Input.IsActionPressed("move_forward"))
            {
                direction.Z -= 1.0f;
                interacting = false;
                interactTime = -1;

            }
            if (direction != Vector3.Zero)
            {
                direction = direction.Normalized();
            }
        }
        if (Input.IsActionPressed("jump") && currentJumpCooldown < 0 && stamina > jumpStaminaUsage && IsOnFloor())
        {
            direction.Y += 1.0f;
            currentJumpCooldown = jumpCooldown;
            stamina -= jumpStaminaUsage;
            if (timeTillStaminaRefill < 2f)
            {
                timeTillStaminaRefill = 2f;
            }
            interacting = false;
            interactTime = -1;
        }
        //Translate();
        if (stunTime < 0f)
        {
            _targetVelocity.X = direction.X * (currentSpeed * (1 - speedDebuff));
            _targetVelocity.Z = direction.Z * (currentSpeed * (1 - speedDebuff));
        }
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
        if (canAttack && !isDummy && stunTime < 0f)
        {
            if (Input.IsActionPressed("light_attack") && selectedItem != 0)
            {
                if (selectedItem == 1|| inventory[selectedItem - 1] == null)
                {
                    attackTime = lightAttDuration;
                        
                }
                else if (inventory[selectedItem - 1].Name == "Amoxicillin Tablets" && stageTime < timeTillNextStage[virusStage - 1])
                {
                    itemUseTime += (float)delta;
                    if(itemUseTime > inventory[selectedItem-1].useTime)
                    {
                        stageTime = timeTillNextStage[virusStage - 1];
                        itemUseTime = 0;
                        inventory[selectedItem - 1].uses -= 1;
                        if (inventory[selectedItem-1].uses < 1)
                        {
                            inventory[selectedItem - 1].QueueFree();
                            inventory[selectedItem - 1] = null;
                        }
                    }
                }
                else if (inventory[selectedItem - 1].Name == "Augmentin Antibiotics")
                {
                    itemUseTime += (float)delta;
                    if (itemUseTime > inventory[selectedItem - 1].useTime)
                    {
                        stageTime = timeTillNextStage[virusStage - 1] + 90;
                        itemUseTime = 0;
                        inventory[selectedItem - 1].uses -= 1;
                        if (inventory[selectedItem - 1].uses < 1)
                        {
                            inventory[selectedItem - 1].QueueFree();
                            inventory[selectedItem - 1] = null;
                        }
                    }
                }
            }
            if (Input.IsActionJustPressed("heavy_attack"))
            {

            }
            if (Input.IsActionJustPressed("shove") && stamina > shoveStaminaUsage)
            {
                shoveTime = shoveDuration;
                if(timeTillStaminaRefill < 3f)
                {
                    timeTillStaminaRefill = 3f;
                }
                stamina -= shoveStaminaUsage;
            }
            if (Input.IsActionJustPressed("parry"))
            {
                parryTime = parryDuration;
                if(timeTillStaminaRefill < parryDuration)
                {
                    timeTillStaminaRefill = parryDuration;
                }
            }

        }
        if (!isDummy)
        {
            if (GetMeta("Attacking").AsBool() == false && attackTime > 0)
            {
                SetMeta("Attacking", true);
                foreach (var item in areaObjects.enemiesInArea)
                {
                    GD.Print(item.Name);
                    if(item.GetMeta("Blocking").AsBool() != true)
                    {
                        HealthScript hp = item.GetNode<Area3D>("MobDetector") as HealthScript;
                        hp.TakeDamage(GetMeta("Damage").AsInt32());
                    }
                    else
                    {
                        stunTime = 5f;
                    }
                }
                GD.Print("Attacking");
            }
            else if (attackTime < 0 && GetMeta("Attacking").AsBool() == true)
            {
                SetMeta("Attacking", false);
                GD.Print("Not Attacking");
            }
            if (shoveTime < shoveDuration && shoveTime >= shoveDuration - shoveStunTiming)
            {
                foreach (var item in areaObjects.enemiesInArea)
                {
                    PlayerMovement mov = item as PlayerMovement;
                    mov.stunTime = shoveStun;
                    if (firstHitShove)
                    {
                        HealthScript hp = item.GetNode<Area3D>("MobDetector") as HealthScript;
                        hp.TakeDamage(shoveDamage);
                        firstHitShove = false;
                    }
                }
                SetMeta("Shoving", true);
                canAttack = false;
                GD.Print("Shoving");
            }
            else if (shoveTime < shoveDuration - shoveStunTiming && shoveTime >= 0)
            {
                firstHitShove = true;
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
        #region Interaction
        if (!isDummy)
        {

            if (Input.IsActionJustPressed("Interact") || interacting)
            {
                foreach (Node3D nod in rayObjects.enemiesInArea)
                {
                    if (nod.GetParent() as InteractionScript != null)
                    {
                        InteractionScript inter = nod.GetParent() as InteractionScript;
                        lastUsedScript = inter;
                        if (interactTime < -1)
                        {
                            interactTime = inter.timeToUse;
                            interacting = true;
                            GD.Print("Inter");
                        }
                    }
                }
                if (interactTime < 0 && interacting)
                {
                    lastUsedScript.Interact();
                    interacting = false;
                    GD.Print("Unter");
                }
            }
            else
            {
                if(lastUsedScript != null)
                {
                    lastUsedScript.used = false;
                }
            }

        }
        #endregion
        if (!isDummy)
        {
            if (Input.IsActionJustPressed("InventoryOne") && selectedItem != 1 || forceOpenSlot == 1)
            {
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
                forceOpenSlot = 0;
            }
            if (Input.IsActionJustPressed("InventoryTwo") && selectedItem != 2 || forceOpenSlot == 2)
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
                forceOpenSlot = 0;

            }
            if (Input.IsActionJustPressed("InventoryThree") && selectedItem != 3 || forceOpenSlot == 3)
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
                forceOpenSlot = 0;

            }
            if (Input.IsActionJustPressed("InventoryFour") && selectedItem != 4 || forceOpenSlot == 4)
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
                forceOpenSlot = 0;

            }
            if (Input.IsActionJustPressed("InventoryFive") && selectedItem != 5 && hasBackpack || forceOpenSlot == 5 && hasBackpack)
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
                forceOpenSlot = 0;

            }
            if (Input.IsActionJustPressed("InventorySix") && selectedItem != 6 && hasBackpack || forceOpenSlot == 1 && hasBackpack)
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
                forceOpenSlot = 0;

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