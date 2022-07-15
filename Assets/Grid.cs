using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject GridBlock;
    // Start is called before the first frame update
    void Start()
    {
        LinearGrid(10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LinearGrid(int size) {
        for (int i = 0; i<size; i++)
        {
          for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k++)
                {
                    Transform point = Instantiate(GridBlock.transform, new Vector3(i,j,k), Quaternion.identity);
                }
            }
        }
    
    }
}
