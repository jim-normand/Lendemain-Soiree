using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class EyeOpening : MonoBehaviour
{
	public int mapWidth = 19200;
	public int mapHeight = 1080;
	public float planeDistance = 0.05f;

	[Range(0, 1)]
	public float animationRate = 0;

	float t;

	bool animationRunning;

	public Material closedEyeMaterial;

	private Camera mainCamera;

	private MeshRenderer textureRender;
	Texture2D texture;

	// Black and opaque
	Color eyeClosed = new Color(0, 0, 0, 1);
	// Transparent
	Color eyeOpen = new Color(0, 0, 0, 0);

	private void Start()
	{
		mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

		transform.SetParent(mainCamera.transform);
		CreateMesh();

		textureRender = GetComponent<MeshRenderer>();
		texture = new Texture2D(mapWidth, mapHeight);
<<<<<<< HEAD:Assets/Animations/Eye opening/EyeOpening.cs
		textureRender.sharedMaterial.mainTexture = texture;
=======
		transform.localPosition = Mathf.Max(mainCamera.nearClipPlane + 0.01f, planeDistance) * Vector3.forward;
>>>>>>> 8f40639e6ee4d3fa9eb46e9060987807edfb1b79:Assets/Scripts/EyeOpening.cs

		t = 1.1f;
		animationRunning = false;
		SetUniformColor(eyeClosed);
	}

    private void Update()
	{
		// Update texture
		if (animationRunning)
		{
			UpdateTexture();

			t += Time.deltaTime * animationRate;

			if (t > 1f)
            {
				animationRunning = false;
				SetUniformColor(eyeOpen);
			}
		}
	}

	public void ResetAnimation()
    {
		t = 0.001f;
		animationRunning = true;
    }

	private void UpdateTexture()
    {
		if (t > 1f)
		{
			return;
		}

		float midH = mapHeight / 2f;
		for (int i = 0; i < mapHeight; i++)
		{
			for (int j = 0; j < mapWidth; j++)
			{
				if (Mathf.Abs(i - midH) > t * midH)
				{
					texture.SetPixel(i, j, eyeClosed);
				} else
				{
					texture.SetPixel(i, j, Color.Lerp(eyeOpen, eyeClosed, (i - midH)/midH));
				}
			}
		}
		texture.Apply();
	}

	private void SetUniformColor(Color color)
    {
		for (int i = 0; i < mapHeight; i++)
		{
			for (int j = 0; j < mapWidth; j++)
			{
				texture.SetPixel(i, j, color);
			}
		}
		texture.Apply();
	}

	private void CreateMesh()
    {
		// Could be replaced by setting camera's field of view height
		float fov = mainCamera.fieldOfView;
		float aspect = mainCamera.aspect;
		float dist = Mathf.Max(mainCamera.nearClipPlane + 0.01f, planeDistance);
		float width = Mathf.Abs(2f * dist * Mathf.Tan(fov / 2f));
		float height = width / aspect;

		Vector3[] vertices;
		Vector2[] uvs = new Vector2[4];
		int[] triangles = new int[6];

		// Very strange since vertices are in local space
		Vector3 right = width / 2 * mainCamera.transform.right;
		Vector3 up = height / 2 * mainCamera.transform.up;
		vertices = new Vector3[] { -right + up, right + up, right - up, - right - up};

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

		GetComponent<MeshFilter>().sharedMesh = mesh;
		GetComponent<MeshRenderer>().sharedMaterial = closedEyeMaterial;
		GetComponent<MeshFilter>().transform.localScale = Vector3.one;
		GetComponent<MeshRenderer>().transform.localScale = Vector3.one;
	}
}