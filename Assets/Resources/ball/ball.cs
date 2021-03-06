﻿using UnityEngine;
using System.Collections;

// 擴充 Vector3 的 Instance 方法
public static class vector3Method{
    public static Vector3 toScreenCoordinate(this Vector3 x){
        Vector3 pos=Camera.mainCamera.WorldToScreenPoint(x);
        return new Vector3(pos.x, Screen.height - pos.y, pos.z) ;
    }
}

public class ball : MonoBehaviour {
    // 球的三維座標
    public Vector3 originPos;
    public AudioClip[] sound = new AudioClip[3];
    public bool letBallMoveWithMouse=false;
    public bool theBallCanFly=false;
    public bool rubberLineReturnOrigin = false;
    public bool hasHitedObject = false;
    public bool hasHitedFloor = false;
    public bool theBallisFlying = false;
    public bool ifCanPlayAudioWoohoo = false;
    public bool ifBallAlreadyShot = false;
    public bool canCreateBall = true;
    public bool canAddForce = false;
    public bool denyAllowMouseControl = false;
    public bool canPlayAudio = true;
    float ballDistanceFromHisOriginPosition;
    Vector3 lineRenderOriginPosition = new Vector3(-15.56f, 1f, -8.57f);
    Vector3 lineRenderReturnOriginChangingPosition;
    Vector3 lineRenderMovePositon;
    Vector3 lastPostition;

	// Use this for initialization
	void Start () {
        originPos=this.transform.position;

        sound[0] = Resources.Load<AudioClip>("Audio/Woohoo");
        sound[1] = Resources.Load<AudioClip>("Audio/Yipee");
        sound[2] = Resources.Load<AudioClip>("Audio/Boing");

        lastPostition = this.gameObject.transform.position;

        this.gameObject.transform.GetComponent<MeshRenderer>().enabled = false;
        //if(this.gameObject.renderer.material.mainTextureOffset.x>=)0
	}
	
	// Update is called once per frame
	void Update () {

        // 計算球體與自己圓點的座標
        ballDistanceFromHisOriginPosition = Vector3.Distance(this.gameObject.transform.position,this.originPos);

        bool mouseDown = Input.GetMouseButton(0);

        // 當滑鼠點擊到球體的時候
        if (mouseDown && letBallMoveWithMouse == false && denyAllowMouseControl==false)
        {
            Ray r = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
            //Debug.DrawRay(r.origin, r.direction, Color.red);
            RaycastHit hitInfo;
            Physics.Raycast(r.origin, r.direction, out hitInfo, 1000);

            if (hitInfo.collider && hitInfo.collider.gameObject.tag == "ball")
            {
                hitInfo.collider.rigidbody.isKinematic = true;
                letBallMoveWithMouse = true;
            }
        }
        // 當滑鼠下壓球體並移動
        else if (mouseDown && letBallMoveWithMouse == true && denyAllowMouseControl==false) 
        {
            /*
                // 計算滑鼠的平面位置
                Vector3 pos = Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition);
                pos.z=GameObject.FindGameObjectWithTag("ball").transform.position.z;

                // 讓物體隨著向量位移
                GameObject.FindGameObjectWithTag("ball").transform.position = pos;
            */

            // 把物體擺回原點
            GameObject.FindGameObjectWithTag("ball").transform.position = this.originPos;

            // 用滑鼠座標重新計算向量, 並設定臨界值
            Vector3 directionValue=Vector3.ClampMagnitude(Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition) - this.originPos,5.5f);

            // 將深度Z向量設定為 0;
            directionValue.z = 0;

            // 使用原點+向量計算出物件座標
            this.gameObject.transform.position = this.originPos+directionValue;//new Vector3(this.originPos.x + directionValue.x, this.originPos.y + directionValue.y, this.originPos.z);
        }
        // 當放開滑鼠時
        else if(letBallMoveWithMouse==true)
        {
            
            for(int i=0;i<GameObject.Find("MainPlayObject").transform.childCount;i++)
            {
                if(GameObject.Find("MainPlayObject").transform.GetChild(i).tag=="ball"){
                    GameObject.Find("MainPlayObject").transform.GetChild(i).rigidbody.isKinematic = false;
                }
            }
            letBallMoveWithMouse = false;
            theBallCanFly = true;
        }
        // 讓力只會作用一次
        else if (letBallMoveWithMouse == false && theBallCanFly==true)
        {
            theBallCanFly = false;
            rubberLineReturnOrigin = false;
            canAddForce = true;
            theBallisFlying = true;
            ifCanPlayAudioWoohoo = true;
            ifBallAlreadyShot = true;
            denyAllowMouseControl = true;
        }

        // 確定這是二維座標
        // Debug.Log(Input.mousePosition);
        
        // 橡皮筋的恢復臨界值Flag開啟, 球體將橡皮筋拉超逾 4 時
        if (ballDistanceFromHisOriginPosition >= 6 && this.transform.position.x >= this.originPos.x + 4 && rubberLineReturnOrigin==false)
        {
            rubberLineReturnOrigin = true;
            // 將恢復座標起算點帶入
            lineRenderReturnOriginChangingPosition = this.gameObject.transform.position;
        }
        else if (rubberLineReturnOrigin == false)
        {
            // 計算橡皮筋位置需要的增減量
            lineRenderMovePositon = lineRenderOriginPosition - (this.originPos - this.transform.position);
            float len = Vector3.Distance(lineRenderMovePositon, originPos);
            GameObject.Find("rubberBand").GetComponent<LineRenderer>().SetPosition(1, lineRenderMovePositon);
        }
        else if (rubberLineReturnOrigin == true) {
            // Rubber 開始趨近原點
            lineRenderReturnOriginChangingPosition = Vector3.Lerp(lineRenderReturnOriginChangingPosition, this.lineRenderOriginPosition, 10 * Time.deltaTime);
            GameObject.Find("rubberBand").GetComponent<LineRenderer>().SetPosition(1, lineRenderReturnOriginChangingPosition);

            if (Vector3.Distance(lineRenderReturnOriginChangingPosition, this.lineRenderOriginPosition) < 0.01f)
            {
                lineRenderReturnOriginChangingPosition = this.lineRenderOriginPosition;
                // rubberLineReturnOrigin = false; // 必須在當前的物件消失後才開啟Flag
            }
            else {
                // Debug.Log(Vector3.Distance(lineRenderReturnOriginChangingPosition, this.lineRenderOriginPosition));
            };
        }

        // 當物體又撞過地板或是物件的時候才會觸發向量過低時就停止物件的方法
        // 不能用 rigidbody.velocity.magnitude 算, 否則垂直也會觸發。
        if (rigidbody.velocity.magnitude < 0.1f && rigidbody.velocity.magnitude > 0f && (hasHitedFloor == true || hasHitedObject == true))
        {
            theBallisFlying = false;
            rigidbody.Sleep();
        }

        // 當向量為0時刪除物件
        if (rigidbody.velocity.magnitude <= 0f && rigidbody.IsSleeping() && (hasHitedFloor == true || hasHitedObject == true))
        {
            // 這段程式只會調用一次
            hasHitedFloor = false;
            hasHitedObject = false;

            Destroy(this.gameObject, 2f);
        }

        // 當球在飛行的時候
        if (theBallisFlying) {
            Camera.mainCamera.BroadcastMessage("movingCamera", this.gameObject);
        }

        // Debug.Log(ifCanPlayAudioWoohoo);

        if (ifCanPlayAudioWoohoo == true) {
            ifCanPlayAudioWoohoo = false;
            int i = Random.Range(0, 2);
            this.gameObject.GetComponent<AudioSource>().clip = sound[i];
            this.gameObject.GetComponent<AudioSource>().Play();
        }

        // 事情全部做完之後紀錄球體座標
        lastPostition = this.gameObject.transform.position;
	}

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "floor")
        {
            hasHitedFloor = true;
        }
        else {
            hasHitedObject = true;
        };

        StartCoroutine(playAnimation(0f,0.33f,0.1f));
    }

    void OnCollisionStay(Collision collisionInfo) {
        if (this.gameObject.GetComponent<AudioSource>().isPlaying == false && canPlayAudio==true) {
            canPlayAudio=false;
            StartCoroutine( playAudioSound() );
        }
    }

    IEnumerator playAudioSound() {
        this.gameObject.GetComponent<AudioSource>().clip=sound[2];
        this.gameObject.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1);
        canPlayAudio=true;
    }

    void FixedUpdate() {
        //Debug.Log(1);
        if (this.canAddForce == true) {
            this.canAddForce = false;
            GameObject.FindGameObjectWithTag("ball").rigidbody.AddForce((this.originPos - this.transform.position) * 3000);
        }
    }

    void OnDestroy() {
        rubberLineReturnOrigin = false;     // 告訴橡皮筋可以重新起算了
        Camera.mainCamera.gameObject.BroadcastMessage("prepareCreateBall", 0.5f);
    }

    void OnGUI() {
        //Texture pp=Resources.Load<Texture>("Texture/enemy2");
        //Vector3 pos=Camera.mainCamera.WorldToScreenPoint(this.gameObject.transform.position);
        //GUI.DrawTexture(new Rect(pos.x-15,Screen.height-pos.y-15,30,30), pp);

        //Debug.Log(this.gameObject.transform.localRotation.eulerAngles);
    }

    IEnumerator playAnimation(float s,float n,float sec) {
        this.gameObject.transform.FindChild("Cube").renderer.material.mainTextureOffset += new Vector2(0.33f, 0f);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.transform.FindChild("Cube").renderer.material.mainTextureOffset -= new Vector2(0.33f, 0f);
        if (this.gameObject.transform.FindChild("Cube").renderer.material.mainTextureOffset.x > 0.66f)
        {
            this.gameObject.transform.FindChild("Cube").renderer.material.mainTextureOffset = new Vector2(0f, 0f);
        }
        if (this.gameObject.transform.FindChild("Cube").renderer.material.mainTextureOffset.x <= 0f) {
            this.gameObject.transform.FindChild("Cube").renderer.material.mainTextureOffset = new Vector2(0f, 0f);
        }
    }
}
