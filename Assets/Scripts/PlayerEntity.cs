﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerEntity : BaseBehaviour
{

	[SerializeField]
	private float movingSpeed = 3;

	public float MovingSpeed
	{
		get { return movingSpeed; }
		set { movingSpeed = value; }
	}

	[SerializeField]
	private float rotatingSpeed = 3;

	public float RotateSpeed
	{
		get { return rotatingSpeed; }
		set { rotatingSpeed = value; }
	}

	public int Health { get; set; }

	[SerializeField]
	private float jumpSpeed = 5;

	public float JumpSpeed
	{
		get { return jumpSpeed; }
		set { jumpSpeed = value; }
	}

	[SerializeField]
	private Transform eye;

	public Transform Eye
	{
		get { return eye; }
		set { eye = value; }
	}


	[SerializeField]
	int maxHealth = 20;

	CharacterController cc;
	Vector3 moveDir;

	public Vector3 Velocity => moveDir;

	float moveY;
	GameObject lookingObject;

	public LocationInfo LookingBlock => ChunkRenderer.Instance.GetBlockInfoOf(lookingObject);

	public int MaxHealth
	{
		get { return maxHealth; }
		set { maxHealth = value; }
	}

	public bool IsGrounded => cc.isGrounded;

	// Use this for initialization
	void Start()
	{
		Health = MaxHealth;
		cc = GetComponent<CharacterController>();
	}

	float timeTmp;

	public bool IsDead {get; private set; }

	// Update is called once per frame
	void Update()
	{
		if (IsDead) return;
		ProcessInput();

		if (transform.position.y < 0 && timeTmp >= 0.25f)
		{
			Damage(4, "{0} は奈落に落ちた");
			timeTmp = 0;
		}

		timeTmp += Time.deltaTime;

	}

	public void Damage(int amount, string killerMessage)
	{
		Health -= amount;
		if (Health <= 0)
		{
			IsDead = true;
			UniCraft.ShowDeathGUI(killerMessage);
		}
	}

	void ProcessInput()
	{
		Look();
		Walk();
	}

	void Walk()
	{
		moveDir = transform.forward * MovingSpeed * Input.GetAxis("Vertical") + transform.right * MovingSpeed * Input.GetAxis("Horizontal");

		moveDir *= Input.GetButton("Dash") ? 2 : 1;

		if (cc.isGrounded)
		{
			if (Input.GetButtonDown("Jump"))
			{
				moveY = JumpSpeed;
			}
			else
			{
				moveY = 0;
			}
		}

		moveDir.y = moveY += Physics.gravity.y * Time.deltaTime;
		
		cc.Move(moveDir * Time.deltaTime);
	}

	void Look()
	{
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * RotateSpeed);
		Eye.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * RotateSpeed);

		RaycastHit hit;
		if (Physics.Raycast(Eye.position, Eye.forward, out hit, 10f))
		{
			if (!(lookingObject == hit.collider.gameObject))
			{
				ClearLookingObject();

				lookingObject = hit.collider.gameObject;
				var mesh = lookingObject.GetComponent<MeshRenderer>();
				if (mesh != null)
				{
					var block = new MaterialPropertyBlock();
					block.SetColor("_Color", new Color(0.4f, 0.4f, 0.4f, 1));
					mesh.SetPropertyBlock(block);
				}
			}

			if (Input.GetButtonDown("Punch"))
			{
				Chunk.SetBlock("unicraft:air", LookingBlock.Location);
			}

			if (Input.GetButtonDown("Interact"))
			{
				BlockBase bl;
				if ((bl = BlockRegister.Instance[LookingBlock.BlockId]) is IInteractable)
				{
					(bl as IInteractable).OnInteract(LookingBlock.Location, this);
				}
				else
				{
					var candidate = Vector3Int.CeilToInt(LookingBlock.Location + hit.normal);
					Chunk.SetBlock(UniCraft.BlockIdInHand, candidate);
				}
			}


		}
		else
		{
			ClearLookingObject();
		}
	}

	void ClearLookingObject()
	{
		if (lookingObject != null)
		{	
			var renderer = lookingObject.GetComponent<MeshRenderer>();
			if (renderer != null)
			{
				var block = new MaterialPropertyBlock();
				block.SetColor("_Color", Color.white);
				renderer.SetPropertyBlock(block);
			}
		}
	}
}