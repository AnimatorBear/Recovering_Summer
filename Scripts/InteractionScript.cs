using Godot;
using System;

public partial class InteractionScript : Node
{
	[Export]
	public bool used = true;
	[Export]
	public float timeToUse {  get; set; }
	[Export]
	public bool givesItem { get; set; } = false;
	[Export]
	public int amountOfUses { get; set; } = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Interact()
	{
		if (!used)
		{
			used = true;
			amountOfUses -= 1;
            GD.Print("A SEE YUU");
			if (givesItem)
			{
				ItemScript item = GetNode("Item").GetChild(0) as ItemScript;

                Node3D no = GetParent().GetNode("Player").GetNode("Pivot").GetNode("MeshInstance3D").GetNode("Camera3D") as Node3D;
				PlayerMovement plmov = GetParent().GetNode("Player") as PlayerMovement;

                bool hasSlot = false;
				foreach(Node3D currentNode in no.GetChildren())
				{
					for(int i = 2; i < 7; i++)
					{
                        if (currentNode.Name == "Inventory" + i)
						{
							if(currentNode.GetChild(0) == null)
							{
								GD.Print($"Put that stuff in {currentNode.Name}");
								hasSlot = true;
								ItemScript item2 = item.Duplicate() as ItemScript;
								currentNode.AddChild(item2);
								plmov.inventory[i - 1] = item2;
								plmov.forceOpenSlot = i;
								break;
							}
						}

                    }
					if (hasSlot)
					{
						break;
					}
				}
				if (!hasSlot)
				{
					amountOfUses += 1;
				}
			}

			if(amountOfUses <= 0)
			{
				QueueFree();
			}
        }
	}
}
