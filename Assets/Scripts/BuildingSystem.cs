using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    [Header("GENERAL")]
    public bool AllowSnapping;
    public float SnappingDistance;
    public LayerMask BuildableLayers;
    public Camera CharacterCamera;

    [Header("BUILDING COMPONENT CATALOGUE")]
    public List<BuildingComponent> BuildingComponents = new List<BuildingComponent>();

    [Header("SNAPPING:")]
    public float GridSize;
    public float GridOffset;

    private GameObject currentPreviewGameObject;
    private BuildingComponent currentlyPreviewedComponent;
    private Vector3 currentPosition;
    private RaycastHit raycastHit;
    private bool previewing;
    private Preview currentPreview;

    private static BuildingSystem instance;
    public static BuildingSystem Instance { get { return instance; } }
    [HideInInspector] public bool snapping;
    [HideInInspector] public GameObject objectToSnap;
    [HideInInspector] public Vector3 snappingOffset;
    [HideInInspector] public float heightOffset;

    private void Start()
    {
        instance = this;
        for (int i = 0; i < BuildingComponents.Count; i++)
        {
            UserInterface.Instance.MenuElements.Add(BuildingComponents[i].MenuElement);
        }
        UserInterface.Instance.Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (UserInterface.Instance.Active)
            {
                UserInterface.Instance.Deactivate();
            }
            else
            {
                UserInterface.Instance.Activate();
            }
        }

        if (previewing && !UserInterface.Instance.Active)
        {
            ShowPreview();
            if (Input.GetMouseButtonDown(1))
            {
                if (currentPreview != null && currentPreview.Buildable)
                {
                    Place();
                }
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                Rotate();
            }
        }
    }

    private void InstantiatePreview()
    {
        if (currentPreviewGameObject != null)
        {
            GameObject temp = currentPreviewGameObject;
            Destroy(temp);
        }

        currentPreviewGameObject = Instantiate(currentlyPreviewedComponent.PreviewPrefab);
        currentPreview = currentPreviewGameObject.GetComponentInChildren<Preview>();
        previewing = true;
    }

    private void Rotate()
    {
        if (currentPreviewGameObject == null)
        {
            return;
        }

        currentPreviewGameObject.transform.Rotate(Vector3.up, 90f * Input.mouseScrollDelta.y);
    }

    public void SwitchToIndex(int _index)
    {
        currentlyPreviewedComponent = BuildingComponents[_index];
        InstantiatePreview();
    }

    private void ShowPreview()
    {
        if (Physics.Raycast(CharacterCamera.ScreenPointToRay(Input.mousePosition), out raycastHit, float.MaxValue, BuildableLayers,
        QueryTriggerInteraction.Ignore))
        {
            currentPosition = raycastHit.point;
            UpdatePreview();
        }
    }

    private void UpdatePreview()
    {
        if (!AllowSnapping || !snapping || objectToSnap == null)
        {
            currentPosition -= Vector3.one * GridOffset;
            currentPosition /= GridSize;
            currentPosition = new Vector3(Mathf.Round(currentPosition.x), Mathf.Round(currentPosition.y), Mathf.Round(currentPosition.z));
            currentPosition *= GridSize;
            currentPosition += Vector3.one * GridOffset;
        }
        else
        {
            MeshCollider objectCollider = objectToSnap.GetComponent<MeshCollider>();
            Vector3[] position = new Vector3[4];
            position[0] = new Vector3((objectCollider.bounds.center.x + objectCollider.bounds.size.x / 2f),
                            objectCollider.bounds.center.y, objectCollider.bounds.center.z) + snappingOffset;

            position[1] = new Vector3((objectCollider.bounds.center.x - objectCollider.bounds.size.x / 2f),
                            objectCollider.bounds.center.y, objectCollider.bounds.center.z) + snappingOffset;

            position[2] = new Vector3(objectCollider.bounds.center.x, objectCollider.bounds.center.y,
                            (objectCollider.bounds.center.z + objectCollider.bounds.size.z / 2f)) + snappingOffset;

            position[3] = new Vector3(objectCollider.bounds.center.x, objectCollider.bounds.center.y,
                            (objectCollider.bounds.center.z - objectCollider.bounds.size.z / 2f)) + snappingOffset;

            Vector3 updatedPosition = currentPosition;
            float minDistance = float.MaxValue;
            int index = 0;

            for (int i = 0; i < 4; i++)
            {
                if (Vector3.Distance(currentPosition, position[i]) < minDistance)
                {
                    minDistance = Vector3.Distance(currentPosition, position[i]);
                    updatedPosition = position[i];
                    index = i;
                }
            }

            switch (index)
            {
                case 0:
                    currentPreviewGameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    break;

                case 1:
                    currentPreviewGameObject.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
                    break;

                case 2:
                    currentPreviewGameObject.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                    break;

                case 3:
                    currentPreviewGameObject.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                    break;
            }

            updatedPosition += new Vector3(0f, heightOffset, 0f);
            currentPosition = (Vector3.Distance(currentPosition, updatedPosition) <= SnappingDistance) ? updatedPosition : currentPosition;
        }

        currentPreviewGameObject.transform.position = currentPosition;
    }



    private void Place()
    {
        previewing = false;
        GameObject instanctiated = Instantiate(currentlyPreviewedComponent.BuildingElementPrefab, currentPosition, currentPreviewGameObject.transform.rotation);
        instanctiated.GetComponentInChildren<Rigidbody>().isKinematic = true;
        objectToSnap = null;
        snapping = false;

        if (currentPreviewGameObject != null)
        {
            GameObject temp = currentPreviewGameObject;
            Destroy(temp);
        }
    }


}
