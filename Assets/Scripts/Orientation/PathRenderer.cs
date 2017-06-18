using UnityEngine;
using System.Collections;

public class PathRenderer : MonoBehaviour
{

    public Material material;

    private static Material m;
    private GameObject g;
    private Vector3[] lp;
    private Vector3[] sp;
    private Vector3 s;

    void Awake()
    {
        m = material;
        g = new GameObject("g");
        g.transform.parent = this.transform;
        lp = new Vector3[0];
        sp = new Vector3[0];
    }

    public void Draw(Vector3 e)
    {
        if (s != Vector3.zero)
        {
            for (int i = 0; i < lp.Length; i += 2)
            {
                float d = Vector3.Distance(lp[i], e);
                if (d < 1 && Random.value > 0.9f) sp = AddLine(sp, lp[i], e, false);
            }

            lp = AddLine(lp, s, e, false);
        }

        s = e;
    }

    Vector3[] AddLine(Vector3[] l, Vector3 s, Vector3 e, bool tmp)
    {
        int vl = l.Length;
        if (!tmp || vl == 0) l = resizeVertices(l, 2);
        else vl -= 2;

        l[vl] = s;
        l[vl + 1] = e;
        return l;
    }

    Vector3[] resizeVertices(Vector3[] ovs, int ns)
    {
        Vector3[] nvs = new Vector3[ovs.Length + ns];
        for (int i = 0; i < ovs.Length; i++) nvs[i] = ovs[i];
        return nvs;
    }

    void OnPostRender()
    {
        m.SetPass(0);
        GL.PushMatrix();
        GL.MultMatrix(g.transform.transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(new Color(0, 0, 0, 0.4f));

        for (int i = 0; i < lp.Length; i++)
        {
            GL.Vertex3(lp[i].x, lp[i].y, lp[i].z);
        }

        GL.Color(new Color(0, 0, 0, 0.1f));

        for (int i = 0; i < sp.Length; i++)
        {
            GL.Vertex3(sp[i].x, sp[i].y, sp[i].z);
        }

        GL.End();
        GL.PopMatrix();
    }

}
