using UnityEditor;
using UnityEngine;

public class FPSchecker : EditorWindow
{
	string myString = "Hello World";
	bool groupEnabled;
	bool myBool;
	float myFloat;
	
	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/My Window")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(FPSchecker));
	}
	
	void OnGUI()
	{
		GUILayout.Label ("FPSchecker", EditorStyles.boldLabel);
		myString = EditorGUILayout.TextField ("Text Field", myString);
		
		groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
		if (groupEnabled) {
			getFPS ();
			myBool = getVsync ();
		}
		GUILayout.Label ("Vsync:"+myBool);
		myFloat = EditorGUILayout.Slider ("targetFPS", myFloat, 30, 120);
		Application.targetFrameRate = (int)myFloat;
		EditorGUILayout.EndToggleGroup ();
	}
	bool getVsync()
	{
		bool isVsync = false;
		if (QualitySettings.vSyncCount != 0) {
			isVsync=true;
		}
		return isVsync;
	}
	void getFPS()
	{
		myFloat = Application.targetFrameRate;
	}
}