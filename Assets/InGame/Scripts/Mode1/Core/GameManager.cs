using System.Collections.Generic;
using UnityEngine;

#region Editor
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private ReorderableList managersList;

    private void OnEnable()
    {
        managersList = new ReorderableList(serializedObject, serializedObject.FindProperty("managers"), true, true, true, true)
        {
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Managers", EditorStyles.boldLabel),
            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = managersList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            }
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        managersList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
#endregion

public class GameManager : manager
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
                instance?.Init();
            }
            return instance;
        }
    }

    [HideInInspector]
    [SerializeField] private List<manager> managers = new();

    private GameData gameData;

    /// <summary>
    /// Retrieves a manager of the specified type.
    /// </summary>
    public T GetManager<T>() where T : manager
    {
        return managers.Find(manager => manager is T) as T;
    }

    /// <summary>
    /// Displays a popup text animation.
    /// </summary>
    public void ShowPopUpText(string message)
    {
        var gameView = GetManager<GameView>();
        if (gameView != null)
        {
            var textPrefab = gameView.GetGameData()?.TextPrefab;
            if (textPrefab != null)
            {
                DoTweenAnimation.AnimatePopUpText(textPrefab, message, 1f, 2f);
            }
            else
            {
                Debug.LogError("TextPrefab is not assigned in GameData.");
            }
        }
        else
        {
            Debug.LogError("GameView manager not found.");
        }
    }

    /// <summary>
    /// Displays floating text above a specific transform in world space.
    /// </summary>
    public void ShowFloatingText(string message, Transform targetTransform)
    {
        var gameView = GetManager<GameView>();
        if (gameView != null)
        {
            var floatingTextPrefab = gameView.GetGameData()?.FloatingTextPrefab;
            if (floatingTextPrefab != null)
            {
                var offset = new Vector3(0, 2f, 0); // Offset above the target
                DoTweenAnimation.AnimateFloatingText(floatingTextPrefab, targetTransform.position + offset, message);
            }
            else
            {
                Debug.LogError("FloatingTextPrefab is not assigned in GameData.");
            }
        }
        else
        {
            Debug.LogError("GameView manager not found.");
        }
    }

    /// <summary>
    /// Initializes all managers.
    /// </summary>
    public override void Init()
    {
        foreach (var manager in managers)
        {
            manager?.Init();
        }
    }
}
