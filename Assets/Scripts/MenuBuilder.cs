using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Xml;
using System;
using System.Reflection;

public class MenuBuilder : MonoBehaviour {

	XmlDocument menuXML;
	TrainingModeManager player;
	Vector3 offset;
	Type playerType;
	MenuController pointer;

	// Use this for initialization
	void Start () {
		RectTransform rect = Resources.Load<GameObject>("prefabs/MenuAction").GetComponent<RectTransform>();
		offset = new Vector3(rect.rect.width, rect.rect.height, 0);
		player = GameObject.Find("Game Manager").GetComponent<TrainingModeManager>();
		TextAsset menuData = Resources.Load("MenuXML") as TextAsset;

		playerType = Type.GetType("TrainingModeManager");
		menuXML = new XmlDocument();
		menuXML.LoadXml(menuData.text);

		XmlNode topNode = menuXML.ChildNodes[1].FirstChild;

		pointer = GetComponent<MenuController>();

		pointer.Initialize(GenMenuList(topNode, transform, new Vector3(390, 130, 0)));


	}
	
	// Update is called once per frame
	Transform GenMenuList (XmlNode topNode, Transform parent, Vector3 position) {
		Vector3 newPos = position;
		//newPos.x += offset.x;
		GameObject listOption = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/OptionList"), newPos, Quaternion.identity);
		listOption.transform.SetParent(parent, false);
		int numOptionsInList = 0;
		foreach(XmlNode node in topNode.ChildNodes) {
			if(node.Name == "Action") {
				GameObject actionOption = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/MenuAction"), newPos, Quaternion.identity);
				actionOption.transform.SetParent(listOption.transform, false);
				actionOption.GetComponent<MenuActionScript>().Initialize(playerType.GetMethod(node.InnerText.Replace(" ", string.Empty)), player);
				actionOption.GetComponentInChildren<Text>().text = node.InnerText;
			}
			else if(node.Name == "List") {
				GameObject actionOption = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/MenuAction"), newPos, Quaternion.identity);
				actionOption.transform.SetParent(listOption.transform, false);
				Vector3 childPos = new Vector3(offset.x / 2, 0, 0);
				actionOption.GetComponent<MenuActionScript>().InitializeAsList(GenMenuList(node, actionOption.transform, childPos));
				actionOption.GetComponentInChildren<Text>().text = node.Attributes[0].Value;
			}
			else Debug.Log(node.InnerText);

			numOptionsInList++;
			newPos.y -= offset.y;
		}

		return listOption.transform;
	}
}
