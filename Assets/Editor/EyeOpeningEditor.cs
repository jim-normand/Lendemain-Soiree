using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EyeOpening))]
public class EyeOpeningEditor : Editor
{
	public override void OnInspectorGUI()
	{
		EyeOpening eyeOpening = (EyeOpening)target;

		if (DrawDefaultInspector())
		{
			//Nothing
		}

		if (GUILayout.Button("Play animation"))
		{
			eyeOpening.ResetAnimation();
		}
	}
}