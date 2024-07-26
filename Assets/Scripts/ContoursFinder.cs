using OpenCvSharp;
using OpenCvSharp.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 參考資料
// https://www.youtube.com/watch?v=ZV5eejYG6NI

public class ContoursFinder : WebCamera
{
	[SerializeField] private FlipMode imageFlip;
	[SerializeField] private float threshold = 96.4f;
	[SerializeField] private bool bShowProcessingImage = true;
	[SerializeField] private float curveAccuracy = 10f;
	[SerializeField] private float minArea = 5000f;
	[SerializeField] private PolygonCollider2D polygonCollider2D;

	private Mat image;
	private Mat processImage = new Mat();
	private Point[][] contoursArr;
	private HierarchyIndex[] hierachy;
	private Vector2[] vectorArr;

	protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
	{
		image = OpenCvSharp.Unity.TextureToMat(input);

		Cv2.Flip(image, image, imageFlip);
		Cv2.CvtColor(image, processImage, ColorConversionCodes.BGR2GRAY);
		Cv2.Threshold(processImage, processImage, threshold, 255, ThresholdTypes.BinaryInv);
		Cv2.FindContours(processImage, out contoursArr, out hierachy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);

		polygonCollider2D.pathCount = 0;

		foreach (Point[] contour in contoursArr)
		{
			Point[] points	= Cv2.ApproxPolyDP(contour, curveAccuracy, true);
			double area		= Cv2.ContourArea(contour);

			if (area > minArea)
			{
				DrawContours(processImage, new Scalar(127, 127, 127), 2, points);

				++polygonCollider2D.pathCount;
				polygonCollider2D.SetPath(polygonCollider2D.pathCount - 1, ToVector2(contour));
			}
			else
			{

			}
		}

		if (output == null)
		{
			output = OpenCvSharp.Unity.MatToTexture(bShowProcessingImage ? processImage : image);
		}
		else
		{
			OpenCvSharp.Unity.MatToTexture(bShowProcessingImage ? processImage : image, output);
		}

		return true;
	}

	private Vector2[] ToVector2(Point[] pointArr)
	{
		vectorArr = new Vector2[pointArr.Length];
		for(int i = 0; i < pointArr.Length; ++i)
		{
			vectorArr[i] = new Vector2(pointArr[i].X, pointArr[i].Y);
		}
		return vectorArr;
	}

	private void DrawContours(Mat image, Scalar color, int thickness, Point[] pointArr)
	{
		for(int i = 1; i < pointArr.Length; ++i)
		{
			Cv2.Line(image, pointArr[i - 1], pointArr[i], color, thickness);
		}

		Cv2.Line(image, pointArr[pointArr.Length - 1], pointArr[0], color, thickness);
	}
}
