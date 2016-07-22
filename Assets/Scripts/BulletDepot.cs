using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class BulletDepot : ScriptableObject {

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
	public class Character {
		[XmlArray("Character")]
		[XmlArrayItem("ProjectileType")]
		public List<ProjectileType> projectileTypes = new List<ProjectileType>();

		[XmlAttribute("name")]
		public string characterName;
	}

	public Character[] types;
	// Use this for initialization
	public void Load () {
		types = new Character[2];
		var serializer = new XmlSerializer(typeof(Character));
		TextAsset bulletData = Resources.Load("orpheusBullets") as TextAsset;
		TextReader reader = new StringReader(bulletData.text);
		types[0] = (Character)serializer.Deserialize(reader);
		bulletData = Resources.Load("orpheusBullets") as TextAsset;
		reader = new StringReader(bulletData.text);
		types[1] = (Character)serializer.Deserialize(reader);
	}
}
