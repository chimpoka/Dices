using UnityEngine;
using System.Collections;

public class Die : MonoBehaviour
{
	// �������� �� ������. ���� 0, �� �������� �� ���������� (����� ��� �� �����������)
	public int value = 0;	
    // ��������������� ������ �� ������ ������, ������������ �����. � ��������� ����������� ����������, ����� ������� ������
    private Vector3 localHitNormalized;
	// ��������� ��� �������� ��������� �������
    private float validMargin = 0.45F;
    // ��������, ����� �� ����� �� �����������
    public bool onGround = false;

	// true, ���� ����� ��� �� �����������
    public bool rolling
    {
        get
        {
            return !(GetComponent<Rigidbody>().velocity.magnitude == 0 && GetComponent<Rigidbody>().angularVelocity.magnitude == 0);
        }
    }

    // ������� ��������������� ������ �� ������ ������, ������������ �����.� ��������� ����������� ����������, ����� ������� ������
    private bool localHit
    {
        get
        {
            // ������� ���, ��������� ���� ���� ������, ������������ ����
            Ray ray = new Ray(transform.position + (new Vector3(0, 2, 0) * transform.localScale.magnitude), Vector3.up * -1);
            RaycastHit hit = new RaycastHit();
            // ������� ��� � ��������� ������������ � �������
            if (GetComponent<Collider>().Raycast(ray, out hit, 3 * transform.localScale.magnitude))
            {
                // ��������� ����� ����������� ���� � ������ �������� ������ �� ������ ������ � ��� ����� 
                // � ��������� ������ � ��������� ������� ���������
                localHitNormalized = transform.InverseTransformPoint(hit.point.x, hit.point.y, hit.point.z).normalized;
                return true;
            }
			// � ������ �� ����� ����� �� �������
            return false;
        }
    }

	// ������� ��������, �������� �� ������
    private void GetValue()
    {
		// value = 0 -> �������������� ��������
        value = 0;
        float delta = 1;
        int side = 1;
        Vector3 testHitVector;
        // ��������� ��� ������� ������. �������, ������� ����� ���������� ��������� ������ (�� ������ ������ �����) 
        // � ���������� ������� ������������ ���������� ������� � ���������� �������, ����� ��������� ������
        do
        {
            // �������� ������ ������� (��������� ������) ������� �������
            testHitVector = HitVector(side);
            if (testHitVector != Vector3.zero)
            {
                // ���������� ������������ ���������� ������� � ��������� �������� ������� ������� (� ��������� ������������)
                if (valid(localHitNormalized.x, testHitVector.x) &&
                    valid(localHitNormalized.y, testHitVector.y) &&
                    valid(localHitNormalized.z, testHitVector.z))
                {
                    // ������� ���������� ������� ������������ ���������� ������� � ���������� �������
                    float nDelta = Mathf.Abs(localHitNormalized.x - testHitVector.x) + Mathf.Abs(localHitNormalized.y - testHitVector.y) + Mathf.Abs(localHitNormalized.z - testHitVector.z);
                    if (nDelta < delta)
                    {
                        value = side;
                        delta = nDelta;
                    }
                }
            }
            side++;
            // ���� �� ��������� ��� �������, �� (testHitVector == Vector3.zero)
        } while (testHitVector != Vector3.zero);
    }

    private void Update()
    {
        // ������� �������� ������, ���� �� �� ����������� � �� ��������
        if (onGround && !rolling && localHit)
            GetValue();
    }

    // ����������, ��������� �� ��������� ������� � ���������� ���������
    private bool valid(float t, float v)
    {
        if (t > (v - validMargin) && t < (v + validMargin))
            return true;
        else
            return false;
    }

	// �������� ��������� ������ ��� ������ ������� ������
    private Vector3 HitVector(int side)
    {
        switch (side)
        {
            case 1: return new Vector3(0F, 0F, 1F);
            case 2: return new Vector3(0F, -1F, 0F);
            case 3: return new Vector3(-1F, 0F, 0F);
            case 4: return new Vector3(1F, 0F, 0F);
            case 5: return new Vector3(0F, 1F, 0F);
            case 6: return new Vector3(0F, 0F, -1F);
        }
        return Vector3.zero;
    }

    // �������� ������ �� ������������ � �����������
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "Table")
            onGround = true;
    }

}
