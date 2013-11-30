using UnityEngine;
using System.Collections;

public class ball : MonoBehaviour {

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
        bool mouseDown = Input.GetMouseButton(0);

        // 計算上的球體座標與本次的座標位移差值
        //float substractLastAndThisPostion = Mathf.Abs((this.gameObject.transform.position - this.lastPostition).x) + Mathf.Abs((this.gameObject.transform.position - this.lastPostition).y);

        //Debug.Log(substractLastAndThisPostion);

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
            // 計算滑鼠的平面位置
            Vector3 pos = Camera.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            pos.z=GameObject.FindGameObjectWithTag("ball").transform.position.z;

            // 讓物體隨著向量位移
            GameObject.FindGameObjectWithTag("ball").transform.position = pos;
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

        // 計算橡皮筋位置需要的增減量
        Vector3 lineRendererCenterPositionValue = new Vector3(-15.8f, 1f, -8.56f);
        Vector3 lrpvc = lineRendererCenterPositionValue - (this.originPos - this.transform.position);
        float len = Vector3.Distance(lrpvc, originPos);

        //橡皮筋的恢復臨界值, 球體將橡皮筋拉超逾 4 時
        if (len >= 4 && this.transform.position.x >= this.originPos.x + 4)
        {
            rubberLineReturnOrigin = true;
            lrpvc = Vector3.Lerp(lrpvc, lineRendererCenterPositionValue, 5 * Time.deltaTime);
            GameObject.Find("rubberBand").GetComponent<LineRenderer>().SetPosition(1, lrpvc);
        }
        else if (rubberLineReturnOrigin == false)
        {
            
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
            //if (Camera.mainCamera.camera.orthographicSize <= 11f) Camera.mainCamera.camera.orthographicSize += 0.01f;
            //Vector3 pp=Camera.mainCamera.camera.transform.position;
            //Vector3 tt=this.transform.position;
            //pp=Vector3.Lerp(pp, tt,5f);

            Camera.mainCamera.BroadcastMessage("movingCamera", this.gameObject);
        }

        // Debug.Log(ifCanPlayAudioWoohoo);

        if (ifCanPlayAudioWoohoo == true) {
            ifCanPlayAudioWoohoo = false;
            int i = Random.Range(0, 2);
            this.gameObject.GetComponent<AudioSource>().clip = sound[i];
            this.gameObject.GetComponent<AudioSource>().Play();
        }

        // 事情全部握完之後紀錄球體座標
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
