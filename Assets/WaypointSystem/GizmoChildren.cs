using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointSystem
{
// #if UNITY_EDITOR
//     [ExecuteInEditMode]
// #endif
    public class GizmoChildren : MonoBehaviour
    {
        [Header("Pathfinding Settings")] public GameObject player;

        public float lineSpeed = 1, fadeSpeed = 1, triggerAreaSize = 5;
        public LineRenderer lineRenderer;
        public List<Vector3> positions = new();
        private readonly Vector3 posToAdd = Vector3.zero;


        private Vector3 newPlayerPos;
        private RaycastHit rayHit;

        private void Start()
        {
            if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
            var mat = lineRenderer.sharedMaterial;
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0);
        }

        private void Update()
        {
            var playerPos = player.transform.position;
            newPlayerPos = new Vector3(playerPos.x, playerPos.y, playerPos.z);
            UpdateChildren();
            var speed = Time.time * lineSpeed;
            lineRenderer.sharedMaterial.mainTextureOffset = new Vector2(-speed, 0);
            if (Input.GetKeyDown(KeyCode.Q)) StartCoroutine(FadeInOut());
            // RayCastNextWaypoint();
        }

        private void UpdateChildren()
        {
            positions.Clear();
            if (player) positions.Add(newPlayerPos);

            foreach (Transform child in transform)
                if (child.gameObject.activeSelf)
                    positions.Add(child.position);

            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
        }

        #region gizmos

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            #region WAYPOINT

            if (positions.Count == 0) return;
            var prevIndex = 0;
            foreach (var position in positions)
            {
                if (prevIndex == 0)
                {
                    Handles.Label(position + Vector3.up, "START");
                }
                else if (prevIndex == positions.Count)
                {
                    Handles.Label(position + Vector3.up, "END");
                }
                else
                {
                    Gizmos.DrawLine(positions[prevIndex], position);
                    Handles.Label(position + Vector3.up, $"Pos: {prevIndex}");
                }

                if (prevIndex > 0)
                    Gizmos.DrawWireSphere(position, triggerAreaSize);
                prevIndex++;
            }

            #endregion

            // #region RAYCAST
            //
            // if (posToAdd != Vector3.zero)
            //     Gizmos.DrawWireSphere(posToAdd, .5f);
            //
            // Gizmos.color = Color.red;
            // Gizmos.DrawSphere(rayHit.point + rayHit.normal, .2f);
            //
            // Gizmos.color = Color.green;
            // Gizmos.DrawSphere(rayHit.point, 0.1f);
            // Gizmos.DrawLine(newPlayerPos, rayHit.point);
            // Gizmos.color = Color.red;
            // // Gizmos.DrawLine(newPlayerPos + Vector3.up, rayHit.point);
            //
            // #endregion
        }

        private void OnValidate()
        {
            UpdateChildren();
        }
#endif

        #endregion

        #region FADE

        private IEnumerator FadeInOut()
        {
            var material = lineRenderer.sharedMaterial;
            //forever
            while (true)
            {
                // fade out
                yield return Fade(material, 1);
                // wait
                yield return new WaitForSeconds(.1f);
                // fade in
                yield return Fade(material, 0);
                // wait
                yield return new WaitForSeconds(.1f);
                break;
            }
        }

        private IEnumerator Fade(Material mat, float targetAlpha)
        {
            while (mat.color.a != targetAlpha)
            {
                var newAlpha = Mathf.MoveTowards(mat.color.a, targetAlpha, fadeSpeed * Time.deltaTime);
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, newAlpha);
                yield return null;
            }
        }

        #endregion

        // [Header("Raycast Settings")] public LayerMask environmentLayerMask;
        // public float wallHitAngle = 90, distanceToCheckEdge;
        // private void RayCastNextWaypoint()
        // {
        //     var position = player.transform.position + Vector3.up;
        //     var distanceToNextPoint = Vector3.Distance(position, positions[1]);
        //     var direction = new Vector3(positions[1].x, position.y, positions[1].z) - position;
        //     var hit = Physics.Raycast(
        //         position,
        //         direction,
        //         out rayHit,
        //         distanceToNextPoint,
        //         environmentLayerMask
        //     );
        //     if (hit)
        //     {
        //         var normal = rayHit.normal;
        //         posToAdd = Vector3.zero;
        //
        //         // right offset rayCast
        //         RaycastHit rRayHit;
        //         var Offset = Quaternion.AngleAxis(45f, Vector3.up);
        //         var Dir = Offset * normal;
        //         Physics.Raycast(rayHit.point + normal, Dir,
        //             out rRayHit, distanceToCheckEdge, environmentLayerMask);
        //         Debug.DrawLine(rayHit.point + normal, rRayHit.point);
        //         posToAdd = rRayHit.normal;
        //
        //    
        //     }
        // }
    }
}