using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class EyeOpening : MonoBehaviour
{
	public int mapWidth = 100;
	public int mapHeight = 100;
	public float planeDistance = 0.05f;
	float t = 0f;

	public Material closedEyeMaterial;

	private Camera mainCamera;

	private MeshRenderer textureRender;
	Color[] colorMap;
	Texture2D texture;

	List<Vector3> vertices = new List<Vector3>();

	// Black and opaque
	Color eyeClosed = new Color(0, 0, 0, 1);
	// Transparent
	Color eyeOpen = new Color(0, 0, 0, 0);

	private void Start()
	{
		mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		colorMap = new Color[mapWidth * mapHeight];

		CreateMesh();

		textureRender = this.GetComponent<MeshRenderer>();
		texture = new Texture2D(mapWidth, mapHeight);
	}

    private void Update()
	{
		this.transform.localPosition = -Mathf.Max(mainCamera.nearClipPlane +0.01f, planeDistance) * mainCamera.transform.forward;
		if (t > 1f)
        {
			return;
        }
		for (int i = 0; i < mapWidth; i++)
        {
			for (int j = 0; j < mapHeight; j++)
			{
				texture.SetPixel(i, j, Color.Lerp(eyeClosed, eyeOpen, t));
			}
		}
		texture.Apply();

		textureRender.sharedMaterial.mainTexture = texture;
		t += 0.005f;
	}

	private void CreateMesh()
    {
		// The mesh could be actuated if the fov/aspect change during the execution...
		float fov = mainCamera.fieldOfView;
		float aspect = mainCamera.aspect;
		float dist = Mathf.Max(mainCamera.nearClipPlane + 0.01f, planeDistance);
		float width = Mathf.Abs(2f * dist * Mathf.Tan(fov / 2f));
		float height = width / aspect;

		Vector3[] vertices = new Vector3[4];
		Vector2[] uvs = new Vector2[4];
		int[] triangles = new int[6];

		this.transform.SetParent(mainCamera.transform);
		vertices[0] = - width / 2f * mainCamera.transform.right + height / 2f * mainCamera.transform.up;
		vertices[1] = + width / 2f * mainCamera.transform.right + height / 2f * mainCamera.transform.up;
		vertices[2] = + width / 2f * mainCamera.transform.right - height / 2f * mainCamera.transform.up;
		vertices[3] = - width / 2f * mainCamera.transform.right - height / 2f * mainCamera.transform.up;

		uvs[0] = new Vector2(0, 1);
		uvs[1] = new Vector2(0, 0);
		uvs[2] = new Vector2(1, 0);
		uvs[3] = new Vector2(1, 1);

		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 2;
		triangles[3] = 2;
		triangles[4] = 3;
		triangles[5] = 0;

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();

		this.GetComponent<MeshFilter>().sharedMesh = mesh;
		this.GetComponent<MeshRenderer>().sharedMaterial = closedEyeMaterial;
		this.GetComponent<MeshFilter>().transform.localScale = Vector3.one;
		this.GetComponent<MeshRenderer>().transform.localScale = Vector3.one;
	}
}