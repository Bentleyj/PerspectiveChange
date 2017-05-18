//MeMyselfEye Component
using UnityEngine;
using System.Collections;

public class OVR_PlayerMove : MonoBehaviour
{
	[SerializeField]
	private float velocity = 5.0f;

	private int direction = 100;
	private CharacterController controller;
	private Transform head;
	private Transform body;

	void Start()
	{
		controller = GetComponent<CharacterController>();
		head = Camera.main.transform;
		body = transform.Find("Body");
	}

	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			controller.SimpleMove(head.transform.forward * velocity * direction * Time.deltaTime);
			body.transform.rotation = Quaternion.Euler(new Vector3(0.0f, head.transform.eulerAngles.y, 0.0f));
		}else
			if (Input.GetMouseButtonUp(0) && direction == -100)
			{
				direction = 100;
			}
		if (Input.GetMouseButtonDown(0))
		{
			if (Input.GetAxisRaw("Mouse X") < 0)
			{
				direction = -100;
			}
			else
			{
				direction = 100;
			}
		}
	}
}	