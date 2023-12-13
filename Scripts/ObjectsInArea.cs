using Godot;
using System;
using System.Collections.Generic;

public partial class ObjectsInArea : Node
{

	public List<Node3D> enemiesInArea = new List<Node3D>();
	private void OnArea3DBodyEntered(Node3D body)
	{
		if(body != null && body.Name != "Player" && body.Name != "StaticBody3D")
		{
            enemiesInArea.Add(body);
        }
	}

    private void OnArea3DBodyExited(Node3D body)
    {
        if (body != null)
        {
            enemiesInArea.Remove(body);
        }
    }
}
