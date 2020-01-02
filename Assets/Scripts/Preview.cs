using UnityEngine;

public class Preview : MonoBehaviour
{
    public Material BuildableMaterial,UnbuildableMaterial;
    public bool Buildable;

    private MeshCollider meshcollider;
    private MeshRenderer meshrenderer;
    private Rigidbody rigidbody;
    private Material[] materials;
    private int contacts;

    private void Awake()
    {
        contacts = 0;
        meshcollider = GetComponent<MeshCollider>();
        meshcollider.convex = true;
        meshcollider.isTrigger = true;

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;

        meshrenderer = GetComponent<MeshRenderer>();
        materials = meshrenderer.materials;
    }

    private void Update()
    {
        Buildable = contacts == 0;
        if(Buildable)
        {
            SetMaterials(BuildableMaterial);
        }
        else
        {
            SetMaterials(UnbuildableMaterial);
        }
    }

    private void SetMaterials(Material _material)
    {
        if(materials != null && materials[0] == _material)
        {
            return;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = _material;
        }
        meshrenderer.materials = materials;
    }

    private void OnTriggerEnter(Collider _other)
    {

        MeshCollider otherCollider = _other.GetComponent<MeshCollider>();

        if (_other.tag != "Floor")
        {
            if (tag == "Wall" || tag == "Door")
            {
                if (_other.tag == "Foundation")
                {
                    BuildingSystem.Instance.objectToSnap = _other.gameObject;
                    BuildingSystem.Instance.snapping = true;
                    BuildingSystem.Instance.snappingOffset += new Vector3(0, otherCollider.bounds.size.y / 2f, 0);
                    Debug.Log("W or D hitting F");
                }
                else if (_other.tag == "Wall")
                {
                    BuildingSystem.Instance.objectToSnap = _other.gameObject;
                    BuildingSystem.Instance.snapping = true;
                    BuildingSystem.Instance.snappingOffset += new Vector3(0, otherCollider.bounds.size.y / 2f, 0);
                    Debug.Log("W or D hitting W");
                }
            }
            else if (tag == "Roof")
            {
                if (_other.tag == "Wall" || _other.tag == "Door")
                {
                    BuildingSystem.Instance.objectToSnap = _other.gameObject;
                    BuildingSystem.Instance.snapping = true;
                    //BuildingSystem.Instance.heightOffset = otherCollider.bounds.size.y / 2f;
                    BuildingSystem.Instance.snappingOffset += new Vector3(otherCollider.bounds.center.x, 0, 0);

                    Debug.Log("R hitting W or D" + BuildingSystem.Instance.heightOffset);
                }
            }
            else
            {
                contacts++;
            }
        }
        else
        {
            if(tag == "Wall")
            {
                contacts++;
            }
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.tag != "Floor")
        {
            if (tag == "Wall" || tag == "Door")
            {
                if (_other.tag == "Foundation")
                {
                    BuildingSystem.Instance.objectToSnap = null;
                    BuildingSystem.Instance.snapping = false;
                    BuildingSystem.Instance.snappingOffset = Vector3.zero;
                }
                else if (_other.tag == "Wall" || _other.tag == "Door")
                {
                    BuildingSystem.Instance.objectToSnap = null;
                    BuildingSystem.Instance.snapping = false;
                    BuildingSystem.Instance.snappingOffset = Vector3.zero;
                }
            }
            else if(tag == "Roof")
            {
                if (_other.tag == "Wall" || _other.tag == "Door")
                {
                    BuildingSystem.Instance.objectToSnap = null;
                    BuildingSystem.Instance.snapping = false;
                }
            }
            else
            {
                if (contacts > 0)
                    contacts--;
            }
        }
    }
}
