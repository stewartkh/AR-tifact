using System.IO;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
public class scri : MonoBehaviour
{
	// Concerning mesher--------------------------
	public GameObject mesher; //require

	public List<Vector3> vertices;
	public  List<int> triangles;

	public Vector3 point0;
	public Vector3 point1;
	public Vector3 point2;
	public Vector3 point3;

	public int loop;
	public float size;

	public Mesh meshFilterMesh;
	public Mesh meshColliderMesh;

	// Sprite work
	public Color[] pixels;

	public Texture2D newTexture;  
	public Texture2D oldTexture;  //require

	private Sprite mySprite;

	public int pathCount;

	public GameObject displayerComponent; //require

	public PolygonCollider2D polygonColliderAdded; //require

	void Start()
	{
		// Mesher
		vertices = new List<Vector3> (); 
		triangles = new List<int> (); 
		meshFilterMesh= mesher.GetComponent<MeshFilter>().mesh;
		meshColliderMesh= mesher.GetComponent<MeshCollider>().sharedMesh;
		size = 10; // lenght of the mesh in Z direction
		loop=0;

		// Sprite
		pixels = oldTexture.GetPixels();
		newTexture =new Texture2D(oldTexture.width,oldTexture.height,TextureFormat.ARGB32, false);

		ConvertSpriteAndCreateCollider (pixels);
		BrowseColliderToCreateMesh (polygonColliderAdded);
	}

	public void ConvertSpriteAndCreateCollider (Color[] pixels) {
		for (int i = 0 ; i < pixels.Length ; i++ ) 
		{ 
			// delete all black pixel (black is the circuit, white is the walls)
			if ((pixels[i].r==0 && pixels[i].g==0 && pixels[i].b==0 && pixels[i].a==1)) {
				pixels[i] = Color.clear;
			}
		}
		// Set a new texture with this pixel list
		newTexture.SetPixels(pixels);
		newTexture.Apply();

		// Create a sprite from this texture
		mySprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(10.0f,10.0f), 10.0f, 0, SpriteMeshType.Tight,new Vector4(0,0,0,0),false);

		// Add it to our displayerComponent
		displayerComponent.GetComponent<SpriteRenderer>().sprite=mySprite;

		// Add the polygon collider to our displayer Component and get his path count
		polygonColliderAdded = displayerComponent.AddComponent<PolygonCollider2D>();

	}

	// Method to browse the collider and launch makemesh
	public void BrowseColliderToCreateMesh (PolygonCollider2D polygonColliderAdded){
		//browse all path from collider
		pathCount=polygonColliderAdded.pathCount;
		for (int i = 0; i < pathCount; i++)
		{
			Vector2[] path = polygonColliderAdded.GetPath(i);

			// browse all path point
			for (int j = 1; j < path.Length; j++)
			{
				if (j != (path.Length - 1)) // if we aren't at the last point
				{
				point0 = new Vector3(path[j-1].x ,path[j-1].y ,0);
				point1 = new Vector3(path[j-1].x ,path[j-1].y ,size);
				point2 = new Vector3(path[j].x ,path[j].y ,size);
				point3 = new Vector3(path[j].x ,path[j].y ,0);
					MakeMesh(point0,point1,point2,point3);

				} 
				else if(j == (path.Length - 1))// if we are at the last point, we need to close the loop with the first point
				{
				point0 = new Vector3(path[j-1].x ,path[j-1].y ,0);
				point1 = new Vector3(path[j-1].x ,path[j-1].y ,size);
				point2 = new Vector3(path[j].x ,path[j].y ,size);
				point3 = new Vector3(path[j].x ,path[j].y ,0);
					MakeMesh(point0,point1,point2,point3);
				point0 = new Vector3(path[j].x ,path[j].y ,0);
				point1 = new Vector3(path[j].x ,path[j].y ,size);
				point2 = new Vector3(path[0].x ,path[0].y ,size); // First point
				point3 = new Vector3(path[0].x ,path[0].y ,0); // First point
					MakeMesh(point0,point1,point2,point3);
				}
			}
		}
	}


	//Method to generate 2 triangles mesh from the 4 points 0 1 2 3 and add it to the collider
	public void MakeMesh (Vector3 point0,Vector3 point1,Vector3 point2, Vector3 point3){

		// Vertice add
		vertices.Add(point0);
		vertices.Add(point1);
		vertices.Add(point2);
		vertices.Add(point3);

		//Triangle order
		triangles.Add(0+loop*4);
		triangles.Add(2+loop*4);
		triangles.Add(1+loop*4);
		triangles.Add(0+loop*4);
		triangles.Add(3+loop*4);
		triangles.Add(2+loop*4);
		loop = loop + 1; 

		// create mesh 
		meshFilterMesh.vertices=vertices.ToArray();
		meshFilterMesh.triangles=triangles.ToArray();

		// add this mesh to the MeshCollider
		mesher.GetComponent<MeshCollider>().sharedMesh=meshFilterMesh;
	}
}
