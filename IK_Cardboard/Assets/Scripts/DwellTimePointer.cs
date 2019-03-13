using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DwellTimePointer : GvrBasePointer {

    [Tooltip("Teleport can be activated using dwell time or using click (0 means click)")]
    public float dwellTime = 0;

    // If dwell time is used, remember the last time gaze was started 
    private float lastGazedAt;
    private bool trigger;

    /// The constants below are expsed for testing. Minimum inner angle of the reticle (in degrees).
    public const float RETICLE_MIN_INNER_ANGLE = 0.0f;

    /// Minimum outer angle of the reticle (in degrees).
    public const float RETICLE_MIN_OUTER_ANGLE = 0.5f;

    /// Angle at which to expand the reticle when intersecting with an object (in degrees).
    public const float RETICLE_GROWTH_ANGLE = 1.5f;

    /// Minimum distance of the reticle (in meters).
    public const float RETICLE_DISTANCE_MIN = 0.45f;

    /// Maximum distance of the reticle (in meters).
    public float maxReticleDistance = 20.0f;

    /// Number of segments making the reticle circle.
    public int reticleSegments = 20;

    /// Growth speed multiplier for the reticle/
    public float reticleGrowthSpeed = 8.0f;

    /// Sorting order to use for the reticle's renderer.
    /// Range values come from https://docs.unity3d.com/ScriptReference/Renderer-sortingOrder.html.
    /// Default value 32767 ensures gaze reticle is always rendered on top.
    [Range(-32767, 32767)]
    public int reticleSortingOrder = 32767;

    /// <summary>The material used to render the reticle.</summary>
    public Material MaterialComp { private get; set; }

    /// <summary>Current inner angle of the reticle (in degrees).</summary>
    /// <remarks>Exposed for testing.</remarks>
    public float ReticleInnerAngle { get; private set; }

    /// <summary>Current outer angle of the reticle (in degrees).</summary>
    /// <remarks>Exposed for testing.</remarks>
    public float ReticleOuterAngle { get; private set; }

    /// <summary>Current distance of the reticle (in meters).</summary>
    /// <remarks>Getter exposed for testing.</remarks>
    public float ReticleDistanceInMeters { get; private set; }

    /// <summary>Current inner and outer diameters of the reticle,
    ///   before distance multiplication. </summary>
    /// <remarks>Getters exposed for testing.</remarks>
    public float ReticleInnerDiameter { get; private set; }

    /// <summary>Current outer diameter of the reticle (in meters).</summary>
    public float ReticleOuterDiameter { get; private set; }

    /// <summary>Returns the max distance from the pointer that
    /// raycast hits will be detected.</summary>
    public override float MaxPointerDistance
    {
        get { return maxReticleDistance; }
    }

    public override void OnPointerEnter(RaycastResult raycastResultResult, bool isInteractive)
    {
        SetPointerTarget(raycastResultResult.worldPosition, isInteractive);

        if (isInteractive)
        {
            lastGazedAt = Time.time;
        }

        float inner_half_angle_radians = Mathf.Deg2Rad * ReticleInnerAngle * 0.5f;
        float outer_half_angle_radians = Mathf.Deg2Rad * ReticleOuterAngle * 0.5f;

        ReticleInnerDiameter = 2.0f * Mathf.Tan(inner_half_angle_radians);
        ReticleOuterDiameter = 2.0f * Mathf.Tan(outer_half_angle_radians);

        MaterialComp.SetFloat("_InnerDiameter", ReticleInnerDiameter * ReticleDistanceInMeters);
        MaterialComp.SetFloat("_OuterDiameter", ReticleOuterDiameter * ReticleDistanceInMeters);
        MaterialComp.SetFloat("_DistanceInMeters", ReticleDistanceInMeters);
    }

    public override void OnPointerHover(RaycastResult raycastResultResult, bool isInteractive)
    {
        SetPointerTarget(raycastResultResult.worldPosition, isInteractive);
    }

    public override void OnPointerExit(GameObject previousObject)
    {
        ReticleDistanceInMeters = maxReticleDistance;
        ReticleInnerAngle = RETICLE_MIN_INNER_ANGLE;
        ReticleOuterAngle = RETICLE_MIN_OUTER_ANGLE;

        lastGazedAt = 0;
        
    }

    public override void OnPointerClickDown()
    {
    }

    public override void OnPointerClickUp()
    {
    }

    public override bool TriggerDown
    {
        get
        {
            bool trigger = Time.time - lastGazedAt >= dwellTime;
            if (trigger)
            {
                lastGazedAt = 0;
            }
            return trigger;
        }
    }

    public override void GetPointerRadius(out float enterRadius, out float exitRadius)
    {
        float min_inner_angle_radians = Mathf.Deg2Rad * RETICLE_MIN_INNER_ANGLE;
        float max_inner_angle_radians = Mathf.Deg2Rad * (RETICLE_MIN_INNER_ANGLE + RETICLE_GROWTH_ANGLE);

        enterRadius = 2.0f * Mathf.Tan(min_inner_angle_radians);
        exitRadius = 2.0f * Mathf.Tan(max_inner_angle_radians);
    }

    /// <summary>Updates the material based on the reticle properties.</summary>
    public void UpdateDiameters()
    {

        ReticleDistanceInMeters =
        Mathf.Clamp(ReticleDistanceInMeters, RETICLE_DISTANCE_MIN, maxReticleDistance);

        ReticleInnerAngle = RETICLE_MIN_INNER_ANGLE;
        ReticleOuterAngle = RETICLE_MIN_OUTER_ANGLE;

        float inner_half_angle_radians = Mathf.Deg2Rad * ReticleInnerAngle * 0.5f;
        float outer_half_angle_radians = Mathf.Deg2Rad * ReticleOuterAngle * 0.5f;

        float inner_diameter = 2.0f * Mathf.Tan(inner_half_angle_radians);
        float outer_diameter = 2.0f * Mathf.Tan(outer_half_angle_radians);

        float ReticleInnerAngleBig = RETICLE_MIN_INNER_ANGLE + RETICLE_GROWTH_ANGLE;
        float ReticleOuterAngleBig = RETICLE_MIN_OUTER_ANGLE + RETICLE_GROWTH_ANGLE;

        float inner_half_angle_radians_big = Mathf.Deg2Rad * ReticleInnerAngleBig * 0.5f;
        float outer_half_angle_radians_big = Mathf.Deg2Rad * ReticleOuterAngleBig * 0.5f;

        float inner_diameter_big = 2.0f * Mathf.Tan(inner_half_angle_radians_big);
        float outer_diameter_big = 2.0f * Mathf.Tan(outer_half_angle_radians_big);

        if (lastGazedAt != 0)
        {
            float fraction = (Time.time - lastGazedAt) / dwellTime;
            
            inner_diameter =
          Mathf.Lerp(inner_diameter_big, inner_diameter, fraction);
            outer_diameter =
          Mathf.Lerp(outer_diameter_big, outer_diameter, fraction);
        }

        MaterialComp.SetFloat("_InnerDiameter", inner_diameter * ReticleDistanceInMeters);
        MaterialComp.SetFloat("_OuterDiameter", outer_diameter * ReticleDistanceInMeters);
        MaterialComp.SetFloat("_DistanceInMeters", ReticleDistanceInMeters);    
    }

    void Awake()
    {
        ReticleInnerAngle = RETICLE_MIN_INNER_ANGLE;
        ReticleOuterAngle = RETICLE_MIN_OUTER_ANGLE;
    }

    /// @cond
    protected override void Start()
    {
        base.Start();

        Renderer rendererComponent = GetComponent<Renderer>();
        rendererComponent.sortingOrder = reticleSortingOrder;

        MaterialComp = rendererComponent.material;

        CreateReticleVertices();
    }

    /// @endcond

    void Update()
    {

        UpdateDiameters();

    }

    

    private bool SetPointerTarget(Vector3 target, bool interactive)
    {
        if (base.PointerTransform == null)
        {
            Debug.LogWarning("Cannot operate on a null pointer transform");
            return false;
        }

        Vector3 targetLocalPosition = base.PointerTransform.InverseTransformPoint(target);

        ReticleDistanceInMeters = Mathf.Clamp(targetLocalPosition.z, RETICLE_DISTANCE_MIN, maxReticleDistance);
        if (interactive)
        {
            ReticleInnerAngle = RETICLE_MIN_INNER_ANGLE + RETICLE_GROWTH_ANGLE;
            ReticleOuterAngle = RETICLE_MIN_OUTER_ANGLE + RETICLE_GROWTH_ANGLE;
        }
        else
        {
            ReticleInnerAngle = RETICLE_MIN_INNER_ANGLE;
            ReticleOuterAngle = RETICLE_MIN_OUTER_ANGLE;
        }

        return true;
    }

    private void CreateReticleVertices()
    {
        Mesh mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>();
        GetComponent<MeshFilter>().mesh = mesh;

        int segments_count = reticleSegments;
        int vertex_count = (segments_count + 1) * 2;

        #region Vertices

        Vector3[] vertices = new Vector3[vertex_count];

        const float kTwoPi = Mathf.PI * 2.0f;
        int vi = 0;
        for (int si = 0; si <= segments_count; ++si)
        {
            // Add two vertices for every circle segment: one at the beginning of the
            // prism, and one at the end of the prism.
            float angle = (float)si / (float)(segments_count) * kTwoPi;

            float x = Mathf.Sin(angle);
            float y = Mathf.Cos(angle);

            vertices[vi++] = new Vector3(x, y, 0.0f); // Outer vertex.
            vertices[vi++] = new Vector3(x, y, 1.0f); // Inner vertex.
        }
        #endregion

        #region Triangles
        int indices_count = (segments_count + 1) * 3 * 2;
        int[] indices = new int[indices_count];

        int vert = 0;
        int idx = 0;
        for (int si = 0; si < segments_count; ++si)
        {
            indices[idx++] = vert + 1;
            indices[idx++] = vert;
            indices[idx++] = vert + 2;

            indices[idx++] = vert + 1;
            indices[idx++] = vert + 2;
            indices[idx++] = vert + 3;

            vert += 2;
        }
        #endregion

        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateBounds();
#if !UNITY_5_5_OR_NEWER
        // Optimize() is deprecated as of Unity 5.5.0p1.
        mesh.Optimize();
#endif  // !UNITY_5_5_OR_NEWER
    }
}
