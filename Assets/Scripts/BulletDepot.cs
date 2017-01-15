using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class BulletDepot : ScriptableObject {

	List<GameObject> bulletPool;
	GameObject bulletPrefab;

	public class Bullet{
		public float size;
		public float speed;
		public int damage;
		public int angle;
	}

	public class Volley {
		[XmlElement("Bullet")]
		public Bullet[] volley;
	}

	public class ProjectileType {
		[XmlElement("Volley")]
		public Volley[] volleys;

		[XmlAttribute("name")]
		public string typeName;

		[XmlAttribute("desc")]
		public string bulletDescription;
	}

	[XmlRoot("Root")]
	public class CharacterData {
		[XmlArray("Character")]
		[XmlArrayItem("ProjectileType")]
		public List<ProjectileType> projectileTypes = new List<ProjectileType>();

		[XmlAttribute("name")]
		public string characterName;
	}

	public CharacterData[] types;
	// Use this for initialization
	public void Load () {
		string[] characterNames = System.Enum.GetNames(typeof(Character));
		types = new CharacterData[characterNames.Length];
		var serializer = new XmlSerializer(typeof(CharacterData));
		for(int i = 0; i <  characterNames.Length; i++) {
			TextAsset bulletData = Resources.Load(string.Concat(characterNames[i].ToLower(), "Bullets")) as TextAsset;
			TextReader reader = new StringReader(bulletData.text);
			types[i] = (CharacterData)serializer.Deserialize(reader);
		}
		bulletPool = new List<GameObject>();
		bulletPrefab = Resources.Load<GameObject>("prefabs/Bullet");
	}

	public GameObject GetBullet () {
		GameObject obj;
		int lastAvailableIndex = bulletPool.Count - 1;
		if (lastAvailableIndex >= 0) {
			obj = bulletPool[lastAvailableIndex];
			bulletPool.RemoveAt(lastAvailableIndex);
			obj.gameObject.SetActive(true);
		}
		else {
			obj = Instantiate<GameObject>(bulletPrefab);
			obj.GetComponent<BulletLogic>().theDepot = this;
			}
		return obj;
	}

	public void AddObject (GameObject obj) {
		obj.gameObject.SetActive(false);
		bulletPool.Add(obj);
	}
}
