using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class SpriteMover : MonoBehaviour {

	NavMeshAgent myAgent;
	Transform myAgentT;

	Camera camera;

	SpriteRenderer render;

	// Use this for initialization
	void Start () {
		myAgent = GetComponentInParent<NavMeshAgent>();
		myAgentT = myAgent.transform;
		camera = Camera.main;

		render = GetComponent<SpriteRenderer>();
	}

	public bool isMoving;
	float movementTime;

	public bool isInteracting;
	float interactTime;

	// Update is called once per frame
	void Update () {
		isMoving = myAgent.velocity.sqrMagnitude > 0.1f;

		FaceCam();

		UpdateFacing();

		// If we're moving, wobble +- ~5degrees 
		if (isMoving) {
			movementTime += Time.deltaTime * 10f * myAgent.velocity.magnitude;

			var T = transform;

			T.localRotation *= Quaternion.AngleAxis(Mathf.Sin(movementTime) * 10f, T.TransformDirection(Vector3.forward));
		}

		var rt = render.transform;
		if (isInteracting) {
			interactTime += Time.deltaTime * 30f;

			var adj = Mathf.Sin(interactTime);
			adj = 1 + (adj * 0.2f);

			rt.localScale = new Vector3(1,adj,1);
		} else {
			rt.localScale = Vector3.one;
		}
	}

	void OnDrawGizmos() {
		// Draw an arrow based on our facing

		if (myAgentT == null) {
			myAgent = GetComponentInParent<NavMeshAgent>();
			myAgentT = myAgent.transform;
		}

		Gizmos.color = Color.green;

		Gizmos.DrawLine(myAgentT.TransformPoint(Vector3.forward), myAgentT.TransformPoint(new Vector3( 0.5f, 0, 0)));
		Gizmos.DrawLine(myAgentT.TransformPoint(Vector3.forward), myAgentT.TransformPoint(new Vector3(-0.5f, 0, 0)));
	}

	void UpdateFacing() {
		var screenX = camera.WorldToViewportPoint(myAgentT.position).x;
		var forwardX = camera.WorldToViewportPoint(myAgentT.TransformPoint(Vector3.forward)).x;

		//Debug.LogFormat("S/F {0} {1}", screenX, forwardX);

		render.flipX = screenX > forwardX;
	}

	void FaceCam() {
		var activeCam = camera;

		if (activeCam) { // Ignore if the camera is null (No camera assigned)
			var camT = activeCam.transform;
			var camP = camT.position;
			var myT = transform; // Cache transform references as the getter is annoyingly slow
			var myPos = myT.position;

			var diff = myPos - camP;
			// Zero the Y, normalize
			diff.y = 0;
			diff = diff.normalized;

			myT.rotation = Quaternion.LookRotation(diff, Vector3.up);
		}
	}
}
