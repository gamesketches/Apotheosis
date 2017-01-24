using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {

	Transform currentOption;
	Transform[] currentOptionList;
	int listPosition;
	GameObject pointer;
	public bool active;
	Transform topOptionList;
	// Use this for initialization
	void Awake () {
		active = false;
		pointer = (GameObject)Instantiate(Resources.Load<GameObject>("prefabs/MenuSelector"));
		pointer.transform.SetParent(GameObject.Find("Canvas").transform);
		listPosition = 1;
	}

	public void Initialize(Transform optionList) {
		topOptionList = optionList;
		currentOptionList = optionList.GetComponentsInChildren<Transform>();
		currentOption = currentOptionList[listPosition];
		pointer.transform.position = currentOption.position - new Vector3(230, 0, 0);
		pointer.GetComponent<RectTransform>().position = currentOption.position - new Vector3(230, 0, 0);
		Debug.Log(pointer.transform.position);
		Debug.Log(pointer.GetComponent<RectTransform>().position);
		Debug.Log(currentOption.position);
		if(!active) {
			topOptionList.gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(active) {
			if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
				SetHighlight(currentOption, false);
				int siblingIndex = currentOption.GetSiblingIndex() + 1;
				if(siblingIndex >= currentOption.transform.parent.childCount) {
					siblingIndex = 0;
				}
				currentOption = currentOption.transform.parent.GetChild(siblingIndex);
				SetHighlight(currentOption, true);
			}

			if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
				SetHighlight(currentOption, false);
				int siblingIndex = currentOption.GetSiblingIndex() - 1;
				if(siblingIndex < 0) {
					siblingIndex = currentOption.transform.parent.childCount - 1;
				}
				currentOption = currentOption.transform.parent.GetChild(siblingIndex);
				SetHighlight(currentOption, true);
			}
			if(Input.GetKeyDown(KeyCode.Space)) {
				MenuActionScript actionScript = currentOption.GetComponent<MenuActionScript>();
				actionScript.Activate();
				if(actionScript.listOption) {
					currentOptionList = currentOption.transform.GetChild(1).GetComponentsInChildren<Transform>();
					listPosition = 1;
					currentOption = currentOptionList[listPosition];
				}
			}
			if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
				Transform grandParent = currentOption.transform.parent.parent;
				if(grandParent != transform.root) {
					currentOptionList = grandParent.parent.GetComponentsInChildren<Transform>();
					for(int i = 0; i < currentOptionList.Length; i++) {
						if(currentOptionList[i] == grandParent) {
							listPosition = i;
							break;
						}
					}
					SetHighlight(currentOption, false);
					currentOption = grandParent;
					SetHighlight(currentOption, true);
					grandParent.GetComponent<MenuActionScript>().DeActivate();
				}
			}
			UpdatePosition();
		}
	}

	public void Toggle() {
		if(active) {
			topOptionList.gameObject.SetActive(false);
			active = false;
		}
		else {
			topOptionList.gameObject.SetActive(true);
			active = true;
		}
	}

	void UpdatePosition() {
		pointer.transform.position = currentOption.position - new Vector3(230, 0, 0);
	}

	void SetHighlight(Transform option, bool toggleOn) {
		Image buttonImage = option.gameObject.GetComponent<Image>();
		Color targetColor = buttonImage.color;
		if(toggleOn) {
			targetColor.a = 0.2f;
		}
		else {
			targetColor.a = 0.0f;
		}
		buttonImage.color = targetColor;
	}
}
