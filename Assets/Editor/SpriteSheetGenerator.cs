using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class SpriteSheetGenerator : MonoBehaviour {

	static int frameRate = 12;
	static int spriteWidth = 64;
	static int spriteHeight = 64;
	static void SliceSprite(string name) {

		string filename = string.Concat("SpriteSheet", name);
		Texture2D myTexture = (Texture2D)Resources.Load<Texture2D>(string.Concat("sprites/", name, "Animation/p1/", filename));

         string path = AssetDatabase.GetAssetPath(myTexture);
         TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
         ti.isReadable = true;
         ti.spriteImportMode = SpriteImportMode.Multiple;
 
         List<SpriteMetaData> newData = new List<SpriteMetaData>();
 
         int SliceWidth = spriteWidth;
         int SliceHeight = spriteHeight; 
 
         for (int i = 0; i < myTexture.width; i += SliceWidth)
         {
             for(int j = myTexture.height; j > 0;  j -= SliceHeight)
             {
                 SpriteMetaData smd = new SpriteMetaData();
                 smd.pivot = new Vector2(0.5f, 0.5f);
                 smd.alignment = 9;
                 smd.name = (myTexture.height - j)/SliceHeight + ", " + i/SliceWidth;
                 smd.rect = new Rect(i, j-SliceHeight, SliceWidth, SliceHeight);
 
                 newData.Add(smd);
             }
         }
 
         ti.spritesheet = newData.ToArray();
         AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
     }

	static void GenerateSpriteSheet() {
		string[] cmdArgs = System.Environment.GetCommandLineArgs();
		string characterName = cmdArgs[cmdArgs.Length - 1];
		Debug.Log(characterName);
		SliceSprite(characterName);
		Sprite[] sprites = Resources.LoadAll<Sprite>(string.Concat("sprites/", characterName, "Animation/p1/", characterName)); // load all sprites in "assets/Resources/sprite" folder
   		AnimationClip animClip = new AnimationClip();
  		animClip.frameRate = frameRate;
  		EditorCurveBinding spriteBinding = new EditorCurveBinding();
    	spriteBinding.type = typeof(SpriteRenderer);
    	spriteBinding.path = "";
    	spriteBinding.propertyName = "m_Sprite"; 
    	ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[sprites.Length];
    	for(int i = 0; i < (sprites.Length); i++) {
        	spriteKeyFrames[i] = new ObjectReferenceKeyframe();
        	spriteKeyFrames[i].time = i;
        	spriteKeyFrames[i].value = sprites[i];
    	}
		AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames);
		AssetDatabase.CreateAsset(animClip, "assets/walk.anim");
     	AssetDatabase.SaveAssets();
    
    }
}
