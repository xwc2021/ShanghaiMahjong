using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MahjongMap : MonoBehaviour {

    public static float xUnit = 1.0f;
    public static float yUnit = 1.5f;
    public static float heightUnit = 0.5f;

    [SerializeField]
    int X=10;
    [SerializeField]
    int Y=10;//Z方向定義成Y
    [SerializeField]
    int Floor = 10;//Y方向定義成Height

    [SerializeField]
    int nowFlower = 1;

    public int GetX() { return X; }
    public int GetY() { return Y; }
    public int GetAllFloor() { return Floor; }
    public Vector3 GetNowFlowerHeight() { return Vector3.up * MahjongMap.heightUnit * (nowFlower - 1); }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
