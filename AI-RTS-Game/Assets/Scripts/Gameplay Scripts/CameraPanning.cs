using UnityEngine;
using System.Collections;

public class CameraPanning : MonoBehaviour {

	// Use this for initialization
    private GameObject cam;
    public float MoveSpeed;
	void Start () {
        cam = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.UpArrow))
            cam.transform.position += new Vector3( 0,1 * MoveSpeed,0);
        if (Input.GetKey(KeyCode.DownArrow))
            cam.transform.position += new Vector3(0, -1 * MoveSpeed, 0);
        if (Input.GetKey(KeyCode.LeftArrow))
            cam.transform.position += new Vector3( -1 * MoveSpeed,0, 0);
        if (Input.GetKey(KeyCode.RightArrow))
            cam.transform.position += new Vector3(1 * MoveSpeed,0, 0);
        if(Input.GetKey(KeyCode.KeypadMinus))
            cam.GetComponent<Camera>().orthographicSize--;
        if (Input.GetKey(KeyCode.KeypadPlus))
            cam.GetComponent<Camera>().orthographicSize++;
	
	}
}
