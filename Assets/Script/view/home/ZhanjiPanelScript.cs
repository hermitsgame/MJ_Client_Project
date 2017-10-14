﻿using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using LitJson;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class ZhanjiPanelScript : MonoBehaviour {
	public GameObject RoomPanel;//房间显示
	public GameObject DetailPanel;//详细信息显示
	public GameObject roomcontaner;//房间容器
	public GameObject playercontaner;//每局数据

	public GameObject roomIdInputDialog;
	public InputField inputField;


	public List<Text> playerTexts;

	private int currentDisplayFlag=0;//当前显示的面板 0 表示第一个面板    1表示第二个面板
	private List<GameObject> detailItemPanelList;//详细数据每项数据集合

	private List<GameObject> roomPanelList;



	// Use this for initialization
	void Start () {
		detailItemPanelList = new List<GameObject> ();
		roomPanelList = new List<GameObject> ();
		getRoomRequest ();

		GameManager.getInstance ().Server.onResponse += onResponse;

	}

	void onResponse (ClientResponse response)
	{
		switch (response.headCode) {
		case APIS.ZHANJI_REPORTER_REPONSE://房间战绩
			zhanjiResponse (response);
			break;
		case APIS.ZHANJI_DETAIL_REPORTER_REPONSE://房间详细战绩
			zhanjiDetailResponse (response);
			break;
		}
	}

	private ZhanjiRoomList roomZhanjiData;
	private ZhanjiDataList roomDetailData;


	public void BackSpaceBtnClick()
    {
		if (currentDisplayFlag == 1) {
			RoomPanel.SetActive (true);
			DetailPanel.SetActive (false);
			for (int i = 0; i < detailItemPanelList.Count; i++) {
				Destroy (detailItemPanelList [i]);
			}
			detailItemPanelList.Clear ();
			currentDisplayFlag = 0;
		} else {
			Destroy(this.gameObject);
		}
		

    }


	private void getRoomRequest(){
		GameManager.getInstance().Server.requset (new ZhanjiRequest("0"));
	}


	private  void zhanjiResponse(ClientResponse response){
		
		roomZhanjiData = new ZhanjiRoomList ();
		roomZhanjiData.roomDataList  = JsonMapper.ToObject<List<ZhanjiRoomDataItem>> (response.message);
		if (roomPanelList != null && roomPanelList.Count > 0) {
			for (int i = 0; i <roomPanelList.Count; i++) {
				Destroy (roomPanelList [i]);
			}
			roomPanelList.Clear ();
		}


		if (roomZhanjiData.roomDataList.Count != 0) {
			currentDisplayFlag = 0;
			RoomPanel.SetActive (true);
			DetailPanel.SetActive (false);

			for (int i = 0; i < roomZhanjiData.roomDataList.Count; i++) {
				ZhanjiRoomDataItem itemData	=roomZhanjiData.roomDataList [i];
				GameObject itemTemp = Instantiate (Resources.Load("Prefab/ZhanRoomItem")) as GameObject;
				itemTemp.transform.parent = roomcontaner.transform;
				itemTemp.transform.localScale = Vector3.one;
				itemTemp.GetComponent<ZhanjiRoomItemScript>().setUI(itemData,i+1);
				roomPanelList.Add (itemTemp);
		
			}
		}
	}


	private void zhanjiDetailResponse(ClientResponse response){
		currentDisplayFlag = 1;
		RoomPanel.SetActive (false);
		DetailPanel.SetActive (true);
		roomDetailData = new ZhanjiDataList ();
		roomDetailData.standingDetail = JsonMapper.ToObject<List<ZhanjiDataItemVo>> (response.message);

		for (int i = 0; i < detailItemPanelList.Count; i++) {
			Destroy (detailItemPanelList [i]);
		}
		detailItemPanelList.Clear ();

		if (roomDetailData.standingDetail.Count > 0) {
			string content = roomDetailData.standingDetail [0].content;
			if (!string.IsNullOrEmpty(content)) {
				string[] infoList = content.Split (new char[1]{','});
				for (int i = 0; i < infoList.Length-1; i++) {
					string name = infoList [i].Split (new char[1]{':'})[0];
					playerTexts [i].text = name;
				}
			}
		}
		for (int i = 0; i <roomDetailData.standingDetail.Count; i++) {
			ZhanjiDataItemVo itemData = roomDetailData.standingDetail [i];
			GameObject itemTemp = Instantiate (Resources.Load("Prefab/ZhanItem")) as GameObject;
			itemTemp.transform.parent = playercontaner.transform;
			itemTemp.transform.localScale = Vector3.one;
			itemTemp.GetComponent<ZhanjiItemScript>().setUI(itemData,i+1);
			detailItemPanelList.Add (itemTemp);
		}

	}



	public void openRoomIdInput(){
		roomIdInputDialog.SetActive (true);
	}


	public void closeRoomIdDialog(){
		roomIdInputDialog.SetActive (false);
	}


	public void sureClick(){
		string inputStr = inputField.text;
		if (inputStr == null || inputStr.Length <= 0 || inputStr.Length>6) {
			MyDebug.Log ("输入的数据长度不正确");
			return;
		}


		if (!isStrNum (inputStr)) {
			MyDebug.Log ("只能输入数字");
			return;
		}

		GameManager.getInstance().Server.requset (new ZhanjiSearchRequest (inputStr));
		closeRoomIdDialog ();
	}


	private bool isStrNum(string s){
		int Flag = 0;
		char[]str = s.ToCharArray();
		for(int i = 0;i < str.Length ;i++)
		{
			if (Char.IsNumber(str[i]))
			{
				Flag++;
			}
			else
			{
				Flag = -1;
				break;
			}
		}
		if ( Flag > 0 )
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	void OnDestroy(){
		GameManager.getInstance ().Server.onResponse -= onResponse;
	}
}
