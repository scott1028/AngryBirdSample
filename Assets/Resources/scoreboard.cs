using UnityEngine;
using System.Collections;
using System;

public class scoreboard : MonoBehaviour {

    public float x = 0;
    public float y = 0;
    public float w = 20;
    public float h = 40;
    public float m = 2;
    public int score;
    public Texture[] scoreTexture=new Texture[10];

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnGUI() {
        char[] chr=this.score.ToString().ToCharArray();
        //Debug.Log(chr[8]);

        for (int i = 0; i < chr.Length; i++) {
            GUI.DrawTexture(new Rect(x + i * (m + w), y, w, h), scoreTexture[Convert.ToInt32(chr[i]) - 48]);
        }
    }

    void AddScore() {
        this.score += 10;
    }
}
