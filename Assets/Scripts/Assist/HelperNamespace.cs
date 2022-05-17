using System.Collections;
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

        public static Mesh DrawWedgeMesh(float angle, float distance, float height)
        {
            Mesh mesh = new Mesh();

            int segments = 10;
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
    }
}
