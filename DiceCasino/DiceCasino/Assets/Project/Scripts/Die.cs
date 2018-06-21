using UnityEngine;
using System.Collections;

public class Die : MonoBehaviour
{
	// Значение на кубике. Если 0, то значение не определено (кубик еще не остановился)
	public int value = 0;	
    // Нормализованный вектор из центра кубика, направленный вверх. В локальных координатах определяет, какая сторона выпала
    private Vector3 localHitNormalized;
	// Константа для проверки выпадения стороны
    private float validMargin = 0.45F;
    // Проверка, лежит ли кубик на поверхности
    public bool onGround = false;

	// true, если кубик еще не остановился
    public bool rolling
    {
        get
        {
            return !(GetComponent<Rigidbody>().velocity.magnitude == 0 && GetComponent<Rigidbody>().angularVelocity.magnitude == 0);
        }
    }

    // Считает нормализованный вектор из центра кубика, направленный вверх.В локальных координатах определяет, какая сторона выпала
    private bool localHit
    {
        get
        {
            // Создает луч, исходящий чуть выше кубика, направленный вниз
            Ray ray = new Ray(transform.position + (new Vector3(0, 2, 0) * transform.localScale.magnitude), Vector3.up * -1);
            RaycastHit hit = new RaycastHit();
            // Пускает луч и проверяет столкновение с кубиком
            if (GetComponent<Collider>().Raycast(ray, out hit, 3 * transform.localScale.magnitude))
            {
                // Используя точку пересечения луча и кубика получаем вектор из центра кубика в эту точку 
                // и переводим вектор в локальную систему координат
                localHitNormalized = transform.InverseTransformPoint(hit.point.x, hit.point.y, hit.point.z).normalized;
                return true;
            }
			// В теории до этого места не доходим
            return false;
        }
    }

	// Считает значение, выпавшее на кубике
    private void GetValue()
    {
		// value = 0 -> неопределенное значение
        value = 0;
        float delta = 1;
        int side = 1;
        Vector3 testHitVector;
        // Проверяет все стороны кубика. Сторона, которая имеет допустимый локальный вектор (из центра кубика вверх) 
        // и наименьшую разницу составляющих локального вектора и шаблонного вектора, будет значением кубика
        do
        {
            // Получаем шаблон вектора (идеальный вектор) текущей стороны
            testHitVector = HitVector(side);
            if (testHitVector != Vector3.zero)
            {
                // Сравниваем составляющие локального вектора с идеальным вектором текущей стороны (с некоторой погрешностью)
                if (valid(localHitNormalized.x, testHitVector.x) &&
                    valid(localHitNormalized.y, testHitVector.y) &&
                    valid(localHitNormalized.z, testHitVector.z))
                {
                    // Считаем наименьшую разницу составляющих локального вектора и шаблонного вектора
                    float nDelta = Mathf.Abs(localHitNormalized.x - testHitVector.x) + Mathf.Abs(localHitNormalized.y - testHitVector.y) + Mathf.Abs(localHitNormalized.z - testHitVector.z);
                    if (nDelta < delta)
                    {
                        value = side;
                        delta = nDelta;
                    }
                }
            }
            side++;
            // Если мы проверили все стороны, то (testHitVector == Vector3.zero)
        } while (testHitVector != Vector3.zero);
    }

    private void Update()
    {
        // Считаем значение кубика, если он на поверхности и не движется
        if (onGround && !rolling && localHit)
            GetValue();
    }

    // Определяет, находится ли компонент вектора в допустимом диапазоне
    private bool valid(float t, float v)
    {
        if (t > (v - validMargin) && t < (v + validMargin))
            return true;
        else
            return false;
    }

	// Получаем идеальный вектор для данной стороны кубика
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

    // Проверка кубика на столкновение с поверностью
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "Table")
            onGround = true;
    }

}
