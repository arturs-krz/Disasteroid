using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataManager : MonoBehaviour
{
    // Start is called before the first frame update
    // Within that radius, there are some injuries.
    public int effectRadius;
    // Within that radius, everyone dies.
    public int exterminationRadius;
    // pop_table is a tab with dimensions 180 x 360
    // pop_table[i][j] corresponds to the population density of the lattitude i-89.5 and longitude j-179.5
    public static float[][] pop_table;
    public static long totalPop;
    public static float[][] veg_table;
    public static float totalVeg;
    void Start()
    {
        using (var reader = new StreamReader(@"Assets/Data/population_data.csv"))
        {
            pop_table = new float[180][];
            
            var line = reader.ReadLine();
            int k = 0;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                var values = line.Split(',');
                pop_table[179 - k] = new float[360];
                for (int i = 1; i < 361; i++) {
                    if (values[i] == "99999.0")
                    {
                        pop_table[179-k][i - 1] = 0;
                    }
                    else {
                        pop_table[179-k][i - 1] = float.Parse(values[i].Replace(".",","));
                    }
                    
                }
                k += 1;
            }
        }
        using (var reader = new StreamReader(@"Assets/Data/vegetation_data.csv"))
        {
            veg_table = new float[180][];

            var line = reader.ReadLine();
            int k = 0;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                var values = line.Split(',');
                veg_table[179 - k] = new float[360];
                for (int i = 1; i < 361; i++)
                {
                    if (values[i] == "99999.0")
                    {
                        veg_table[179 - k][i - 1] = 0;
                    }
                    else
                    {
                        veg_table[179 - k][i - 1] = float.Parse(values[i].Replace(".", ","))+0.1f;
                    }

                }
                k += 1;
            }
        }
        totalVeg = TotalVegetation();
        totalPop = TotalPopulation();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public long TotalPopulation() {
        long pop = 0;
        for (int i = 0; i < 180; i++)
        {
            for (int j = 0; j < 360; j++)
            {
                pop += (int)(pop_table[i][j] * 12345 * Math.Cos(((double)i - 89.5) * Math.PI / 180d));
            }
        }
        return pop;
    }

    public long TotalVegetation()
    {
        long veg = 0;
        for (int i = 0; i < 180; i++)
        {
            for (int j = 0; j < 360; j++)
            {
                veg += (int)(veg_table[i][j] * 12345 * Math.Cos(((double)i - 89.5) * Math.PI / 180d));
            }
        }
        return veg;
    }



    public long Explosion(float latitude, float longitude, out int number_of_dead, out float dead_vegetation)
    {
        number_of_dead = 0;
        dead_vegetation = 0;
        for (int i = 0; i < 180; i++) {
            for (int j = 0; j < 360; j++) {
                var x = (latitude - (i - 89.5f)) * Math.Cos((longitude + ((float)j - 89.5f))*Math.PI/180 / 2);
                var y = (longitude - (j - 179.5f));
                var explosionDistance = Math.Sqrt(x * x + y * y) ;
                if (explosionDistance < exterminationRadius) {
                    float density_dead = pop_table[i][j];
                    pop_table[i][j] = 0;
                    number_of_dead += (int) (density_dead* 12345 * Math.Cos(((double)i - 89.5)*Math.PI/ 180d));
                    dead_vegetation += (float) (veg_table[i][j] * 12345 * Math.Cos(((double)i - 89.5) * Math.PI / 180d));
                    veg_table[i][j] = 0;
                }
                else if(explosionDistance < effectRadius){
                    float proportion = (effectRadius - (float) explosionDistance) / (effectRadius - exterminationRadius);
                    float density_dead = proportion*pop_table[i][j];
                    number_of_dead += (int)(density_dead * 12345 * Math.Cos(((double)i - 89.5) * Math.PI / 180d));
                    pop_table[i][j] *= (1 - proportion);
                    dead_vegetation += (float)(veg_table[i][j]*proportion* 12345 * Math.Cos(((double)i - 89.5) * Math.PI / 180d));
                    veg_table[i][j] *= (1 - proportion);
                }
            }
        }
        totalPop -= number_of_dead;
        totalVeg -= dead_vegetation;
        return totalPop;
    }
    public long Explosion(Vector2 v, out int number_of_dead, out float dead_vegetation) {
        return Explosion(v.x, v.y, out number_of_dead, out dead_vegetation);
    }
}
