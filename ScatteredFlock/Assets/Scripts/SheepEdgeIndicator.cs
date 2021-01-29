using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Sheep;

public class SheepEdgeIndicator : MonoBehaviour
{

    public Image arrow;
   
    public void Update()
    {
        if (GameManager.Instance.level != null)
        {
            Level level = GameManager.Instance.level;
            if (level.mainCamera != null && level.sheeps != null && level.shepherd != null)
            {
                float minDistance = -1;
                Sheep sheep = null;
                for (int i = 0; i < level.sheeps.Count; i++)
                {
                    if (level.sheeps[i].active)
                    {
                        float distance = (level.shepherd.transform.position - level.sheeps[i].transform.position).magnitude;
                        if (minDistance < 0 || distance < minDistance)
                        {
                            sheep = level.sheeps[i];
                            minDistance = distance;
                        }
                    }
                }

                if (sheep != null)
                {
                    Vector3 viewportTargetPos = level.mainCamera.WorldToViewportPoint(sheep.transform.position);
                    Vector3 viewportSourcePos = level.mainCamera.WorldToViewportPoint(level.shepherd.transform.position);
                    Vector3 delta = viewportTargetPos - viewportSourcePos;

                    float angle = Mathf.Atan2(delta.y, delta.x);
                    float slope = Mathf.Tan(angle);

                    bool offscreen = false;

                    if (viewportTargetPos.y > 1 || viewportTargetPos.y < 0)
                    {
                        float excess = viewportTargetPos.y - Mathf.Clamp01(viewportTargetPos.y);
                        delta.y -= excess;
                        delta.x = delta.y / slope;
                        offscreen = true;
                    }

                    viewportTargetPos = viewportSourcePos + delta;

                    if (viewportTargetPos.x > 1 || viewportTargetPos.x < 0)
                    {
                        float excess = viewportTargetPos.x - Mathf.Clamp01(viewportTargetPos.x);
                        delta.x -= excess;
                        delta.y = delta.x * slope;
                        offscreen = true;
                    }

                    if (offscreen)
                    {
                        if (arrow != null && arrow.gameObject.activeInHierarchy == false)
                        {
                            arrow.gameObject.SetActive(true);
                        }
                        Vector3 screenPos = level.mainCamera.ViewportToScreenPoint(viewportSourcePos + delta);
                        transform.position = screenPos;
                        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

                        if (arrow != null)
                        {
                            switch (sheep.type)
                            {
                                case SheepType.Red:
                                {
                                    arrow.color = Color.red;
                                    break;
                                }

                                case SheepType.Green:
                                {
                                    arrow.color = Color.green;
                                    break;
                                }

                                case SheepType.Blue:
                                {
                                    arrow.color = Color.blue;
                                        
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (arrow != null && arrow.gameObject.activeInHierarchy)
                        {
                            arrow.gameObject.SetActive(false);
                        }
                    }

                    return;
                }
            }
        }

        if (arrow != null && arrow.gameObject.activeInHierarchy)
        {
            arrow.gameObject.SetActive(false);
        }
    }
}


