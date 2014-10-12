using UnityEngine;
using System.Collections;

public class PlayerSpawnManager : MonoBehaviour {

	public static PlayerSpawnManager instance { get; private set; }

	private Transform[] playerSpawns;

	void Start()
	{
		instance = this;
		playerSpawns = gameObject.GetComponentsInChildren<Transform>();
	}

	/**
	 * Return one of the spawn locations randomly
	 */
	public Vector3 newSpawnPosition()
	{
		int n = Random.Range(0, playerSpawns.Length);

		return playerSpawns[n].position;
	}
}
