using UnityEngine;
using System.Collections;
using Kalagaan;


/// <summary>
/// Share VertExmotion deformations to another instance
/// </summary>
public class VertExmotionShare : MonoBehaviour {

	//public MeshRenderer m_meshRendererRef;
	public VertExmotion m_reference;
	public bool m_copyDeltaPosition = true;
	public bool m_copyRotation = true;
	MeshRenderer m_meshRenderer;
	SkinnedMeshRenderer m_skinMeshRenderer;
	MaterialPropertyBlock m_matPropBlk;		
	Vector3 m_referenceLastPosition;

	void Start()
	{
		if( m_reference == null )
		{
			Debug.LogError( "VertExmotionShare need a VertExmotion instance as reference." );
			return;
		}

		m_meshRenderer = GetComponent<MeshRenderer>();
		if( m_meshRenderer != null && m_reference.GetComponent<MeshRenderer>() != null )
		{
			m_meshRenderer.sharedMaterial = m_reference.GetComponent<MeshRenderer>().material;
			GetComponent<MeshFilter> ().sharedMesh = m_reference.GetComponent<MeshFilter> ().sharedMesh;
		}

		m_skinMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		if( m_skinMeshRenderer != null && m_reference.GetComponent<SkinnedMeshRenderer>() != null )
		{
			m_skinMeshRenderer.sharedMaterial = m_reference.GetComponent<SkinnedMeshRenderer>().material;
			m_skinMeshRenderer.sharedMesh = m_reference.GetComponent<SkinnedMeshRenderer> ().sharedMesh;
		}

		m_referenceLastPosition = m_reference.transform.position;


	}

    Vector4[] newSensorsPos = new Vector4[VertExmotion.MAX_SENSOR];
    //Vector4[] originalSensorsPos = new Vector4[VertExmotion.MAX_SENSOR];

	void LateUpdate()
	{
		if( m_reference == null )		
			return;

		if( m_meshRenderer == null && m_skinMeshRenderer == null )
			return;

		//get shader properties from original
		m_matPropBlk = m_reference.m_matPropBlk;

        if (newSensorsPos.Length != m_reference.m_shaderSensorPos.Length)
            newSensorsPos = new Vector4[m_reference.m_shaderSensorPos.Length];


        //for (int i=0; i<VertExmotion.MAX_SENSOR; ++i)
        for(int i = 0; i < m_reference.m_shaderSensorPos.Length; ++i)
		{
            newSensorsPos[i] = transform.TransformPoint(m_reference.transform.InverseTransformPoint(m_reference.m_shaderSensorPos[i]));
        }
        m_matPropBlk.SetVectorArray("KVM_SensorPosition",newSensorsPos);

        m_meshRenderer.SetPropertyBlock( m_reference.m_matPropBlk );

        //restore values of the original
        m_matPropBlk.SetVectorArray("KVM_SensorPosition", m_reference.m_shaderSensorPos);

        if ( m_copyDeltaPosition )
		{
			transform.position += m_reference.transform.position - m_referenceLastPosition;
		}

		if( m_copyRotation )
		{
			transform.rotation = m_reference.transform.rotation;
		}

		m_referenceLastPosition = m_reference.transform.position;

	}


}
