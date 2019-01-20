using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleIKChainJoint : MonoBehaviour
{
	private class Effector
	{
		public Transform transform;
		public Vector3 position;
        public Vector3 direction;
        public Quaternion targetRotation;
        public Vector3 targetPosition;
        public float length;
    }

	[SerializeField]
	private Transform rootNode;

    [SerializeField]
    private Transform target;

    private List<Effector> effectors = new List<Effector>();

    private int endEffector;

    private void Awake()
	{
        Transform currentTransform = this.transform;
		while(currentTransform != rootNode)
		{
            Vector3 vector = currentTransform.parent.position - currentTransform.position;

            effectors.Add(new Effector() { transform = currentTransform, position = currentTransform.position, direction = vector.normalized, length = vector.magnitude});
            currentTransform = currentTransform.parent;
        }

        // Add root
        effectors.Add(new Effector() { transform = currentTransform, position = currentTransform.position, direction = Vector3.zero, length = 0.0f });

        effectors.Reverse();
        endEffector = effectors.Count - 1;
    }

	private void LateUpdate()
	{
        for (int i = 1; i <= endEffector; i++)
        {
            effectors[i].targetPosition = effectors[i].transform.localPosition;
            effectors[i].targetRotation = effectors[i].transform.localRotation;
        }

        Forward();
        Backward();

        for (int i = 1; i <= endEffector; i++)
        {
            effectors[i].transform.localPosition = effectors[i - 1].transform.InverseTransformPoint(effectors[i].position);
            effectors[i].transform.localRotation = Quaternion.LookRotation(effectors[i - 1].transform.InverseTransformDirection(effectors[i].direction));
        }
    }

    public void Forward()
    {
        Vector3 rootPosition = effectors[0].position;

        effectors[endEffector].position = target.position;

        for (int i = endEffector; i > 0; i--)
        {
            effectors[i].direction = Vector3.Normalize(effectors[i - 1].position - effectors[i].position);
            effectors[i - 1].position = effectors[i].position + effectors[i].direction * effectors[i].length;
        }

        effectors[0].position = rootPosition;
    }

    public void Backward()
    {
        for (int i = 0; i < endEffector; i++)
        {
            effectors[i].direction = Vector3.Normalize(effectors[i + 1].position - effectors[i].position);
            effectors[i + 1].position = effectors[i].position + effectors[i].direction * effectors[i + 1].length;
        }
    }
}
