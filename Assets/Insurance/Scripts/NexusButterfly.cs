using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NexusButterfly : MonoBehaviour
{
    [SerializeField]
    private float m_speed;
    [SerializeField]
    private GameObject m_nexusPrefab;

    private float m_transformOdds;
    private bool m_willTransform;
    private int m_dir; // starting from left or right side
    private float m_transformX; // x value of transform position if it will occur
    private float m_startY;
    private Nexus.Type m_type;

    public void SetFields(float odds, Nexus.Type type) {
        m_transformOdds = odds;
        m_type = type;
    }

    public void ManualAwake() {
        m_willTransform = (Random.Range(0f, 1f) <= m_transformOdds);
        m_startY = TilemapManager.instance.GetRandomY();

        m_dir = Random.Range(0, 2) == 0 ? -1 : 1;
        if (m_dir == 1) {
            this.transform.position = new Vector3(TilemapManager.instance.GetBound("left"), m_startY, this.transform.position.z);
        }
        else {
            this.transform.position = new Vector3(TilemapManager.instance.GetBound("right"), m_startY, this.transform.position.z);
            this.GetComponent<SpriteRenderer>().flipX = true;
        }
        Debug.Log("this Y: " + this.transform.position.y);

        if (m_willTransform) {
            m_transformX = TilemapManager.instance.GetRandomX();
        }
        else {
            m_transformX = m_dir == 1 ? TilemapManager.instance.GetBound("right")  + 1 : TilemapManager.instance.GetBound("left") - 1;
        }
    }

    private void Update() {
        this.transform.position = new Vector3(this.transform.position.x, m_startY + Mathf.PerlinNoise(this.transform.position.x, m_startY) * 4, this.transform.position.z);
        Vector3 moveVector = new Vector3(m_dir * m_speed * Time.deltaTime, 0, 0);
        this.transform.Translate(moveVector);

        if ((m_dir == 1 && (this.transform.position.x >= m_transformX))
            || (m_dir == -1 && (this.transform.position.x <= m_transformX))) {
            if (m_willTransform) {
                Transform();
            }
            else {
                // butterfly has reach edge of screen
                Destroy(this.gameObject);
            }
        }
    }

    private void Transform() {
        // TODO: Settle time

        // Transform
        GameObject nexusObj = Instantiate(m_nexusPrefab);
        nexusObj.transform.position = this.transform.position;
        Nexus nexus = nexusObj.GetComponent<Nexus>();
        nexus.SetType(m_type);
        nexus.ManualAwake();

        Destroy(this.gameObject);
    }
}
