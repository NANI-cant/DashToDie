using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunkochanControl : MonoBehaviour {
	private float InputH;//Horizontal Input (A & D key)
	private float InputV;//Vertical Input (W & S key)
	private GameObject JKCCam;//Camera which chases Junkochan
	private CharacterController JKCController;//Character controller component attached to Junkochan
	private Animator JKCAnim;//Animator component attached to Junkochan
	private float JumpTime;//Jump power limit
	private float MoveSpeed;//Horizontal move speed
	private float VertSpeed;//Verical move speed
	private float Height;//Current height of Junkochan (y value of transform.position)

	// Use this for initialization
	void Start () {
		JKCCam = GameObject.Find("JunkochanCam");
		JKCController = this.GetComponent<CharacterController>();
		JKCAnim = this.GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update() {

#region ACTION
		if (CheckGrounded() && Input.GetMouseButtonDown(0)) {//When left clicked while Junkochan is on the ground
			JKCAnim.SetTrigger("Slash");//Play "Sword_Iai" animation in "ActionLayer" in Animator
		}
		if (CheckGrounded() && Input.GetMouseButton(1)) {//When right clicked while Junkochan is on the ground
			JKCAnim.SetBool("Guard", true);//Play "Sword_Guard" animation in "ActionLayer" in Animator
		} else {//When the end of right click pressing 
			JKCAnim.SetBool("Guard", false);//End of "Sword_Guard" animation
		}
		
		if (JKCAnim.GetCurrentAnimatorStateInfo(1).IsName("Sword_Iai") || JKCAnim.GetCurrentAnimatorStateInfo(1).IsName("Sword_Guard") || JKCAnim.GetCurrentAnimatorStateInfo(1).IsName("Sword_Store")) {
			return;//While the attacking / guarding animation is playing, do not go to below "MOVEMENT" process  
		}
#endregion


#region MOVEMENT
		InputH = Input.GetAxis("Horizontal");//Get keyboard input
		InputV = Input.GetAxis("Vertical");//Get keyboard input
		Vector3 CamForward = Vector3.Scale(JKCCam.transform.forward, new Vector3(1, 0, 1)).normalized;//Camera's forward direction
		Vector3 MoveDirection = CamForward * InputV + JKCCam.transform.right * InputH;//Get Junkochan's forward direction seen from camera

		if (MoveDirection.magnitude > 0) {//When any WASD key is pushed
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(MoveDirection), 0.2f);//Rotate Junkochan to Inputting direction
		}
		if (CheckGrounded()) {//When Junkochan is on the ground
			JKCAnim.SetBool("Grounded", true);//Set Junkochan's "Grounded" parameter in Animator component
			JKCAnim.SetBool("Jump", false);
			JumpTime = 0;//Reset jump inputting time
			VertSpeed = 0;//Reset vertical speed (jumping & falling)
		} else {
			JKCAnim.SetBool("Grounded", false);
		}

		if (MoveDirection.magnitude > 0) {//When any WASD key is pushed
			MoveSpeed = 2f;//Set Junkochan's moving speed as walking speed
			JKCAnim.SetBool("Move", true);//Set Junkochan's "Move" parameter in Animator component
		} else {
			JKCAnim.SetBool("Move", false);
		}

		if (Input.GetKey(KeyCode.LeftShift)) {//When shift key is pushed
			MoveSpeed *= 2f;//Set Junkochan's moving speed as Running speed
			JKCAnim.SetBool("Run", true);//Set Junkochan's "Run" parameter in Animator component
		} else {
			JKCAnim.SetBool("Run", false);
		}
		if (Input.GetKey(KeyCode.LeftControl)) {//When Control key is pushed
			MoveDirection *= 0.4f;//Set Junkochan's moving speed as crouching walk speed
			JKCAnim.SetBool("Crouch", true);//Set Junkochan's "Crouch" parameter in Animator component
		} else {
			JKCAnim.SetBool("Crouch", false);
		}

		if (CheckGrounded() && Input.GetKeyDown(KeyCode.Space)) {//When Space key is pushed while Junkochan is on the ground (Called only once)
			VertSpeed =10f;//Set vertical jumping speed
			JKCAnim.SetBool("Jump",true);
		}


		if (!CheckGrounded() && Input.GetKey(KeyCode.Space) && JumpTime < 0.3f) {//When Space key is pushed while Junkochan is in the air (Called while Space key is pushed, no more than 0.3 sec)
			JumpTime += Time.deltaTime;//Add jumping time to prevent infinite jump
			VertSpeed+= 0.1f;//Additional jump power
		}
		
		if (!CheckGrounded() && transform.position.y - Height < 0) {//When Junkochan is falling, tansit animation state from "Ascending" to "Falling"
			JKCAnim.SetBool("Fall", true);//Set Junkochan's "Run" parameter in Animator component
		}
		else{
			JKCAnim.SetBool("Fall", false);
		}
		Height = transform.position.y;//Memory current Junkochan's height

		//JunckoChan Movement
		if(!CheckGrounded())VertSpeed -= 0.2f;//Increase falling speed (worked as gravity acceleration)
		JKCController.Move(MoveDirection*MoveSpeed*Time.deltaTime);//Horizontal movement
		JKCController.Move(Vector3.up*VertSpeed*Time.deltaTime);//Vertical movement

#endregion
	}

	bool CheckGrounded() {//Judge whether Junkochan is on the ground or not
		Ray ray = new Ray(this.transform.position+Vector3.up*0.05f,Vector3.down*0.1f);//Shoot ray at 0.05f upper from Junkochan's feet position to the ground with its length of 0.1f
		return Physics.Raycast(ray, 0.1f);//If the ray hit the ground, return true
	}

}
