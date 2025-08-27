using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharicMove : MonoBehaviour {

	public Transform target;
	public CharacterController kCharacterController;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButton(0))			
			Move (2f);
	}

	void Move(float _speed)
	{
		transform.LookAt (target);

		Vector3 m_Move = transform.forward * _speed * Time.deltaTime;

		kCharacterController.Move (m_Move);
	}
}
