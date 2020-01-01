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
        if (_other.tag != "Floor")
        {
            if (tag == "Wall" || tag == "Door")
            {
                if (_other.tag == "Foundation")
                {
                    BuildingSystem.Instance.objectToSnap = _other.gameObject;
                    BuildingSystem.Instance.snapping = true;
                    Debug.Log("W or D hitting F");
                }
            }
            else if (tag == "Roof")
            {
                if (_other.tag == "Wall" || _other.tag == "Door")
                {
                    MeshCollider otherCollider = _other.GetComponent<MeshCollider>();
                    BuildingSystem.Instance.heightOffset = _other.transform.position.y + otherCollider.bounds.size.y / 2f;
                    BuildingSystem.Instance.snappingOffset = meshcollider.bounds.center;
                    Debug.Log("R hitting W or D");
                }
            }
            else if(tag == "Foundation")
            {
                if(_other.tag == "Foundation")
                {
                    BuildingSystem.Instance.objectToSnap = _other.gameObject;
                    BuildingSystem.Instance.snapping = true;
                    Debug.Log("F hitting F");
                }
            }
            else
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
