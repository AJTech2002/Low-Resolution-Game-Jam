using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVMesh : MonoBehaviour
{
    public LayerMask mask;
    private Mesh viewMesh;
    public float maskCutawayDistance;
    public MeshFilter viewMeshFilter;
    public Transform pointer;
    // Start is called before the first frame update
    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    // Update is called once per frame
    void Update()
    {
        DrawFieldOfView();
    }

    public float meshResolution;
    public float viewAngle;
    public float edgeDistanceThreshold;
    public float viewRadius;

    public float generalViewAngle = 80;
    public float generalViewRadius = 10;
    public float torchViewAngle;
    public float torchViewRadius;

    private bool useDot = true;

    public void PickupTorch ()
    {
        useDot = false;
        viewAngle = torchViewAngle;
        viewRadius = torchViewRadius;
    }

    public void RemoveTorch ()
    {
        useDot = true;
        viewAngle = generalViewAngle;
        viewRadius = generalViewRadius;
    }

    void DrawFieldOfView ()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i < stepCount; i++)
        {
            float angle = transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
            
            ViewCastInfo viewCast = ViewCast(angle);
            
            if (i > 0)
            {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.dst - viewCast.dst) > edgeDistanceThreshold;
                if (oldViewCast.hit != viewCast.hit || (oldViewCast.hit && viewCast.hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, viewCast);
                    if (edge.pointA != Vector3.zero)
                        viewPoints.Add(edge.pointA);
                    
                    if (edge.pointB != Vector3.zero)
                        viewPoints.Add(edge.pointB);
                }
            }

            viewPoints.Add(viewCast.point);
            oldViewCast = viewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;

        viewMesh.triangles = triangles;

        viewMesh.RecalculateNormals();
    }

    private Vector3 DirFromAngle (float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z;
        }

        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad),Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }
    
    ViewCastInfo ViewCast (float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position,dir,viewRadius,mask);
        
        if (hit.collider != null)
        {
            //Modification: For Cutaway, use Vector3 Dot to ensure the intensity of the cutaway is based on how 'much' the player is looking at it; looks more natural
            float maskFactor = (useDot) ? Vector3.Dot(pointer.forward, dir) : 0.2f;
            Vector3 dirPush = (useDot) ? pointer.forward : dir;
            return new ViewCastInfo(true, new Vector3(hit.point.x, hit.point.y, transform.position.z) + dirPush * maskCutawayDistance * maskFactor, hit.distance, globalAngle);

        }
        else {
            return new ViewCastInfo(false, transform.position+viewRadius*dir, viewRadius, globalAngle);
        }
    }

    public int edgeResolution;

    EdgeInfo FindEdge (ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;

        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolution; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded) {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    public struct ViewCastInfo {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo (bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo (Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
