using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arc : MonoBehaviour
{
    public float curveSize { get; set; } = 1.0f;
    public IEnumerator TravelArc(Vector3 destination, float duration)
    {
        var startPosition = transform.position;
        var percentComplete = 0.0f;
        while (percentComplete < 1.0f)
        {
            //deltTime - количество истекшего времени с тех пор, как последний фрейм был нарисован
            percentComplete += Time.deltaTime / duration;

            var currentHeight = Mathf.Sin(Mathf.PI * percentComplete);
            //лин.интерполяция, возвращающая позицию между точками в зависимости от процента выполнения пути
            //transform.position = Vector3.Lerp(startPosition, destination, percentComplete);
            Vector3 curveAdjust = Vector3.up * currentHeight * curveSize;
            transform.position = Vector3.Lerp(startPosition, destination, percentComplete) + curveAdjust;

            //пауза до след. фрейма
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
