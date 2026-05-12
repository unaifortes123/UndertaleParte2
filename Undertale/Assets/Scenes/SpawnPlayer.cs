using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
	public Transform spawnPoint;
	public GameObject player;

	void Start()
	{
		player.transform.position = spawnPoint.position;
	}
}
