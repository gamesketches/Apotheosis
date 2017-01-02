using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public class MenuActionScript : MonoBehaviour {

	private MethodInfo action;
	private object actionObject;
	private object[] parameters;
	public bool listOption;
	private Transform list;
	// Use this for initialization
	void Awake () {
		listOption = false;
		parameters = new object[0];
	}
	
	public void Initialize(MethodInfo method, object obj) {
		action = method;
		actionObject = obj;
	}

	public void Initialize(MethodInfo method, object obj, object[] methodParameters) {
		action = method;
		actionObject = obj;
		parameters = methodParameters;
	}

	public void InitializeAsList(Transform newList) {
		listOption = true;
		list = newList;
		list.gameObject.SetActive(false);
	}

	public void Activate() {
		if(listOption) {
			list.gameObject.SetActive(true);
		}
		else {
			action.Invoke(actionObject, parameters);
		}
	}

	public void DeActivate() {
		list.gameObject.SetActive(false);
	}
}
