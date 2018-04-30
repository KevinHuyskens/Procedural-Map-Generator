using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

	public GameObject map;
	public float rotationSpeed = 2f;
	
	void Update () {

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Rotate(Vector3.up*rotationSpeed*Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			transform.Rotate(-Vector3.up*rotationSpeed*Time.deltaTime);
		}
		transform.RotateAround(map.transform.position, Vector3.up, 20 * Time.deltaTime * 2f);

	}
}
