using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetClamp : MonoBehaviour
{
	public Transform player;
	public PolygonCollider2D mapBounds;
	

	void LateUpdate()
	{
		if (player == null || mapBounds == null) return;

		// 1. seguir al player
		Vector2 targetPos = player.position;

		// 2. comprobar si está dentro del collider
		if (mapBounds.OverlapPoint(targetPos))
		{
			transform.position = targetPos;
		}
		else
		{
			// si está fuera, lo “empujamos” al borde
			Vector2 closest = mapBounds.ClosestPoint(targetPos);
			transform.position = closest;
		}
	}
}
