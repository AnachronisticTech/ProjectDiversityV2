using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HelperNamespace
{
    public static class EditorTools
    {
        public static void DrawDestinationArrow(Vector3 from, Vector3 to, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowPosition = 0.5f)
        {
            Gizmos.DrawLine(from, to);
            Vector3 direction = to - from;
            DrawArrowEnd(from, direction, arrowHeadLength, arrowHeadAngle, arrowPosition);
        }
        private static void DrawArrowEnd(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f, float arrowPosition = 0.5f)
        {
            //Vector3 right = (Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back) * arrowHeadLength;
            //Vector3 left = (Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back) * arrowHeadLength;
            Vector3 up = (Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back) * arrowHeadLength;
            Vector3 down = (Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back) * arrowHeadLength;

            Vector3 arrowTip = pos + (direction * arrowPosition);

            //Gizmos.DrawRay(arrowTip, right);
            //Gizmos.DrawRay(arrowTip, left);
            Gizmos.DrawRay(arrowTip, up);
            Gizmos.DrawRay(arrowTip, down);
        }

        public static Mesh DrawWedgeMesh(float angle, float distance, float height, int segments = 10)
        {
            Mesh mesh = new Mesh();

            int numTriangles = (segments * 4) + 4;
            int numVertices = numTriangles * 3;

            Vector3[] vertices = new Vector3[numVertices];
            int[] triangles = new int[numVertices];

            Vector3 bottomCenter = Vector3.zero;
            Vector3 bottomLeft = Quaternion.Euler(0, -angle / 2.0f, 0) * Vector3.forward * distance;
            Vector3 bottomRight = Quaternion.Euler(0, angle / 2.0f, 0) * Vector3.forward * distance;

            Vector3 topCenter = bottomCenter + Vector3.up * height;
            Vector3 topRight = bottomRight + Vector3.up * height;
            Vector3 topLeft = bottomLeft + Vector3.up * height;

            int vert = 0;

            // left side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = topLeft;

            vertices[vert++] = topLeft;
            vertices[vert++] = topCenter;
            vertices[vert++] = bottomCenter;

            // right side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = topCenter;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomCenter;

            float currentAngle = -angle / 2.0f;
            float deltaAngle = angle / segments;
            for (int i = 0; i < segments; i++)
            {
                bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
                bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

                topRight = bottomRight + Vector3.up * height;
                topLeft = bottomLeft + Vector3.up * height;

                // far side
                vertices[vert++] = bottomLeft;
                vertices[vert++] = bottomRight;
                vertices[vert++] = topRight;

                vertices[vert++] = topRight;
                vertices[vert++] = topLeft;
                vertices[vert++] = bottomLeft;

                // top
                vertices[vert++] = topCenter;
                vertices[vert++] = topLeft;
                vertices[vert++] = topRight;

                // bottom
                vertices[vert++] = bottomCenter;
                vertices[vert++] = bottomRight;
                vertices[vert++] = bottomLeft;

                currentAngle += deltaAngle;
            }

            for (int i = 0; i < numVertices; i++)
            {
                triangles[i] = i;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        public static void Line(float height = 1.0f, float topSpace = 5.0f, float bottomSpace = 5.0f, Color? lineColor = null)
        {
            GUILayout.Space(topSpace);
            
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;

            Color color = lineColor ?? Color.white;
            EditorGUI.DrawRect(rect, color);

            GUILayout.Space(bottomSpace);
        }

        public static void Label(string labelText, int fontSize = 13, GUIStyle fontStyle = null, Color? textColor = null, TextAnchor labelAlignment = TextAnchor.MiddleLeft,  float topSpace = 5.0f, float bottomSpace = 5.0f, bool wordWrap = true, bool supportHTML = false)
        {
            GUILayout.Space(topSpace);

            if (fontStyle == null)
                fontStyle = GUIStyle.none;

            GUIStyle style = new (fontStyle)
            {
                fontSize = fontSize,
                alignment = labelAlignment,
                wordWrap = wordWrap,
                richText = supportHTML,
            };
            style.normal.textColor = textColor ?? new Color(0.8f, 0.8f, 0.8f, 1.0f);

            EditorGUILayout.LabelField(labelText, style);

            GUILayout.Space(bottomSpace);
        }

        private static readonly List<DictGUI> dictGUI = new();
        private class DictGUI
        {
            public string key;
            public string value;

            public DictGUI(string key, string value)
            {
                this.key = key;
                this.value = value;
            }
        }
        public static void ShowStatDictionaryInInspector(Dictionary<string, Stat> dict)
        {
            if (Application.isPlaying)
            {
                if (GUILayout.Button("Update Dictionary GUI"))
                {
                    dictGUI.Clear();

                    foreach (KeyValuePair<string, Stat> statEntry in dict)
                    {
                        dictGUI.Add(new DictGUI(statEntry.Key, statEntry.Value.GetValue().ToString()));
                    }
                }

                if (dictGUI.Count > 0)
                {
                    foreach (DictGUI dictGUIEntry in dictGUI)
                    {
                        EditorGUILayout.LabelField("Key:", dictGUIEntry.key);
                        EditorGUILayout.LabelField("Value:", dictGUIEntry.value);
                        Line();
                    }
                }
            }
            else
            {
                Label("Dictionary is only visible in Play mode", 14, textColor: new Color(255.0f, 140.0f, 0.0f), labelAlignment: TextAnchor.MiddleCenter);
            }
        }
    }

    public static class SortingTools
    {
        public static GameObject GetNearestGameObject(Transform origin, List<GameObject> listOfObjects)
        {
            GameObject closestObj = null;

            float minDistance = Mathf.Infinity;
            Vector3 currentPosition = origin.position;

            foreach (GameObject obj in listOfObjects)
            {
                float dist = Vector3.Distance(obj.transform.position, currentPosition);
                if (dist < minDistance)
                {
                    closestObj = obj;
                    minDistance = dist;
                }
            }

            return closestObj;
        }
    }
    
    public static class ValueCheck
    {
        public static bool IsFloatLessThan(float value1, float value2, float precision = 0.005f)
        {
            return Mathf.Abs(value1 - value2) < precision;
        }

        public static bool IsFloatMoreThan(float value1, float value2, float precision = 0.005f)
        {
            return Mathf.Abs(value1 - value2) > precision;
        }
    }
}
