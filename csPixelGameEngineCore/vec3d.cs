namespace csPixelGameEngineCore;

public struct vec3d
{
    public static vec3d Unit = new vec3d(0, 0, 0, 1);

    public float x;
    public float y;
    public float z;
    public float w;

    public vec3d(float x = 0, float y = 0, float z = 0, float w = 1)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
}
