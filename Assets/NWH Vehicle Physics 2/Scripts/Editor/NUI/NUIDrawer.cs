using System;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NWH.NUI
{
    /// <summary>
    ///     A small Editor GUI library designed as a replacement for EditorGUILayout.
    /// </summary>
    public class NUIDrawer
    {
        public enum DrawerType
        {
            Property,
            Editor
        }

        private DrawerType _drawerType = DrawerType.Property;
        private Rect customPositionRect = new Rect(-1, -1, -1, -1);
        private string parentKey;
        public Rect positionRect = new Rect(-1, -1, -1, -1);
        public SerializedObject serializedObject;
        public SerializedProperty serializedProperty;

        private float sidePadding = 6f;
        public float totalHeight;

        private GUIStyle bgStyle = new GUIStyle();
        private GUIStyle unitStyle = new GUIStyle();

        private string _cachedKey;
        private Color _bgColor;

        private Texture2D _expandedArrow;
        private Texture2D _foldedArrow;

        /// <summary>
        ///     Bug fix for SerializedObject.isExpanded not serializing property in 2019.3.0f5
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                bool isExpanded = true;
                EditorCache.GetCachedValue("isExpanded", ref isExpanded, _cachedKey);
                return isExpanded;
            }
            set { EditorCache.SetCachedValue("isExpanded", value, _cachedKey); }
        }

        private void InitializeGUIElements()
        {
            // Background style
            bgStyle = EditorStyles.helpBox;

            // Initialize styles
            unitStyle.fontSize = 9;
            unitStyle.alignment = TextAnchor.MiddleRight;
            unitStyle.normal.textColor = Color.grey;

            // Get textures
            _expandedArrow = Resources.Load(NUISettings.RESOURCES_PATH + "arrow_expanded") as Texture2D;
            _foldedArrow = Resources.Load(NUISettings.RESOURCES_PATH + "arrow_folded") as Texture2D;
        }

        public void UpdateCachedKey()
        {
            _cachedKey = GenerateKey();
        }

        public static string GenerateKey(SerializedProperty property)
        {
            return GenerateKey(property.serializedObject) + property.propertyPath;
        }

        public static string GenerateKey(SerializedObject obj)
        {
            return obj.targetObject.GetInstanceID().ToString();
        }

        public static string GenerateKey(Object obj)
        {
            return obj.GetInstanceID().ToString();
        }

        public static int GetPropertyDepth(SerializedProperty property)
        {
            return property.propertyPath.Split('.').Length - 1;
        }

        public static void SetTabIndex(string tabName, int value)
        {
            //EditorCache.SetCachedValue("tabIndex", value, tabName);
            EditorPrefs.SetInt("NWH" + tabName, value);
        }

        public void AdvancePosition(float height = NUISettings.FIELD_HEIGHT)
        {
            positionRect = new Rect(positionRect.x, positionRect.y + height, positionRect.width,
                NUISettings.FIELD_HEIGHT);
            totalHeight += height;
        }

        public void BeginEditor(SerializedObject serializedObject)
        {
            _drawerType = DrawerType.Editor;
            this.serializedObject = serializedObject;
            serializedProperty = null;
            _cachedKey = GenerateKey();

            if (serializedObject == null)
            {
                Debug.LogError("Cannot draw editor for null serializedObject.");
                return;
            }

            serializedObject.Update();

            if (customPositionRect.x < 0 && customPositionRect.width < 0)
            {
                positionRect = EditorGUILayout.GetControlRect();
                positionRect.x = positionRect.x - 3;
                positionRect.width = positionRect.width + 6;
            }
            else
            {
                positionRect = customPositionRect;
            }

            totalHeight = 0;
            InitializeGUIElements();
            EditorGUI.BeginChangeCheck();

            DrawBackgroundRect(new Rect(positionRect.x, positionRect.y, positionRect.width, GetHeight()));
        }

        public void BeginProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property == null)
            {
                Debug.LogError("Cannot draw property drawer for null property.");
                return;
            }

            _drawerType = DrawerType.Property;
            serializedProperty = property;
            serializedObject = property.serializedObject;
            _cachedKey = GenerateKey();

            totalHeight = 0;
            positionRect = position;

            bool guiWasEnabled = true;
            if (EditorCache.GetCachedValue("guiWasEnabled", ref guiWasEnabled, GenerateKey()))
            {
                GUI.enabled = guiWasEnabled;
            }

            InitializeGUIElements();
            
            Rect totalRect = new Rect(positionRect.x, positionRect.y, positionRect.width, GetHeight());
            DrawBackgroundRect(new Rect(positionRect.x, positionRect.y, positionRect.width, GetHeight()));
            //EditorGUI.BeginProperty(totalRect, label, property);

            EditorGUI.BeginChangeCheck();
        }

        public void BeginSubsection(string label)
        {
            Title(label);
            IncreaseIndent();
        }

        public bool Button(string label, GUIStyle style = null)
        {
            Rect buttonRect = positionRect;
            buttonRect.y += 2f;
            buttonRect.height = NUISettings.FIELD_HEIGHT - 4f;

            GUIStyle buttonStyle = style != null ? new GUIStyle(style) : new GUIStyle(EditorStyles.miniButton);
            buttonStyle.fixedHeight = buttonRect.height;

            bool buttonState = GUI.Button(buttonRect, label, buttonStyle);

            AdvancePosition(buttonRect.height + NUISettings.FIELD_SPACING);
            return buttonState;
        }

        public void DecreaseIndent()
        {
            positionRect = new Rect(positionRect.x - 6, positionRect.y, positionRect.width + 6, positionRect.height);
        }

        public void DrawBackgroundRect(Rect rect)
        {
            if (rect.height < 2f)
            {
                return;
            }

            GUI.Box(rect, "", bgStyle);
            EditorGUI.DrawRect(rect, new Color(1, 1, 1, 0.1f));
        }

        public void DrawEditorTexture(Rect rect, string path, ScaleMode scaleMode = ScaleMode.StretchToFill)
        {
            Texture2D tex = GetTexture(path);
            GUI.DrawTexture(rect, tex, scaleMode);
        }

        public void EmbeddedObjectEditor(Object obj, Rect rect, float leftMargin = 0)
        {
            if (obj == null)
            {
                return;
            }

            Editor editor = null;
            string key = GenerateKey(obj);
            EditorCache.GetCachedValue("NUIEditor", ref editor, key);
            NUIEditor nuiEditor = editor as NUIEditor;

            if (nuiEditor == null)
            {
                nuiEditor = Editor.CreateEditor(obj) as NUIEditor;

                if (nuiEditor == null)
                {
                    Debug.LogError("Failed to create scriptable object editor.");
                    return;
                }

                EditorCache.SetCachedValue("NUIEditor", nuiEditor, GenerateKey(obj));
            }

            rect.y += 8;

            if (nuiEditor != null)
            {
                Space();
                nuiEditor.drawer.customPositionRect = positionRect;
                nuiEditor.drawer.parentKey = GenerateKey();
                nuiEditor.OnInspectorNUI();

                float editorHeight = GetHeight(key);
                GUILayout.Space(
                    -editorHeight); // Negate space that embedded editor has already added to prevent overly-large final editor
                AdvancePosition(editorHeight);
            }
            else
            {
                Debug.LogError("Cannot draw null editor.");
            }

            UpdateCachedKey();
        }

        public void EndEditor(NUIEditor nuiEditor = null)
        {
            if (totalHeight > 32f)
            {
                totalHeight += 5; // Add padding if expanded
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.targetObject != null)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }

            SetHeight(totalHeight);
            GUILayout.Space(totalHeight);
        }

        public virtual void EndProperty()
        {
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            if (totalHeight > 30f)
            {
                Space(6); // Add padding if expanded
            }

            bool wasEnabled = true;
            EditorCache.GetCachedValue("guiWasEnabled", ref wasEnabled, GenerateKey());
            if (!GUI.enabled && wasEnabled)
            {
                EditorGUI.EndDisabledGroup();
            }

            SetHeight(totalHeight);
            GUI.enabled = true;
        }

        public void EndSubsection()
        {
            DecreaseIndent();
            Space();
        }

        public SerializedProperty Field(string propertyRelativeName, bool enabled = true, string unit = null,
            string alternateLabel = null, float fieldHeight = -1)
        {
            SerializedProperty property = FindProperty(propertyRelativeName);

            if (property == null)
            {
                Debug.LogWarning($"Could not find property '{propertyRelativeName}'");
                return null;
            }

            bool guiWasEnabled = GUI.enabled;
            EditorGUI.BeginDisabledGroup(!enabled);

            if (fieldHeight < 0)
            {
                fieldHeight = (int) EditorGUI.GetPropertyHeight(property, false);
            }

            Rect fieldRect = positionRect;
            fieldRect.height = fieldHeight;

            // Draw field
            if (string.IsNullOrEmpty(alternateLabel))
            {
                EditorGUI.PropertyField(fieldRect, property, true);
            }
            else
            {
                EditorGUI.PropertyField(fieldRect, property, new GUIContent(alternateLabel), true);
            }

            // Draw unit
            if (unit != null)
            {
                float unitRectWidth = 8f + unit.Length * 8f;
                Rect unitRect = new Rect(fieldRect.x + fieldRect.width - unitRectWidth - 5, fieldRect.y,
                    unitRectWidth, fieldRect.height);
                EditorGUI.LabelField(unitRect, new GUIContent(unit), unitStyle);
            }

            if (guiWasEnabled)
            {
                EditorGUI.EndDisabledGroup();
            }

            AdvancePosition(fieldHeight + NUISettings.FIELD_SPACING);
            return property;
        }

        public SerializedProperty FindProperty(string name)
        {
            return _drawerType == DrawerType.Property
                ? serializedProperty.FindPropertyRelative(name)
                : serializedObject.FindProperty(name);
        }

        public string GenerateKey()
        {
            return _drawerType == DrawerType.Property ? GenerateKey(serializedProperty) : GenerateKey(serializedObject);
        }

        public float GetHeight()
        {
            return GetHeight(_cachedKey);
        }

        public float GetHeight(string key)
        {
            float height = 0;
            EditorCache.GetCachedValue("height", ref height, key);
            return height;
        }

        public T GetObject<T>() where T : class
        {
            if (_drawerType == DrawerType.Property)
            {
                return SerializedPropertyHelper.GetTargetObjectOfProperty(serializedProperty) as T;
            }

            return serializedObject.targetObject as T;
        }

        public int GetTabIndex(string tabName)
        {
            return EditorPrefs.GetInt("NWH" + tabName, 0);
        }

        public Texture2D GetTexture(string resourcesPath)
        {
            Texture2D tex = null;
            EditorCache.GetCachedValue("Texture2D", ref tex, resourcesPath);
            if (tex == null)
            {
                tex = Resources.Load(resourcesPath) as Texture2D;
                if (tex == null)
                {
                    Debug.LogError($"{resourcesPath} not found or not Texture2D.");
                }
                else
                {
                    EditorCache.SetCachedValue("Texture2D", tex, resourcesPath);
                }
            }

            return tex;
        }

        public bool Header(string label, bool drawFoldoutButton = true)
        {
            label = AddSpacesToSentence(label, true);

            Rect backgroundRect = new Rect(positionRect.x, positionRect.y, positionRect.width, 22);

            GUIContent content = new GUIContent(label);
            EditorStyles.label.CalcMinMaxWidth(content, out float minLabelWidth, out float maxLabelWidth);

#if UNITY_2019_3_OR_NEWER
            Rect labelRect = new Rect(positionRect.x + NUISettings.textMargin + 15f, positionRect.y - 1, maxLabelWidth,
                NUISettings.FIELD_HEIGHT);
#else
            Rect labelRect = new Rect(positionRect.x + NUISettings.textMargin + 15f, positionRect.y + 3, maxLabelWidth,
                NUISettings.FIELD_HEIGHT);
#endif

            // If object, check if scriptable or normal
            bool isScriptableObject = false;
            if (_drawerType == DrawerType.Editor)
            {
                isScriptableObject = serializedObject.targetObject is ScriptableObject;
            }

            // Draw background
            Color bgColor = _drawerType == DrawerType.Property ? NUISettings.propertyHeaderColor :
                isScriptableObject ? NUISettings.scriptableObjectHeaderColor : NUISettings.editorHeaderColor;
            EditorGUI.DrawRect(backgroundRect, bgColor);

            // Draw foldout button
            bool _isExpanded = true;
            _isExpanded =
                IsExpanded; // Due to a bug serializedProperty.isExpanded will not serialize property, use cache instead
            GUI.color = Color.white;
            Rect foldButtonRect = new Rect(positionRect.x + 1, positionRect.y + 2, 18f, 18f);
            GUIStyle foldoutButtonStyle = new GUIStyle();
            Texture2D background = _isExpanded ? _expandedArrow : _foldedArrow;
            foldoutButtonStyle.normal.background = background;
            foldoutButtonStyle.hover.background = background;
            foldoutButtonStyle.active.background = background;

            if (GUI.Button(foldButtonRect, "", foldoutButtonStyle))
            {
                IsExpanded = !IsExpanded;
                _isExpanded = IsExpanded;
            }

            // Draw label
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.normal.textColor = Color.white;
            EditorGUI.LabelField(labelRect, new GUIContent(label), style);

            // Draw help link
            Rect helpRect = new Rect(labelRect.x + labelRect.width + 5f, positionRect.y + 3, 16, 16);
            DrawEditorTexture(helpRect, "NUI/help");
            if (Event.current.type == EventType.MouseUp && helpRect.Contains(Event.current.mousePosition))
            {
                string fullTypeName = _drawerType == DrawerType.Property
                    ? SerializedPropertyHelper.GetTargetObjectOfProperty(serializedProperty).GetType().FullName
                    : serializedObject.targetObject.GetType().FullName;
                string objectPath = fullTypeName.Replace('.', '/');
                Application.OpenURL($"{NUISettings.DOCUMENTATION_BASE_URL}/{objectPath}");
            }

            AdvancePosition();

            // Apply padding
            positionRect = new Rect(
                positionRect.x + sidePadding,
                positionRect.y,
                positionRect.width - sidePadding * 2f,
                positionRect.height);

            if (_drawerType == DrawerType.Editor && _isExpanded)
            {
                if (serializedObject.targetObject is ScriptableObject)
                {
                    Info("This is a ScriptableObject. All changes are global.");
                }
            }

            SetHeight(totalHeight);
            return _isExpanded;
        }

        public void HorizontalRuler(float thickness = 1f)
        {
            EditorGUI.DrawRect(new Rect(positionRect.x, positionRect.y + 2, positionRect.width, thickness),
                new Color(0.5f, 0.5f, 0.5f, 1f));
            AdvancePosition(thickness + 4);
        }

        public int HorizontalToolbar(string name, string[] textureResourcePaths, float toolbarHeight)
        {
            int tabIndex = GetTabIndex(name);

            int n = textureResourcePaths.Length;

            if (n == 0)
            {
                return -1;
            }

            if (positionRect.width > 20f)
            {
                Rect initRect = positionRect;
                float rowWidth = positionRect.width;

                Rect bottomLine = new Rect(initRect.x, initRect.y + toolbarHeight, initRect.width, 1f);
                EditorGUI.DrawRect(bottomLine, new Color(0.4f, 0.4f, 0.5f, 1f));

                int counter = 0;
                float buttonWidth = rowWidth / n;

                Texture2D[] icons = new Texture2D[n];
                for (int i = 0; i < n; i++)
                {
                    icons[i] = GetTexture(textureResourcePaths[i]);
                }

                for (int i = 0; i < n; i++)
                {
                    GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonMid);
                    buttonStyle.fixedHeight = toolbarHeight;

                    Color initColor = GUI.color;
                    Rect buttonRect = new Rect(initRect.x + i * buttonWidth, initRect.y, buttonWidth, toolbarHeight);

                    if (tabIndex == counter)
                    {
                        GUI.color = new Color(1f, 1f, 1f, 1f);
                    }
                    else
                    {
                        GUI.color = new Color(0.9f, 0.9f, 0.9f, 1f);
                    }

                    if (GUI.Button(buttonRect, icons[i], buttonStyle))
                    {
                        tabIndex = counter;
                    }

                    if (tabIndex == counter)
                    {
                        Rect highlightRect = new Rect(buttonRect.x, buttonRect.y + buttonRect.height - 3,
                            buttonRect.width,
                            3);
                        EditorGUI.DrawRect(highlightRect, NUISettings.lightBlueColor);
                    }

                    GUI.color = initColor;
                    counter++;
                }

                SetTabIndex(name, tabIndex);
                EditorCache.SetCachedValue("height", toolbarHeight, GenerateKey() + name);
            }
            else
            {
                EditorCache.GetCachedValue("height", ref toolbarHeight, GenerateKey() + name);
            }

            AdvancePosition(toolbarHeight + 2);
            return tabIndex;
        }

        public int HorizontalToolbar(string name, string[] texts, bool fillWidth = true, bool singleLine = false,
            float targetButtonWidth = 80f, float buttonHeight = 18f)
        {
            float toolbarHeight = 0;
            int tabIndex = GetTabIndex(name);

            if (positionRect.width > 20f)
            {
                Rect initRect = positionRect;
                float rowHeight = buttonHeight;
                int buttonCount = texts.Length;
                float rowWidth = positionRect.width;
                float singleLineWidth = buttonCount * targetButtonWidth;
                int rowCount = singleLine ? 1 : rowWidth > 10 ? Mathf.CeilToInt(singleLineWidth / rowWidth) : 1;
                int maxButtonsPerRow = Mathf.FloorToInt(rowWidth / targetButtonWidth);
                int lastRowButtons = buttonCount - maxButtonsPerRow * (rowCount - 1);
                toolbarHeight = rowCount * rowHeight;

                Rect bottomLine = new Rect(initRect.x, initRect.y + toolbarHeight, initRect.width, 1f);
                EditorGUI.DrawRect(bottomLine, new Color(0.4f, 0.4f, 0.5f, 1f));

                string[][] subTexts = new string[rowCount][];
                int offset = 0;
                for (int i = 0; i < rowCount; i++)
                {
                    if (i == rowCount - 1)
                    {
                        subTexts[i] = new string[lastRowButtons];
                        for (int j = 0; j < lastRowButtons; j++)
                        {
                            subTexts[i][j] = texts[offset++];
                        }
                    }
                    else
                    {
                        subTexts[i] = new string[maxButtonsPerRow];
                        for (int j = 0; j < maxButtonsPerRow; j++)
                        {
                            subTexts[i][j] = texts[offset++];
                        }
                    }
                }


                int counter = 0;
                for (int x = 0; x < rowCount; x++)
                {
                    float buttonWidth = fillWidth
                        ? x == rowCount - 1 ? rowWidth / lastRowButtons : rowWidth / maxButtonsPerRow
                        : targetButtonWidth;
                    for (int y = 0; y < subTexts[x].Length; y++)
                    {
                        GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButtonMid);
                        buttonStyle.fixedHeight = buttonHeight;

                        Color initColor = GUI.color;

                        Rect buttonRect = new Rect(initRect.x + y * buttonWidth, initRect.y, buttonWidth,
                            rowHeight + 1);

                        if (tabIndex == counter)
                        {
                            GUI.color = new Color(1f, 1f, 1f, 1f);
                        }
                        else
                        {
                            GUI.color = new Color(0.9f, 0.9f, 0.9f, 1f);
                        }

                        if (GUI.Button(buttonRect, subTexts[x][y], buttonStyle))
                        {
                            tabIndex = counter;
                        }

                        if (tabIndex == counter)
                        {
                            Rect highlightRect = new Rect(buttonRect.x, buttonRect.y + buttonRect.height - 3,
                                buttonRect.width,
                                3);
                            EditorGUI.DrawRect(highlightRect, NUISettings.lightBlueColor);
                        }

                        GUI.color = initColor;
                        counter++;
                    }

                    initRect.y += rowHeight;
                }

                SetTabIndex(name, tabIndex);
                EditorCache.SetCachedValue("height", toolbarHeight, GenerateKey() + name);
            }
            else
            {
                EditorCache.GetCachedValue("height", ref toolbarHeight, GenerateKey() + name);
            }

            AdvancePosition(toolbarHeight + 2);
            return tabIndex;
        }

        public void IncreaseIndent()
        {
            positionRect = new Rect(positionRect.x + 6, positionRect.y, positionRect.width - 6, positionRect.height);
        }

        public void Indent(int indent, float step = 10f)
        {
            positionRect = new Rect(positionRect.x + indent * step, positionRect.y, positionRect.width - indent * step,
                positionRect.height);
        }

        public void Info(string text, MessageType messageType = MessageType.Info)
        {
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.alignment = TextAnchor.MiddleCenter;
            style.wordWrap = true;

            float height = 0;
            if (positionRect.width > 20f)
            {
                float width = positionRect.width;
                GUIContent content = new GUIContent(text);
                height = style.CalcHeight(content, positionRect.width);
                style.fixedHeight = height;
                Rect infoRect = new Rect(positionRect.x, positionRect.y, positionRect.width, height);
                EditorGUI.HelpBox(infoRect, text, messageType);

                EditorCache.SetCachedValue("height", height,
                    GenerateKey() + text.GetHashCode()); // not the most optimal
            }
            else
            {
                EditorCache.GetCachedValue("height", ref height, GenerateKey() + text.GetHashCode());
            }

            AdvancePosition(height + NUISettings.FIELD_SPACING);
        }

        public void Label(string label, bool bold = false, bool active = true)
        {
            bool wasActive = GUI.enabled;
            EditorGUI.BeginDisabledGroup(!active);
            GUIStyle style = bold ? EditorStyles.boldLabel : EditorStyles.label;
            EditorGUI.LabelField(new Rect(positionRect.x, positionRect.y, positionRect.width, NUISettings.FIELD_HEIGHT),
                new GUIContent(label), style);
            if (wasActive)
            {
                EditorGUI.EndDisabledGroup();
            }

            AdvancePosition();
        }


        public void Property(string propertyName, bool drawChildren = true, bool expanded = true)
        {
            Property(FindProperty(propertyName), drawChildren, expanded);
        }

        public void Property(SerializedProperty p, bool includeChildren = true, bool expanded = true,
            bool disabled = false)
        {
            if (p == null)
            {
                Debug.LogWarning("Property could not be found.");
                return;
            }

            bool wasEnabled = GUI.enabled;
            EditorGUI.BeginDisabledGroup(disabled);

            p.isExpanded = expanded;
            EditorGUI.PropertyField(positionRect, p, includeChildren);

            AdvancePosition(GetHeight(GenerateKey(p)) + NUISettings.FIELD_SPACING);

            if (wasEnabled)
            {
                EditorGUI.EndDisabledGroup();
            }

            UpdateCachedKey();
        }

        public void ReorderableList(string propertyName, string label = null, bool draggable = true,
            bool showAddRemoveButtons = true,
            Type baseType = null, float elementSpacing = 0)
        {
            SerializedProperty listProperty = FindProperty(propertyName);
            if (listProperty == null)
            {
                Debug.LogError("Property is null.");
                return;
            }

            ReorderableList reorderableList = null;
            EditorCache.GetCachedValue("ReorderableList", ref reorderableList, GenerateKey(listProperty));
            if (reorderableList == null)
            {
                reorderableList = new ReorderableList(listProperty.serializedObject, listProperty, draggable,
                    true, true, true);
            }

            reorderableList.serializedProperty = listProperty;
            reorderableList.displayAdd = showAddRemoveButtons;
            reorderableList.displayRemove = showAddRemoveButtons;

            string headerLabel = string.IsNullOrEmpty(label) ? listProperty.displayName : label;
            reorderableList.drawHeaderCallback = rect => { EditorGUI.LabelField(rect, new GUIContent(headerLabel)); };

            reorderableList.drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty p = listProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, p, true);
            };

            reorderableList.elementHeightCallback = index =>
            {
                SerializedProperty p = listProperty.GetArrayElementAtIndex(index);
                float height = EditorGUI.GetPropertyHeight(p, true);
                return height + elementSpacing;
            };

            float listHeight = reorderableList.GetHeight();
            Rect listRect = new Rect(positionRect.x, positionRect.y + 3f, positionRect.width, listHeight);
            reorderableList.DoList(listRect);

            AdvancePosition(listHeight + NUISettings.FIELD_SPACING);

            EditorCache.SetCachedValue("ReorderableList", reorderableList, GenerateKey(listProperty));
        }

        public void SetHeight(float height)
        {
            SetHeight(_cachedKey, height);
        }

        public void SetHeight(string key, float height)
        {
            EditorCache.SetCachedValue("height", height, key);
        }

        public void Space(float spaceSize = 5f)
        {
            AdvancePosition(spaceSize);
        }

        public void SplitRectVertically(Rect inRect, float splitPoint, out Rect rectA, out Rect rectB,
            float centerMargin = 4f)
        {
            float rectAWidth = inRect.width * splitPoint;
            rectA = new Rect(inRect.x, inRect.y, rectAWidth, inRect.height);
            rectB = new Rect(inRect.x + rectAWidth + centerMargin, inRect.y, inRect.width - rectAWidth, inRect.height);
        }

        public void Title(string label)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.black;
#if UNITY_2019_3_OR_NEWER
            EditorGUI.LabelField(positionRect, new GUIContent(" " + label), style);
#else
            EditorGUI.LabelField(new Rect(positionRect.x, positionRect.y + 3f, positionRect.width, positionRect.height),
                new GUIContent(" " + label), style);
#endif
            EditorGUI.DrawRect(new Rect(positionRect.x, positionRect.y, positionRect.width, 1f),
                NUISettings.lightGreyColor);
            AdvancePosition();
        }

        private string AddSpacesToSentence(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                {
                    if (text[i - 1] != ' ' && !char.IsUpper(text[i - 1]) ||
                        preserveAcronyms && char.IsUpper(text[i - 1]) &&
                        i < text.Length - 1 && !char.IsUpper(text[i + 1]))
                    {
                        newText.Append(' ');
                    }
                }

                newText.Append(text[i]);
            }

            return newText.ToString();
        }
    }
}