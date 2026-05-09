using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
	public PolygonCollider2D newBounds;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			CameraTargetClamp camTarget =
				FindObjectOfType<CameraTargetClamp>();

			if (camTarget != null)
			{
				camTarget.mapBounds = newBounds;
			}
		}
	}
}
