using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class EyeOpening : MonoBehaviour
{
	public int mapWidth = 1920;
	public int mapHeight = 1080;
	public float planeDistance = 0.01f;

	[Range(0, 1)]
	public float animationRate = 0;

	float animationTime;

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
		textureRender.sharedMaterial.mainTexture = texture;
		transform.localPosition = Mathf.Max(mainCamera.nearClipPlane + 0.01f, planeDistance) * Vector3.forward;
		transform.localRotation = Quaternion.identity;

		animationTime = 1.1f;
		animationRunning = false;
		SetUniformColor(eyeClosed);
	}

    private void Update()
	{
		// Update texture
		if (animationRunning)
		{
			UpdateTexture();

			animationTime += Time.deltaTime * animationRate;

			if (animationTime > 1f)
            {
				animationRunning = false;
				SetUniformColor(eyeOpen);
			}
		}
	}

	public void ResetAnimation()
    {
		animationTime = 0.001f;
		animationRunning = true;
    }

	private void UpdateTexture()
    {
		if (animationTime > 1f)
		{
			return;
		}

		float midH = mapHeight / 2f;
		for (int i = 0; i < mapHeight; i++)
		{
			for (int j = 0; j < mapWidth; j++)
			{
				if (Mathf.Abs(i - midH) > animationTime * midH)
				{
					texture.SetPixel(i, j, eyeClosed);
				} else
				{
					texture.SetPixel(i, j, Color.Lerp(eyeOpen, eyeClosed, (i - midH)/midH));
				}
			}
		}
		texture.Apply();
		textureRender.sharedMaterial.mainTexture = texture;
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
		// Could all this function be replaced by setting camera's field of view height ?
		float fov = mainCamera.fieldOfView * Mathf.Deg2Rad;
		float aspect = mainCamera.aspect;
		float dist = Mathf.Max(mainCamera.nearClipPlane + 0.01f, planeDistance);
		// Note this additional factor 2 that avoid see white lines in the edge of the vision
		float width = Mathf.Abs(2f * dist * Mathf.Tan(fov / 2f)) * 2;
		float height = width / aspect * 2;

		Vector3[] vertices;
		Vector2[] uvs = new Vector2[4];
		int[] triangles;

		Vector3 right = width / 2 * Vector3.right;
		Vector3 up = height / 2 * Vector3.up;
		vertices = new Vector3[] { 
			- right + up, 
			  right + up, 
			  right - up, 
			- right - up
		};

		uvs[0] = new Vector2(0, 1);
		uvs[1] = new Vector2(0, 0);
		uvs[2] = new Vector2(1, 0);
		uvs[3] = new Vector2(1, 1);

		triangles = new int[] {
			0, 1, 2,
			2, 3, 0
		};

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