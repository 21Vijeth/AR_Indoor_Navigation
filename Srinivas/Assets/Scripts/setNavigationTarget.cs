using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class setNavigationTarget : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown navigationTargetDropDown;
    [SerializeField]
    private List<Target> navigationTargetObjects = new List<Target>();
    [SerializeField]
    private Slider navigationYOffset;

    private NavMeshPath path;
    private LineRenderer line;
    private Vector3 targetPosition = Vector3.zero;

    private bool lineToggle = false;

    private void Start() {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
        line.enabled = lineToggle;
    }

    private void Update() {
        if (lineToggle && targetPosition != Vector3.zero) {
            // Use GetClosestNavMeshPoint to ensure valid positions
            Vector3 startPosition = GetClosestNavMeshPoint(transform.position);
            Vector3 endPosition = GetClosestNavMeshPoint(targetPosition);

            // Calculate path and update LineRenderer
            bool pathFound = NavMesh.CalculatePath(startPosition, endPosition, NavMesh.AllAreas, path);
            if (pathFound && path.corners.Length > 1) {
                line.positionCount = path.corners.Length;
                Vector3[] calculatedPathAndOffset = AddLineOffset();
                line.SetPositions(calculatedPathAndOffset);
            } else {
                Debug.LogWarning($"Path calculation failed! Start: {startPosition}, End: {endPosition}, Path Found: {pathFound}, Corners: {path.corners.Length}");
            }
        }
    }

    public void SetCurrentNavigationTarget(int selectedValue) {
        targetPosition = Vector3.zero;
        string selectedText = navigationTargetDropDown.options[selectedValue].text;
        Target currentTarget = navigationTargetObjects.Find(x => x.Name.Equals(selectedText));
        if (currentTarget != null) {
            targetPosition = GetClosestNavMeshPoint(currentTarget.PositionObject.transform.position);
            Debug.Log($"Target position set to: {targetPosition}");
        } else {
            Debug.LogWarning("Target not found!");
        }
    }

    public void ToggleVisibility() {
        lineToggle = !lineToggle;
        line.enabled = lineToggle;
    }

    private Vector3 GetClosestNavMeshPoint(Vector3 position) {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 2.0f, NavMesh.AllAreas)) {
            return hit.position;
        }
        Debug.LogWarning($"Position {position} is not on the NavMesh!");
        return position; // Return original position as a fallback
    }

    private Vector3[] AddLineOffset(){
        if(navigationYOffset.value==0)
        {
            return path.corners;
        }
        
        Vector3[] calculatedLine = new Vector3[path.corners.Length];
        for(int i = 0; i < path.corners.Length; i++)
        {
            calculatedLine[i] = path.corners[i] + new Vector3(0, navigationYOffset.value, 0);
        }
        return calculatedLine;
    }
}