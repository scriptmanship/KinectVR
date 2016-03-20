using UnityEngine;
using System.Collections;


public class PointCloudView : MonoBehaviour
{
	[Tooltip("Target mesh width in meters.")]
	public float meshWidth = 5.12f;

	[Tooltip("Target mesh height in meters.")]
	public float meshHeight = 4.24f;


    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;

	private KinectManager manager = null;

	private Vector2[] colorCoords = null;
	private ushort[] depthData = null;

	private int depthWidth = 0;
	private int depthHeight = 0;
	private int colorWidth = 0;
	private int colorHeight = 0;

	private const int SampleSize = 2;
	

    void Start()
    {
		manager = KinectManager.Instance;

		if (manager != null)
        {
			depthWidth = manager.GetDepthImageWidth();
			depthHeight = manager.GetDepthImageHeight();
			
			colorWidth = manager.GetColorImageWidth();
			colorHeight = manager.GetColorImageHeight();
			
			CreateMesh(depthWidth / SampleSize, depthHeight / SampleSize);
        }
    }

    void CreateMesh(int width, int height)
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[width * height];
        uvs = new Vector2[width * height];
        triangles = new int[6 * ((width - 1) * (height - 1))];

		float scaleX = meshWidth / width;
		float scaleY = meshHeight / height;

		float centerX = meshWidth / 2;
		float centerY = meshHeight / 2;

        int triangleIndex = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width) + x;

				float xScaled = x * scaleX - centerX;
				float yScaled = y * scaleY - centerY;

				vertices[index] = new Vector3(xScaled, -yScaled, 0);
                uvs[index] = new Vector2(((float)x / (float)width), ((float)y / (float)height));

                // Skip the last row/col
                if (x != (width - 1) && y != (height - 1))
                {
                    int topLeft = index;
                    int topRight = topLeft + 1;
                    int bottomLeft = topLeft + width;
                    int bottomRight = bottomLeft + 1;

                    triangles[triangleIndex++] = topLeft;
                    triangles[triangleIndex++] = topRight;
                    triangles[triangleIndex++] = bottomLeft;
                    triangles[triangleIndex++] = bottomLeft;
                    triangles[triangleIndex++] = topRight;
                    triangles[triangleIndex++] = bottomRight;
                }
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
    
    void Update()
    {
        if (manager == null)
            return;

		// get color texture
		gameObject.GetComponent<Renderer>().material.mainTexture = manager.GetUsersClrTex();

		// update the mesh
		UpdateMesh();
    }
    
    private void UpdateMesh()
    {
		if(manager.MapDepthFrameToColorCoords(ref colorCoords))
		{
			depthData = manager.GetRawDepthMap();

			for (int y = 0; y < depthHeight; y += SampleSize)
			{
				for (int x = 0; x < depthWidth; x += SampleSize)
				{
					int indexX = x / SampleSize;
					int indexY = y / SampleSize;
					int smallIndex = (indexY * (depthWidth / SampleSize)) + indexX;
					
					float avg = GetAvg(depthData, x, y);
					vertices[smallIndex].z = avg;
					
					// Update UV mapping with CDRP
					Vector2 colorCoord = colorCoords[(y * depthWidth) + x];
					uvs[smallIndex] = new Vector2(colorCoord.x / colorWidth, colorCoord.y / colorHeight);
				}
			}
			
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
		}
        
    }
    
    private float GetAvg(ushort[] depthData, int x, int y)
    {
        float sum = 0f;
        
		for (int y1 = y; y1 < y + SampleSize; y1++)
        {
			for (int x1 = x; x1 < x + SampleSize; x1++)
            {
                int fullIndex = (y1 * depthWidth) + x1;
                
                if (depthData[fullIndex] == 0)
                    sum += 4500;
                else
                    sum += depthData[fullIndex];
            }
        }

		return sum / (1000f * SampleSize * SampleSize);
    }

}
