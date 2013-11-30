using UnityEngine;
using System.Collections;

public class main : MonoBehaviour {

    float originOrthographicSize;

	// Use this for initialization
	void Start () {
        originOrthographicSize = Camera.mainCamera.camera.orthographicSize;
        Time.timeScale = 1;

        // Application.OpenURL("http://unity3d.com/");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void prepareCreateBall(float sec) {
        StartCoroutine(createBall(sec));
    }
    IEnumerator createBall(float sec) {
        yield return new WaitForSeconds(sec);
        GameObject ball = Resources.Load<GameObject>("ball/ball");
        GameObject nextBall=Instantiate(ball) as GameObject;
        nextBall.transform.parent = GameObject.Find("MainPlayObject").transform;
        //Debug.Log(ball.transform.position);
        nextBall.transform.localPosition = ball.transform.position;

        Camera.mainCamera.camera.orthographicSize = this.originOrthographicSize;

        yield break;
    }

    void OnGUI() {
        bool ifMouseButtonHoldDown = Input.GetMouseButton(0);
        if (ifMouseButtonHoldDown) {
            GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height-Input.mousePosition.y, 30, 30), Resources.Load<Texture>("pointer2"));
        }
        else{
            GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height-Input.mousePosition.y, 30, 30), Resources.Load<Texture>("pointer1"));
        }
    }

    void movingCamera(GameObject ball) {

        // Vector3 pos=Camera.mainCamera.WorldToScreenPoint(ball.transform.position);

        // 攝影機原本的位置
        //Vector3 mpos=Camera.mainCamera.transform.position;

        //// 攝影機新的位置
        //Vector3 ppos = new Vector3(mpos.x, mpos.y, mpos.z);

        //Debug.Log(mpos);
        //Debug.Log(ppos);

        //ppos.x=ball.transform.position.x;

        // 
        //mpos=Vector3.Lerp(mpos, ppos, 100);

        //Camera.mainCamera.transform.position = mpos;



        if (Camera.mainCamera.camera.orthographicSize <= 11f) Camera.mainCamera.camera.orthographicSize += 0.01f;
        Vector3 pp = Camera.mainCamera.camera.transform.position;
        Vector3 tt = ball.transform.position;
        pp = Vector3.Lerp(pp, tt, 5f);
    }
}
