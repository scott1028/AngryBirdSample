﻿using UnityEngine;
using System.Collections;
using System;

public class enemy : MonoBehaviour {

    public float health;

    public bool ifEnableColliderProcessor = false;

    public bool isDestroying=false;

	// Use this for initialization
	void Start () {
        this.health = 30f;
	}
	
	// Update is called once per frame
	void Update () {
        if (isDestroying == false && this.health <= 0) {
            isDestroying = true;
            Destroy(this.gameObject, 0f);
        };

        // 開啟碰撞器處理程序, 讓物體落下後靜止後才開啟
        if (ifEnableColliderProcessor == false) {
            ifEnableColliderProcessor = true;
        }

        //Debug.Log(this.gameObject.transform.rigidbody.velocity);
	}

    void OnCollisionEnter(Collision collision)
    {
        if (ifEnableColliderProcessor==true && collision.collider.gameObject.tag == "ball" || collision.collider.gameObject.tag == "enemy" && GameObject.Find("ball") && GameObject.Find("ball").GetComponent<ball>().ifBallAlreadyShot == true)
        {
            if (collision.collider.gameObject.tag == "ball")
            {
                // 被球體碰到就直接死亡，球體並不會反彈，可見物體摧毀後 Collider 也不會運算
                Destroy(this.gameObject);
                isDestroying = true;
            }
            else{
                // 不直接死亡採用扣血
                this.health -= 5;
                Debug.Log(this.health);
                this.gameObject.GetComponent<AudioSource>().Play();
            }
        }
        else
        { 
            // 其他物體
        }
    }

    void OnDestroy() {

        if (GameObject.Find("ScoreBoard"))
        {
            GameObject.Find("ScoreBoard").BroadcastMessage("AddScore");
        }

        //if (GameObject.FindGameObjectWithTag("score"))
        //{
        //    int oriScore = Convert.ToInt32(GameObject.FindGameObjectWithTag("score").guiText.text);
        //    GameObject.FindGameObjectWithTag("score").guiText.text = (oriScore + 10).ToString();
        //}
    }
}
