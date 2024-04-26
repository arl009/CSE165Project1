using UnityEngine;

public class GenerateWall : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject brick;

    private int worldSizeX = 15;
    private int worldSizeY = 5;
    private float gridOffsetY = 1.1f;
    void Start()
    {
        for(int x = 0; x < worldSizeX; x++){
            for(int y = 0; y < worldSizeY; y++){
                int offset = 360/worldSizeX / 2;

                if( y % 2 == 0){
                    offset = 0;
                }

                int degrees = 360/worldSizeX * x;
                Vector3 pos = new Vector3(5 * Mathf.Sin((degrees + offset) * Mathf.PI / 180),
                y * gridOffsetY + 0.5f,
                5 * Mathf.Cos((degrees + offset) * Mathf.PI / 180));

                GameObject block = Instantiate(brick,
                pos,
                new Quaternion(0, 0, 0, 1)) as GameObject;

                block.transform.eulerAngles = new Vector3(
                    block.transform.eulerAngles.x,
                    block.transform.eulerAngles.y + degrees + offset,
                    block.transform.eulerAngles.z
                );


                block.transform.SetParent(this.transform);
            }
        }
    }
}
